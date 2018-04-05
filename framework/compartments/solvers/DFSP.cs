using System;
using compartments.emod;

namespace compartments.solvers
{
    public class DFSP : TransportSSA
    {
        /*************************************************************************************************
         (d/dt) p = A p    ---->  p(t) = exp(A)*p(0) = M(L,G(t)) where G(0) = p(0) is a Multinomial
         and the Lth element of p(0) = 1, i.e. the initial distribution p(0) is a delta function - 
         which is a type of Multinomial distribution - and the entire mass is located in the Lth element.
         A represents the master equation matrix.  
          
         DFSP : TransportSSA : SolverBase 
         **************************************************************************************************/

        protected double[][][] dictionary;     // a set of cumulative distribution functions
                                               // (number of nodes) x (possible node values) x (number of neighbors+1)
                                               // jagged array since (number of neighbors+1) varies depending on the node index
        protected int          UMax;

        private const int MaximumValueForMultinomial = 150;
        private const int DefaultValueForU = 120;

        public DFSP(ModelInfo modelInfo, double duration, int repeats, int samples)
               : base(modelInfo, duration, repeats, samples)
        {
            // SolverBase constructor has been executed at this point
            // ISSA constructor has been executed at this point

            dictionary = new double[model.Species.Count][][];

            Configuration config = Configuration.CurrentConfiguration;
            UMax = Math.Min(config.GetParameterWithDefault("dfsp.umax", DefaultValueForU), MaximumValueForMultinomial);
            verbose = config.GetParameterWithDefault("dfsp.verbose", false);

            for (int iSpecies = 0; iSpecies < model.Species.Count; ++iSpecies)
            {
                dictionary[iSpecies] = new double[UMax][];
            }

            for (int iSpecies = 0; iSpecies < model.Species.Count; ++iSpecies)
            {
                for (int u = 0; u < UMax; ++u)
                {
                    dictionary[iSpecies][u] = new double[kernels[iSpecies].Length];
                }
            }

            ComputeCumulativeDistributionFunctions();
            IntegrateAndNormalize();
        }

        private void ComputeCumulativeDistributionFunctions()
        {
            for (int iSpecies = 0; iSpecies < model.Species.Count; ++iSpecies)
            {
                double[] p           = kernels[iSpecies];
                int numberOfNodes   = kernels[iSpecies].Length;
                
                for (int u = 1; u <= UMax; ++u)
                {
                    for (int z = 0; z < numberOfNodes; ++z)
                    {
                        // x denotes a possible state in the finite state projection 
                        var x     = new int[numberOfNodes];
                        x[0]      = u - 1;                    // sic
                        x[z]      += 1;

                        // evaluate the probability of the state
                        double probability = EvaluateMultinomial(u, p, x);

                        // save the state & its probability in a lookup table 
                        dictionary[iSpecies][u-1][z] = probability;
                    }
                }
            }
        }

        private void IntegrateAndNormalize()
        {
            for (int iSpecies = 0; iSpecies < model.Species.Count; ++iSpecies)
            {
                for (int u = 0; u < UMax; ++u)
                {
                    int numberOfNodes = dictionary[iSpecies][u].Length;

                    // cumulative distributions in place
                    // z = 1 sic
                    for (int z = 1; z < numberOfNodes; ++z)
                    {
                        dictionary[iSpecies][u][z] += dictionary[iSpecies][u][z-1];
                    }

                    for (int z = 0; z < numberOfNodes; ++z)
                    {
                        dictionary[iSpecies][u][z] /= dictionary[iSpecies][u][numberOfNodes-1];
                    }

                }
            }
        }

        // the cumulative distribution function (cdf) is already essentially sorted in that the first 
        // element - which represents the current node - has the largest probability
        protected int InvertCumulativeDistribution(double uniformRandomNumber, double[] cdf)
        {
            int mu = 0;

            for (int i = 0; i < cdf.Length; i++)
            {
                if (cdf[i] >= uniformRandomNumber)
                {
                    mu = i;
                    break;
                }
            }

            return mu;
        }

        // explicitly hides TransportSSA.ApplyLambda (the function in the parent class)
        protected override void ApplyLambda()
        {
            for (int iSpecies = 0; iSpecies < model.Species.Count; ++iSpecies)
            {
                lambda[iSpecies] = 0;
            }

            for (int iSpecies = 0; iSpecies < model.Species.Count; ++iSpecies)
            {
                // skip the node if there is nothing to do
                if (model.Species[iSpecies].Count == 0)
                {
                    continue;
                }

                int currentCount = model.Species[iSpecies].Count;

                while (currentCount > 0)
                {
                    int inputCount = Math.Min(currentCount, UMax);

                    double uniformRandomNumber = rng.GenerateUniformOO();
                    int mu = InvertCumulativeDistribution(uniformRandomNumber, dictionary[iSpecies][inputCount - 1]);

                    lambda[speciesIndices[iSpecies][0]]     -= 1;
                    lambda[speciesIndices[iSpecies][mu]]    += 1;

                    currentCount -= inputCount;
                }

            }

            for (int iSpecies = 0; iSpecies < model.Species.Count; ++iSpecies)
            {
                model.Species[iSpecies].Count += lambda[iSpecies];
            }
        }

        // recursion causes a stack overflow exception, so iteratively compute the factorial
        protected double Factorial(int n)
        {
            double f = 1.0;
            for (int i = 2; i <= n; ++i)
            {
                f *= i;
            }

            return f;
        }

        // Using double precision for arithmetic, then typecast the result - which is in [0,1] - to a double
        protected double EvaluateMultinomial(int L, double[] p, int[] x)
        {
            double numerator = 1.0;
            for (int i = 0; i < x.Length; ++i)
            {
                numerator *= Math.Pow(p[i], x[i]);
            }

            double denominator = 1.0;
            for (int i = 0; i < x.Length; ++i)
            {
                denominator *= Factorial( x[i] );
            }

            double probability = Factorial(L) * numerator / denominator;

            return (probability);
        }

        public override string ToString()
        {
            return "DFSP";
        }
    }
}
