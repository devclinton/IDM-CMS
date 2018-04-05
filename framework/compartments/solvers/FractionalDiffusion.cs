using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using compartments.emod;
using compartments.emod.utils;
using compartments.solvers.solverbase;
using distlib;
using distlib.samplers;

namespace compartments.solvers
{
    public class FractionalDiffusion : SolverBase
    {
        #region fields
        protected List<Reaction>    _reactions;                     // list of reactions, not including diffusion events
        protected double[]           _currentRates;                  // current propensities of reactions
        private readonly int[]      _field;                         // the field that is subject to fractional diffusion
        private readonly int        _n;                             // the number of nodes in field, i.e. the number of nodes in the domain
        private readonly double     _alpha;                         // order of the fractional derivative
        private readonly double     _Dalpha;                        // diffusion coefficient, with units distance^alpha / time
        private readonly double     _fourierNumber;                 // fourier number in the CFL condition
        private readonly double     _constant;                      // constant in the CFL condition
        private readonly double     _h;                             // physical distance between the _n nodes
        private readonly bool       _verbose;                       // flag for verbose output
        private readonly int        _numberOfNonDiscretizedSpecies; // number of species that are subject to fractional diffusion
        private readonly int        _truncationDistance;            // maximum distance to diffuse
        private double              _tau;                           // timestep
        private int[]               _dispersalChanges;              // changes to apply for the fractional diffusion operator
        private int[]               _executionsPerReaction;         // changes to apply for the reactions
        private DistributionSampler _distributionSampler;
        #endregion

        public FractionalDiffusion(ModelInfo modelInfo, double duration, int repeats, int samples)
            : base(modelInfo, duration, repeats, samples, new ModelBuilder())
        {
            Configuration config            = Configuration.CurrentConfiguration;

            #region initializeFields
            _reactions                      = new List<Reaction>(model.Reactions);
            _alpha                          = config.GetParameterWithDefault("fd.alpha", 0.5);
            _verbose                        = config.GetParameterWithDefault("fd.verbose", false);
            _Dalpha                         = config.GetParameterWithDefault("fd.Dalpha", 1.0);
            _constant                       = config.GetParameterWithDefault("fd.constant", 0.25);
            _h                              = config.GetParameterWithDefault("fd.h", 1.0);
            _truncationDistance             = config.GetParameterWithDefault("fd.truncation", (int)Math.Round(((double)(modelInfo.Locales.Count() - 1))/4.0));
            _fourierNumber                  = 0.5;
            _n                              = modelInfo.Locales.Count() - 1;             
            _currentRates                   = new double[_reactions.Count];
            _field                          = new int[_n];
            _numberOfNonDiscretizedSpecies  = modelInfo.Species.Count() / _n;
            _tau                            = TimeStep(_fourierNumber, _constant, _h, _alpha, _Dalpha);
            _dispersalChanges               = new int[model.Species.Count];
            _executionsPerReaction          = new int[model.Reactions.Count];
            _distributionSampler            = RandLibSampler.CreateRandLibSampler(rng);
            #endregion

            if (_verbose == true)
            {
                Console.WriteLine("number of locales = {0}", modelInfo.Locales.Count()-1);
                Console.WriteLine("number of species = {0}", _numberOfNonDiscretizedSpecies);
                Console.WriteLine("truncation = {0}", _truncationDistance);
                Console.WriteLine("alpha = {0}", _alpha);
                Console.WriteLine("D_alpha = {0}", _Dalpha);
                Console.WriteLine("Fouier number (1d) = {0}", _fourierNumber);
                Console.WriteLine("h = {0}", _h);
                Console.WriteLine("C = {0}", _constant);
                Console.WriteLine("fractional diffusion tau = {0}", _tau);
            }
        }

        #region SolverBaseMethods

        protected override double CalculateProposedTau(double tauLimit)
        {
            double tau = TimeStep(_fourierNumber, _constant, _h, _alpha, _Dalpha);

            if (CurrentTime + tau <= tauLimit)
            {
                _tau = tau;
            }
            else
            {
                _tau = tauLimit - CurrentTime;
            }

            if (_verbose == true)
            {
                Console.WriteLine("t = {0}", CurrentTime);
            }

            return Math.Min(CurrentTime + _tau, tauLimit);
        }
        protected override void ExecuteReactions()
        {
            for (int i = 0; i < model.Species.Count; ++i)
            {
                _dispersalChanges[i] = 0;
            }

            for (int j = 0; j < model.Reactions.Count; ++j)
            {
                _executionsPerReaction[j] = 0;
            }

            ApplyDiffusionOperator(_dispersalChanges, _field, _tau, _h, _alpha, _Dalpha);
            ApplyReactionOperator(_executionsPerReaction, _tau);
            ApplyOperatorChanges(_dispersalChanges, _executionsPerReaction);
        }    

