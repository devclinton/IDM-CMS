/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System.Collections.Generic;
using compartments.emod.interfaces;

namespace compartments.emod.expressions
{
    public abstract class BinaryBooleanOp : IBooleanOperator
    {
        private readonly IBooleanOperator _operatorA;
        private readonly IBooleanOperator _operatorB;
        protected IBoolean ArgumentA;
        protected IBoolean ArgumentB;

        protected BinaryBooleanOp(IBooleanOperator firstArgument, IBooleanOperator secondArgument)
        {
            _operatorA = firstArgument;
            _operatorB = secondArgument;
            ArgumentA  = null;
            ArgumentB  = null;
        }

        public IBoolean ResolveReferences(IDictionary<string, IBoolean> bmap, IDictionary<string, IValue> nmap)
        {
            if (ArgumentA == null)
                ArgumentA = _operatorA.ResolveReferences(bmap, nmap);

            if (ArgumentB == null)
                ArgumentB = _operatorB.ResolveReferences(bmap, nmap);

            return GetValue();
        }

        public abstract IBoolean GetValue();
        public abstract string OperatorName { get; }

        public override string ToString()
        {
            return "(" + OperatorName + " " + _operatorA + " " + _operatorB + ")";
        }
    }

    public class AndOperator : BinaryBooleanOp
    {
        public AndOperator(IBooleanOperator a, IBooleanOperator b) : base(a, b) { }

        public override IBoolean GetValue() { return new And(ArgumentA, ArgumentB); }
        public override string OperatorName { get { return "&"; } }
    }

    public class OrOperator : BinaryBooleanOp
    {
        public OrOperator(IBooleanOperator a, IBooleanOperator b) : base(a, b) { }

        public override IBoolean GetValue() { return new Or(ArgumentA, ArgumentB); }
        public override string OperatorName { get { return "or"; } }
    }

    public class XorOperator : BinaryBooleanOp
    {
        public XorOperator(IBooleanOperator a, IBooleanOperator b) : base(a, b) { }

        public override IBoolean GetValue() { return new Xor(ArgumentA, ArgumentB); }
        public override string OperatorName { get { return "^"; } }
    }
}
