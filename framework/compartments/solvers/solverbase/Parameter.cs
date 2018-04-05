/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using compartments.emod;
using compartments.emod.interfaces;

namespace compartments.solvers.solverbase
{
    public class Parameter : IValue, IUpdateable
    {
        public Parameter(ParameterInfo parameterInfo)
        {
            Info  = parameterInfo;
            Value = parameterInfo.Value;
        }

        public ParameterInfo Info { get; private set; }

        public string Name { get { return Info.Name; } }

        public double Value { get; set; }

        public void Update(double value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Info.ToString();
        }
    }
}
