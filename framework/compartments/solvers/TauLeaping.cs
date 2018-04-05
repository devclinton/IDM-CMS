using System;
using System.Collections.Generic;
using System.Linq;
using compartments.emod;
using compartments.solvers.solverbase;
using distlib;
using distlib.samplers;

namespace compartments.solvers
{
    public class TauLeaping : SolverBase
    {
        private enum Regime
        {
            SSA,
            NonCritical,
            Critical,
            Leaping
        };

        private readonly double[] _currentRates;
        private double[,] _jacobianMatrix;
        private readonly double[] _criticalRates;
        private readonly double[] _noncriticalRates;

        // ReSharper disable InconsistentNaming
        protected double epsilon;
        protected int nc;
        protected int multiple;
        protected int SSAruns;
        private Regime _regime = Regime.Leaping;
        private int _remainingSSAsteps;
        private Reaction _ssaReaction;
        private double _leapTau;
        private List<Reaction> _nonCriticalReactions;
        private List<Reaction> _criticalReactions;
        private double _a0Critical;
        private DistributionSampler _distributionSampler;
        // ReSharper restore InconsistentNaming

        // The algorithm presented here (specifically in regards to the critical
        // versus noncritical reactions to avoid negative species) can be found in
        // 'Avoiding Negative Populations in Explicit Poisson Tau-Leaping'
        // Cao, Gillespie, Petzold.  Journal of Chemical Physics.

        public TauLeaping(ModelInfo modelInfo, double duration, int repeats, int samples,ModelBuilder modelBuilder = null)
            : base(modelInfo, duration, repeats, samples, modelBuilder)
        {
            Configuration config = Configuration.CurrentConfiguration;

            _currentRates = new double[model.Reactions.Count];
            _jacobianMatrix = new double[model.Reactions.Count, model.Species.Count];

            _nonCriticalReactions = new List<Reaction>() ;
            _criticalReactions = new List<Reaction>();

            // We'll need at most reactions.Count values in one or the other array
            _criticalRates = new double[model.Reactions.Count];
            _noncriticalRates = new double[model.Reactions.Count];

            epsilon = config.GetParameterWithDefault("tau-leaping.epsilon", 0.001);
            nc = config.GetParameterWithDefault("tau-leaping.Nc", 2);
            multiple = config.GetParameterWithDefault("tau-leaping.Multiple", 10);
            SSAruns = config.GetParameterWithDefault("tau-leaping.SSARuns", 100);

            CheckParameters();

            _distributionSampler = RandLibSampler.CreateRandLibSampler(rng);

            Console.WriteLine("epsilon =  {0}", epsilon);
            Console.WriteLine("nc =       {0}", nc);
            Console.WriteLine("multiple = {0}", multiple);
            Console.WriteLine("SSA runs = {0}", SSAruns);
        }

        private void CheckParameters()
        {
            if (epsilon <= 0)
                throw new ApplicationException("Epsilon was set to less than or equal to zero.");

            if (nc <= 0)
                throw new ApplicationException("Nc was et to less than zero or equal to zero.");

            if (multiple <= 0)
                throw new ApplicationException("Multiple was set to less than or equal to zero.");

            if (SSAruns < 1)
                throw new ApplicationException("SSAruns was set to less than one.");
        }