        #endregion

        #region ApplyOperators
        protected void ApplyOperatorChanges(int[] dispersalChanges, int[] executionsPerReaction)
        {
            // diffusion
            for (int s = 0; s < _numberOfNonDiscretizedSpecies; ++s)
            {
                // place the dispersed values back in the species field 
                int speciesIndex = s;
                for (int j = 0; j < modelInfo.Locales.Count() - 1; ++j)
                {
                    model.Species.ElementAt(speciesIndex).Count += dispersalChanges[speciesIndex];
                    speciesIndex += _numberOfNonDiscretizedSpecies;
                }
            }

            // reactions
            for (int j = 0; j < modelInfo.Reactions.Count(); j++)
            {
                FireReaction(_reactions.ElementAt(j), executionsPerReaction[j]);
            }

            for (int k = 0; k < model.Species.Count; ++k)
            {
                if (model.Species.ElementAt(k).Count < 0)
                {
                    model.Species.ElementAt(k).Count = 0;
                }
            }
        }

        protected void ApplyDiffusionOperator(int[] dispersalChanges, int[] field, double tau, double h, double alpha, double Dalpha)
        {
            int speciesIndex;

            // levy flight for each species 
            for (int s = 0; s < _numberOfNonDiscretizedSpecies; ++s) 
            {
                // load the species information into the field
                speciesIndex = s;
                for (int j = 0; j < modelInfo.Locales.Count() - 1; ++j)
                {
                    _field[j] = model.Species.ElementAt(speciesIndex).Count;
                    speciesIndex += _numberOfNonDiscretizedSpecies;
                }

                // dispersal of the field
                DiffusionOperator(_field, tau, _h, _alpha, _Dalpha);

                // save the changes
                speciesIndex = s;
                for (int j = 0; j < modelInfo.Locales.Count() - 1; ++j)
                {
                    dispersalChanges[speciesIndex] =  _field[j] - model.Species.ElementAt(speciesIndex).Count;
                    speciesIndex += _numberOfNonDiscretizedSpecies;
                }
            }
        }

        protected void ApplyReactionOperator(int[] executionsPerReaction, double tau)
        {
            double a0 = UpdateAndSumRates(_reactions, _currentRates);

            for (int j = 0; j < modelInfo.Reactions.Count(); j++)
            {
                executionsPerReaction[j] = _distributionSampler.GeneratePoisson(_currentRates[j] * tau);
            }
        }
        #endregion

        // dy/dt = d^alpha y / d x^alpha, y is the field subject to fractional diffusion
        protected void DiffusionOperator(int[] y, double tau, double h, double alpha, double Dalpha)
        {
            var lambda = new int[y.Length];

            for (int i = 0; i < y.Length; ++i)
            {
                lambda[i] = 0;
            }

            for (int i = 0; i < y.Length; ++i)
            {
                // skip the node if there is nothing to do
                if (y[i] == 0)
                {
                    continue;
                }

                var numberOfExecutions  = new int[lambda.Length];
                var pr                  = new double[lambda.Length];

                // fill the probability vector
                for (int j = 0; j < pr.Length; ++j)
                {
                    if (Math.Abs(i - j) > _truncationDistance) // truncated levy flight
                    {
                        pr[j] = 0.0;
                    }
                    else
                    {
                        pr[j] = FractionalDiffusionKernel(tau, h, alpha, Dalpha, i, j);
                    }
                }

                Normalize(pr);

                _distributionSampler.GenerateMultinomial(y[i], pr, numberOfExecutions);

                // finite-precision error in the multinomial distribution (conditional binomial), nothing should occur when p = 0
                if ((numberOfExecutions[pr.Length - 1] != 0) && (pr[pr.Length - 1] == 0.0))
                {
                    numberOfExecutions[i] += numberOfExecutions[pr.Length - 1];
                    numberOfExecutions[pr.Length - 1] = 0;
                }

                lambda[i] -= (y[i] - numberOfExecutions[i]);

                for (int j = 0; j < numberOfExecutions.Length; ++j)
                {
                    if (j != i)
                    {
                        lambda[j] += numberOfExecutions[j];
                    }
                }
            }

            for (int i = 0; i < y.Length; ++i)
            {
                y[i] += lambda[i];
            }
        }

