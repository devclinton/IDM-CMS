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
    public class Observable : IValue
    {
        private readonly ObservableInfo _info;
        private readonly IValue _expression;

        public Observable(ObservableInfo observableInfo, IDictionary<string, IValue> map)
        {
            _info = observableInfo;
            _expression = observableInfo.Expression.ResolveReferences(map);
        }

        public string Name { get { return _info.Name; } }

        public double Value
        {
            get { return _expression.Value; }
        }

        public override string ToString()
        {
            return _info.ToString();
        }
    }
}
