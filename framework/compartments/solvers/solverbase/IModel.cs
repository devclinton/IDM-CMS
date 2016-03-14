using System.Collections.Generic;
using compartments.emod;

namespace compartments.solvers.solverbase
{
    public interface IModel
    {
        string Name { get; }
        IList<Constraint> Constraints { get; }
        IList<ScheduledEvent> ScheduledEvents { get; }
        IList<TriggeredEvent> TriggeredEvents { get; }
        IList<Expression> Expressions { get; }
        IList<LocaleInfo> Locales { get; }
        IList<Observable> Observables { get; }
        IList<Parameter> Parameters { get; }
        IList<Predicate> Predicates { get; }
        IList<Reaction> Reactions { get; }
        IList<Species> Species { get; }
    }
}