        protected override double CalculateProposedTau(double tauLimit)
        {
            double actualTau = tauLimit;

            double a0 = UpdateAndSumRates(model.Reactions, _currentRates);

            if (_regime != Regime.SSA)
            {
                // Calculate proposed leap
                if (a0 > 0.0)
                {
                    // Find Critical and NonCritical Reactions

                    _criticalReactions.Clear();
                    _nonCriticalReactions.Clear();

                    ComputeL(out _criticalReactions, out _nonCriticalReactions, nc);

                    //  Decide Which Tau prime is going to be used.

                    double tauPrime = double.PositiveInfinity;

                    if (_nonCriticalReactions.Count > 0)
                    {
                        // Compute Jacobian
                        _jacobianMatrix = ComputeJacobian();

                        tauPrime = ComputeTau(_jacobianMatrix, epsilon, a0, _nonCriticalReactions);
                    }
                    // Decide if SSA should be run a number of times

                    if (tauPrime < (multiple * 1 / a0))           //Parameter Multiple is arbitrary can be changed
                    {
                        _regime = Regime.SSA;
                        _remainingSSAsteps = SSAruns;
                        double proposedTau = CurrentTime + GillespieTau(a0);
                        _ssaReaction = proposedTau < tauLimit ? GillespieReaction(a0) : null;
                        actualTau = Math.Min(proposedTau, tauLimit);
                    }
                    else // leap
                    {
                        _a0Critical = UpdateAndSumRates(_criticalReactions, _criticalRates);

                        // Generate Tau'' = taudoubleprime.
                        double r1 = rng.GenerateUniformOO();
                        double tauDoublePrime = Math.Log(1.0 / r1) / _a0Critical;

                        _regime = Regime.NonCritical;

                        // Decide if tauprime versus taudoubleprime should be used for Taumin
                        if (tauPrime < tauDoublePrime)
                        {
                            actualTau = Math.Min(CurrentTime + tauPrime, tauLimit);
                        }
                        else // taudoubleprime < tauPrime
                        {
                            if (tauDoublePrime < tauLimit)
                            {
                                _regime = Regime.Critical;
                            }
                            actualTau = Math.Min(CurrentTime + tauDoublePrime, tauLimit);
                        }

                        _leapTau = actualTau - CurrentTime;

                        // Whether or not we do a critical reaction, we need to update the non-critical rates.
                        UpdateAndSumRates(_nonCriticalReactions, _noncriticalRates);
                    }
                }
                else
                {
                    // All the rates are zero (0), nothing will change from here on out.
                    CurrentTime = duration;
                    SamplingParams = trajectories.RecordObservables(model.Observables, SamplingParams, CurrentTime, duration);
                }
            }
            else // SSA mode
            {
                // Calculate proposed tau based on SSA
                if (a0 > 0.0)
                {
                    double proposedTau = CurrentTime + GillespieTau(a0);
                    _ssaReaction = proposedTau < tauLimit ? GillespieReaction(a0) : null;
                    actualTau = Math.Min(proposedTau, tauLimit);
                }
                else
                {
                    // All the rates are zero (0), nothing will change from here on out.
                    CurrentTime = duration;
                    SamplingParams = trajectories.RecordObservables(model.Observables, SamplingParams, CurrentTime, duration);
                }
            }

            return actualTau;
        }

        protected override void ExecuteReactions()
        {
            switch (_regime)
            {
                case Regime.SSA:
                    if (_ssaReaction != null)
                    {
                        FireReaction(_ssaReaction);
                        _ssaReaction = null;
                        if (--_remainingSSAsteps == 0)
                        {
                            _regime = Regime.Leaping;
                        }
                    }
                    break;

                case Regime.NonCritical:
                    // Update Noncritical Reactions
                    FireNonCriticalReactions(_leapTau, _nonCriticalReactions, _noncriticalRates);
                    _regime = Regime.Leaping;
                    break;

                case Regime.Critical:
                    //Find and Execute Critical Reaction
                    FireCriticalReaction(_criticalReactions, _criticalRates, _a0Critical);

                    // Update Noncritical Reactions
                    if (_nonCriticalReactions != null)
                    {
                        FireNonCriticalReactions(_leapTau, _nonCriticalReactions, _noncriticalRates);
                    }
                    
                    _regime = Regime.Leaping;
                    break;

                default:
                    throw new ApplicationException("Bad solver mode.");
            }
        }

        protected double[,] ComputeJacobian()
        {
            var jacobian        = new double[model.Reactions.Count, model.Species.Count];
            double[] actualrates = _currentRates;
            var perturbedrates  = new double[model.Reactions.Count];
            int iSpecies        = 0;

            foreach (Species s in model.Species)
            {
                s.Increment();    // What about if number of species is zero?

                int index = 0;
                foreach (Reaction r in model.Reactions)
                {
                    double av = r.Rate;
                    perturbedrates[index++] = av;
                }

                for (int i = 0; i < model.Reactions.Count; i++)
                {
                    // Simple Derivative Calculation with a single increment of species
                    jacobian[i, iSpecies] = (perturbedrates[i] - actualrates[i]) / 1;
                }

                iSpecies++; 

                s.Decrement();
            }

            return jacobian;
        }

