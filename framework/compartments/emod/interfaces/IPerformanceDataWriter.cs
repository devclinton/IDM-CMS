using compartments.emod.perf;

namespace compartments.emod.interfaces
{
    public interface IPerformanceDataWriter
    {
        void WritePerformanceMeasurements(PerformanceMeasurements measurements,
                                          PerformanceMeasurementConfigurationParameters perfConfig);
    }
}
