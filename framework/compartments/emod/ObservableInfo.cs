using System;

namespace compartments.emod
{
    public class ObservableInfo
    {
        private readonly String _name;
        private readonly NumericExpressionTree _expression;

        public ObservableInfo(NumericExpressionTree observableExpression)
        {
            _name       = String.Empty;
            _expression = observableExpression;
        }

        public ObservableInfo(String name, NumericExpressionTree observableExpression)
        {
            _name       = name;
            _expression = observableExpression;
        }

        public String Name
        {
            get { return _name; }
        }

        public NumericExpressionTree Expression { get { return _expression; } }

        public override String ToString() { return string.Format("(observe {0} {1})", Name, _expression); }
    }
}
