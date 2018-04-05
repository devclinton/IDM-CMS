/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System.Collections.Generic;
using compartments.emod.expressions;

namespace compartments.emod
{
    public abstract class EventInfoBase
    {
        public class StatementInfo
        {
            public StatementInfo(TargetReference target, NumericExpressionTree expression)
            {
                Target = target;
                Expression = expression;
            }

            public TargetReference Target { get; set; }
            public NumericExpressionTree Expression { get; set; }

            public override string ToString()
            {
                return string.Format("({0} {1})", Target, Expression);
            }
        }

        protected EventInfoBase(string name)
        {
            Name       = name;
            Statements = new List<StatementInfo>();
        }

        public string Name { get; private set; }
        public IList<StatementInfo> Statements { get; private set; }
    }
}