        protected double ComputeTau(double[,] jacobian, double epsilonTau, double a0, List<Reaction> subreactions)
        {
            var f = new double[model.Reactions.Count, subreactions.Count];
            var possibleTauvalues = new double[model.Reactions.Count];
            var possibleTauvalues2 = new double[model.Reactions.Count];

            // Here we compute Eta. This section of code needs to be moved outside
            // of the SolveOnce() loop.  
            for (int k = 0; k < subreactions.Count; k++)
            {
                for (int i = 0; i < model.Reactions.Count; i++)
                {
                    for (int j = 0; j < model.Species.Count; j++)
                    {
                        int reactionvec = -subreactions[k].Reactants.Count(s => s == model.Species[j]);

                        reactionvec += subreactions[k].Products.Count(s => s == model.Species[j]);

                        f[i, k] += jacobian[i, j] * reactionvec;        //Simple Derivative Calculation with a single increment of species
                    }
                }
            }

            // Here we compute the Tauvector
            for (int iReaction = 0; iReaction < model.Reactions.Count; iReaction++)
            {
                double tempsum = 0;
                double tempsum2 = 0;

                for (int jReaction = 0; jReaction < subreactions.Count; jReaction++)
                {

                    tempsum += f[iReaction, jReaction] * subreactions[jReaction].Rate;
                    tempsum2 += Math.Pow(f[iReaction, jReaction], 2.0) * subreactions[jReaction].Rate;
                }

                possibleTauvalues[iReaction] = epsilonTau * a0 / Math.Abs(tempsum);
                possibleTauvalues2[iReaction] = Math.Pow(epsilonTau,  2.0) * Math.Pow(a0 , 2.0) / tempsum2;
            }

            double tauMin = Double.MaxValue;

            tauMin = Math.Min(tauMin, possibleTauvalues.Min());
            tauMin = Math.Min(tauMin, possibleTauvalues2.Min());

            return tauMin;
        }

        protected virtual void FireNonCriticalReactions(double tauMin, List<Reaction> subreactions, double[] noncritrates)
        {
            for (int jReaction = 0; jReaction < subreactions.Count; jReaction++)
            {
                double change        = noncritrates[jReaction]*tauMin;
                int howManyReactions = _distributionSampler.GeneratePoisson(change);

                FireReaction(subreactions[jReaction], howManyReactions);
            }
        }

        protected void ComputeL(out List<Reaction> criticalReactions, out List<Reaction> nonCriticalReactions, int ncTau)
        {
// ReSharper disable InconsistentNaming
            var L = new double[model.Reactions.Count];
// ReSharper restore InconsistentNaming
            criticalReactions = new List<Reaction>();
            nonCriticalReactions = new List<Reaction>();

            // Compute L for each reaction
            for (int jReaction = 0; jReaction < model.Reactions.Count; jReaction++)
            {
                double tempmin = 0;

                foreach (Species s in model.Reactions[jReaction].Reactants)   //Go through each of the reactants of reaction j
                {
                    int index = -model.Reactions[jReaction].Reactants.Count(species => species.Name == s.Name);

                    index += model.Reactions[jReaction].Products.Count(species => species.Name == s.Name);

                    if (index < 0)                              // Find the smallest x/v
                    {
                        double tempminproposed = (double)s.Count / Math.Abs(index);

                        if ((Math.Abs(tempmin) <= Single.Epsilon) || (tempmin > tempminproposed))
                        {
                            tempmin = Math.Floor(tempminproposed);
                        }
                    }
                }

                var tempmin2 = tempmin;

                L[jReaction] = tempmin2;

// ReSharper disable CompareOfFloatsByEqualityOperator
                if ((L[jReaction] < ncTau) && (L[jReaction] != 0))
// ReSharper restore CompareOfFloatsByEqualityOperator
                {
                    criticalReactions.Add(model.Reactions[jReaction]);
                }
                else
                {
                    nonCriticalReactions.Add(model.Reactions[jReaction]);
                }
            }
        }

        protected double GillespieTau(double a0)
        {
            double r1 = rng.GenerateUniformOO();
            double tau = Math.Log(1.0 / r1) / a0;

            return tau;
        }

        protected Reaction GillespieReaction(double a0)
        {
            double r2 = rng.GenerateUniformOC();
            double threshold = r2 * (double)a0;
            int mu = GetReactionIndex(_currentRates, threshold);
            Reaction reaction = model.Reactions[mu];

            return reaction;
        }

        protected void FireCriticalReaction(List<Reaction> criticalReactions, double[] criticalRates, double a0Critical)
        {
            double r2 = rng.GenerateUniformOC();
            double threshold = r2 * a0Critical;
            int mu = GetReactionIndex(criticalRates, threshold);

            FireReaction(criticalReactions[mu]);
        }

        public override string ToString()
        {
            return "Tau-Leaping";
        }
    }
}
