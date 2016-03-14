using System.Collections.Generic;

namespace compartments.emod.interfaces
{
    public interface INumericOperator
    {
        IValue ResolveReferences(IDictionary<string, IValue> map);
    }
}
