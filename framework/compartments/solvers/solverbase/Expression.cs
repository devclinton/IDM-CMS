using System.Collections.Generic;
using compartments.emod;
using compartments.emod.interfaces;

namespace compartments.solvers.solverbase
{
    public class Expression : IValue
    {
        private readonly NumericExpressionTree _tree;
        private readonly IValue _expression;

        public Expression(NumericExpressionTree expressionTree, IDictionary<string, IValue> map)
        {
            _tree = expressionTree;
            _expression = expressionTree.ResolveReferences(map);
        }

        public string Name { get { return _tree.Name; } }

        public float Value
        {
            get { return _expression.Value; }
        }

        public override string ToString()
        {
            return _tree.ToString();
        }
    }
}
