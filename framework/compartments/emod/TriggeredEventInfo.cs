/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System.Text;
using compartments.emod.expressions;

namespace compartments.emod
{
    public class TriggeredEventInfo : EventInfoBase
    {
        public class Builder
        {
            private readonly TriggeredEventInfo _event;

            public Builder()
            {
                _event = new TriggeredEventInfo(null);
            }

            public Builder(string name)
            {
                _event = new TriggeredEventInfo(name);
            }

            public void SetPredicate(BooleanExpressionTree predicate)
            {
                _event.Predicate = predicate;
            }

            public void AddAction(TargetReference target, NumericExpressionTree expression)
            {
                _event.Statements.Add(new StatementInfo(target, expression));
            }

            public TriggeredEventInfo Event { get { return _event; } }
        }

        protected TriggeredEventInfo(string name) : base(name)
        {
            Predicate = null;
            Repeats   = false;
        }

        public BooleanExpressionTree Predicate { get; protected set; }
        public bool Repeats { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder(string.Format("(state-event {0} ", Name));
            sb.Append(Predicate.ToString());
            sb.Append(" (");
            foreach (var stmnt in Statements)
            {
                sb.Append(stmnt.ToString());
                sb.Append(' ');
            }
            if (Statements.Count > 0)
                sb.Remove(sb.Length - 1, 1);
            sb.Append("))");

            return sb.ToString();
        }
    }
}
