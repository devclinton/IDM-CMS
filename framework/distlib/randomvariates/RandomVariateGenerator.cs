// ReSharper disable InconsistentNaming
namespace distlib.randomvariates
{
    public interface RandomVariateGenerator
    {
        double GenerateUniformOO();
        double GenerateUniformOC();
        double GenerateUniformCO();
        double GenerateUniformCC();
    }
}
// ReSharper restore InconsistentNaming
