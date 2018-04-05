/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System.Collections.Generic;
using System.Text;
using compartments.emod;

namespace compartments.solvers.solverbase
{
    public class Model : IModel
    {
        public Model(ModelInfo modelInfo)
        {
            Info    = modelInfo;
            Locales = new List<LocaleInfo>(modelInfo.Locales);
        }

        public string Name { get { return Info.Name; } }
        public ModelInfo Info { get; private set; }
        public IList<Constraint> Constraints { get; set; }
        public IList<ScheduledEvent> ScheduledEvents { get; set; }
        public IList<TriggeredEvent> TriggeredEvents { get; set; }
        public IList<Expression> Expressions { get; set; }
        public IList<LocaleInfo> Locales { get; private set; }
        public IList<Observable> Observables { get; set; }
        public IList<Parameter> Parameters { get; set; }
        public IList<Predicate> Predicates { get; set; }
        public IList<Reaction> Reactions { get; set; }
        public IList<Species> Species { get; set; }

        public override string ToString()
        {
            return Info.ToString();
        }
    }
}
