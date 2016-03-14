namespace compartments.demographics.elements
{
    public class JsonNull : JsonElement
    {
        public object Value { get { return null; } }

        public override string ToString()
        {
            return "null";
        }
    }
}
