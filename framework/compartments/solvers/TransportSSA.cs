using System;
using System.Collections.Generic;
using System.Linq;
using compartments.emod;
using compartments.solvers.solverbase;
using distlib;
using distlib.samplers;

namespace compartments.solvers
{
    public class TransportSSA : SolverBase
    {
        /*************************************************************************************
         The following is a generalization of the Inhomogeneous Stochastic Simulation 
         Algorithm by S. Lampoudi, D.T. Gillespie, & L.R. Petzold.  The theorem was proved 
         by T. Jahnke and W. Huisinga  
          
         TransportSSA : SolverBase 
         *************************************************************************************/

        protected float[]           currentReactionRates;   // reaction events only

        protected float[][]         kernels;                // (number of species) x (number of neighbors + 1) jagged array, probability density function for the Multinomial distribution
        protected int[][]           speciesIndices;         // (number of species) x (number of neighbors + 1) jagged array, indices for the nearest neighbors
        protected int[]             lambda;                 // (number of species), change for each species count from diffusion/transport events
        protected float[]           diffusionRates;         // (number of diffusion events)

        protected int               maxInitialCount;        // used to compute the diffusion time-step constraint
        protected float             diffusionTau;           // diffusion time-step
        protected float             epsilon;                // used for diffusion time-step constraint, unrelated to epsilon in R- and tau-Leaping
        protected float             graphDimension;         // maximum number of neighbors in the graph
        protected int               greensFunctionIterations; // number of integration steps for Green's function
        protected bool              verbose;                // prints the current time to standard output

        private readonly IList<int> _partiallyExecutedReactionsEvents;   // the reactions executed in a time-step, usually of length one
        protected IList<Reaction>   diffusionReactions;     // diffusion/transport events
        protected IList<Reaction>   reactionReactions;      // reaction events

        protected static int        reactionCountThreshold = 5;   // operator splitting works best when one reaction per time-step is exectued
        private float _actualLeap;

        protected DistributionSampler _distributionSampler;

        public TransportSSA(ModelInfo modelInfo, float duration, int repeats, int samples)
               : base(modelInfo, duration, repeats, samples)
        {
            Configuration config    = Configuration.CurrentConfiguration;

            // Transport events
            diffusionReactions      = model.Reactions.Where(r => !r.IsLocal).ToList();

            // Reaction events
            reactionReactions       = model.Reactions.Where(r => r.IsLocal).ToList();

            currentReactionRates                = new float[reactionReactions.Count];
            kernels                             = new float[model.Species.Count][];           // jagged array (i.e. array of arrays)
            speciesIndices                      = new int[model.Species.Count][];             // nearest neighbors of the current species
            lambda                              = new int[model.Species.Count];               // X(t+tau) = X(t) + lambda, diffusion changes
            diffusionRates                      = new float[diffusionReactions.Count];
            _partiallyExecutedReactionsEvents   = new List<int>();

            // Jagged array initialization 
            for (int iSpecies = 0; iSpecies < model.Species.Count; ++iSpecies)
            {
                int numberOfNieghbors       = NumberOfNeighbors(iSpecies) + 1; // +1 for the current node + number of neighbors
                kernels[iSpecies]           = new float[numberOfNieghbors];
                speciesIndices[iSpecies]    = new int[numberOfNieghbors];
            }

            epsilon                     = config.GetParameterWithDefault("tssa.epsilon", 0.01f);    
            greensFunctionIterations    = config.GetParameterWithDefault("tssa.greensFunctionIterations", 100);
            verbose                     = config.GetParameterWithDefault("tssa.verbose", false);

            _distributionSampler = RandLibSampler.CreateRandLibSampler(rng);

            // Get the transport rates
            FindDiffusionRates(ref diffusionRates);
        }

        protected override void StartRealization()
        {
            base.StartRealization();

            // Compute the fundamental solutions for the transport process
            ComputeFundamentalSolutions();
        }

        protected override float CalculateProposedTau(float tauLimit)
        {
            _actualLeap = Math.Min(diffusionTau, tauLimit - CurrentTime);
            return Math.Min(CurrentTime + diffusionTau, tauLimit);
        }

