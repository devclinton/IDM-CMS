using System.Collections.Generic;
using compartments.emod.interfaces;
using distlib.randomvariates;

namespace compartments.emod.utils
{
    public class RandomNumber : INumericOperator, IValue
    {
        private static RandomVariateGenerator _rng = RNGFactory.GetRNG();

        public virtual float Value { get { return _rng.GenerateUniformCC(); } }

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
