/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using System.Collections.Generic;
using compartments.emod.interfaces;

namespace compartments.emod.expressions
{
    public abstract class ComparisonOp : IBooleanOperator
    {
        private readonly INumericOperator _operatorA;
        private readonly INumericOperator _operatorB;
        protected IValue ArgumentA;
        protected IValue ArgumentB;

        protected ComparisonOp(INumericOperator firstOperator, INumericOperator secondOperator)
        {
            _operatorA = firstOperator;
            _operatorB = secondOperator;
            ArgumentA = null;
            ArgumentB = null;
        }

        public abstract IBoolean GetValue();

        public IBoolean ResolveReferences(IDictionary<string, IBoolean> bmap, IDictionary<string, IValue> nmap)
        {
            if (ArgumentA == null)
                ArgumentA = _operatorA.ResolveReferences(nmap);

            if (ArgumentB == null)
                ArgumentB = _operatorB.ResolveReferences(nmap);

            return GetValue();
        }

        public abstract string OperatorName { get; }
        public override string ToString()
        {
            return "(" + OperatorName + " " + _operatorA + " " + _operatorB + ")";
        }
    }

    public class LessThanOperator : ComparisonOp
    {
        public LessThanOperator(INumericOperator a, INumericOperator b) : base(a, b) { }

        public override IBoolean GetValue() { return new LessThan(ArgumentA, ArgumentB); }
        public override string OperatorName { get { return "<"; } }
    }

    public class LessThanOrEqualOperator : ComparisonOp
    {
        public LessThanOrEqualOperator(INumericOperator a, INumericOperator b) : base(a, b) { }

        public override IBoolean GetValue() { return new LessThanOrEqual(ArgumentA, ArgumentB); }
        public override string OperatorName { get { return "<="; } }
    }

    public class GreaterThanOperator : ComparisonOp
    {
        public GreaterThanOperator(INumericOperator a, INumericOperator b) : base(a, b) { }

        public override IBoolean GetValue() { return new GreaterThan(ArgumentA, ArgumentB); }
        public override string OperatorName { get { return ">"; } }
    }

    public class GreaterThanOrEqualOperator : ComparisonOp
    {
        public GreaterThanOrEqualOperator(INumericOperator a, INumericOperator b) : base(a, b) { }

        public override IBoolean GetValue() { return new GreaterThanOrEqual(ArgumentA, ArgumentB); }
        public override string OperatorName { get { return ">="; } }
    }

    public class EqualToOperator : ComparisonOp
    {
        public EqualToOperator(INumericOperator a, INumericOperator b) : base(a, b) { }

        public override IBoolean GetValue() { return new EqualTo(ArgumentA, ArgumentB); }
        public override string OperatorName { get { return "=="; } }
    }

    public class NotEqualToOperator : ComparisonOp
    {
        public NotEqualToOperator(INumericOperator a, INumericOperator b) : base(a, b) { }

        public override IBoolean GetValue() { return new NotEqualTo(ArgumentA, ArgumentB); }
        public override string OperatorName { get { return "!="; } }
    }
}
