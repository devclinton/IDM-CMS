using System.Collections.Generic;
using compartments.emod;
using compartments.emod.interfaces;

namespace compartments.solvers.solverbase
{
    public class Constraint : IBoolean
    {
        private readonly ConstraintInfo _info;
        private readonly IBoolean _predicate;

        public Constraint(ConstraintInfo info, IDictionary<string, IBoolean> bmap, IDictionary<string, IValue> nmap)
        {
            _info      = info;
            _predicate = info.Predicate.ResolveReferences(bmap, nmap);
        }

        public string Name { get { return _info.Name; } }

        public bool Value
        {
            get { return _predicate.Value; }
        }

        public override string ToString()
        {
            return _info.ToString();
        }
    }
}
