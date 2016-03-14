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
