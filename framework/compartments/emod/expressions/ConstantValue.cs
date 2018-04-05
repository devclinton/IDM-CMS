/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using compartments.emod.interfaces;

namespace compartments.emod.expressions
{
    public class ConstantValue : IValue
    {
        private readonly double _value;

        public ConstantValue(double value)
        {
            _value = value;
        }

        public double Value
        {
            get { return _value; }
        }
    }
}
