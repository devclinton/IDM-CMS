/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using compartments.emod;
using compartments.solvers.solverbase;
using distlib;
using distlib.samplers;

namespace compartments.solvers
{
    // The time step is chosen according to the method presented in 'Efficient step size selection for the tau-
    // leaping simulation method' by Yang Cao, Daniel T. Gillespie, and Linda R. Petzold.
    // Ref.: Journal of Chemical Physics 124, 044109 (2006)
    public class RLeapingFast : SolverBase
    {
        #region fields
        protected List<Reaction>    reactions;
        protected double[]           currentRates;

        private readonly double[] _probabilityVector;

        private readonly int[]   _highestOrderReaction;   // highest order reaction in which a species is involved (Ref. denoted by HOR(i))
        private readonly double[] _muHat;                  // expected change for each species during the next time step (Ref. Eq. 32 a)
        private readonly double[] _sigmaHat2;              // expected variance for each species during the next time step (Ref. Eq. 32 b)
        private readonly double[] _varHat;                 // R-Leaping corrected variance (sigmaHat2 rescaled, see R-Leaping article)

        private readonly int[]   _executionsPerReaction;
        private readonly bool    _verbose;

        private readonly double _epsilon;          // error control parameter
        private readonly double _sortingInterval;  // when to sort
        private int _sortingIterationNumber;
        private double _a0;
        private int _actualL;

        private DistributionSampler _distributionSampler;
        #endregion

        public RLeapingFast(ModelInfo modelInfo, double duration, int repeats, int samples)
            : base(modelInfo, duration, repeats, samples)
        {
            Configuration config = Configuration.CurrentConfiguration;

            reactions    = new List<Reaction>(model.Reactions);
            currentRates = new double[reactions.Count];

            _highestOrderReaction = new int[model.Species.Count];
            _muHat                = new double[model.Species.Count];
            _sigmaHat2            = new double[model.Species.Count];
            _varHat               = new double[model.Species.Count];

            _probabilityVector     = new double[reactions.Count];
            _executionsPerReaction = new int[reactions.Count];

            _epsilon         = config.GetParameterWithDefault("r-leaping.epsilon", 0.1);
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

        protected double ComputeProposedLeap(double a0, double leapLimit, out int L)
        {
            double tau          = ComputeTau(a0);
            L                   = Math.Max((int)(a0 * tau), 1);
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
                L = (int)(leapLimit * a0);
                actualLeap = leapLimit;
            }

            return actualLeap;
        }

        protected double ComputeTau(double a0)
        {
            ComputeHor();
            ComputeMuHatAndSigmaHat2();
            ComputeVarHat(a0);

            double tau = double.MaxValue;

            for (int iSpecies = 0; iSpecies < model.Species.Count; ++iSpecies)
            {
                double xi = model.Species[iSpecies].Count;

                double epsixi;
                double epsixi2;

                switch (_highestOrderReaction[iSpecies]) // highest order reaction that the species participates in
                {
                    case 0:
                        break;
                    case 1:
                        epsixi      = Math.Max(_epsilon*xi, 1.0);
                        epsixi2     = Math.Pow(epsixi,     2.0);

                        tau         = Math.Min(tau, epsixi  / Math.Abs(_muHat[iSpecies]) );
                        tau         = Math.Min(tau, epsixi2 / _varHat[iSpecies]          );
                        break;

                    case 2:
                        epsixi      = Math.Max(0.5*_epsilon*xi, 1.0);
                        epsixi2     = Math.Pow(epsixi,         2.0);

                        tau         = Math.Min(tau, epsixi  / Math.Abs(_muHat[iSpecies]) );
                        tau         = Math.Min(tau, epsixi2 / _varHat[iSpecies]          );
                        break;
                }
            }
            return tau;
        }

        protected void ComputeHor()
        {
            for (int iSpecies = 0; iSpecies < _highestOrderReaction.Length; ++iSpecies)
            {
                _highestOrderReaction[iSpecies] = 0;
            }

            for (int iSpecies = 0; iSpecies < _highestOrderReaction.Length; ++iSpecies)
            {
                Species s = model.Species[iSpecies];
                foreach (Reaction r in reactions)
                {
                    List<Species> reactants = r.Reactants.ToList();
                    List<Species> nuR       = reactants.FindAll(sp => sp.Name.Equals(s.Name));

                    // species s participates as a reactant
                    if (nuR.Count > 0) 
                    {
                        // save the order of the reaction for this species if it is higher than previously found
                        if (reactants.Count > _highestOrderReaction[iSpecies])
                        {
                            _highestOrderReaction[iSpecies] = reactants.Count;
                        }
                    }
                }
            }
        }

        protected void ComputeMuHatAndSigmaHat2()
        {
            for (int iSpecies = 0; iSpecies < model.Species.Count; ++iSpecies)
            {
                _muHat[iSpecies]     = 0.0;
                _sigmaHat2[iSpecies] = 0.0;
            }

            for (int iSpecies = 0; iSpecies < model.Species.Count; ++iSpecies)
            {
                Species s = model.Species[iSpecies];
                for (int iReaction = 0; iReaction < reactions.Count; ++iReaction)
                {
                    // compute the order of iSpecies for this reaction
                    Reaction r = reactions[iReaction];

                    List<Species> nuR = r.Reactants.FindAll(sp => sp.Name.Equals(s.Name));
                    List<Species> nuP =  r.Products.FindAll(sp => sp.Name.Equals(s.Name));
                    
                    // the changes of species s in reaction r
                    int nu = nuP.Count - nuR.Count;
                    if (nu != 0)
                    {
                        _muHat[iSpecies]     += nu * currentRates[iReaction];
                        _sigmaHat2[iSpecies] += (Math.Pow(nu, 2.0) * currentRates[iReaction]);
                    }
                }
            }
        }

        // only for R-Leaping, for tau-Leaping use varhat = sigmaHat2.
        protected void ComputeVarHat(double a0)
        {
            for (int iSpecies = 0; iSpecies < _varHat.Length; ++iSpecies)
            {
                double rLeapingVarianceRescale = (1.0 / a0) * Math.Pow(_muHat[iSpecies], 2.0);
                double rescaledVariance        = _sigmaHat2[iSpecies] - rLeapingVarianceRescale;

                _varHat[iSpecies] = rescaledVariance;

                if (_varHat[iSpecies] <= 0.0) 
                {
                    _varHat[iSpecies] = 0.0;
                }
            }
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

            // nullify the negatives
            foreach (Species species in model.Species)
            {
                if (species.Count < 0)
                {
                    species.Count = 0;
                }
            }
        }

        public override string ToString()
        {
            return "R-Leaping (fast)";
        }
    }
}
