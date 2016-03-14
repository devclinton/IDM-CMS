using System;
using compartments.emod;
using compartments.solvers.solverbase;

namespace compartments.solvers
{
    public class GillespieFirstReaction : SolverBase
    {
        protected float[] currentRates;
        private int _reactionIndex;
        private Reaction _selectedReaction;

        public GillespieFirstReaction(ModelInfo modelInfo, float duration, int repeats, int samples) :
            base(modelInfo, duration, repeats, samples)
        {
            currentRates = new float[model.Reactions.Count];
            _selectedReaction = null;
        }

        protected override float CalculateProposedTau(float tauLimit)
        {
            float actualTau = tauLimit;
            float a0 = UpdateAndSumRates(model.Reactions, currentRates);

            if (a0 > 0.0f)
            {
                float minDelta = float.MaxValue;

                for (int j = 0; j < model.Reactions.Count; j++)
                {
                    if (currentRates[j] > 0.0f)
                    {
                        float randomTemp = rng.GenerateUniformOO();

                        float proposedDelta = (float)Math.Log(1.0f / randomTemp) / currentRates[j];

                        if (proposedDelta < minDelta)
                        {
                            minDelta = proposedDelta;
                            _reactionIndex = j;
                        }
                    }
                }

                float proposedTau = CurrentTime + minDelta;
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
