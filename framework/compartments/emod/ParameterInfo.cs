using System;
using compartments.emod.interfaces;

namespace compartments.emod
{
    public class ParameterInfo : IValue
    {
        private readonly String _name;

        public ParameterInfo(String name, double value)
        {
            _name  = name;
            Value = value;
        }

        public String Name
        {
            get { return _name; }
        }

        public double Value { get; set; }

        public static implicit operator double(ParameterInfo p)
        {
            return p.Value;
        }

        public override String ToString() { return string.Format("(param {0} {1})", Name, Value); }
    }
}
