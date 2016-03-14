using System.Collections.Generic;
using compartments.emod.interfaces;

namespace compartments.emod.expressions
{
    public class TargetReference
    {
        private readonly string _name;
        private IUpdateable _target;

        public TargetReference(string name)
        {
            _name = name;
            _target = null;
        }

        public IUpdateable ResolveReferences(IDictionary<string, IUpdateable> map)
        {
            return _target ?? (_target = map[_name]);
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
