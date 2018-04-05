/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

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
