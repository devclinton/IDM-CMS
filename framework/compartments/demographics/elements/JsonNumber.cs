using System.Globalization;

namespace compartments.demographics.elements
{
    public class JsonNumber : JsonElement
    {
        public double Value { get; set; }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
