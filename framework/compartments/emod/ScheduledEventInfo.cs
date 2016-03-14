using System.Text;
using compartments.emod.expressions;

namespace compartments.emod
{
    public class ScheduledEventInfo : EventInfoBase
    {
        public class Builder
        {
            private readonly ScheduledEventInfo _event;

            public Builder()
            {
                _event = new ScheduledEventInfo(null);
            }

            public Builder(string name)
            {
                _event = new ScheduledEventInfo(name);
            }

            public void SetTime(float eventTime)
            {
                _event.Time = eventTime;
            }

            public void SetInterval(float interval)
            {
                _event.Interval = interval;
            }

            public void AddAction(TargetReference target, NumericExpressionTree expression)
            {
                _event.Statements.Add(new StatementInfo(target, expression));
            }

            public ScheduledEventInfo Event { get { return _event; } }
        }

        public ScheduledEventInfo(string name) : base(name)
        {
            Time     = float.PositiveInfinity;
            Interval = 0.0f;
        }

        public float Time { get; protected set; }
        public float Interval { get; protected set; }

        public override string ToString()
        {
            var sb = new StringBuilder(string.Format("(time-event {0} {1} (", Name, Time));
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
