using System;

namespace compartments.emod.utils
{
    public class SimulationResults
    {
        private readonly string[] _labels;
        private readonly float[][] _data;

        public SimulationResults(string[] labels, float[][] data)
        {
            _labels = labels;
            _data   = data;
        }

        public String[] Labels { get { return _labels; } }

        public float[][] Data { get { return _data; } }
    }
}
