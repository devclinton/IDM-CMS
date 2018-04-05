/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using compartments.emod.interfaces;

namespace compartments.emod
{
    public class ParameterInfo : IValue
    {
        private readonly String _name;

        public ParameterInfo(String name, double value)
        {
            _name  = name;
            Value = value;
        }

        public String Name
        {
            get { return _name; }
        }

        public double Value { get; set; }

        public static implicit operator double(ParameterInfo p)
        {
            return p.Value;
        }

        public override String ToString() { return string.Format("(param {0} {1})", Name, Value); }
    }
}
