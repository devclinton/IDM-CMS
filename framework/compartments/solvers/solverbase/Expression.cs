/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

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

        public double Value
        {
            get { return _expression.Value; }
        }

        public override string ToString()
        {
            return _tree.ToString();
        }
    }
}
