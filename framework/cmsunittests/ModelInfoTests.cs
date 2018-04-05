using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

using compartments.emod;
using compartments.emod.interfaces;
using compartments.emodl;
using compartments.emod.expressions;

namespace cmsunittests
{
    [TestFixture, Description("Model representation tests")]
    class ModelInfoTests : AssertionHelper
    {
        private static readonly Constant Two = new Constant(2.0);
        private static readonly Constant Three = new Constant(3.0);
        private static readonly Constant Ten = new Constant(10.0);
        private static readonly Constant TwentyOne = new Constant(21.0);

        [Test]
        public void BooleanExpressionTreeTest()
        {
            const string name = "bit";
            IBooleanOperator expression = new GreaterThanOperator(Three, Two);
            var bet = new BooleanExpressionTree(name, expression);

            Console.Write("Created new BooleanExpressionTree: ");
            Console.WriteLine(bet);
            Console.WriteLine("BooleanExpressionTreeTest: test Name property...");
            Expect(bet.Name == name);
            Console.WriteLine("BooleanExpressionTreeTest: test expression and ResolveReferences()...");
            IDictionary<string, IBoolean> bmap = new Dictionary<string, IBoolean>();
            IBoolean predicate = bet.ResolveReferences(bmap, null);
            Expect(predicate.Value == true);
            Console.WriteLine("BooleanExpressionTreeTest: testing map size (1)...");
            Expect(bmap.Count == 1);
            Console.WriteLine("BooleanExpressionTreeTest: testing map contents...");
            Expect(bmap[name] == predicate);
        }

        [Test]
        public void ConstraintInfoTest()
        {
            IBooleanOperator trueExpression = new GreaterThanOperator(Three, Two);
            var trueTree = new BooleanExpressionTree("true", trueExpression);
            var constraintOne = new ConstraintInfo(trueTree);

            Console.Write("Created constraint: ");
            Console.WriteLine(constraintOne);
            Console.WriteLine("ConstraintInfoTest: testing name of anonymous constraint...");
            Expect(constraintOne.Name == string.Empty);
            Console.WriteLine("ConstraintInfoTest: testing Predicate property (constraintOne)...");
            Expect(constraintOne.Predicate == trueTree);

            const string constraintName = "ConstraintTwo";
            IBooleanOperator falseExpression = new LessThanOperator(Three, Two);
            var falseTree = new BooleanExpressionTree("false", falseExpression);
            var constraintTwo = new ConstraintInfo(constraintName, falseTree);

            Console.Write("Created constraint: ");
            Console.WriteLine(constraintTwo);
            Console.WriteLine("ConstraintInfoTest: testing Name of named constraint...");
            Expect(constraintTwo.Name == constraintName);
            Console.WriteLine("ConstraintInfoTest: testing Predicate property (constraintTwo)...");
            Expect(constraintTwo.Predicate == falseTree);
        }

        [Test]
        [Ignore("EventInfo is under construction.")]
        public void EventInfoTest()
        {
/*
            IBooleanOperator trueExpression = new GreaterThanOperator(Three, Two);
            var trueTree = new BooleanExpressionTree("true", trueExpression);
            int eventCount = 0;
            var eventOne = new EventInfo(trueTree, () => { eventCount++; });

            Console.Write("Created event: ");
            Console.WriteLine(eventOne);
            Console.WriteLine("EventInfoTest: testing name of anonymous constraint...");
            Expect(eventOne.Name == string.Empty);
            Console.WriteLine("EventInfoTest: testing Predicate property (eventOne)...");
            Expect(eventOne.Predicate == trueTree);
            Console.WriteLine("EventInfoTest: testing Trigger() method (eventOne)...");
            eventOne.Trigger();
            Expect(eventCount == 1);

            const string eventName = "ConstraintTwo";
            IBooleanOperator falseExpression = new LessThanOperator(Three, Two);
            var falseTree = new BooleanExpressionTree("false", falseExpression);
            var eventTwo = new EventInfo(eventName, falseTree, () => { eventCount += 2; });

            Console.Write("Created event: ");
            Console.WriteLine(eventTwo);
            Console.WriteLine("EventInfoTest: testing Name of named event...");
            Expect(eventTwo.Name == eventName);
            Console.WriteLine("EventInfoTest: testing Predicate property (eventTwo)...");
            Expect(eventTwo.Predicate == falseTree);
            Console.WriteLine("EventInfoTest: testing Trigger() method (eventTwo)...");
            eventTwo.Trigger();
            Expect(eventCount == 3);
            Console.WriteLine("EventInfoTest: testing default Enabled property...");
            Expect(!eventTwo.Enabled);
            Console.WriteLine("EventInfoTest: testing setting Enabled property...");
            eventTwo.Enabled = true;
            Expect(eventTwo.Enabled);
            Console.WriteLine("EventInfoTest: testing default Repeats property...");
            Expect(!eventTwo.Repeats);
            Console.WriteLine("EventInfoTest: testing setting Repeats property...");
            eventTwo.Repeats = true;
            Expect(eventTwo.Repeats);
*/
        }

