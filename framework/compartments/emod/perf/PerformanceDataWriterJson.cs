/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using compartments.emod.utils;

namespace compartments.emod.perf
{
    public class PerformanceDataWriterJson : PerformanceDataWriterBase
    {
        protected override void SerializeMeasurementsToStream(PerformanceMeasurements measurements, TextWriter textWriter)
        {
            var jsonSerializer = new JsonSerializer();
            jsonSerializer.Converters.Add(new HistogramConverter());

            using (var jsonWriter = new JsonTextWriter(textWriter))
            {
                SetJsonWriterParameters(jsonWriter);
                jsonSerializer.Serialize(jsonWriter, measurements);
            }
        }

        private static void SetJsonWriterParameters(JsonTextWriter jsonWriter)
        {
            jsonWriter.Formatting  = Formatting.Indented;
            jsonWriter.Indentation = 1;
            jsonWriter.IndentChar  = '\t';
        }
    }
}
