/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System.Collections.Generic;
using compartments.emod.interfaces;

namespace compartments.emod.expressions
{
    public class Constant : INumericOperator
    {
        readonly double _constant;

        public Constant(double value) { _constant = value; }

        public IValue ResolveReferences(IDictionary<string, IValue> map)
        {
            return new ConstantValue(_constant);
        }

        public override string ToString()
        {
            return _constant.ToString();
        }
    }
}
