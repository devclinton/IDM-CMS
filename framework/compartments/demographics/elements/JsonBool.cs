namespace compartments.demographics.elements
{
    public class JsonBool : JsonElement
    {
        public bool Value { get; set; }

        public override string ToString()
        {
            return Value ? "true" : "false";
        }
    }
}
