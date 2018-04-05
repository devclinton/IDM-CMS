using distlib.randomvariates;

namespace compartments.emod.interfaces
{
    interface IDistribution
    {
        double pdf();
        double pdf(double x);
        double cdf();
        double cdf(double x);
        void SetRNG(RandomVariateGenerator rng);
    }
}
