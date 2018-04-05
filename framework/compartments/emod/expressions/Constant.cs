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
