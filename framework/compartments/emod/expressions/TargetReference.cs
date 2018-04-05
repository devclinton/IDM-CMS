/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System.Collections.Generic;
using compartments.emod.interfaces;

namespace compartments.emod.expressions
{
    public class TargetReference
    {
        private readonly string _name;
        private IUpdateable _target;

        public TargetReference(string name)
        {
            _name = name;
            _target = null;
        }

        public IUpdateable ResolveReferences(IDictionary<string, IUpdateable> map)
        {
            return _target ?? (_target = map[_name]);
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
