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
        protected double[]       currentRates;
        protected double[,]      jacobianMatrix;

        private readonly double[] _probabilityVector;
        private readonly int[]   _executionsPerReaction;
        private readonly bool    _verbose;

        private readonly double  _epsilon;
        private readonly double  _theta;
        private readonly double  _sortingInterval;
        private int              _sortingIterationNumber;
        private double           _a0;
        private int              _actualL;

        private DistributionSampler _distributionSampler;

        public RLeaping(ModelInfo modelInfo, double duration, int repeats, int samples)
            : base(modelInfo, duration, repeats, samples)
        {
            Configuration config   = Configuration.CurrentConfiguration;

            reactions              = new List<Reaction>(model.Reactions);
            currentRates           = new double[reactions.Count];
            jacobianMatrix         = new double[reactions.Count, model.Species.Count];
            _probabilityVector     = new double[reactions.Count];
            _executionsPerReaction = new int[reactions.Count];

            _epsilon         = config.GetParameterWithDefault("r-leaping.epsilon",            0.1);
            _theta           = config.GetParameterWithDefault("r-leaping.theta",              0.0);
            _sortingInterval = config.GetParameterWithDefault("r-leaping.sorting interval", 365.0);
            _verbose         = config.GetParameterWithDefault("r-leaping.verbose", false);

            _distributionSampler = RandLibSampler.CreateRandLibSampler(rng);
        }

        protected override void StartRealization()
        {
            base.StartRealization();

            _sortingIterationNumber = 0;
        }

        protected override double CalculateProposedTau(double tauLimit)
        {
            double actualTau = tauLimit;

            SortReactionsIfNecessary();
            _a0 = UpdateAndSumRates(reactions, currentRates);

            if (_a0 > 0.0)
            {
                actualTau = CurrentTime + ComputeProposedLeap(_a0, tauLimit - CurrentTime, out _actualL);

                if (_verbose)
                {
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
            if (CurrentTime >= (_sortingIterationNumber * _sortingInterval))
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

        protected int ComputeLeapLength(double a0)
        {
            jacobianMatrix     = ComputeJacobian();
            double[] tauvector = PossibleTaus(jacobianMatrix, _epsilon, a0);
            double tau         = tauvector.Min();
            int Ltau           = Math.Max((int)(a0 * tau), 1);
            int L              = ComputeLFromLtauAndA0UsingTheta(Ltau, a0);

            return L;
        }

        protected double ComputeProposedLeap(double a0, double leapLimit, out int L)
        {
            jacobianMatrix      = ComputeJacobian();
            double[] tauvector  = PossibleTaus(jacobianMatrix, _epsilon, a0);
            double tau          = tauvector.Min();
            int Ltau            = Math.Max((int)(a0 * tau), 1);
            L                   = ComputeLFromLtauAndA0UsingTheta(Ltau, a0);
            double proposedLeap = (1.0 / a0) * _distributionSampler.StandardGamma(L);
            double actualLeap   = proposedLeap;

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

        protected double[,] ComputeJacobian()
        {
            var jacobian       = new double[reactions.Count, model.Species.Count];
            var actualRates    = new double[reactions.Count];
            var perturbedRates = new double[reactions.Count];
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
                    double av = r.Rate;
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

        protected double[] PossibleTaus(double[,] jacobian, double epsilon, double a0)
        {
            var eta          = new double[model.Species.Count];
            var possibleTaus = new double[reactions.Count];

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
                double sum = 0.0;
                for (int iSpecies = 0; iSpecies < model.Species.Count; iSpecies++)
                {
                    sum += eta[iSpecies] * jacobian[iReaction, iSpecies];
                }

                possibleTaus[iReaction] = epsilon * a0 / Math.Abs(sum);
            }

            return possibleTaus;
        }

        protected void ExecuteLReactions(int L, double a0)
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

        protected int ComputeLFromLtauAndA0UsingTheta(int Ltau, double a0)
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

                int proposedL = (int)((1.0 - _theta * (1.0 - a0 / r.Rate)) * ((double)lj));

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
