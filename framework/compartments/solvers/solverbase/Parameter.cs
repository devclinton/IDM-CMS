using compartments.emod;
using compartments.emod.interfaces;

namespace compartments.solvers.solverbase
{
    public class Parameter : IValue, IUpdateable
    {
        public Parameter(ParameterInfo parameterInfo)
        {
            Info  = parameterInfo;
            Value = parameterInfo.Value;
        }

        public ParameterInfo Info { get; private set; }

        public string Name { get { return Info.Name; } }

        public float Value { get; set; }

        public void Update(float value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Info.ToString();
        }
    }
}
