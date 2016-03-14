using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace compartments.emod
{
    public class ModelInfo
    {
        private String _name;
        private readonly List<ConstraintInfo> _constraints;
        private readonly List<ScheduledEventInfo> _scheduledEvents; 
        private readonly List<TriggeredEventInfo> _triggeredEvents;
        private readonly Dictionary<NumericExpressionTree, NumericExpressionTree> _expressions;
        private readonly List<LocaleInfo> _locales;
        private readonly List<ObservableInfo> _observables;
        private readonly List<ParameterInfo> _parameters;
        private readonly Dictionary<BooleanExpressionTree, BooleanExpressionTree> _predicates;
        private readonly List<ReactionInfo> _reactions;
        private readonly List<SpeciesDescription> _species;

        public class ModelBuilder
        {
            private readonly ModelInfo _model;

            public ModelBuilder(String modelName)
            {
                _model = new ModelInfo { _name = modelName };
            }

            public void AddConstraint(ConstraintInfo constraint)
            {
                if ((constraint.Name != null) && _model._constraints.Any(ci => ci.Name == constraint.Name))
                    throw new ArgumentException("Constraint '" + constraint.Name + "' already exists in the model.");

                _model._constraints.Add(constraint);
                AddPredicate(constraint.Predicate);
            }

            public void AddScheduledEvent(ScheduledEventInfo eventInfo)
            {
                if ((eventInfo.Name != null) && _model._triggeredEvents.Any(ei => ei.Name == eventInfo.Name))
                    throw new ArgumentException("Event '" + eventInfo.Name + "' already exists in the model.");

                _model._scheduledEvents.Add(eventInfo);
            }

            public void AddTriggeredEvent(TriggeredEventInfo eventInfo)
            {
                if ((eventInfo.Name != null) && _model._triggeredEvents.Any(ei => ei.Name == eventInfo.Name))
                    throw new ArgumentException("Event '" + eventInfo.Name + "' already exists in the model.");

                _model._triggeredEvents.Add(eventInfo);
                AddPredicate(eventInfo.Predicate);
            }

            public void AddExpression(NumericExpressionTree expression)
            {
                if (!_model._expressions.ContainsKey(expression))
                {
                    if ((expression.Name != null) && _model.Expressions.Any(net => net.Name == expression.Name))
                        throw new ArgumentException("Expression '" + expression.Name + "' already exists in the model.");

                    _model._expressions.Add(expression, expression);
                }
            }

            public void AddLocale(LocaleInfo locale)
            {
                if (_model._locales.Any(l => l.Name == locale.Name))
                    throw new ArgumentException("Locale '" + locale.Name + "' already exists in the model.");

                _model._locales.Add(locale);
            }

            public void AddObservable(ObservableInfo observable)
            {
                if (_model._observables.Any(o => o.Name == observable.Name))
                    throw new ArgumentException("Observable '" + observable.Name + "' already exists in the model.");

                _model._observables.Add(observable);
                AddExpression(observable.Expression);
            }

            public void AddParameter(ParameterInfo parameter)
            {
                if (_model._parameters.Any(p => p.Name == parameter.Name))
                    throw new ArgumentException("Parameter '" + parameter.Name + "' already exists in the model.");

                _model._parameters.Add(parameter);
            }

            public void AddPredicate(BooleanExpressionTree predicate)
            {
                if (!_model._predicates.ContainsKey(predicate))
                {
                    if ((predicate.Name != null) && _model.Predicates.Any(bet => bet.Name == predicate.Name))
                        throw new ArgumentException("Boolean expression '" + predicate.Name + "' already exists in the model.");

                    _model._predicates.Add(predicate, predicate);
                }
            }

            public void AddReaction(ReactionInfo reaction)
            {
                _model._reactions.Add(reaction);
                AddExpression(reaction.RateExpression);
                if (reaction.HasDelay)
                    AddExpression(reaction.DelayExpression);
            }

            public void AddSpecies(SpeciesDescription species)
            {
                if (_model._species.Where(si => si.Locale == species.Locale).Any(si => si.Name == species.Name))
                    throw new ArgumentException("Species '" + species + "' already exists in the model.");

                _model._species.Add(species);
            }

            public ModelInfo Model
            {
                get { return _model; }
            }
        }

        private ModelInfo()
        {
            _name = "model";

            _constraints     = new List<ConstraintInfo>();
            _scheduledEvents = new List<ScheduledEventInfo>();
            _triggeredEvents = new List<TriggeredEventInfo>();
            _expressions     = new Dictionary<NumericExpressionTree, NumericExpressionTree>();
            _locales         = new List<LocaleInfo>();
            _observables     = new List<ObservableInfo>();
            _parameters      = new List<ParameterInfo>();
            _predicates      = new Dictionary<BooleanExpressionTree, BooleanExpressionTree>();
            _reactions       = new List<ReactionInfo>();
            _species         = new List<SpeciesDescription>();
        }

        public String Name { get { return _name; } }
        public IEnumerable<ConstraintInfo> Constraints { get { return _constraints; } }
        public IEnumerable<ScheduledEventInfo> ScheduledEvents { get { return _scheduledEvents; } }
        public IEnumerable<TriggeredEventInfo> TriggeredEvents { get { return _triggeredEvents; } }
        public IEnumerable<NumericExpressionTree> Expressions { get { return _expressions.Values; } }
        public IEnumerable<LocaleInfo> Locales { get { return _locales; } }
        public IEnumerable<ObservableInfo> Observables { get { return _observables; } }
        public IEnumerable<ParameterInfo> Parameters { get { return _parameters; } }
        public IEnumerable<BooleanExpressionTree> Predicates { get { return _predicates.Values; } }
        public IEnumerable<ReactionInfo> Reactions { get { return _reactions; } }
        public IEnumerable<SpeciesDescription> Species { get { return _species; } }

        public ConstraintInfo GetConstraintByName(string name) { return _constraints.First(ci => ci.Name == name); }
        public TriggeredEventInfo GetEventByName(string name) { return _triggeredEvents.First(ei => ei.Name == name); }
        public NumericExpressionTree GetExpressionByName(string name) { return _expressions.Values.First(net => net.Name == name); }
        public LocaleInfo GetLocaleByName(string name) { return _locales.First(li => li.Name == name); }
        public ObservableInfo GetObservableByName(string name) { return _observables.First(oi => oi.Name == name); }
        public ParameterInfo GetParameterByName(string name) { return _parameters.First(pi => pi.Name == name); }
        public BooleanExpressionTree GetPredicateByName(string name) { return _predicates.Values.First(bet => bet.Name == name); }
        public ReactionInfo GetReactionByName(string name) { return _reactions.First(ri => ri.Name == name); }
        public SpeciesDescription GetSpeciesByName(string name) { return _species.First(si => si.Name == name); }

        public override string ToString()
        {
            var sb = new StringBuilder(string.Format("; {0}\n\n(import (rnrs) (emodl cmslib))\n\n(start-model \"{0}\")\n\n", Name));

            if (Constraints.Any())
            {
                foreach (var constraint in Constraints)
                    sb.AppendLine(constraint.ToString());

                sb.AppendLine();
            }

            if (ScheduledEvents.Any())
            {
                foreach (var evt in ScheduledEvents)
                    sb.AppendLine(evt.ToString());

                sb.AppendLine();
            }

            if (TriggeredEvents.Any())
            {
                foreach (var evt in TriggeredEvents)
                    sb.AppendLine(evt.ToString());

                sb.AppendLine();
            }

            var expressions = Expressions.Where(e => (e.Name != null && e.Name != "rand"));
            if (expressions.Any())
            {
                foreach (var expression in expressions)
                    sb.AppendLine(expression.ToString());

                sb.AppendLine();
            }

            if (Observables.Any())
            {
                foreach (var observable in Observables)
                    sb.AppendLine(observable.ToString());

                sb.AppendLine();
            }

            var parameters = Parameters.Where(p => (p.Name != "time" && p.Name != "pi"));
            if (parameters.Any())
            {
                foreach (var parameter in parameters)
                    sb.AppendLine(parameter.ToString());

                sb.AppendLine();
            }

            var predicates = Predicates.Where(p => p.Name != null);
            if (predicates.Any())
            {
                foreach (var predicate in predicates)
                    sb.AppendLine(predicate.ToString());

                sb.AppendLine();
            }

            foreach (var locale in Locales)
            {
                if (locale.Name != "global")
                {
                    sb.AppendLine(locale.ToString());
                    sb.AppendLine(string.Format("(set-locale {0})\n", locale.Name));
                }
                var currentLocale = locale;
                foreach (var species in Species.Where(s => s.Locale == currentLocale))
                    sb.AppendLine(species.ToString());
                sb.AppendLine();
            }

            if (Reactions.Any())
            {
                foreach (var reaction in Reactions)
                    sb.AppendLine(reaction.ToString());

                sb.AppendLine();
            }

            sb.Append("(end-model)");

            return sb.ToString();
        }
    }
}
