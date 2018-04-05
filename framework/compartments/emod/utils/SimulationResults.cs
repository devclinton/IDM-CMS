using System;

namespace compartments.emod.utils
{
    public class SimulationResults
    {
        private readonly string[] _labels;
        private readonly double[][] _data;

        public SimulationResults(string[] labels, double[][] data)
        {
            _labels = labels;
            _data   = data;
        }

        public String[] Labels { get { return _labels; } }

        public double[][] Data { get { return _data; } }
    }
}
