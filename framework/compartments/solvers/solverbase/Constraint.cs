/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System.Collections.Generic;
using compartments.emod;
using compartments.emod.interfaces;

namespace compartments.solvers.solverbase
{
    public class Constraint : IBoolean
    {
        private readonly ConstraintInfo _info;
        private readonly IBoolean _predicate;

        public Constraint(ConstraintInfo info, IDictionary<string, IBoolean> bmap, IDictionary<string, IValue> nmap)
        {
            _info      = info;
            _predicate = info.Predicate.ResolveReferences(bmap, nmap);
        }

        public string Name { get { return _info.Name; } }

        public bool Value
        {
            get { return _predicate.Value; }
        }

        public override string ToString()
        {
            return _info.ToString();
        }
    }
}
