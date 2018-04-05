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
    public class GillespieFirstReaction : SolverBase
    {
        protected double[] currentRates;
        private int _reactionIndex;
        private Reaction _selectedReaction;

        public GillespieFirstReaction(ModelInfo modelInfo, double duration, int repeats, int samples) :
            base(modelInfo, duration, repeats, samples)
        {
            currentRates = new double[model.Reactions.Count];
            _selectedReaction = null;
        }

        protected override double CalculateProposedTau(double tauLimit)
        {
            double actualTau = tauLimit;
            double a0 = UpdateAndSumRates(model.Reactions, currentRates);

            if (a0 > 0.0)
            {
                double minDelta = double.MaxValue;

                for (int j = 0; j < model.Reactions.Count; j++)
                {
                    if (currentRates[j] > 0.0)
                    {
                        double randomTemp = rng.GenerateUniformOO();

                        double proposedDelta = Math.Log(1.0 / randomTemp) / currentRates[j];

                        if (proposedDelta < minDelta)
                        {
                            minDelta = proposedDelta;
                            _reactionIndex = j;
                        }
                    }
                }

                double proposedTau = CurrentTime + minDelta;
                if (proposedTau < actualTau)
                {
                    actualTau         = proposedTau;
                    _selectedReaction = model.Reactions[_reactionIndex];
                }
            }

            return actualTau;
        }

        protected override void ExecuteReactions()
        {
            if (_selectedReaction != null)
            {
                FireReaction(_selectedReaction);
                _selectedReaction = null;
            }
        }

        public override string ToString()
        {
            return "First Reaction";
        }
    }
}
