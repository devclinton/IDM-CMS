using System.Collections.Generic;
using System.Text;

namespace compartments.demographics.elements
{
    public class JsonObject : JsonElement
    {
        private readonly Dictionary<string, JsonElement> _contents;

        public JsonObject()
        {
            _contents = new Dictionary<string, JsonElement>();
        }

        public void Add(string name, JsonElement value)
        {
            _contents.Add(name, value);
        }

        public IDictionary<string, JsonElement> Items
        {
            get { return _contents; }
        }

        public JsonElement this[string name]
        {
            get { return _contents[name]; }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (_contents.Count > 0)
            {
                sb.Append("{\n");
                foreach (var key in _contents.Keys)
                {
                    sb.Append('"');
                    sb.Append(key);
                    sb.Append("\" : ");
                    sb.Append(_contents[key].ToString());
                    sb.Append(",\n");
                }
                sb.Remove(sb.Length - 2, 2);
                sb.Append("\n}");
            }
            else
            {
                sb.Append("{ }");
            }

            return sb.ToString();
        }
    }
}