        protected void Normalize(double[] p)
        {
            double z = 0.0;
            for (int j = 0; j < p.Length; ++j)
            {
                z += p[j];
            }

            for (int j = 0; j < p.Length; ++j)
            {
                p[j] /= z;
            }
        }

        #region GammaFunction
        public double GammaPositive(double z)
        {
            return Math.Sqrt(2.0 * Math.PI / z) * Math.Pow((z / Math.Exp(1.0)) * Math.Sqrt(z * Math.Sinh(1.0 / z) + (1.0 / (810.0 * Math.Pow(z, 6.0)))), z);
        }

        public double GammaNegative(double z)
        {
            return (-Math.PI / (Math.Abs(z) * Math.Sin(Math.Abs(z) * Math.PI))) * (1.0 / GammaPositive(Math.Abs(z)));
        }

        public double Gamma(double z)
        {
            if (z >= 0.0)
            { return GammaPositive(z); }
            else
            { return GammaNegative(z); }
        }
        #endregion

        #region FractionalDiffusionKernels

        // Generalized Binomial Coefficient for real numbers
        protected double BinomialCoefficient(double n, double m)
        {
            return Gamma(n + 1.0) / (Gamma(m + 1.0) * Gamma(n - m + 1.0));
        }

        protected double Mu(double tau, double h, double alpha, double Dalpha)
        {
            return ((Dalpha * tau) / Math.Pow(h, alpha)) * (1.0 / (2.0 * Math.Abs(Math.Cos(alpha * Math.PI / 2.0))));
        }

        // alpha in (0, 1)
        protected double FractionalDiffusionKernel01(double tau, double h, double alpha, double Dalpha, int i, int j)
        {
            if (i == j)
            {
                return 1.0 - 2.0 * Mu(tau, h, alpha, Dalpha);
            }
            else
            {
                return Mu(tau, h, alpha, Dalpha) * Math.Abs(BinomialCoefficient(alpha, Math.Abs(i - j))); 
            }
        }

        // alpha in (1, 2]
        protected double FractionalDiffusionKernel12(double tau, double h, double alpha, double Dalpha, int i, int j)
        {
            if (i == j)
            {
                return 1.0 - 2.0 * Mu(tau, h, alpha, Dalpha) * alpha;
            }
            else if (Math.Abs(i - j) == 1)
            {
                return Mu(tau, h, alpha, Dalpha) * (1.0 + BinomialCoefficient(alpha, 2.0));
            }
            else
            {
                if (alpha == 2.0)
                {
                    return 0.0;
                }
                else
                {
                    return Mu(tau, h, alpha, Dalpha) * Math.Abs(BinomialCoefficient(alpha, 1.0 + Math.Abs(i - j)));
                }
            }
        }

        // alpha = 1
        protected double CauchyKernel(double tau, double h, double Dalpha, int i, int j)
        {
            double y1 = (Math.Abs(i - j) * h + (h / 2.0));
            double y2 = (Math.Abs(i - j) * h - (h / 2.0));
            double x = Dalpha*tau;

            return (1.0 / Math.PI) * (Math.Atan2(y1, x) - Math.Atan2(y2, x));
        }
        #endregion

        protected double TimeStep(double fourierNumber, double constant, double h, double alpha, double Dalpha)
        {
            // fourierNumber = 1/(2*d) in d dimensions, constant << 1

            if (alpha == 1.0)
            {
                return constant * (1.0 / Dalpha) * fourierNumber; // Dalpha * t < 1
            }
            else if (alpha < 1.0)
            {
                return constant * (1.0 / Dalpha) * fourierNumber * (2.0 * Math.Abs(Math.Cos(alpha * Math.PI / 2.0))) * Math.Pow(h, alpha);
            }
            else
            {
                return constant * (1.0 / Dalpha) * fourierNumber * (1.0 / alpha) * (2.0 * Math.Abs(Math.Cos(alpha * Math.PI / 2.0))) * Math.Pow(h, alpha);
            }
        }

        protected double FractionalDiffusionKernel(double tau, double h, double alpha, double Dalpha, int i, int j)
        {
            if (alpha == 1.0)
            {
                return CauchyKernel(tau, h, Dalpha, i, j);
            }
            else
            {
                if (alpha < 1.0)
                {
                    return FractionalDiffusionKernel01(tau, h, alpha, Dalpha, i, j);
                }
                else
                {
                    return FractionalDiffusionKernel12(tau, h, alpha, Dalpha, i, j);
                }
            }
        }
         
        public override string ToString()
        {
            return "Fractional Diffusion";
        }
    }
}
  