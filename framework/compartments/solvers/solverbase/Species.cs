/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using System.Collections.Generic;
using compartments.emod;
using compartments.emod.interfaces;

namespace compartments.solvers.solverbase
{
    public class Species : IValue, IUpdateable
    {
        private readonly IValue _initialPopulation;

        public Species(SpeciesDescription speciesDescription, IDictionary<string, IValue> map)
        {
            Description = speciesDescription;
            _initialPopulation = speciesDescription.InitialPopulation.ResolveReferences(map);
        }

        public SpeciesDescription Description { get; private set; }
        public string Name { get { return Description.Name; } }
        public LocaleInfo Locale { get { return Description.Locale; } }

        public virtual int Count { get; set; }

        public virtual double Value
        {
            get { return Count; }
            set { ;}
        }

        public int Reset()
        {
            Count = Math.Max((int)Math.Round(_initialPopulation.Value), 0);
            return Count;
        }

        public virtual void Update(double value)
        {
            Count = (int)value;
        }

        public virtual int Increment() { return ++Count; }
        public virtual int Increment(int delta) { return Count += delta; }
        public virtual int Decrement()
        {
            --Count;
            if (Count < 0)
                throw new ApplicationException($"Count is less than 0 ({Name}:{Count})");
            return Count;
        }

        public virtual int Decrement(int delta)
        {
            Count -= delta;
            if (Count < 0)
                throw new ApplicationException($"Count is less than 0 ({Name}:{Count})");
            return Count;
        }

        public override string ToString()
        {
            return Description.ToString();
        }
    }
}
