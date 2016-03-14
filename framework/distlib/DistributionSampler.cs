// ReSharper disable InconsistentNaming
using distlib.randomvariates;

namespace distlib
{
    public interface DistributionSampler
    {
        RandomVariateGenerator VariateGenerator { get; }
        float GenerateUniformOO();
        float GenerateUniformOO(float min, float max);
        float GenerateUniformOC();
        float GenerateUniformOC(float min, float max);
        float GenerateUniformCO();
        float GenerateUniformCO(float min, float max);
        float GenerateUniformCC();
        float GenerateUniformCC(float min, float max);
        float StandardNormal();
        float GenerateNormal(float mean, float variance);
        int GeneratePoisson(float mu);
        float StandardExponential();
        float GenerateExponential(float mean);
        float StandardGamma(float shape);
        float GenerateGamma(float shape, float scale);
        int GenerateBinomial(int trials, float probabilitySuccess);
        void GenerateMultinomial(int totalEvents, float[] probabilityVector, int[] events);
    }
}
// ReSharper restore InconsistentNaming
