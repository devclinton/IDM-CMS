using System;
using System.Collections.Generic;
using compartments.emod.interfaces;
using compartments.emod.utils;
using distlib.randomvariates;

namespace compartments.emod.distributions
{
    public class Uniform : INumericOperator, IValue
    {
        private readonly double _min;
        private readonly double _max;
        private readonly RandomVariateGenerator _rng;

        public Uniform(double minimum, double maximum)
        {
            if (minimum > maximum)
                throw new ArgumentException("Minimum must be <= maxmimum.", "minimum");

            _min = minimum;
            _max = maximum;
            _rng = RNGFactory.GetRNG();
        }

        public double Value
        {
            get { return _min + (_rng.GenerateUniformOO() * (_max - _min)); }
        }

        public IValue ResolveReferences(IDictionary<string, IValue> map)
        {
            return this;
        }

        public override string ToString()
        {
            return string.Format("(uniform {0} {1})", _min, _max);
        }
    }
}
