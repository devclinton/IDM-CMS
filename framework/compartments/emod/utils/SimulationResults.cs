/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

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
