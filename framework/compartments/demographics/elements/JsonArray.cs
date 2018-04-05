/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System.Collections.Generic;
using System.Text;

namespace compartments.demographics.elements
{
    public class JsonArray : JsonElement
    {
        private readonly List<JsonElement> _items;

        public JsonArray()
        {
            _items = new List<JsonElement>();
        }

        public void Add(JsonElement element)
        {
            _items.Add(element);
        }

        public IList<JsonElement> Items { get { return _items; } }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (_items.Count > 0)
            {
                sb.Append("[\n");
                foreach (JsonElement element in _items)
                {
                    sb.Append(element.ToString());
                    sb.Append(", ");
                }
                sb.Remove(sb.Length - 2, 1);    // Remove trailing ','
                sb.Append("]");
            }
            else
            {
                sb.Append("[ ]");
            }

            return sb.ToString();
        }

        public JsonElement this[int i]
        {
            get { return _items[i]; }
        }
    }
}
