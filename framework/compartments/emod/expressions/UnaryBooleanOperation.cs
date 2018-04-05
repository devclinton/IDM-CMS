/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

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
