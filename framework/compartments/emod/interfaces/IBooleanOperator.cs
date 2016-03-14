using System.Collections.Generic;

namespace compartments.emod.interfaces
{
    public interface IBooleanOperator
    {
        IBoolean ResolveReferences(IDictionary<string, IBoolean> bmap, IDictionary<string, IValue> nmap);
    }
}
