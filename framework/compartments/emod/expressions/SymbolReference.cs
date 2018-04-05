/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using System.Collections.Generic;
using compartments.emod.interfaces;

namespace compartments.emod.expressions
{
    public class SymbolReference : INumericOperator
    {
        private readonly string _name;
        private IValue _value;

        public SymbolReference(string name)
        {
            _name  = name;
            _value = null;
        }

        public IValue ResolveReferences(IDictionary<string, IValue> map)
        {
            try
            {
                return _value ?? (_value = map[_name]);
            }
            catch (KeyNotFoundException e)
            {
                Console.Error.Write(e);
                Console.Error.WriteLine($":\n\t'{_name}'");
                throw;
            }
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
