/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using System.Collections.Generic;
using compartments.emod.interfaces;

namespace compartments.emod
{
    public class NumericExpressionTree : INumericOperator
    {
        private readonly String _name;
        private readonly INumericOperator _rootOperator;
        private IValue _value;

        public NumericExpressionTree(INumericOperator rootOperator)
        {
            _name         = null;
            _rootOperator = rootOperator;
            _value        = null;
        }

        public NumericExpressionTree(String name, INumericOperator rootOperator)
        {
            _name         = name;
            _rootOperator = rootOperator;
            _value        = null;
        }

        public String Name
        {
            get { return _name; }
        }

        public IValue ResolveReferences(IDictionary<string, IValue> map)
        {
            if (_value == null)
            {
                _value = _rootOperator.ResolveReferences(map);
                if (_name != null)
                    map.Add(_name, _value);
            }

            return _value;
        }

        public override String ToString()
        {
            return (Name != null) ? string.Format("(func {0} {1})", Name, _rootOperator) : _rootOperator.ToString();
        }
    }
}