        [Test]
        public void LocaleInfoTest()
        {
            const string name = "locale";
            var locale = new LocaleInfo(name);
            Expect(locale.Name == name);
        }

        [Test]
        public void ModelInfoTest()
        {
            const string modelName = "test model";
            var builder = new ModelInfo.ModelBuilder(modelName);

            const string constraintName = "test constraint";
            const string constraintPredicateName = "constraint predicate";
            var constraintExpression = new EqualToOperator(Two, Two);
            var constraintPredicate = new BooleanExpressionTree(constraintPredicateName, constraintExpression);
            var constraint = new ConstraintInfo(constraintName, constraintPredicate);
            builder.AddConstraint(constraint);

/*
            const string eventName = "test event";
            const string eventPredicateName = "event predicate";
            var eventExpression = new GreaterThanOperator(Three, Two);
            var eventPredicate = new BooleanExpressionTree(eventPredicateName, eventExpression);
            int eventCount = 0;
            var testEvent = new EventInfo(eventName, eventPredicate, () => { eventCount++; });
            builder.AddEvent(testEvent);
*/

            const string expressionName = "test expression";
            var expression = new AddOperator(Three, Two);
            var expressionTree = new NumericExpressionTree(expressionName, expression);
            builder.AddExpression(expressionTree);

            const string localeName = "test locale";
            var locale = new LocaleInfo(localeName);
            builder.AddLocale(locale);

            const string observableName = "test observable";
            const string observableExpressionName = "observable expression";
            var rootOperator = new PowerOperator(Two, Three);
            var observableExpressionTree = new NumericExpressionTree(observableExpressionName, rootOperator);
            var observable = new ObservableInfo(observableName, observableExpressionTree);
            builder.AddObservable(observable);

            const string parameterName = "test parameter";
            var parameter = new ParameterInfo(parameterName, 3.14159265);
            builder.AddParameter(parameter);

            const string predicateName = "test predicate";
            var predicateExpression = new LessThanOperator(Two, Three);
            var predicate = new BooleanExpressionTree(predicateName, predicateExpression);
            builder.AddPredicate(predicate);

            const string speciesName = "reactant";
            var reactant = new SpeciesDescription(speciesName, 2012, locale);
            builder.AddSpecies(reactant);

            const string reactionName = "test reaction";
            var rBuilder = new ReactionInfo.ReactionBuilder(reactionName);
            rBuilder.AddReactant(reactant);
            var rateExpression = new MultiplyOperator(Ten, new SymbolReference(speciesName));
            var reactionRate = new NumericExpressionTree(null, rateExpression);
            rBuilder.SetRate(reactionRate);
            var reaction = rBuilder.Reaction;
            builder.AddReaction(reaction);

            var modelInfo = builder.Model;

            Console.WriteLine("ModelInfoTests: checking model name...");
            Expect(modelInfo.Name == modelName);
            Console.WriteLine("ModelInfoTests: checking constraint count (1)...");
            Expect(modelInfo.Constraints.Count() == 1);
            Console.WriteLine("ModelInfoTests: checking constraint...");
            Expect(modelInfo.Constraints.First() == constraint);
/*
            Console.WriteLine("ModelInfoTests: checking event count (1)...");
            Expect(modelInfo.Events.Count() == 1);
            Console.WriteLine("ModelInfoTests: checking event...");
            Expect(modelInfo.Events.First() == testEvent);
*/
            Console.WriteLine("ModelInfoTests: checking expression count (3)...");
            Expect(modelInfo.Expressions.Count() == 3);
            Console.WriteLine("ModelInfoTests: checking expressions...");
            var expressions = modelInfo.Expressions.Where(net => net.Name == expressionName).ToList();
            Expect(expressions.Count == 1);
            Expect(expressions[0] == expressionTree);
            Console.WriteLine("ModelInfoTests: checking locale count (1)...");
            Expect(modelInfo.Locales.Count() == 1);
            Console.WriteLine("ModelInfoTests: checking locale...");
            Expect(modelInfo.Locales.First() == locale);
            Console.WriteLine("ModelInfoTests: checking observable count (1)...");
            Expect(modelInfo.Observables.Count() == 1);
            Console.WriteLine("ModelInfoTests: checking observable...");
            Expect(modelInfo.Observables.First() == observable);
            Console.WriteLine("ModelInfoTests: checking parameter count (1)...");
            Expect(modelInfo.Parameters.Count() == 1);
            Console.WriteLine("ModelInfoTests: checking parameter...");
            Expect(modelInfo.Parameters.First() == parameter);
/*
            Console.WriteLine("ModelInfoTests: checking predicate count (3)...");
            Expect(modelInfo.Predicates.Count() == 3);
*/
            Console.WriteLine("ModelInfoTests: checking predicate count (2)...");
            Expect(modelInfo.Predicates.Count() == 2);
            Console.WriteLine("ModelInfoTests: checking predicate...");
            var predicates = modelInfo.Predicates.Where(pi => pi.Name == predicateName).ToList();
            Expect(predicates.Count == 1);
            Expect(predicates[0] == predicate);
            Console.WriteLine("ModelInfoTests: checking reaction count (1)...");
            Expect(modelInfo.Reactions.Count() == 1);
            Console.WriteLine("ModelInfoTests: checking reaction...");
            Expect(modelInfo.Reactions.First() == reaction);
            Console.WriteLine("ModelInfoTests: checking species count (1)...");
            Expect(modelInfo.Species.Count() == 1);
            Console.WriteLine("ModelInfoTests: checking species...");
            Expect(modelInfo.Species.First() == reactant);
        }

