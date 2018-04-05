// ReSharper disable InconsistentNaming
using distlib.randomvariates;

namespace distlib
{
    public interface DistributionSampler
    {
        RandomVariateGenerator VariateGenerator { get; }
        double GenerateUniformOO();
        double GenerateUniformOO(double min, double max);
        double GenerateUniformOC();
        double GenerateUniformOC(double min, double max);
        double GenerateUniformCO();
        double GenerateUniformCO(double min, double max);
        double GenerateUniformCC();
        double GenerateUniformCC(double min, double max);
        double StandardNormal();
        double GenerateNormal(double mean, double variance);
        int GeneratePoisson(double mu);
        double StandardExponential();
        double GenerateExponential(double mean);
        double StandardGamma(double shape);
        double GenerateGamma(double shape, double scale);
        int GenerateBinomial(int trials, double probabilitySuccess);
        void GenerateMultinomial(int totalEvents, double[] probabilityVector, int[] events);
    }
}
// ReSharper restore InconsistentNaming
