using System;
using System.Collections.Generic;

namespace compartments.demographics
{
    public class DemographicsSet
    {
        private DemographicsLayer _baseLayer;
        private readonly Stack<DemographicsLayer> _layers;
        private readonly IDictionary<int, NodeData> _cachedNodeData;
        private IList<int> _nodes; 

        public IEnumerable<DemographicsLayer> Layers
        {
            get
            {
                return _layers;
            }
        }

        public static DemographicsSet CreateDemographicsSet()
        {
            return new DemographicsSet();
        }

        protected DemographicsSet()
        {
            _layers         = new Stack<DemographicsLayer>(1);
            _cachedNodeData = new Dictionary<int, NodeData>();
            _nodes          = null;
        }

        public void AddLayer(string fileName)
        {
            var layer = DemographicsLayer.CreateDemographicsLayer(fileName);

            if (_baseLayer == null)
            {
                _baseLayer = layer;
            }

            _layers.Push(layer);
        }

        public NodeData GetNodeData(int nodeId)
        {
            NodeData nodeData;

            if (_cachedNodeData.ContainsKey(nodeId))
            {
                nodeData = _cachedNodeData[nodeId];
            }
            else
            {
                if (_baseLayer.ContainsNode(nodeId))
                {
                    nodeData = NodeData.CreateNodeData(nodeId, this);
                    _cachedNodeData.Add(nodeId, nodeData);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("nodeId", nodeId, string.Format("DemographicsSet baselayer '{0}' doesn't contain an entry for node {1}.", _baseLayer.FileName, nodeId));
                }
            }

            return nodeData;
        }

        public IList<int> NodeIDs
        {
            get
            {
                if ((_nodes == null) && (_baseLayer != null))
                {
                    _nodes = _baseLayer.NodeIDs;
                }

                return _nodes;
            }
        }
    }
}