        [Test]
        public void NumericExpressionTreeTest()
        {
            const string name = "net";
            var expression = new MultiplyOperator(Two, TwentyOne);
            var net = new NumericExpressionTree(name, expression);

            Console.Write("Created new NumericExpressionTree: ");
            Console.WriteLine(net);
            Console.WriteLine("NumericExpressionTreeTest: test Name property...");
            Expect(net.Name == name);
            Console.WriteLine("NumericExpressionTreeTest: test expression and ResolveReferences()...");
            IDictionary<string, IValue> nmap = new Dictionary<string, IValue>();
            IValue value = net.ResolveReferences(nmap);
            Expect(value.Value == 42.0);
            Console.WriteLine("NumericExpressionTreeTest: testing map size (1)...");
            Expect(nmap.Count == 1);
            Console.WriteLine("NumericExpressionTreeTest: testing map contents...");
            Expect(nmap[name] == value);
        }

        [Test]
        public void ObservableInfoTest()
        {
            var operatorOne = new MultiplyOperator(TwentyOne, Two);
            var expressionOne = new NumericExpressionTree(null, operatorOne);
            var observableOne = new ObservableInfo(expressionOne);

            Console.Write("Created new observable: ");
            Console.WriteLine(observableOne);
            Console.WriteLine("ObservableTest: testing anonymous observable Name property...");
            Expect(observableOne.Name == string.Empty);
            Console.WriteLine("ObservableTest: testing Expression property (observableOne)...");
            Expect(observableOne.Expression == expressionOne);

            const string observableName = "observableTwo";
            var operatorTwo = new AddOperator(Two, Three);
            var expressionTwo = new NumericExpressionTree(null, operatorTwo);
            var observableTwo = new ObservableInfo(observableName, expressionTwo);

            Console.Write("Created new ObservableInfo: ");
            Console.WriteLine(observableTwo);
            Console.WriteLine("ObservableTest: testing Name property...");
            Expect(observableTwo.Name == observableName);
            Console.WriteLine("ObservableTest: testing Expression property (observableTwo)...");
            Expect(observableTwo.Expression == expressionTwo);
        }

        [Test]
        public void ParameterInfoTest()
        {
            const string name = "param";
            var parameter = new ParameterInfo(name, 42.0);

            Console.Write("Created new parameter: ");
            Console.WriteLine(parameter);
            Console.WriteLine("ParameterInfoTest: testing Name property...");
            Expect(parameter.Name == name);
            Console.WriteLine("ParameterInfoTest: testing Value property (get)...");
            Expect(parameter.Value == 42.0);
            Console.WriteLine("ParameterInfoTest: testing Value property (set)...");
            parameter.Value = 2.82842712475;
            Expect(parameter.Value == 2.82842712475);
        }

