/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using System.Collections.Generic;
using compartments.demographics.elements;

namespace compartments.demographics
{
    public class NodeData
    {
        private readonly int _nodeId;
        private readonly DemographicsSet _parentSet;

        public static NodeData CreateNodeData(int nodeId, DemographicsSet parentSet)
        {
            return new NodeData(nodeId, parentSet);
        }

        protected NodeData(int nodeId, DemographicsSet parentSet)
        {
            _nodeId    = nodeId;
            _parentSet = parentSet;
        }

        protected JsonElement GetValue(string key)
        {
            string[] keys = key.Split(new[] { '.', '/', '\\', ':' }, StringSplitOptions.RemoveEmptyEntries);
            JsonElement element = null;

            foreach (DemographicsLayer layer in _parentSet.Layers)
            {
                // Check the current layer for an entry for this node
                // If not found in a node specific entry, see if there is a default
                // at this layer for the value in question.
                var stringTable = layer.StringTable;
                element = GetObjectEntry(keys, layer.GetNode(_nodeId), stringTable) ?? GetObjectEntry(keys, layer.Defaults, stringTable);

                // If we found a value in this layer, exit the foreach(layer)....
                if (element != null)
                {
                    break;
                }
            }

            return element;
        }

        private static JsonElement GetObjectEntry(IEnumerable<string> keys, JsonElement obj, IDictionary<string, string> stringTable)
        {
            JsonElement element = null;

            if (obj != null)
            {
                foreach (string subKey in keys)
                {
                    if (obj != null)
                    {
                        if (obj is JsonObject)
                        {
                            if (stringTable.ContainsKey(subKey))
                            {
                                string compressedKey = stringTable[subKey];
                                obj = ((JsonObject)obj).Items.ContainsKey(compressedKey) ? ((JsonObject)obj)[compressedKey] : null;
                            }
                            else
                            {
                                obj = null;
                                break;
                            }
                        }
                        else if (obj is JsonArray)
                        {
                            int index;
                            if (Int32.TryParse(subKey, out index) && (index < ((JsonArray)obj).Items.Count))
                            {
                                obj = ((JsonArray)obj)[index];
                            }
                            else
                            {
                                obj = null;
                                break;
                            }
                        }
                        else
                        {
                            obj = null;
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                element = obj;
            }

            return element;
        }

        public string GetString(string key)
        {
            return ((JsonString) GetValue(key)).Value;
        }

        public int GetInteger(string key)
        {
            return (int)(((JsonNumber) GetValue(key)).Value);
        }

        public float GetFloat(string key)
        {
            return (float)(((JsonNumber)GetValue(key)).Value);
        }

        public double GetDouble(string key)
        {
            return ((JsonNumber)GetValue(key)).Value;
        }
    }
}
