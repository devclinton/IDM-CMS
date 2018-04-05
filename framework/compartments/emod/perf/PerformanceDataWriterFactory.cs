/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

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
