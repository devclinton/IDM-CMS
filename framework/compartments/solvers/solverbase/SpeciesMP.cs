using compartments.emod;
using compartments.emod.interfaces;
using System.Collections.Generic;

namespace compartments.solvers.solverbase
{
    public class SpeciesMP : Species 
    {
        public SpeciesMP(SpeciesDescription speciesDescription, IDictionary<string, IValue> map)
            : base(speciesDescription, map)
        {
        }

        public override int Count { get { return (int)Value; } set { Value = (float)value; } }

        public override float Value
        {
            get;
            set;
        }

        public override void Update(float value)
        {
            Value = value;
        }

        public override int Increment() { Value = Value + 1; return Count; }
        public override int Increment(int delta) {Value = Value + (float)delta;  return Count;}
        public override int Decrement() { Value = Value - 1; return Count; }
        public override int Decrement(int delta) { Value = Value - (float)delta;  return Count;}
        public float Increment(float delta) { return Value += delta; }
        public float Decrement(float delta) { return Value -= delta; }
    }
}