        protected override void ExecuteReactions()
        {
            float a0 = UpdateAndSumRates(reactionReactions, currentReactionRates);
            _partiallyExecutedReactionsEvents.Clear();

            if (a0 > 0.0f) // reaction & transport, using operator splitting
            {
                // Operator Splitting

                // Substep 1. REACTIONS
                OperatorSplittingReactions(_partiallyExecutedReactionsEvents, a0, _actualLeap);

                // Substep 2. TRANSPORT 
                OperatorSplittingTransport(_actualLeap);

                // Substep 3. COMPLETE
                CompleteOperatorSplittingReactions(_partiallyExecutedReactionsEvents);
            }
            else // transport only
            {
                // Execute transport events
                OperatorSplittingTransport(_actualLeap);
            }
        }

        // Partially execute reactions till diffusionTime
        private void OperatorSplittingReactions(IList<int> partiallyExecutedReactionsEvents, float a0, float leapLimit)
        {
            float virtualReactionTime = 0.0f;
            float u1 = rng.GenerateUniformOO();
            float reactionTau = (float)(-Math.Log(u1) / a0);

            while ((virtualReactionTime + reactionTau) < leapLimit)
            {
                virtualReactionTime += reactionTau;

                float u2 = rng.GenerateUniformCC();
                double threshold = u2 * (double)a0;
                int mu = GetReactionIndex(currentReactionRates, threshold);

                // Partially execute the reaction
                FireReactants(reactionReactions[mu]);
                partiallyExecutedReactionsEvents.Add(mu);

                a0 = UpdateAndSumRates(reactionReactions, currentReactionRates);

                u1 = rng.GenerateUniformOO();
                reactionTau = (float)(-Math.Log(u1) / a0);
            }

            if (partiallyExecutedReactionsEvents.Count >= reactionCountThreshold)
            {
                Console.WriteLine("Warning: Diffusion time step may be too large since the operators are apparently on different timescales");
            }
        }

        // Execute the transport/diffusion process and complete the execution of the reactions
        private void OperatorSplittingTransport(float dt)
        {
            float virtualDiffusionTime = 0.0f;
            while (virtualDiffusionTime < dt) // match the times of the processes
            {
                ApplyLambda();
                virtualDiffusionTime += diffusionTau;
            }
        }

        private void CompleteOperatorSplittingReactions(IList<int> partiallyExecutedReactionsEvents)
        {
            // Complete the reaction
            foreach (int t in partiallyExecutedReactionsEvents)
            {
                FireProducts(reactionReactions[t]);
            }
        }

        // Returns the number of neighbors for a node (a node being a species index)
        protected int NumberOfNeighbors(int speciesIndex)
        {
            Species s             = model.Species[speciesIndex];
            int numberOfNeighbors = 0;

            for (int iReaction = 0; iReaction < diffusionReactions.Count; ++iReaction)
            {
                if (diffusionReactions[iReaction].Reactants.Any(sp => sp.Name.Equals(s.Name)) )
                {
                    numberOfNeighbors++;
                }
            }

            return numberOfNeighbors;
        }

        // Sets the species counts to unity so as to find the rate
        protected void FindDiffusionRates(ref float[] rates)
        {
            // Find the diffusion rates from the model file by setting species vales to unity
            foreach (Species s in model.Species)
            {
                s.Count = 1;
            }

            for (int iReaction = 0; iReaction < diffusionReactions.Count; ++iReaction)
            {
                rates[iReaction] = diffusionReactions[iReaction].Rate;
            }
        }

        // Returns the maximum number of neighbors encountered in the graph
        protected float ComputeGraphDimension()
        {
            return (kernels.Max(k => k.Length) - 1);
        }

