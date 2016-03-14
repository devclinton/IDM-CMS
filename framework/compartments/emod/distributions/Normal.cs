using System;
using System.Collections.Generic;
using compartments.emod.interfaces;
using compartments.emod.utils;
using distlib;
using distlib.samplers;

namespace compartments.emod.distributions
{
    public class Normal : INumericOperator, IValue
    {
        private readonly float _mean;
        private readonly float _variance;

        private readonly DistributionSampler _distributionSampler;

        public Normal(float mean, float variance)
        {
            if (variance <= 0.0f)
                throw new ArgumentException("Variance must be greater than 0.");

            _mean     = mean;
            _variance = variance;

            _distributionSampler = RandLibSampler.CreateRandLibSampler(RNGFactory.GetRNG());
        }

        public float Value { get { return _distributionSampler.GenerateNormal(_mean, _variance); } }

        public IValue ResolveReferences(IDictionary<string, IValue> map)
        {
            return this;
        }

        public override string ToString()
        {
            return string.Format("(gaussian {0} {1})", _mean, _variance);
        }
    }
}
