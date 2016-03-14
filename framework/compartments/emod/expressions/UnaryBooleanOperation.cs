using compartments.emod.interfaces;

namespace compartments.emod.expressions
{
    public abstract class UnaryBooleanOperation : IBoolean
    {
        protected IBoolean Argument;

        protected UnaryBooleanOperation(IBoolean argument)
        {
            Argument = argument;
        }

        public abstract bool Value { get; }
    }

    public class Not : UnaryBooleanOperation
    {
        public Not(IBoolean argument) : base(argument)
        {
        }

        public override bool Value
        {
            get { return !Argument.Value; }
        }
    }
}
