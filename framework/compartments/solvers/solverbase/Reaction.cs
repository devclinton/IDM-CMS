using System.Collections.Generic;
using compartments.emod;
using compartments.emod.interfaces;

namespace compartments.solvers.solverbase
{
    public class Reaction
    {
        private readonly IValue _rate;
        private readonly IValue _delay;

        public Reaction(ReactionInfo info, IDictionary<SpeciesDescription, Species> speciesMap, IDictionary<string, IValue> map)
        {
            Info = info;

            Reactants = new List<Species>();
            foreach (SpeciesDescription speciesDescription in info.Reactants)
                Reactants.Add(speciesMap[speciesDescription]);

            Products = new List<Species>();
            foreach (SpeciesDescription speciesDescription in info.Products)
                Products.Add(speciesMap[speciesDescription]);

            _rate = info.RateExpression.ResolveReferences(map);

            _delay = info.HasDelay ? info.DelayExpression.ResolveReferences(map) : null;
        }

        public double Delay
        {
            get { return _delay.Value; }
        }

        public ReactionInfo Info { get; private set; }

        public double Rate
        {
            get { return _rate.Value; }
        }

        public List<Species> Products { get; private set; }
        public List<Species> Reactants { get; private set; }

        public string Name { get { return Info.Name; } }
        public bool HasDelay { get { return Info.HasDelay; } }
        public bool IsLocal { get { return Info.IsLocal; } }

        public override string  ToString()
        {
            return Info.ToString();
        }
    }
}
