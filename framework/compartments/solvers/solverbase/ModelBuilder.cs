using System.Collections.Generic;
using compartments.emod;
using compartments.emod.interfaces;

namespace compartments.solvers.solverbase
{
    public class ModelBuilder : IModelBuilder
    {
        protected readonly IDictionary<SpeciesDescription, Species> _speciesMap;
        protected readonly IDictionary<string, IValue> _nmap;
        protected readonly IDictionary<string, IBoolean> _bmap;
        protected readonly IDictionary<string, IUpdateable> _umap;

        public ModelBuilder()
        {
            _speciesMap = new Dictionary<SpeciesDescription, Species>();
            _nmap = new Dictionary<string, IValue>();
            _bmap = new Dictionary<string, IBoolean>();
            _umap = new Dictionary<string, IUpdateable>();
        }

        public IModel BuildModel(ModelInfo modelInfo)
        {
            var model = new Model(modelInfo);

            _speciesMap.Clear();
            _nmap.Clear();
            _bmap.Clear();
            _umap.Clear();

            ProcessSpecies(model, modelInfo);
            ProcessParameters(model, modelInfo);
            ProcessExpressions(model, modelInfo);
            ProcessObservables(model, modelInfo);
            ProcessReactions(model, modelInfo);
            ProcessPredicates(model, modelInfo);
            ProcessConstraints(model, modelInfo);
            ProcessScheduledEvents(model, modelInfo);
            ProcessTriggeredEvents(model, modelInfo);

            return model;
        }

        protected virtual void ProcessSpecies(Model model, ModelInfo modelInfo)
        {
            model.Species = new List<Species>();
            foreach (SpeciesDescription speciesDescription in modelInfo.Species)
            {
                var newSpecies = new Species(speciesDescription, _nmap);
                model.Species.Add(newSpecies);
                _nmap.Add(newSpecies.Name, newSpecies);
                _umap.Add(newSpecies.Name, newSpecies);
                _speciesMap.Add(speciesDescription, newSpecies);
            }
        }

        protected virtual void ProcessParameters(Model model, ModelInfo modelInfo)
        {
            model.Parameters = new List<Parameter>();
            foreach (ParameterInfo parameterInfo in modelInfo.Parameters)
            {
                var newParameter = new Parameter(parameterInfo);
                model.Parameters.Add(newParameter);
                _nmap.Add(newParameter.Name, newParameter);
                _umap.Add(newParameter.Name, newParameter);
            }
        }

        protected virtual void ProcessExpressions(Model model, ModelInfo modelInfo)
        {
            model.Expressions = new List<Expression>();
            foreach (NumericExpressionTree tree in modelInfo.Expressions)
            {
                var newExpression = new Expression(tree, _nmap);
                model.Expressions.Add(newExpression);
            }
        }

        protected virtual void ProcessObservables(Model model, ModelInfo modelInfo)
        {
            model.Observables = new List<Observable>();
            foreach (ObservableInfo observableInfo in modelInfo.Observables)
            {
                var newObservable = new Observable(observableInfo, _nmap);
                model.Observables.Add(newObservable);
            }
        }

        protected virtual void ProcessReactions(Model model, ModelInfo modelInfo)
        {
            model.Reactions = new List<Reaction>();
            foreach (ReactionInfo reactionInfo in modelInfo.Reactions)
            {
                model.Reactions.Add(new Reaction(reactionInfo, _speciesMap, _nmap));
            }
        }

        protected virtual void ProcessPredicates(Model model, ModelInfo modelInfo)
        {
            model.Predicates = new List<Predicate>();
            foreach (BooleanExpressionTree tree in modelInfo.Predicates)
            {
                var newPredicate = new Predicate(tree, _bmap, _nmap);
                model.Predicates.Add(newPredicate);
            }
        }

        protected virtual void ProcessConstraints(Model model, ModelInfo modelInfo)
        {
            model.Constraints = new List<Constraint>();
            foreach (ConstraintInfo constraintInfo in modelInfo.Constraints)
            {
                model.Constraints.Add(new Constraint(constraintInfo, _bmap, _nmap));
            }
        }

        protected virtual void ProcessScheduledEvents(Model model, ModelInfo modelInfo)
        {
            model.ScheduledEvents = new List<ScheduledEvent>();
            foreach (ScheduledEventInfo eventInfo in modelInfo.ScheduledEvents)
            {
                model.ScheduledEvents.Add(new ScheduledEvent(eventInfo, _nmap, _umap));
            }
        }

        protected virtual void ProcessTriggeredEvents(Model model, ModelInfo modelInfo)
        {
            model.TriggeredEvents = new List<TriggeredEvent>();
            foreach (TriggeredEventInfo eventInfo in modelInfo.TriggeredEvents)
            {
                model.TriggeredEvents.Add(new TriggeredEvent(eventInfo, _bmap, _nmap, _umap));
            }
        }
    }
}
