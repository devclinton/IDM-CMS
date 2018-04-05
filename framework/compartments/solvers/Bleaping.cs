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
    public class BLeaping : SolverBase
    {
        private readonly double[] _currentRates;
        private readonly double _tau;
        private double _deltaTau;
        private int _step;
        private readonly DistributionSampler _distributionSampler;

        public BLeaping(ModelInfo modelInfo, double duration, int repeats, int samples)
            : base(modelInfo, duration, repeats, samples)
        {
            _currentRates = new double[model.Reactions.Count];
            _tau = Configuration.CurrentConfiguration.GetParameterWithDefault("b-leaping.Tau", 0.1);
            _step = 1;
            _deltaTau = 0.0;
            _distributionSampler = RandLibSampler.CreateRandLibSampler(rng);
        }

        protected override void StartRealization()
        {
            base.StartRealization();
            _step = 1;
        }

        protected override double CalculateProposedTau(double tauLimit)
        {
            double desiredTau = _step * _tau;
            double actualTau;

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
            if (_deltaTau > 0.0)
            {
                UpdateAndSumRates(model.Reactions, _currentRates);
                ComputeLambda(_deltaTau, model.Reactions, _currentRates);
            }
        }

        private void ComputeLambda(double tauMin, IList<Reaction> subreactions, double[] noncritrates)
        {
            for (int jReaction = 0; jReaction < subreactions.Count; jReaction++)
            {
                double change = noncritrates[jReaction] * tauMin;
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
