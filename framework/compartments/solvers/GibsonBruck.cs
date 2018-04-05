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
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace compartments.solvers
{
    public class GibsonBruck : SolverBase
    {
        private double _a0;
        private readonly Dictionary<Reaction, double> _currentRates;
        private readonly Dictionary<Reaction, double> _currentTaus;
        private readonly Dictionary<Reaction, List<Reaction>> _dependencyGraph;
        private readonly PriorityQueue<Reaction> _pq;
        private Reaction _nextReaction;

        public GibsonBruck(ModelInfo modelInfo, double duration, int repeats, int samples) : base(modelInfo, duration, repeats, samples)
        {
            _pq = new PriorityQueue<Reaction>(model.Reactions.Count);
            _a0 = 0.0;

            _currentRates = new Dictionary<Reaction, double>(model.Reactions.Count);
            _currentTaus  = new Dictionary<Reaction, double>(model.Reactions.Count);
            _nextReaction = null;

            foreach (Reaction r in model.Reactions)
            {
                _currentRates.Add(r, 0.0);
                _currentTaus.Add(r, 0.0);
            }

            // Create Dependency Graph outside of solver loop
            _dependencyGraph = CreateDependencyGraph();
        }

        protected override void StartRealization()
        {
            base.StartRealization();

            // Update the rates once!  This will initialize the current rates
            _a0 = InitializeReactionRatesAndTaus(model.Reactions, _currentRates, _currentTaus);

            _pq.Clear();
            foreach (Reaction r in model.Reactions)
            {
                _pq.Add(_currentTaus[r], r);
            }
        }

        private double InitializeReactionRatesAndTaus(IEnumerable<Reaction> reactions, IDictionary<Reaction, double> reactionRates, IDictionary<Reaction, double> currentTaus)
        {
            double a0 = 0.0;

            foreach (Reaction r in reactions)
            {
                double aj = r.Rate;

                if (double.IsNaN(aj))
                {
                    var message = $"Reaction propensity evaluated to NaN ('{r.Name}')";
                    Console.Error.WriteLine(message);
                    throw new ApplicationException(message);
                }

                if (aj < 0)
                {
                    var message = $"Reaction propensity evaluated to negative ('{r.Name}')";
                    Console.Error.WriteLine(message);
                    throw new ApplicationException(message);
                }

                if (double.IsInfinity(aj))
                {
                    var message = $"Reaction propensity evaluated to infinity ('{r.Name}')";
                    Console.Error.WriteLine(message);
                    throw new ApplicationException(message);
                }

                a0 += aj;
                reactionRates[r] = aj;

                double tau;
                if (aj > 0.0)
                {
                    double u1 = rng.GenerateUniformOO();
                    tau = Math.Log(1.0 / u1) / aj;
                }
                else
                {
                    tau = double.PositiveInfinity;
                }

                currentTaus[r] = tau;
            }

            return a0;
        }

        protected override double CalculateProposedTau(double tauLimit)
        {
            double actualTau = tauLimit;

            if (_a0 > 0.0)
            {
                double nextTau;

                _pq.Top(out nextTau, out _nextReaction);

                if (nextTau < actualTau)
                {
                    actualTau = nextTau;
                }
                else
                {
                    _nextReaction = null;
                }
            }

            return actualTau;
        }

        protected override void ExecuteReactions()
        {
            if (_nextReaction != null)
            {
                _a0 -= _currentRates[_nextReaction];
                FireReaction(_nextReaction);
                double aj = _nextReaction.Rate;
                _a0 += aj;
                _currentRates[_nextReaction] = aj;

                double newTau;

                if (aj == 0.0)
                {
                    newTau = double.PositiveInfinity;
                }
                else
                {
                    double u1 = rng.GenerateUniformOO();
                    newTau = Math.Log(1.0 / u1) / aj + CurrentTime;
                }

                _currentTaus[_nextReaction] = newTau;
                _pq.UpdateIndex(newTau, _nextReaction);

                // Update the dependent reactions without generating new random numbers. Could put this in a method.

                foreach (Reaction react in _dependencyGraph[_nextReaction])
                {
                    double ajOld = _currentRates[react];
                    _a0 -= ajOld;

                    double ajNew = react.Rate;
                    _a0 += ajNew;
                    _currentRates[react] = ajNew;

                    double updatedTau;

                    if (ajNew == 0.0)
                    {
                        updatedTau = double.PositiveInfinity;
                    }
                    else if (ajOld == 0.0)
                    {
                        double u1 = rng.GenerateUniformOO();
                        updatedTau = Math.Log(1.0 / u1) / ajNew + CurrentTime;
                    }
                    else
                    {
                        double currentTau = _currentTaus[react];
                        updatedTau = ajOld / ajNew * (currentTau - CurrentTime) + CurrentTime;
                    }

                    _currentTaus[react] = updatedTau;
                    _pq.UpdateIndex(updatedTau, react);
                }

                _nextReaction = null;
            }
        }

        protected Dictionary<Reaction, List<Reaction>> CreateDependencyGraph()
        {
            // Create a List of Lists for the Dependency Graph

            var graph = new Dictionary<Reaction, List<Reaction>>();

            // For each reaction r find every other reaction affected if r fires
            foreach (Reaction r in model.Reactions)
            {
                var sublist = new List<Reaction>();

                //List<Species> affects = new List<Species>();

                //foreach (Species s in r.Reactants)
                //{
                //    affects.Add(s);
                //}

                //Construct the Affects() subset

                List<Species> affects = r.Reactants.ToList();

                foreach (Species s in r.Products)
                {
                    if (affects.Exists(item => item == s))
                    {
                        affects.Remove(s);
                    }
                    else 
                    {
                        affects.Add(s);
                    }     
                }

                //Need to add extra dependencies?  Like Time?  or something in the rate 
                //expression?  Here would be the point to add that functionality.

                //Now search through all of the other reactions to see if they are affected.

                foreach (Reaction r2 in model.Reactions)
                {
                    if (r != r2)
                    {
                        //List<Species> dependsOn = new List<Species>();

                        //foreach (Species s in r2.Reactants)
                        //{
                        //    dependsOn.Add(s);
                        //}

                        List<Species> dependsOn = r2.Reactants.ToList();
 
                        foreach (Species s in affects)
                        {
                            if (dependsOn.Exists(item => item == s))
                            {
                                sublist.Add(r2);
                                break;
                            }
                        }
                    }
                }

                graph.Add(r,sublist);
            }

            return graph; 
        }

        public override string ToString()
        {
            return "Gibson-Bruck (Next Reaction)";
        }
    }
}