        // Computes normalized Green's functions and saves them in the 'kernels' variable
        // The integration in virtual time, i.e. (0, diffusionTau], obeys the Fourier and CFL conditions
        protected void ComputeFundamentalSolutions()
        {
            var greensFunction        = new float[model.Species.Count];
            var greensFunctionChanges = new float[model.Species.Count];

            // maximum number of neighbors used for Fourier number constraint
            graphDimension = ComputeGraphDimension(); 
            Console.WriteLine("graph dimension: {0}", graphDimension);

            maxInitialCount = model.Species.Max(s => s.Count);
            Console.WriteLine("max species count: {0}", maxInitialCount);

            float dPrime = diffusionRates.Length > 0 ? diffusionRates.Max() : 0.0f;

            // Fourier and Courant-Friedrichs-Lewy (CFL) conditions
            // Fourier: dt <= C1 h^2 / (2*d*D),
            // CFL:     dt <= C2 h / v,
            // Combining both => dt <= C *(1/ max(D/h^2, v/h) )*(1/d)
            // dt however is for a continuum simulation, so we rescale it by the maximum species count 'maxInitialCount'.

            // h is the distance between nodes, d is the dimension of the graph (graphDimension), v is the convection velocity,
            // D is diffusion coefficient and C, C1 & C2 are arbitrary constants < 1.   Smaller values of C, C1 and C2 result in better accuracy
            // although the convergence rate is nevertheless the same.  We use ISSA notation and let epsilon << 1 take the place of C, C1 and C2,
            // and is supplied in the input file.
            // max(D/h^2, v/h) is denoted by dPrime and is the maximum rate encountered in transport events.
  
            // Diffusion rates in the input file should be D/h^2 and convection rates should be v/h, but this is the responsibility of
            // the modeler.  Diffusion to nodes that are far away should have smaller rates compared to a node that is close.

            diffusionTau = (float)(1.0 / maxInitialCount * Math.Sqrt(epsilon) * (1.0 / dPrime) * (1.0 / graphDimension)); 
            Console.WriteLine("diffusion tau: {0}", diffusionTau);

            // for all nodes in the graph
            for (int iSpecies = 0; iSpecies < model.Species.Count; ++iSpecies)
            {
                SolveGreensFunction(ref greensFunction, ref greensFunctionChanges, iSpecies);
                NormalizeGreensFunction( ref greensFunction );
                FillKernelMatrix( greensFunction, iSpecies );
            }

            // the integral of Green's function may not be unity owing to finite precision arithmetic 
            NormalizeKernelFunction();
        }

        // Solves for Green's function
        // Green's function is a vector, i.e. G = (G_1, ..., G_N), where N is the number of nodes
        // Set G_i = 0 for all i
        // To find Green's function for a node j, let G_j = 1 and solve (d/dt) G = Delta G, where Delta represents the 
        // changes applied to the graph (e.g. for diffusion (d/dt) G_i = -2*G_i+G_(i-1)+G(i+1) ).  Green's function in this 
        // case is similar to a spatially-discretized PDE.
        protected void SolveGreensFunction(ref float[] greensFunction, ref float[] greensFunctionChanges, int iSpecies)
        {
            // reset Green's function
            for (int z = 0; z < model.Species.Count; ++z)
            {
                greensFunction[z] = 0.0f;
            }

            // compute Green's function
            greensFunction[iSpecies] = 1.0f; // Kronecker's delta function
           
            for (int iGreensFunctionIterations = 0; iGreensFunctionIterations < greensFunctionIterations; ++iGreensFunctionIterations) // time
            {
                for (int z = 0; z < model.Species.Count; ++z)
                {
                    greensFunctionChanges[z] = 0.0f;
                }

                for (int iReaction = 0; iReaction < diffusionReactions.Count; ++iReaction)
                {
                    Reaction currentReaction = diffusionReactions[iReaction];
                    var reactants            = currentReaction.Reactants;
                    var products             = currentReaction.Products;

                    int speciesOrigin        = model.Species.IndexOf(reactants[0]);
                    int speciesDestination   = model.Species.IndexOf(products[0]);

                    float change = (float)(greensFunction[speciesOrigin] * diffusionRates[iReaction] * diffusionTau * (1.0 / ((float)greensFunctionIterations)));

                    greensFunctionChanges[speciesOrigin]        -= change;
                    greensFunctionChanges[speciesDestination]   += change;
                }

                for (int k = 0; k < model.Species.Count; ++k)
                {
                    greensFunction[k] += greensFunctionChanges[k];
                }
            }
        }

