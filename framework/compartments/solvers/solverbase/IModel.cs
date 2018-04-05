/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

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
