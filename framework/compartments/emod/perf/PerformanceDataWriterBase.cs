/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using System.IO;
using System.IO.Compression;
using compartments.emod.interfaces;

namespace compartments.emod.perf
{
    public abstract class PerformanceDataWriterBase : IPerformanceDataWriter
    {
        public void WritePerformanceMeasurements(PerformanceMeasurements measurements, PerformanceMeasurementConfigurationParameters configuration)
        {
            ValidateArguments(measurements, configuration);
            using (TextWriter textWriter = GetTextWriterForDestination(configuration))
            {
                SerializeMeasurementsToStream(measurements, textWriter);
            }
        }

        protected abstract void SerializeMeasurementsToStream(PerformanceMeasurements measurements, TextWriter textWriter);

        protected static void ValidateArguments(PerformanceMeasurements measurements,
                                                PerformanceMeasurementConfigurationParameters configuration)
        {
            if (measurements == null) throw new ArgumentNullException("measurements");
            if (configuration == null) throw new ArgumentNullException("configuration");
        }

        protected static TextWriter GetTextWriterForDestination(PerformanceMeasurementConfigurationParameters configuration)
        {
            TextWriter textWriter;
            if (configuration.CompressOutput)
            {
                var fs     = File.Open(configuration.WorkingFilename, FileMode.OpenOrCreate, FileAccess.Write);
                var gz     = new GZipStream(fs, CompressionMode.Compress);
                textWriter = new StreamWriter(gz);
            }
            else
            {
                textWriter = new StreamWriter(configuration.WorkingFilename);
            }

            return textWriter;
        }
    }
}