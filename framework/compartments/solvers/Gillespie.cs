using System;
using compartments.emod;
using compartments.solvers.solverbase;

namespace compartments.solvers
{
    public class Gillespie : SolverBase
    {
        protected float[] currentRates;
        private float _a0;
        private Reaction _nextReaction;

        public Gillespie(ModelInfo modelInfo, float duration, int repeats, int samples) : base(modelInfo, duration, repeats, samples, new ModelBuilder())
        {
            currentRates  = new float[model.Reactions.Count];
            _a0           = 0.0f;
            _nextReaction = null;
        }

        protected override float CalculateProposedTau(float tauLimit)
        {
            float actualTau = tauLimit;
            _a0 = UpdateAndSumRates(model.Reactions, currentRates);

            if (_a0 > 0.0f)
            {
                float r1 = rng.GenerateUniformOO();
                float proposedTau = ((float) Math.Log(1.0f / r1) / _a0) + CurrentTime;

                if (proposedTau < tauLimit)
                {
                    actualTau        = proposedTau;
                    float r2         = rng.GenerateUniformCC();
                    double threshold = r2 * (double)_a0;
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
