// ReSharper disable InconsistentNaming
namespace distlib.randomvariates
{
    public interface RandomVariateGenerator
    {
        float GenerateUniformOO();
        float GenerateUniformOC();
        float GenerateUniformCO();
        float GenerateUniformCC();
    }
}
// ReSharper restore InconsistentNaming
