using compartments.emod.interfaces;

namespace compartments.emod.expressions
{
    public abstract class BinaryBooleanOperation : IBoolean
    {
        protected IBoolean ArgumentA;
        protected IBoolean ArgumentB;

        protected BinaryBooleanOperation(IBoolean argA, IBoolean argB)
        {
            ArgumentA = argA;
            ArgumentB = argB;
        }

        public abstract bool Value { get; }
    }

    public class And : BinaryBooleanOperation
    {
        public And(IBoolean argA, IBoolean argB) : base(argA, argB)
        {
        }

        public override bool Value
        {
            get { return ArgumentA.Value && ArgumentB.Value; }
        }
    }

    public class Or : BinaryBooleanOperation
    {
        public Or(IBoolean argA, IBoolean argB) : base(argA, argB)
        {
        }

        public override bool Value
        {
            get { return ArgumentA.Value || ArgumentB.Value; }
        }
    }

    public class Xor : BinaryBooleanOperation
    {
        public Xor(IBoolean argA, IBoolean argB) : base(argA, argB)
        {
        }

        public override bool Value
        {
            get { return ArgumentA.Value ^ ArgumentB.Value; }
        }
    }
}
