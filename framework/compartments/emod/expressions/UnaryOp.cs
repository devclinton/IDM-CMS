using System.Collections.Generic;
using compartments.emod.interfaces;

namespace compartments.emod.expressions
{
    public abstract class UnaryOp : INumericOperator
    {
        private readonly INumericOperator _operator;
        protected IValue Argument;

        protected UnaryOp(INumericOperator arg) { _operator = arg; }

        public IValue ResolveReferences(IDictionary<string, IValue> map)
        {
            if (Argument == null)
                Argument = _operator.ResolveReferences(map);

            return GetValue();
        }

        public abstract IValue GetValue();
        public abstract string OperatorName { get; }

        public override string ToString()
        {
            return "(" + OperatorName + " " + _operator + ")";
        }
    }

    // Exp,
    // Log,
    // Sin,
    // Cos,
    // Tan,
    // ASin,
    // ACos,
    // ATan,
    // Abs,
    // Floor,
    // Ceil,
    // Sqrt,
    // Neg,
    // HeavisideStop

    public class ExponentiationOperator : UnaryOp
    {
        public ExponentiationOperator(INumericOperator op) : base(op) {}

        public override IValue GetValue() { return new Exponentiate(Argument); }
        public override string OperatorName { get { return "exp"; } }
    }

    public class LogarithmOperator : UnaryOp
    {
        public LogarithmOperator(INumericOperator op) : base(op) {}

        public override IValue GetValue() { return new Logarithm(Argument); }
        public override string OperatorName { get { return "ln"; } }
    }

    public class SineOperator : UnaryOp
    {
        public SineOperator(INumericOperator op) : base(op) {}

        public override IValue GetValue() { return new Sine(Argument); }
        public override string OperatorName { get { return "sin"; } }
    }

    public class CosineOperator : UnaryOp
    {
        public CosineOperator(INumericOperator op) : base(op) {}

        public override IValue GetValue() { return new Cosine(Argument); }
        public override string OperatorName { get { return "cos"; } }
    }
/* NOT YET
    public class TangentOperator : UnaryOp
    {
        public TangentOperator(INumericOperator op) : base(op) {}

        public override IValue GetValue() { return new Tangent(Argument); }
        public override string OperatorName { get { return "tan"; } }
    }

    public class ArcSineOperator : UnaryOp
    {
        public ArcSineOperator(INumericOperator op) : base(op) {}

        public override IValue GetValue() { return new ArcSine(Argument); }
        public override string OperatorName { get { return "asin"; } }
    }

    public class ArcCosineOperator : UnaryOp
    {
        public ArcCosineOperator(INumericOperator op) : base(op) {}

        public override IValue GetValue() { return new ArcCosine(Argument); }
        public override string OperatorName { get { return "acos"; } }
    }

    public class ArcTangentOperator : UnaryOp
    {
        public ArcTangentOperator(INumericOperator op) : base(op) {}

        public override IValue GetValue() { return new ArcTangent(Argument); }
        public override string OperatorName { get { return "atan"; } }
    }
*/
    public class AbsoluteOperator : UnaryOp
    {
        public AbsoluteOperator(INumericOperator op) : base(op) {}

        public override IValue GetValue() { return new Absolute(Argument); }
        public override string OperatorName { get { return "abs"; } }
    }

    public class FloorOperator : UnaryOp
    {
        public FloorOperator(INumericOperator op) : base(op) {}

        public override IValue GetValue() { return new Floor(Argument); }
        public override string OperatorName { get { return "floor"; } }
    }

    public class CeilingOperator : UnaryOp
    {
        public CeilingOperator(INumericOperator op) : base(op) {}

        public override IValue GetValue() { return new Ceiling(Argument); }
        public override string OperatorName { get { return "ceil"; } }
    }

    public class SqrtOperator : UnaryOp
    {
        public SqrtOperator(INumericOperator op) : base(op) {}

        public override IValue GetValue() { return new Sqrt(Argument); }
        public override string OperatorName { get { return "sqrt"; } }
    }

    public class NegateOperator : UnaryOp
    {
        public NegateOperator(INumericOperator op) : base(op) {}

        public override IValue GetValue() { return new Negate(Argument); }
        public override string OperatorName { get { return "-"; } }
    }

    public class HeavisideStepOperator : UnaryOp
    {
        public HeavisideStepOperator(INumericOperator op) : base(op) {}

        public override IValue GetValue() { return new HeavisideStep(Argument); }
        public override string OperatorName { get { return "step"; } }
    }
}
