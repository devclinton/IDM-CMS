using System.Collections.Generic;
using compartments.emod;
using compartments.emod.interfaces;

namespace compartments.solvers.solverbase
{
    public abstract class EventBase
    {
        public class Statement
        {
            private readonly IUpdateable _target;
            private readonly IValue _expression;

            public Statement(EventInfoBase.StatementInfo statementInfo, IDictionary<string, IUpdateable> umap, IDictionary<string, IValue> nmap)
            {
                _target = statementInfo.Target.ResolveReferences(umap);
                _expression = statementInfo.Expression.ResolveReferences(nmap);
            }

            public void Execute()
            {
                _target.Update(_expression.Value);
            }
        }

        protected readonly EventInfoBase info;
        protected readonly IList<Statement> statements;

        protected EventBase(EventInfoBase eventInfo, IDictionary<string, IValue> nmap, IDictionary<string, IUpdateable> umap)
        {
            info       = eventInfo;
            statements = new List<Statement>(eventInfo.Statements.Count);
            foreach (EventInfoBase.StatementInfo statementInfo in eventInfo.Statements)
                statements.Add(new Statement(statementInfo, umap, nmap));
        }

        public string Name { get { return info.Name; } }

        public virtual void Fire()
        {
            foreach (Statement statement in statements)
                statement.Execute();
        }

        public override string ToString()
        {
            return info.ToString();
        }
    }
}
