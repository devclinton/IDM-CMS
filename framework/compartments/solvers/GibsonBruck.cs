using System;
using System.Collections.Generic;
using System.Linq;
using compartments.emod;
using compartments.solvers.solverbase;

namespace compartments.solvers
{
    public class GibsonBruck : SolverBase
    {
        private float _a0;
        private readonly Dictionary<Reaction, float> _currentRates;
        private readonly Dictionary<Reaction, float> _currentTaus;
        private readonly Dictionary<Reaction, List<Reaction>> _dependencyGraph = new Dictionary<Reaction, List<Reaction>>();
        private readonly PriorityQueue<Reaction> _pq;
        private Reaction _nextReaction;

        public GibsonBruck(ModelInfo modelInfo, float duration, int repeats, int samples) : base(modelInfo, duration, repeats, samples)
        {
            _pq = new PriorityQueue<Reaction>(model.Reactions.Count);
            _a0 = 0.0f;

            _currentRates = new Dictionary<Reaction, float>(model.Reactions.Count);
            _currentTaus  = new Dictionary<Reaction, float>(model.Reactions.Count);
            _nextReaction = null;

            foreach (Reaction r in model.Reactions)
            {
                _currentRates.Add(r, 0.0f);
                _currentTaus.Add(r, 0.0f);
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

        private float InitializeReactionRatesAndTaus(IEnumerable<Reaction> reactions, IDictionary<Reaction, float> reactionRates, IDictionary<Reaction, float> currentTaus)
        {
            float a0 = 0.0f;

            foreach (Reaction r in reactions)
            {
                float aj = r.Rate;
                a0 += aj;
                reactionRates[r] = aj;

                float tau;
                if (aj > 0.0f)
                {
                    float u1 = rng.GenerateUniformOO();
                    tau = (float)Math.Log(1.0f / u1) / aj;
                }
                else
                {
                    tau = float.PositiveInfinity;
                }

                currentTaus[r] = tau;
            }

            return a0;
        }

        protected override float CalculateProposedTau(float tauLimit)
        {
            float actualTau = tauLimit;

            if (_a0 > 0.0f)
            {
                float nextTau;

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
                float aj = _nextReaction.Rate;
                _a0 += aj;
                _currentRates[_nextReaction] = aj;

                float newTau;

                if (aj == 0.0f)
                {
                    newTau = float.PositiveInfinity;
                }
                else
                {
                    float u1 = rng.GenerateUniformOO();
                    newTau = (float)Math.Log(1.0f / u1) / aj + CurrentTime;
                }

                _currentTaus[_nextReaction] = newTau;
                _pq.UpdateIndex(newTau, _nextReaction);

                // Update the dependent reactions without generating new random numbers. Could put this in a method.

                foreach (Reaction react in _dependencyGraph[_nextReaction])
                {
                    float ajOld = _currentRates[react];
                    _a0 -= ajOld;

                    float ajNew = react.Rate;
                    _a0 += ajNew;
                    _currentRates[react] = ajNew;

                    float updatedTau;

                    if (ajNew == 0.0f)
                    {
                        updatedTau = float.PositiveInfinity;
                    }
                    else if (ajOld == 0.0f)
                    {
                        float u1 = rng.GenerateUniformOO();
                        updatedTau = (float)Math.Log(1.0f / u1) / ajNew + CurrentTime;
                    }
                    else
                    {
                        float currentTau = _currentTaus[react];
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
