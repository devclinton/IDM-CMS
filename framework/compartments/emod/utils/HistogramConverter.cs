using System;
using Newtonsoft.Json;

namespace compartments.emod.utils
{
    public class HistogramConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DynamicHistogram);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var histogram = value as DynamicHistogram;
            if (histogram != null)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("LowerBound"); writer.WriteValue(histogram.LowerBound);
                writer.WritePropertyName("Width"); writer.WriteValue(histogram.Width);
                writer.WritePropertyName("BinCount"); writer.WriteValue(histogram.BinCount);

                writer.WritePropertyName("Data");
                writer.WriteStartArray();
                foreach (var entry in histogram)
                    writer.WriteValue(entry);
                writer.WriteEndArray();

                writer.WriteEndObject();
            }
        }
    }
}
