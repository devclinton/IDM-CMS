using System;
using System.IO;
using System.Linq;
using compartments.emod;
using compartments.emod.expressions;
using compartments.emod.interfaces;

namespace compartments.cmdl
{
    public class CmdlLoader
    {
        public static ModelInfo LoadCMDLFile(String modelFileName)
        {
            org.systemsbiology.chem.Model cmdlModel = GetModelFromCmdlParser(modelFileName);

            org.systemsbiology.math.SymbolValue[] symValues = GetDisplayCmdlModelSymbols(ref cmdlModel);

            ModelInfo model = TranslateCmdlModel(symValues, ref cmdlModel);

            return model;
        }

        private static org.systemsbiology.chem.Model GetModelFromCmdlParser(String modelFileName)
        {
            org.systemsbiology.chem.IModelBuilder modelBuilder = new org.systemsbiology.chem.ModelBuilderCommandLanguage();
            org.systemsbiology.chem.Model cmdlModel;
            using (var inputStream = new FileStream(modelFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var includeHandler = new org.systemsbiology.util.IncludeHandler
                                         { Directory = new FileInfo(Directory.GetCurrentDirectory()) };
                cmdlModel = modelBuilder.buildModel(inputStream, includeHandler);
            }

            return cmdlModel;
        }

        private static org.systemsbiology.math.SymbolValue[] GetDisplayCmdlModelSymbols(ref org.systemsbiology.chem.Model cmdlModel)
        {
            return cmdlModel.Symbols.ToArray();
        }

        private static ModelInfo TranslateCmdlModel(org.systemsbiology.math.SymbolValue[] symValues, ref org.systemsbiology.chem.Model cmdlModel)
        {
            {
                Console.WriteLine("----- Name: -----");
                String name = cmdlModel.Name;
                Console.WriteLine(name);
                Console.WriteLine();
            }

            var mbuilder = new ModelInfo.ModelBuilder(cmdlModel.Name);

            var compartments = from s in symValues where (s is org.systemsbiology.chem.Compartment) select (org.systemsbiology.chem.Compartment)s;
            Console.WriteLine("----- Compartments: -----");
            foreach (org.systemsbiology.chem.Compartment c in compartments)
            {
                Console.WriteLine(c.ToString());
                mbuilder.AddLocale(new LocaleInfo(c.Name));
            }
            Console.WriteLine();

            mbuilder.AddParameter(new ParameterInfo("time", 0.0f));

            var parameters = from s in symValues where (s is org.systemsbiology.chem.Parameter) select (org.systemsbiology.chem.Parameter)s;
            Console.WriteLine("----- Parameters: -----");
            foreach (org.systemsbiology.chem.Parameter p in parameters)
            {
                Console.WriteLine(p.ToString());
                if (!p.getValue().IsExpression)
                    mbuilder.AddParameter(new ParameterInfo(p.Name, (float)(p.getValue().getValue())));
                else
                {
                    NumericExpressionTree newExpression = ExpressionTreeFromExpression(p.Name, p.getValue().ExpressionValue);
                    mbuilder.AddExpression(newExpression);
                }
            }
            Console.WriteLine();

            var species = (from s in symValues where (s is org.systemsbiology.chem.Species) select (org.systemsbiology.chem.Species)s).ToArray();
            Console.WriteLine("----- Species: -----");
            foreach (org.systemsbiology.chem.Species s in species)
            {
                Console.WriteLine(s.ToString());
                LocaleInfo speciesLocale = mbuilder.Model.Locales.First(li => li.Name == s.Compartment.Name);
                var newSpecies = new SpeciesDescription(s.Name, (int) s.getValue().getValue(), speciesLocale);
                mbuilder.AddSpecies(newSpecies);
                mbuilder.AddObservable(new ObservableInfo(s.Name, new NumericExpressionTree(null, new SymbolReference(newSpecies.Name))));
            }
            Console.WriteLine();

            var reactions = (from s in symValues where (s is org.systemsbiology.chem.Reaction) select (org.systemsbiology.chem.Reaction)s).ToArray();
            Console.WriteLine("----- Reactions: -----");
            foreach (org.systemsbiology.chem.Reaction r in reactions)
            {
                Console.WriteLine(r.ToString());
                ModelInfo m = mbuilder.Model;
                var rbuilder = new ReactionInfo.ReactionBuilder(r.Name);

                foreach (org.systemsbiology.chem.ReactionParticipant participant in r.ReactantsMap.Values)
                {
                    org.systemsbiology.chem.Species cmdlSpecies = participant.Species;
                    SpeciesDescription reactant = m.Species.First(s => s.Name == cmdlSpecies.Name);
                    for (int i = 0; i < participant.Stoichiometry; i++)
                        rbuilder.AddReactant(reactant);
                }

                foreach (org.systemsbiology.chem.ReactionParticipant participant in r.ProductsMap.Values)
                {
                    org.systemsbiology.chem.Species cmdlSpecies = participant.Species;
                    SpeciesDescription product = m.Species.First(s => s.Name == cmdlSpecies.Name);
                    for (int i = 0; i < participant.Stoichiometry; i++)
                        rbuilder.AddProduct(product);
                }

                if (!r.Rate.IsExpression)
                {
                    // Constant rate implies multiplying by the reactant species populations/concentrations
                    INumericOperator rate = new Constant((float)(r.Rate.getValue()));
                    // ReSharper disable LoopCanBeConvertedToQuery
                    foreach (SpeciesDescription s in rbuilder.Reaction.Reactants)
                    {
                        rate = new MultiplyOperator(rate, new SymbolReference(s.Name));
                    }
                    // ReSharper restore LoopCanBeConvertedToQuery
                    rbuilder.SetRate(new NumericExpressionTree(null, rate));
                }
                else
                {
                    NumericExpressionTree newExpression = ExpressionTreeFromExpression(null, r.Rate.ExpressionValue);
                    mbuilder.AddExpression(newExpression);
                    rbuilder.SetRate(newExpression);
                }

                // ReSharper disable CompareOfFloatsByEqualityOperator
                if (r.Delay != 0)
                {
                    var delay = new NumericExpressionTree(null, new Constant((float) r.Delay));
                    rbuilder.SetDelay(delay);
                }
                // ReSharper restore CompareOfFloatsByEqualityOperator

                mbuilder.AddReaction(rbuilder.Reaction);
            }
            Console.WriteLine();

            ModelInfo model = mbuilder.Model;

            return model;
        }

        private static NumericExpressionTree ExpressionTreeFromExpression(string name, org.systemsbiology.math.Expression cmdlExpression)
        {
            return new NumericExpressionTree(name, OperatorFromElement(cmdlExpression.RootElement));
        }

        private static INumericOperator OperatorFromElement(org.systemsbiology.math.Expression.Element cmdlElement)
        {
            INumericOperator value;

            switch (cmdlElement.mCode.mIntCode)
            {
                case org.systemsbiology.math.Expression.ElementCode.ELEMENT_CODE_ABS:
                    value = new AbsoluteOperator(OperatorFromElement(cmdlElement.mFirstOperand));
                    break;
/* NOT YET
                case org.systemsbiology.math.Expression.ElementCode.ELEMENT_CODE_ACOS:
                    value = new ArcCosineOperator(ValueFromElement(cmdlElement.mFirstOperand));
                    break;
*/
                case org.systemsbiology.math.Expression.ElementCode.ELEMENT_CODE_ADD:
                    value = new AddOperator(OperatorFromElement(cmdlElement.mFirstOperand), OperatorFromElement(cmdlElement.mSecondOperand));
                    break;
/* NOT YET
                case org.systemsbiology.math.Expression.ElementCode.ELEMENT_CODE_ASIN:
                    value = new ArcSineOperator(ValueFromElement(cmdlElement.mFirstOperand));
                    break;

                case org.systemsbiology.math.Expression.ElementCode.ELEMENT_CODE_ATAN:
                    value = new ArcTangentOperator(ValueFromElement(cmdlElement.mFirstOperand));
                    break;
*/
                case org.systemsbiology.math.Expression.ElementCode.ELEMENT_CODE_CEIL:
                    value = new CeilingOperator(OperatorFromElement(cmdlElement.mFirstOperand));
                    break;

                case org.systemsbiology.math.Expression.ElementCode.ELEMENT_CODE_COS:
                    value = new CosineOperator(OperatorFromElement(cmdlElement.mFirstOperand));
                    break;

                case org.systemsbiology.math.Expression.ElementCode.ELEMENT_CODE_DIV:
                    value = new DivideOperator(OperatorFromElement(cmdlElement.mFirstOperand), OperatorFromElement(cmdlElement.mSecondOperand));
                    break;

                case org.systemsbiology.math.Expression.ElementCode.ELEMENT_CODE_EXP:
                    value = new ExponentiationOperator(OperatorFromElement(cmdlElement.mFirstOperand));
                    break;

                case org.systemsbiology.math.Expression.ElementCode.ELEMENT_CODE_FLOOR:
                    value = new FloorOperator(OperatorFromElement(cmdlElement.mFirstOperand));
                    break;

                case org.systemsbiology.math.Expression.ElementCode.ELEMENT_CODE_LN:
                    value = new LogarithmOperator(OperatorFromElement(cmdlElement.mFirstOperand));
                    break;

                case org.systemsbiology.math.Expression.ElementCode.ELEMENT_CODE_MAX:
                    value = new MaximumOperator(OperatorFromElement(cmdlElement.mFirstOperand), OperatorFromElement(cmdlElement.mSecondOperand));
                    break;

                case org.systemsbiology.math.Expression.ElementCode.ELEMENT_CODE_MIN:
                    value = new MinimumOperator(OperatorFromElement(cmdlElement.mFirstOperand), OperatorFromElement(cmdlElement.mSecondOperand));
                    break;
/* NOT YET
                case org.systemsbiology.math.Expression.ElementCode.ELEMENT_CODE_MOD:
                    value = new ModuloOperator(ValueFromElement(cmdlElement.mFirstOperand), ValueFromElement(cmdlElement.mSecondOperand));
                    break;
*/
                case org.systemsbiology.math.Expression.ElementCode.ELEMENT_CODE_MULT:
                    value = new MultiplyOperator(OperatorFromElement(cmdlElement.mFirstOperand), OperatorFromElement(cmdlElement.mSecondOperand));
                    break;

                case org.systemsbiology.math.Expression.ElementCode.ELEMENT_CODE_NEG:
                    value = new NegateOperator(OperatorFromElement(cmdlElement.mFirstOperand));
                    break;

                case org.systemsbiology.math.Expression.ElementCode.ELEMENT_CODE_NUMBER:
                    value = new Constant((float)cmdlElement.mNumericValue);
                    break;

                case org.systemsbiology.math.Expression.ElementCode.ELEMENT_CODE_POW:
                    value = new PowerOperator(OperatorFromElement(cmdlElement.mFirstOperand), OperatorFromElement(cmdlElement.mSecondOperand));
                    break;

                case org.systemsbiology.math.Expression.ElementCode.ELEMENT_CODE_SIN:
                    value = new SineOperator(OperatorFromElement(cmdlElement.mFirstOperand));
                    break;

                case org.systemsbiology.math.Expression.ElementCode.ELEMENT_CODE_SQRT:
                    value = new SqrtOperator(OperatorFromElement(cmdlElement.mFirstOperand));
                    break;

                case org.systemsbiology.math.Expression.ElementCode.ELEMENT_CODE_SUBT:
                    value = new SubtractOperator(OperatorFromElement(cmdlElement.mFirstOperand), OperatorFromElement(cmdlElement.mSecondOperand));
                    break;

                case org.systemsbiology.math.Expression.ElementCode.ELEMENT_CODE_SYMBOL:
                    value = new SymbolReference(cmdlElement.mSymbol.Name);
                    break;
/* NOT YET
                case org.systemsbiology.math.Expression.ElementCode.ELEMENT_CODE_TAN:
                    value = new TangentOperator(ValueFromElement(cmdlElement.mFirstOperand));
                    break;
*/
                case org.systemsbiology.math.Expression.ElementCode.ELEMENT_CODE_THETA:
                    value = new HeavisideStepOperator(OperatorFromElement(cmdlElement.mFirstOperand));
                    break;

                /* NOT IMPL
                case org.systemsbiology.math.Expression.ElementCode.ELEMENT_CODE_PAIR:
                case org.systemsbiology.math.Expression.ElementCode.ELEMENT_CODE_NONE:
                */
                default:
                    throw new ApplicationException();
            }

            return value;
        }
    }
}
