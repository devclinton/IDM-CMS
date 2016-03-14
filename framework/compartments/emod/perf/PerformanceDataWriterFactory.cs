using System;
using compartments.emod.interfaces;

namespace compartments.emod.perf
{
    class PerformanceDataWriterFactory
    {
        public static IPerformanceDataWriter GetDataWriter(PerformanceMeasurementConfigurationParameters configuration)
        {
            IPerformanceDataWriter dataWriter;

            switch (configuration.LogFileFormat)
            {
                case PerformanceMeasurementConfigurationParameters.LoggingFileFormat.JSON:
                    dataWriter = new PerformanceDataWriterJson();
                    break;

                case PerformanceMeasurementConfigurationParameters.LoggingFileFormat.CSV:
                    dataWriter = new PerformanceDataWriterCsv();
                    break;

                default:
                    throw new ArgumentException("Unknown log format in performance measurement configuration.");
            }

            return dataWriter;
        }
    }
}
