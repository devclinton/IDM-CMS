using compartments.emod.interfaces;

namespace compartments.emod.expressions
{
    public class ConstantValue : IValue
    {
        private readonly double _value;

        public ConstantValue(double value)
        {
            _value = value;
        }

        public double Value
        {
            get { return _value; }
        }
    }
}
