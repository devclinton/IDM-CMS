/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System.Collections.Generic;
using compartments.emod.interfaces;

namespace compartments.emod.expressions
{
    public abstract class UnaryBooleanOp : IBooleanOperator
    {
        private readonly IBooleanOperator _argument;
        protected IBoolean Argument;

        protected UnaryBooleanOp(IBooleanOperator arg) { _argument = arg; }

        public IBoolean ResolveReferences(IDictionary<string, IBoolean> bmap, IDictionary<string, IValue> nmap)
        {
            if (Argument == null)
                Argument = _argument.ResolveReferences(bmap, nmap);

            return GetValue();
        }

        public abstract IBoolean GetValue();
    }

    public class NotOperator : UnaryBooleanOp
    {
        public NotOperator(IBooleanOperator arg) : base(arg) {}

        public override IBoolean GetValue()
        {
            return new Not(Argument);
        }

        public override string ToString()
        {
            return "(! " + Argument + ")";
        }
    }
}
