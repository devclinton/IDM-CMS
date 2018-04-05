/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

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
