using System;
using System.Collections.Generic;
using System.Linq;
using compartments.emod;
using compartments.solvers.solverbase;
using distlib;
using distlib.samplers;

namespace compartments.solvers
{
    public class RLeaping : SolverBase
    {
        protected List<Reaction> reactions;
        protected float[]       currentRates;
        protected float[,]      jacobianMatrix;

        private readonly float[] _probabilityVector;
        private readonly int[]   _executionsPerReaction;
        private readonly bool    _verbose;

        private readonly float   _epsilon;
        private readonly float   _theta;
        private readonly float   _sortingInterval;
        private int              _sortingIterationNumber;
        private float            _a0;
        private int              _actualL;

        private DistributionSampler _distributionSampler;

        public RLeaping(ModelInfo modelInfo, float duration, int repeats, int samples)
            : base(modelInfo, duration, repeats, samples)
        {
            Configuration config   = Configuration.CurrentConfiguration;

            reactions              = new List<Reaction>(model.Reactions);
            currentRates           = new float[reactions.Count];
            jacobianMatrix         = new float[reactions.Count, model.Species.Count];
            _probabilityVector     = new float[reactions.Count];
            _executionsPerReaction = new int[reactions.Count];

            _epsilon         = config.GetParameterWithDefault("r-leaping.epsilon",            0.1f);
            _theta           = config.GetParameterWithDefault("r-leaping.theta",              0.0f);
            _sortingInterval = config.GetParameterWithDefault("r-leaping.sorting interval", 365.0f);
            _verbose         = config.GetParameterWithDefault("r-leaping.verbose", false);

            _distributionSampler = RandLibSampler.CreateRandLibSampler(rng);
        }

        protected override void StartRealization()
        {
            base.StartRealization();

            _sortingIterationNumber = 0;
        }

        protected override float CalculateProposedTau(float tauLimit)
        {
            float actualTau = tauLimit;

            SortReactionsIfNecessary();
            _a0 = UpdateAndSumRates(reactions, currentRates);

            if (_a0 > 0.0f)
            {
                actualTau = CurrentTime + ComputeProposedLeap(_a0, tauLimit - CurrentTime, out _actualL);

                if (_verbose)
                {
                    // Diagnostics
                    Console.WriteLine("t = {0},  current L = {1}", CurrentTime, _actualL);
                    Console.WriteLine("\n\n");
                }
            }

            return actualTau;
        }

        protected override void ExecuteReactions()
        {
            ExecuteLReactions(_actualL, _a0);
        }

        private void SortReactionsIfNecessary()
        {
            // check to see if it's time to sort
            if (CurrentTime >= (_sortingIterationNumber*_sortingInterval))
            {
                // sort the reaction list in descending order (minus sign) using the new propensities
                reactions.Sort((r1, r2) => -(r1.Rate.CompareTo(r2.Rate)));
                _sortingIterationNumber++;

                if (_verbose)
                {
                    Console.WriteLine("Sorting the reactions at time t = {0}", CurrentTime);
                    Console.WriteLine("Sorted reactions by propensities");
                    foreach (Reaction r in reactions)
                    {
                        Console.WriteLine("{0}", r);
                    }
                }
            }
        }

        protected int ComputeLeapLength(float a0)
        {
            jacobianMatrix    = ComputeJacobian();
            float[] tauvector = PossibleTaus(jacobianMatrix, _epsilon, a0);
            float tau         = tauvector.Min();
            int Ltau          = Math.Max((int)(a0 * tau), 1);
            int L             = ComputeLFromLtauAndA0UsingTheta(Ltau, a0);

            return L;
        }

        protected float ComputeProposedLeap(float a0, float leapLimit, out int L)
        {
            jacobianMatrix     = ComputeJacobian();
            float[] tauvector  = PossibleTaus(jacobianMatrix, _epsilon, a0);
            float tau          = tauvector.Min();
            int Ltau           = Math.Max((int)(a0 * tau), 1);
            L                  = ComputeLFromLtauAndA0UsingTheta(Ltau, a0);
            float proposedLeap = ((float)(1.0 / a0)) * _distributionSampler.StandardGamma(L);
            float actualLeap   = proposedLeap;
            if (proposedLeap > leapLimit)
            {
                #region verbose
                if (_verbose)
                {
                    Console.WriteLine("Limiting leap {0} based on next scheduled event {1}.", proposedLeap, leapLimit);
                }
                #endregion
                L = (int) (leapLimit*a0);
                actualLeap = leapLimit;
            }

            return actualLeap;
        }

