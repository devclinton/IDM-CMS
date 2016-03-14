using System;
using System.Collections.Generic;
using System.Linq;
using compartments.emod;
using compartments.solvers.solverbase;
using distlib;
using distlib.samplers;

namespace compartments.solvers
{
    public class BLeaping : SolverBase
    {
        private readonly float[] _currentRates;
        private readonly float _tau;
        private float _deltaTau;
        private int _step;
        private readonly DistributionSampler _distributionSampler;

        public BLeaping(ModelInfo modelInfo, float duration, int repeats, int samples)
            : base(modelInfo, duration, repeats, samples)
        {
            _currentRates = new float[model.Reactions.Count];
            _tau = Configuration.CurrentConfiguration.GetParameterWithDefault("b-leaping.Tau", 0.1f);
            _step = 1;
            _deltaTau = 0.0f;
            _distributionSampler = RandLibSampler.CreateRandLibSampler(rng);
        }

        protected override void StartRealization()
        {
            base.StartRealization();
            _step = 1;
        }

        protected override float CalculateProposedTau(float tauLimit)
        {
            float desiredTau = _step*_tau;
            float actualTau;

            if (desiredTau < tauLimit)
            {
                actualTau = desiredTau;
                _step++;
            }
            else
            {
                actualTau = tauLimit;
            }

            _deltaTau = actualTau - CurrentTime;

            return actualTau;
        }

        protected override void ExecuteReactions()
        {
            if (_deltaTau > 0.0f)
            {
                UpdateAndSumRates(model.Reactions, _currentRates);
                ComputeLambda(_deltaTau, model.Reactions, _currentRates);
            }
        }

        private void ComputeLambda(float tauMin, IList<Reaction> subreactions, float[] noncritrates)
        {
            for (int jReaction = 0; jReaction < subreactions.Count; jReaction++)
            {
                float change = noncritrates[jReaction] * tauMin;
                int howManyReactions = _distributionSampler.GeneratePoisson(change);

                FireReaction(subreactions[jReaction], howManyReactions);

                foreach (Species s in subreactions[jReaction].Reactants.Where(s => s.Count < 0f))
                {
                    Console.WriteLine("Negative species detected: {0} = {1}", s.Name, s.Count);
                    Console.WriteLine("You may need to decrease Tau for your B-Leaping Simulation");
                    s.Count = 0;
                }
            }
        }

        public override string ToString()
        {
            return "B-Leaping";
        }
    }
}
