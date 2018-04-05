/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

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

        public override int Count { get { return (int)Value; } set { Value = (double)value; } }

        public override double Value
        {
            get;
            set;
        }

        public override void Update(double value)
        {
            Value = value;
        }

        public override int Increment() { Value = Value + 1; return Count; }
        public override int Increment(int delta) {Value = Value + (double)delta;  return Count;}
        public override int Decrement() { Value = Value - 1; return Count; }
        public override int Decrement(int delta) { Value = Value - (double)delta;  return Count;}
        public double Increment(double delta) { return Value += delta; }
        public double Decrement(double delta) { return Value -= delta; }
    }
}
