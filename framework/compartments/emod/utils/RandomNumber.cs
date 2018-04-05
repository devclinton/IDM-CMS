/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System.Collections.Generic;
using compartments.emod.interfaces;
using distlib.randomvariates;

namespace compartments.emod.utils
{
    public class RandomNumber : INumericOperator, IValue
    {
        private static RandomVariateGenerator _rng = RNGFactory.GetRNG();

        public virtual double Value { get { return _rng.GenerateUniformCC(); } }

        public static RandomVariateGenerator Generator
        {
            get { return _rng; }
            set { _rng = value; }
        }

        public IValue ResolveReferences(IDictionary<string, IValue> map)
        {
            return this;
        }
    }
}
