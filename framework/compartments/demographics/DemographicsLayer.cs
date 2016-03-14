using System;
using System.Collections.Generic;
using System.Globalization;
using compartments.demographics.elements;
using compartments.demographics.sources;

namespace compartments.demographics
{
    public class DemographicsLayer
    {
        [Flags]
        internal enum SectionFlags
        {
            None             = 0,
            Metadata         = 1,
            StringTable      = 2,
            Defaults         = 4,
            NodeOffsets      = 8,
            RequiredSections = Metadata | StringTable | NodeOffsets
        }

        private readonly JsonParser _parser;
        private SectionFlags _flags;
        private int _nodeCount;
        private readonly IDictionary<int, uint> _nodeOffsets;
        private readonly IDictionary<int, JsonObject> _cachedNodes;

        public string FileName { get; private set; }

        public static DemographicsLayer CreateDemographicsLayer(string fileName)
        {
            return new DemographicsLayer(fileName);
        }

        protected DemographicsLayer(string fileName)
        {
            StringTable  = new Dictionary<string, string>();
            _nodeOffsets = new Dictionary<int, uint>();
            _flags       = SectionFlags.None;
            _cachedNodes = new Dictionary<int, JsonObject>();

            var source = CharSourceFactory.CharSourceFromFile(FileName = fileName);
            _parser = JsonParser.CreateJsonParser(source);
            _parser.ElementParsed += OnElementParsed;
            Console.WriteLine("Loading demographics layer '{0}'-", fileName);
            /*_root =*/ _parser.ParseElement() /*as JsonObject*/;
            _parser.ElementParsed -= OnElementParsed;
            _parser.Terminate = false;

            if ((_flags & SectionFlags.RequiredSections) != SectionFlags.RequiredSections)
            {
                Console.Error.WriteLine("Demographics layer '{0}' missing one or more required sections (Metadata, StringTable, NodeOffsets).", fileName);
                throw new ApplicationException();
            }
        }

        private void OnElementParsed(object sender, string name, JsonElement element)
        {
            switch (name)
            {
                case "Metadata":
                    {
                        var obj = element as JsonObject;
                        if (obj != null)
                        {
                            if (obj.Items.ContainsKey("DateCreated")) Console.WriteLine("Date created: {0}", ((JsonString)obj["DateCreated"]).Value);
                            if (obj.Items.ContainsKey("Author"))      Console.WriteLine("Author:       {0}", ((JsonString)obj["Author"]).Value);
                            if (obj.Items.ContainsKey("NodeCount"))   Console.WriteLine("Node count:   {0}", _nodeCount = (int)((JsonNumber)obj["NodeCount"]).Value);
                        }
                    }
                    _flags |= SectionFlags.Metadata;
                    break;

                case "StringTable":
                    ProcessStringTable((JsonObject) element);
                    _flags |= SectionFlags.StringTable;
                    break;

                case "Defaults":
                    Defaults = (JsonObject) element;
                    _flags |= SectionFlags.Defaults;
                    break;

                case "NodeOffsets":
                    ProcessNodeOffsets(((JsonString) element).Value);
                    _flags |= SectionFlags.NodeOffsets;
                    break;
            }

            _parser.Terminate = ((_flags & SectionFlags.RequiredSections) == SectionFlags.RequiredSections);
        }

        private void ProcessStringTable(JsonObject stringTable)
        {
            foreach (string key in stringTable.Items.Keys)
            {
                StringTable.Add(key, ((JsonString)stringTable[key]).Value);
            }
        }

        private void ProcessNodeOffsets(string offsetData)
        {
            for (int i = 0; i < _nodeCount; i++)
            {
                var nodeId = Int32.Parse(offsetData.Substring(i * 16, 8), NumberStyles.AllowHexSpecifier);
                var offset = UInt32.Parse(offsetData.Substring(i * 16 + 8, 8), NumberStyles.AllowHexSpecifier);
                _nodeOffsets.Add(nodeId, offset);
            }
        }

        public bool ContainsNode(int nodeId)
        {
            return _nodeOffsets.ContainsKey(nodeId);
        }

        public JsonObject GetNode(int nodeId)
        {
            JsonObject nodeObject = null;

            if (ContainsNode(nodeId))
            {
                if (_cachedNodes.ContainsKey(nodeId))
                {
                    nodeObject = _cachedNodes[nodeId];
                }
                else
                {
                    _parser.CharSource.Seek(_nodeOffsets[nodeId]);
                    _cachedNodes.Add(nodeId, nodeObject = (JsonObject) _parser.ParseElement());
                }
            }

            return nodeObject;
        }

        public JsonObject Defaults { get; private set; }
        public IDictionary<string, string> StringTable { get; private set; }

        public IList<int> NodeIDs
        {
            get
            {
                return new List<int>(_nodeOffsets.Keys);
            }
        } 
    }
}