        // Places Green's function in a matrix
        protected void FillKernelMatrix(float[] greensFunction, int iSpecies)
        {
            // fill the jagged kernel and speciesIndices array  array
            Species s                   = model.Species[iSpecies];
            kernels[iSpecies][0]        = greensFunction[iSpecies];
            speciesIndices[iSpecies][0] = iSpecies;
            int counter                 = 1;

            foreach (Reaction currentReaction in diffusionReactions)
            {
                var reactants                 = currentReaction.Reactants;
                var products                  = currentReaction.Products;
                var testSpeciesAsAReactant    = reactants.Count(sp => sp == s);

                if (testSpeciesAsAReactant > 0) // a neighbor
                {
                    int indexOfProduct                  = model.Species.IndexOf(products[0]);
                    kernels[iSpecies][counter]          = greensFunction[indexOfProduct];
                    speciesIndices[iSpecies][counter]   = indexOfProduct;
                    counter++;
                }
            }
        }

        // Normalizes Green's function so that the integral is unity
        protected void NormalizeGreensFunction(ref float[] greensFunction)
        {
            float summation = 0.0f;
            for (int k = 0; k < model.Species.Count; ++k)
            {
                if (greensFunction[k] < 0.0f)
                {
                    greensFunction[k] = 0.0f;
                }

                summation += greensFunction[k];
            }

            for (int k = 0; k < model.Species.Count; ++k)
            {
                greensFunction[k] /= summation;
            }
        }

        // Distributes the extra probability mass, which is a result of truncating Green's function
        protected void NormalizeKernelFunction()
        {
            for (int iSpecies = 0; iSpecies < model.Species.Count; ++iSpecies)
            {
                float summation = 0.0f;
                for (int iSubgraph = 0; iSubgraph < kernels[iSpecies].Length; ++iSubgraph)
                {
                    summation += kernels[iSpecies][iSubgraph];
                }

                float error         = (float)Math.Abs(1.0 - summation);
                float toDistribute  = error / ((float)(kernels[iSpecies].Length-1));

                // distribute to the neighbors and exclude the current node (iSubgraph = 0)
                for (int iSubgraph = 1; iSubgraph < kernels[iSpecies].Length; ++iSubgraph)
                {
                    kernels[iSpecies][iSubgraph] += toDistribute;
                }
            }
        }

        // Applies the changes to the species counts for the diffusion/transport process
        protected virtual void ApplyLambda()
        {
            for (int iSpecies = 0; iSpecies < model.Species.Count; ++iSpecies)
            {
                lambda[iSpecies] = 0;
            }

            for (int iSpecies = 0; iSpecies < model.Species.Count; ++iSpecies)
            {
                // skip the node if there is nothing to do
                if (model.Species[iSpecies].Count == 0)
                {
                    continue;
                }

                var numberOfExecutions = new int[ kernels[iSpecies].Length ];
                _distributionSampler.GenerateMultinomial(model.Species[iSpecies].Count, kernels[iSpecies], numberOfExecutions);
                lambda[speciesIndices[iSpecies][0]] -= (model.Species[iSpecies].Count - numberOfExecutions[0]);

                // z = 1 (sic), z = 0 current node; z = 1...n neighbors
                for (int z = 1; z < numberOfExecutions.Length; ++z)
                {
                    lambda[ speciesIndices[iSpecies][z] ] += numberOfExecutions[z];
                }
            }

            for (int iSpecies = 0; iSpecies < model.Species.Count; ++iSpecies)
            {
                model.Species[iSpecies].Count += lambda[iSpecies];
            }
        }

        // Partially execute the reaction: reactants and products, respectively
        protected void FireReactants(Reaction rr)
        {
            foreach (Species s in rr.Reactants)
                s.Decrement();
        }

        protected void FireProducts(Reaction rr)
        {
            foreach (Species s in rr.Products)
                s.Increment();
        }

        public override string ToString()
        {
            return "TransportSSA (iSSA)";
        }
    }
}

