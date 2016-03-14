using compartments.emod.interfaces;

namespace compartments.emod.expressions
{
    public class ConstantValue : IValue
    {
        private readonly float _value;

        public ConstantValue(float value)
        {
            _value = value;
        }

        public float Value
        {
            get { return _value; }
        }
    }
}