        [Test]
        public void ReactionInfoTest()
        {
            // Add a reactant, reactant shows up in Reactants list
            var builder = new ReactionInfo.ReactionBuilder();
            ReactionInfo reaction = builder.Reaction;

            var reactant = new SpeciesDescription("reactant", 2012);
            builder.AddReactant(reactant);

            Console.WriteLine("Checking for reactant added to reaction with ReactionBuilder...");
            Expect(reaction.Reactants.First() == reactant);

            // Add a product, product shows up in Products list
            var product = new SpeciesDescription("product", 2112);
            builder.AddProduct(product);

            Console.WriteLine("Checking for product added to reaction with ReactionBuilder...");
            Expect(reaction.Products.First() == product);

            // Set rate, reaction rate is set.
            var expression = new NumericExpressionTree(string.Empty,
                                                       new MultiplyOperator(new SymbolReference("reactant"),
                                                                            new SymbolReference("product")));
            builder.SetRate(expression);

            Console.WriteLine("Checking for reaction rate set from ReactionBuilder...");
            Expect(reaction.RateExpression == expression);

            Console.WriteLine("Checking !reaction.HasDelay (before calling builder.SetDelay())...");
            Expect(!reaction.HasDelay);

            var delay = new NumericExpressionTree(string.Empty, new Constant(3.14159265));
            builder.SetDelay(delay);

            Console.WriteLine("Checking reaction.HasDelay (after calling builder.SetDelay())...");
            Expect(reaction.HasDelay);
            Console.WriteLine("Checking reaction for correct delay...");
            Expect(reaction.DelayExpression == delay);

            Console.WriteLine("");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "must come from the same locale", MatchType = MessageMatch.Contains)]
        public void MixedLocaleReactantTest()
        {
            var builder = new ReactionInfo.ReactionBuilder();

            var species1 = new SpeciesDescription("A-Species", new LocaleInfo("compartment-A"));
            builder.AddReactant(species1);

            Console.WriteLine("Checking for ArgumentException() with reactants from different locales...");
            Console.WriteLine("");

            var species2 = new SpeciesDescription("B-Species", new LocaleInfo("compartment-B"));
            builder.AddReactant(species2);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "must come from the same locale", MatchType = MessageMatch.Contains)]
        public void MixedLocaleproductTest()
        {
            var builder = new ReactionInfo.ReactionBuilder();

            var species1 = new SpeciesDescription("A-Species", new LocaleInfo("compartment-A"));
            builder.AddProduct(species1);

            Console.WriteLine("Checking for ArgumentException() with products from difference locales...");
            Console.WriteLine("");

            var species2 = new SpeciesDescription("B-Species", new LocaleInfo("compartment-B"));
            builder.AddProduct(species2);
        }

        [Test]
        public void ReactionIsLocalTest()
        {
            var builder = new ReactionInfo.ReactionBuilder();
            ReactionInfo reaction = builder.Reaction;

            var reactantA = new SpeciesDescription("reactantA", new LocaleInfo("locale1"));

            var productA = new SpeciesDescription("productA", reactantA.Locale);

            builder.AddReactant(reactantA);
            builder.AddProduct(productA);

            Console.WriteLine("Checking for reaction.IsLocal with reactants and products in same locale...");
            Expect(reaction.IsLocal);
            Expect(!reaction.IsDiffusion);

            builder = new ReactionInfo.ReactionBuilder();
            reaction = builder.Reaction;

            var productB = new SpeciesDescription("productB", new LocaleInfo("locale2"));

            builder.AddReactant(reactantA);
            builder.AddProduct(productB);

            Console.WriteLine("Checking for !reaction.IsLocal with reactants and products from different locales...");
            Expect(!reaction.IsLocal);
            Expect(reaction.IsDiffusion);

            Console.WriteLine("");
        }

        [Test]
        public void MigrationReactions()
        {
            const string modelDescription = @"
            ; migration model

            (import (rnrs) (emodl cmslib))

            (start-model ""migration"")

            (locale site-a)
            (set-locale site-a)

            (species A::S 990)
            (species A::E)
            (species A::I 10)
            (species A::R)

            ;(observe susceptibleA A::S)
            ;(observe exposedA     A::E)
            ;(observe infectiousA  A::I)
            ;(observe recoveredA   A::R)
            (observe populationA  (sum A::S A::E A::I A::R))
            ;(observe prevalenceA (/ A::I (sum A::S A::E A::R)))

            (param Ki 0.0005)
            (param Kl 0.2)
            (param Kr (/ 1 7))
            (param Kw (/ 1 135))

            (reaction exposureA   (A::S) (A::E) (* Ki A::S A::I))
            (reaction infectionA  (A::E) (A::I) (* Kl A::E))
            (reaction recoveryA   (A::I) (A::R) (* Kr A::I))
            (reaction waningA     (A::R) (A::S) (* Kw A::R))

            (locale site-b)
            (set-locale site-b)

            (species B::S 100)
            (species B::E)
            (species B::I)
            (species B::R)

            ;(observe susceptibleB B::S)
            ;(observe exposedB     B::E)
            ;(observe infectiousB  B::I)
            ;(observe recoveredB   B::R)
            (observe populationB  (sum B::S B::E B::I B::R))
            ;(observe prevalenceB (/ B::I (sum B::S B::E B::R)))

            (reaction exposureB   (B::S) (B::E) (* Ki B::S B::I))
            (reaction infectionB  (B::E) (B::I) (* Kl B::E))
            (reaction recoveryB   (B::I) (B::R) (* Kr B::I))
            (reaction waningB     (B::R) (B::S) (* Kw B::R))

            ; migration

            (param Km 0.01)

            (reaction SA->SB (A::S) (B::S) (* Km A::S))
            (reaction EA->EB (A::E) (B::E) (* Km A::E))
            (reaction IA->IB (A::I) (B::I) (* Km A::I))
            (reaction RA->RB (A::R) (B::R) (* Km A::R))
                                
            (reaction SB->SA (B::S) (A::S) (* Km B::S 10))
            (reaction EB->EA (B::E) (A::E) (* Km B::E 10))
            (reaction IB->IA (B::I) (A::I) (* Km B::I 10))
            (reaction RB->RA (B::R) (A::R) (* Km B::R 10))

            (end-model)
            ";
            Console.WriteLine("Building model with local and migratory/diffusive reactions...");
            ModelInfo modelInfo = EmodlLoader.LoadEMODLModel(modelDescription);

            var local = from reaction in modelInfo.Reactions where reaction.IsLocal select reaction;
            var migration = from reaction in modelInfo.Reactions where reaction.IsDiffusion select reaction;

            Console.WriteLine("Checking model for local reactions...");
            Expect(local.Count() == 8);
            Console.WriteLine("Checking model for migratory/diffusive reactions...");
            Expect(migration.Count() == 8);

            Console.WriteLine("");
        }

        [Test]
        public void SpeciesInfoTest()
        {
            const string nameOne = "NameOnly";
            var speciesName = new SpeciesDescription(nameOne);

            Console.Write("Created species: ");
            Console.WriteLine(speciesName);
            Console.WriteLine("SpeciesInfoTest: testing named species Name property...");
            Expect(speciesName.Name == nameOne);
            Console.WriteLine("SpeciesInfoTest: testing named species Locale property...");
            Expect(speciesName.Locale == null);

            const string nameTwo = "NameAndPop";
            const int initialPopOne = 2012;
            var speciesNamePop = new SpeciesDescription(nameTwo, initialPopOne);

            Console.Write("Created species: ");
            Console.WriteLine(speciesNamePop);
            Console.WriteLine("SpeciesInfoTest: testing name+pop species Name property...");
            Expect(speciesNamePop.Name == nameTwo);
            Console.WriteLine("SpeciesInfoTest: testing name+pop species Locale property...");
            Expect(speciesNamePop.Locale == null);

            const string nameThree = "NameAndLocale";
            const string localeOneName = "univ";
            var localeOne = new LocaleInfo(localeOneName);
            var speciesNameLocale = new SpeciesDescription(nameThree, localeOne);

            Console.Write("Created species: ");
            Console.WriteLine(speciesNameLocale);
            Console.WriteLine("SpeciesInfoTest: testing name+locale species Name property...");
            Expect(speciesNameLocale.Name == nameThree);
            Console.WriteLine("SpeciesInfoTest: testing name+locale species Locale property...");
            Expect(speciesNameLocale.Locale == localeOne);

            const string nameFour = "NamePopAndLocale";
            const int initialPopTwo = 2112;
            const string localeTwoName = "bellevue";
            var localeTwo = new LocaleInfo(localeTwoName);
            var speciesNamePopLocale = new SpeciesDescription(nameFour, initialPopTwo, localeTwo);

            Console.Write("Created species: ");
            Console.WriteLine(speciesNamePopLocale);
            Console.WriteLine("SpeciesInfoTest: testing name+pop+locale species Name property...");
            Expect(speciesNamePopLocale.Name == nameFour);
            Console.WriteLine("SpeciesInfoTest: testing name+pop+locale species Locale property...");
            Expect(speciesNamePopLocale.Locale == localeTwo);
        }
    }
}
