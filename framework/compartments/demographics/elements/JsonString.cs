namespace compartments.demographics.elements
{
    public class JsonString : JsonElement
    {
        public string Value { get; set; }

        public override string ToString()
        {
            return '"' + Value + '"';
        }
    }
}
