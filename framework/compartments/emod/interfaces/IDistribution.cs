using distlib.randomvariates;

namespace compartments.emod.interfaces
{
    interface IDistribution
    {
        float pdf();
        float pdf(float x);
        float cdf();
        float cdf(float x);
        void SetRNG(RandomVariateGenerator rng);
    }
}
