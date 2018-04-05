using System;
using compartments.emod;
using distlib;

namespace compartments.solvers
{
    public class OptimalTransportSSA : DFSP
    {
        /*************************************************************************************************
         If the species.Count <= transportSSAThreshold, then run DFSP, otherwise run TransportSSA.  
          
         The optimal value is set such that the expected number of binomial samples (TransportSSA) ~ expected 
         number of uniform samples (DFSP). 
          
         OptimalTransportSSA : DFSP : TransportSSA : SolverBase
         **************************************************************************************************/

        private readonly int _transportSSAThreshold;

        public                  OptimalTransportSSA(ModelInfo modelInfo, double duration, int repeats, int samples)
                                : base(modelInfo, duration, repeats, samples)
        {
            // SolverBase constructor has been executed at this point
            // ISSA constructor has been executed at this point
            // DFSP constructor has been executed at this point

            Configuration config = Configuration.CurrentConfiguration;
            // OptimalTransportSSA constructor code
            int optimalThresholdDefault = (int)(graphDimension*UMax);
            _transportSSAThreshold      = config.GetParameterWithDefault("otssa.threshold", optimalThresholdDefault);
            verbose                     = config.GetParameterWithDefault("otssa.verbose", false);

            Console.WriteLine("Optimal Threshold: {0}", _transportSSAThreshold);

        }

        // explicitly hides DFSP.ApplyLambda (the function in the parent class), and 
        // DFSP.ApplyLambda hides TransportSSA.ApplyLambda
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

                if (model.Species[iSpecies].Count > _transportSSAThreshold) // large numbers -> Transport SSA
                {
                    var numberOfExecutions = new int[kernels[iSpecies].Length];

                    _distributionSampler.GenerateMultinomial(model.Species[iSpecies].Count, kernels[iSpecies], numberOfExecutions);

                    lambda[speciesIndices[iSpecies][0]] -= (model.Species[iSpecies].Count - numberOfExecutions[0]);

                    // z = 1 (sic), z = 0 current node; z = 1...n nieghbors
                    for (int z = 1; z < numberOfExecutions.Length; ++z)
                    {
                        lambda[speciesIndices[iSpecies][z]] += numberOfExecutions[z];
                    }
                }
                else // small numbers -> DFSP dictionary lookup 
                {
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

            }

            for (int iSpecies = 0; iSpecies < model.Species.Count; ++iSpecies)
            {
                model.Species[iSpecies].Increment(lambda[iSpecies]);
            }
        }

        public override string ToString()
        {
            return "OptimalTransportSSA";
        }
    }
}