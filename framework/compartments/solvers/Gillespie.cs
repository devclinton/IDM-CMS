/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using compartments.emod;
using compartments.solvers.solverbase;

namespace compartments.solvers
{
    public class Gillespie : SolverBase
    {
        protected double[] currentRates;
        private double _a0;
        private Reaction _nextReaction;

        public Gillespie(ModelInfo modelInfo, double duration, int repeats, int samples) : base(modelInfo, duration, repeats, samples, new ModelBuilder())
        {
            currentRates  = new double[model.Reactions.Count];
            _a0           = 0.0;
            _nextReaction = null;
        }

        protected override double CalculateProposedTau(double tauLimit)
        {
            double actualTau = tauLimit;
            _a0 = UpdateAndSumRates(model.Reactions, currentRates);

            if (_a0 > 0.0)
            {
                double r1 = rng.GenerateUniformOO();
                double proposedTau = (Math.Log(1.0 / r1) / _a0) + CurrentTime;

                if (proposedTau < tauLimit)
                {
                    actualTau        = proposedTau;
                    double r2        = rng.GenerateUniformCC();
                    double threshold = r2 * _a0;
                    int mu           = GetReactionIndex(currentRates, threshold);
                    _nextReaction    = model.Reactions[mu];
                }
            }

            return actualTau;
        }

        protected override void ExecuteReactions()
        {
            if (_nextReaction != null)
            {
                FireReaction(_nextReaction);
                _nextReaction = null;
            }
        }

        public override string ToString()
        {
            return "SSA (Gillespie Direct)";
        }
    }
}
