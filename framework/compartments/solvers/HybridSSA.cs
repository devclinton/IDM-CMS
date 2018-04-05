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

namespace compartments.solvers
{
    public class HybridSSA : SolverBase
    {
        protected enum Mode
        {
            RejectionMethod = 1,
            ExactMethod = 2
        }

        protected double[] currentRates;
        protected SortedDictionary<double, IList<Reaction>> priorityQueue;
        protected Mode runMode;
        private Reaction _nextReaction;

        public HybridSSA(ModelInfo modelInfo, double duration, int repeats, int samples) :
            base(modelInfo, duration, repeats, samples)
        {
            currentRates  = new double[model.Reactions.Count];
            priorityQueue = new SortedDictionary<double, IList<Reaction>>();

            runMode = Mode.ExactMethod;
#if DEPRECATED
            runMode = (Mode)Enum.Parse(typeof(Mode), Configuration.CurrentConfiguration.GetParameterWithDefault("hybrid.method", "RejectionMethod"), true);
#endif
            switch (runMode)
            {
                case Mode.RejectionMethod: Console.WriteLine("Using rejection method."); break;
                case Mode.ExactMethod: Console.WriteLine("Using exact method."); break;
            }

            _nextReaction = null;
        }

        protected override void StepOnce()
        {
            switch (runMode)
            {
#if DEPRECATED
                case Mode.RejectionMethod:
                    {
                        double timeOfNextReaction = double.MaxValue;
                        double timeOfNextEvent = double.MaxValue;

                        // Calculate time to next reaction
                        double a0 = UpdateRates();

                        if (a0 > 0.0)
                        {
                            double r1 = rng.NextVariate() + double.Epsilon;
                            timeOfNextReaction = CurrentTime + Math.Log(1.0 / r1) / a0;
                        }

                        if (priorityQueue.Count > 0)
                        {
                            timeOfNextEvent = priorityQueue.Keys.First();
                        }

                        // Next reaction occurs before next event (or there are no pending events).
                        if (timeOfNextReaction < timeOfNextEvent)
                        {
                            UpdateTime(timeOfNextReaction - CurrentTime);

                            double r2 = rng.NextVariate();
                            double threshold = r2 * a0;
                            int mu = SelectReaction(threshold);

                            FireReaction(model.Reactions[mu]);
                        }
                        // No active reactions or next event comes first
                        // See if there's a pending event
                        else if (timeOfNextEvent < duration)
                        {
                            UpdateTime(timeOfNextEvent - CurrentTime);

                            var reactionList = priorityQueue[timeOfNextEvent];
                            foreach (Reaction r in reactionList)
                                foreach (Species p in r.Products)
                                    p.Increment();

                            priorityQueue.Remove(timeOfNextEvent);
                        }
                        // No active reactions and no pending events (before the end of the simulation)
                        else
                        {
                            UpdateTime(duration - CurrentTime);
                        }
                    }
                    break;
#endif

                case Mode.ExactMethod:
                    {
                        // Refactored version of Direct Method from
                        // Cai, X. 2007. Exact stochastic simulation of coupled chemical reactions with delays,
                        // The Journal of Chemical Physics 126, 124108 (2007)
                        double u2 = rng.GenerateUniformOO();
                        double a0 = UpdateAndSumRates(model.Reactions, currentRates);

                        if (priorityQueue.Count == 0)
                        {
                            if (a0 > 0)
                            {
                                double tau = (-Math.Log(u2) / a0);

                                if ((CurrentTime + tau) <= duration)
                                {
                                    UpdateTime(tau);

                                    double r2 = rng.GenerateUniformCC();
                                    double threshold = r2 * (double)a0;
                                    int mu = GetReactionIndex(currentRates, threshold);

                                    FireReaction(model.Reactions[mu]);
                                }
                                else
                                {
                                    UpdateTime(duration - CurrentTime);
                                }
                            }
                            else
                            {
                                UpdateTime(duration - CurrentTime);
                            }
                        }
                        else
                        {
                            double timeOfNextEvent = GetTimeOfNextEvent();
                            double eventOffset = 0.0;
                            double aSubt = a0 * (timeOfNextEvent - CurrentTime);
                            double F = (1 - Math.Exp(-aSubt));

                            while (F < u2)
                            {
                                if (timeOfNextEvent <= duration)
                                {
                                    UpdateTime(timeOfNextEvent - CurrentTime);

                                    eventOffset = aSubt;
                                    ExecuteNextEvent();
                                    a0 = UpdateAndSumRates(model.Reactions, currentRates);

                                    if ((priorityQueue.Count > 0) && ((timeOfNextEvent = GetTimeOfNextEvent()) <= duration))
                                    {
                                        aSubt += a0 * (timeOfNextEvent - CurrentTime);
                                        F = (1 - Math.Exp(-aSubt));
                                    }
                                    else
                                    {
                                        // There's no next event to consider (timeOfNextEvent = infinity)
                                        // timeOfNextEvent = infinity
                                        // aSubt += a0 * infinity, which is infinity
                                        // F = 1 - exp(-aSubt), which is 1.0 and breaks us out of the while loop
                                        F = 1.0;
                                    }
                                }
                                else
                                {
                                    // The next event is past the end of the simulation
                                    F = 1.0;
                                }
                            }

                            double tau = -((Math.Log(u2) + eventOffset) / a0);

                            if ((CurrentTime + tau) <= duration)
                            {
                                UpdateTime(tau);

                                double r2 = rng.GenerateUniformCC();
                                double threshold = r2 * a0;
                                int mu = GetReactionIndex(currentRates, threshold);

                                FireReaction(model.Reactions[mu]);
                            }
                            else
                            {
                                UpdateTime(duration - CurrentTime);
                            }
                        }
                    }
                    break;

                default:
                    throw new ApplicationException(string.Format("Unknown hybrid/SSA solver run mode: {0}", runMode));
            }
        }

        protected override double CalculateProposedTau(double tauLimit)
        {
            double actualTau = tauLimit;

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

        double GetTimeOfNextEvent()
        {
            return priorityQueue.First().Key;
        }

        void ExecuteNextEvent()
        {
            IList<Reaction> reactionList = GetNextEvent();
            foreach (Reaction r in reactionList)
                foreach (Species p in r.Products)
                    p.Increment();

            priorityQueue.Remove(GetTimeOfNextEvent());
        }

        IList<Reaction> GetNextEvent()
        {
            return priorityQueue.First().Value;
        }

        protected void UpdateTime(double delta)
        {
            CurrentTime += delta;
            SamplingParams = trajectories.RecordObservables(model.Observables, SamplingParams, CurrentTime, duration);
        }

        protected override void FireReaction(Reaction reaction)
        {
            foreach (Species s in reaction.Reactants)
                s.Decrement();

            reactionsFiredInCurrentRealization++;

            double delay = reaction.HasDelay ? reaction.Delay : 0.0;

            if (delay > 0.0)
            {
                // Put products on queue
                IList<Reaction> reactionList;
                double time = CurrentTime + delay;

                if (!priorityQueue.TryGetValue(time, out reactionList))
                {
                    reactionList = new List<Reaction> {reaction};
                    priorityQueue.Add(time, reactionList);
                }
                else
                {
                    reactionList.Add(reaction);
                }
            }
            else
            {
                foreach (Species p in reaction.Products)
                    p.Increment();
            }
        }

        protected override void FireReaction(Reaction reaction, int delta)
        {
            throw new ApplicationException("HybridSSA doesn't implement FireReaction(reaction, delta).");
        }

        public override string ToString()
        {
            return "Hybrid SSA (SSA + event queues)";
        }
    }
}