        protected float[,] ComputeJacobian()
        {
            var jacobian       = new float[reactions.Count, model.Species.Count];
            var actualRates    = new float[reactions.Count];
            var perturbedRates = new float[reactions.Count];
            int iSpecies       = 0;

            for (int iReaction = 0; iReaction < reactions.Count; iReaction++)
            {
                actualRates[iReaction] = currentRates[iReaction];
            }

            foreach (Species s in model.Species)
            {
                s.Increment();                                                    // What about if number of species is zero?

                int index = 0;
                foreach (Reaction r in reactions)
                {
                    float av = r.Rate;
                    perturbedRates[index++] = av;
                }


                for (int i = 0; i < reactions.Count; i++)
                {
                    jacobian[i, iSpecies] = (perturbedRates[i] - actualRates[i]) / 1;      //Simple Derivative Calculation with a single increment of species
                }

                s.Decrement();
                iSpecies++;
            }

            return jacobian;
        }

        protected float[] PossibleTaus(float[,] jacobian, float epsilon, float a0)
        {
            var eta          = new float[model.Species.Count];
            var possibleTaus = new float[reactions.Count];

            //  Here we compute Eta  
            for (int iReaction = 0; iReaction < reactions.Count; iReaction++)
            {
                for (int iSpecies = 0; iSpecies < model.Species.Count; iSpecies++)
                {
                    int reactionVec = 0;
                    reactionVec -= reactions[iReaction].Reactants.Count(species => species == model.Species[iSpecies]);
                    reactionVec += reactions[iReaction].Products.Count(species => species == model.Species[iSpecies]);

                    eta[iSpecies] += currentRates[iReaction] * reactionVec;        //Simple Derivative Calculation with a single increment of species
                }
            }

            //  Here we compute the Tauvector
            for (int iReaction = 0; iReaction < reactions.Count; iReaction++)
            {
                float sum = 0.0f;
                for (int iSpecies = 0; iSpecies < model.Species.Count; iSpecies++)
                {
                    sum += eta[iSpecies] * jacobian[iReaction, iSpecies];
                }

                possibleTaus[iReaction] = epsilon * a0 / Math.Abs(sum);
            }

            return possibleTaus;
        }

        protected void ExecuteLReactions(int L, float a0)
        {
            // fill the probability vector
            for (int i = 0; i < reactions.Count; i++)
            {
                _probabilityVector[i] = currentRates[i] / a0;
            }

            _distributionSampler.GenerateMultinomial(L, _probabilityVector, _executionsPerReaction);

            for (int i = 0; i < reactions.Count; i++)
            {
                Reaction reaction = reactions[i];
                int delta = _executionsPerReaction[i];

                FireReaction(reaction, delta);
            }
        }

        protected int ComputeLFromLtauAndA0UsingTheta(int Ltau, float a0)
        {
            int L = Ltau;
            foreach (Reaction r in reactions)
            {
                int lj = int.MaxValue; // Maximum integer 2^(32)/2 - 1

                // Compute nu for each species in the current reaction
                var reactantsUnique = r.Reactants.Distinct();
                
                foreach (Species s in reactantsUnique)
                {
                    var nuR = r.Reactants.FindAll(sp => sp.Name.Equals(s.Name));
                    var nuP =  r.Products.FindAll(sp => sp.Name.Equals(s.Name));

                    // the changes of species s in reaction r
                    int nu  = nuP.Count - nuR.Count;

                    if (nu < 0)
                    { lj = Math.Min(lj, -s.Count / nu); }
                }

                int proposedL = (int)((1.0 - _theta * (1.0 - a0 / r.Rate)) * ((float)lj));

                if (proposedL < L && proposedL > 0)
                {
                    if (_verbose)
                    {
                        // Write some diagnostics to the console 
                        Console.WriteLine("Resetting L because of theta from {0} to {1}", L, proposedL);
                        Console.WriteLine("Current Lj {0} ", lj);
                    }

                    L = proposedL;
                }
            }

            return L;
        }

        public override string ToString()
        {
            return "R-Leaping";
        }

    }
}
