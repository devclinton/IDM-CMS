using System;
using System.Collections.Generic;

using org.systemsbiology.chem.app;
using org.systemsbiology.math;

namespace org.systemsbiology.chem
{

    public class ParserInter
    {

        private Model mod = null;
        private DoOut dO = null;

        // parser specific outputs
        private System.Collections.ArrayList reactionsList = null;
        private SymbolValue[] nonDynamicSymbols = null;
        private System.Collections.ArrayList delayedReactionSolvers = null;
        private System.Collections.ArrayList dynamicSymbolsList = null;
        private Species[] dynamicSpeciesSymbol = null;
        private DelayedReactionSolver[] delayedReactionSolversArray = null;


        // statics obtained from configuration in real app
        private static int MIN_NUM_REACTION_STEPS_FOR_USING_DELAY_FUNCTION = 2;


        public ParserInter(Model m, DoOut d)
        {
            mod = m;
            dO = d;

            //UPGRADE_TODO: The differences in the expected value  of parameters for constructor 'java.io.BufferedReader.BufferedReader'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            //UPGRADE_WARNING: At least one expression was used more than once in the target code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1181'"
            new System.IO.StreamReader(new System.IO.StreamReader(System.Console.OpenStandardInput(), System.Text.Encoding.Default).BaseStream, new System.IO.StreamReader(System.Console.OpenStandardInput(), System.Text.Encoding.Default).CurrentEncoding);
        }


        public virtual void  doStuff()
        {
            dO.dOut("\nDo Stuff\n", true);

            // though no shown here these functions are called in an order
            // that constructs the required arrays for other structures
            dO.dOut("\n\nLook at reactions [enter]", true);
            doReactions();

            dO.dOut("\n\nLook at dynamic symbols [enter]", true);
            doDynamicSymbols();

            dO.dOut("\n\nLook at nondynamic symbols [enter]", true);
            doNonDynamicSymbols();

            dO.dOut("\n\nLook at symbols [enter]", true);
            doSymbolArray();


            dO.dOut("\n\nLook at delayed reaction solvers [enter]", true);
            doDelayedReactionSolver();

            dO.dOut("\n\nLook at species [enter]", true);
            doSpecies();
        }

        private void  doNonDynamicSymbols()
        {
            nonDynamicSymbols = mod.constructGlobalNonDynamicSymbolsArray();
            for (int i = 0, n = 1; i < nonDynamicSymbols.Length; ++i, ++n)
            {
                //UPGRD_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                dO.dOut("nonDynSym[" + n + "]=" + nonDynamicSymbols[i], false);
            }
        }

        private void  doDynamicSymbols()
        {
            dynamicSymbolsList = mod.constructDynamicSymbolsList();
            for (int i = 0, n = 1; i < dynamicSymbolsList.Count; ++i, ++n)
            {
                //UPGRD_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                dO.dOut("dynSym[" + n + "]=" + dynamicSymbolsList[i], false);
            }
        }

        private void  doSpecies()
        {
            dynamicSpeciesSymbol = (Species[]) SupportClass.ICollectionSupport.ToArray(dynamicSymbolsList, new Species[0]);
            for (int i = 0, n = 0; i < dynamicSpeciesSymbol.Length; ++i, ++n)
            {
                dO.dOut("Species[" + n + "]=" + dynamicSpeciesSymbol[i].Name, false);
            }
        }


        private void  doSymbolArray()
        {
            System.String[] symbolArray = mod.OrderedResultsSymbolNamesArray;

            dO.dOut("Number in symbol array is " + symbolArray.Length, true);
            for (int i = 0, n = 1; i < symbolArray.Length; ++i, ++n)
            {
                dO.dOut("Symbol[" + n + "] = " + symbolArray[i], false);
            }
        }

        private void  doReactions()
        {

            reactionsList = mod.constructReactionsList();
            dO.dOut("Number of reactions is " + reactionsList.Count, true);
            for (int i = 0, n = 1; i < reactionsList.Count; ++i, ++n)
            {
                //UPGRD_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                dO.dOut("Reaction[" + n + "] = " + reactionsList[i], false);
            }
        }

        private void  doDelayedReactionSolver()
        {
            delayedReactionSolvers = new System.Collections.ArrayList();
            int numReactions = reactionsList.Count;

            // for each multi-step reaction, split the reaction into two separate reactions
            for (int reactionCtr = 0; reactionCtr < numReactions; ++reactionCtr)
            {
                Reaction reaction = (Reaction) reactionsList[reactionCtr];
                int numSteps = reaction.NumSteps;
                double delay = reaction.Delay;
                if (numSteps == 1 && delay == 0.0)
                {
                    continue;
                }
                //J MutableInteger recursionDepth = new MutableInteger(1);
                int recursionDepth = 1;

                handleDelayedReaction(reaction, reactionsList, reactionCtr, dynamicSymbolsList, delayedReactionSolvers, recursionDepth, true); // is stochastic simulator
            }

            delayedReactionSolversArray = (DelayedReactionSolver[]) SupportClass.ICollectionSupport.ToArray(delayedReactionSolvers, new DelayedReactionSolver[0]);

            for (int i = 0; i < delayedReactionSolversArray.Length; i++)
            {
                dO.dOut("Delay[" + i + "]=" + delayedReactionSolversArray[i].Delay, false);
            }

            /*
            int numDelayedReactions = 0;
            mDelayedReactionSolvers = null;
            if(delayedReactionSolversArray.length > 0)
            {
            mDelayedReactionSolvers = delayedReactionSolversArray;
            numDelayedReactions = mDelayedReactionSolvers.length;
            }
            */
        }

        //J private void  handleDelayedReaction(Reaction pReaction, System.Collections.ArrayList pReactions, int pReactionIndex, System.Collections.ArrayList pDynamicSpecies, System.Collections.ArrayList pDelayedReactionSolvers, MutableInteger pRecursionDepth, bool pIsStochasticSimulator)
        private void  handleDelayedReaction(Reaction pReaction, System.Collections.ArrayList pReactions, int pReactionIndex, System.Collections.ArrayList pDynamicSpecies, System.Collections.ArrayList pDelayedReactionSolvers, int pRecursionDepth, bool pIsStochasticSimulator)
        {
            System.String reactionName = pReaction.Name;

            Dictionary<String, ReactionParticipant> reactantsMap = pReaction.ReactantsMap;
            if (reactantsMap.Count != 1)
            {
                throw new System.SystemException("a multi-step reaction must have excactly one reactant species; reaction is: " + reactionName);
            }


            Dictionary<String, ReactionParticipant> productsMap = pReaction.ProductsMap;
            if (productsMap.Count != 1)
            {
                throw new System.SystemException("a multi-step reaction must have exactly one product species; reaction is: " + reactionName);
            }

            int numSteps = pReaction.NumSteps;

            // clorton Species reactant = (Species) ((ReactionParticipant) reactantsMap.Values.GetEnumerator().Current).Species;
            var reactantsEnumerator = reactantsMap.Values.GetEnumerator();
            reactantsEnumerator.MoveNext();
            Species reactant = ((ReactionParticipant)reactantsEnumerator.Current).Species;

            // clorton Species product = (Species) ((ReactionParticipant) productsMap.Values.GetEnumerator().Current).Species;
            var productsEnumerator = productsMap.Values.GetEnumerator();
            productsEnumerator.MoveNext();
            Species product = ((ReactionParticipant)productsEnumerator.Current).Species;

            if (!reactant.Compartment.equals(product.Compartment))
            {
                throw new System.SystemException("the reactant and product for a multi-step reaction must be the same compartment");
            }

            Value rateValue = pReaction.Rate;
            if (rateValue.IsExpression)
            {
                throw new System.SystemException("a multi-step reaction must have a numeric reaction rate, not a custom rate expression");
            }
            double rate = rateValue.getValue();
            Reaction firstReaction = new Reaction(reactionName);
            firstReaction.setRate(rate);
            firstReaction.addReactant(reactant, 1);

            System.String intermedSpeciesName = new System.Text.StringBuilder(reactionName + "___intermed_species_0").ToString();

            Compartment reactantCompartment = reactant.Compartment;

            Species intermedSpecies = new Species(Model.INTERNAL_SYMBOL_PREFIX + intermedSpeciesName, reactantCompartment);

            intermedSpecies.setSpeciesPopulation(0.0);
            firstReaction.addProduct(intermedSpecies, 1);
            pReactions[pReactionIndex] = firstReaction;
            pDynamicSpecies.Add(intermedSpecies);
            numSteps--;
            if (numSteps > 0 && numSteps < MIN_NUM_REACTION_STEPS_FOR_USING_DELAY_FUNCTION)
            {
                Species lastIntermedSpecies = intermedSpecies;

                for (int ctr = 0; ctr < numSteps; ++ctr)
                {
                    Reaction reaction = new Reaction(Model.INTERNAL_SYMBOL_PREFIX + reactionName + "___multistep_reaction_" + ctr);
                    reaction.setRate(rate);

                    reaction.addReactant(lastIntermedSpecies, 1);

                    if (ctr < numSteps - 1)
                    {
                        intermedSpeciesName = new System.Text.StringBuilder(reactionName + "___intermed_species_" + (ctr + 1)).ToString();
                        intermedSpecies = new Species(Model.INTERNAL_SYMBOL_PREFIX + intermedSpeciesName, reactantCompartment);
                        intermedSpecies.setSpeciesPopulation(0.0);
                        reaction.addProduct(intermedSpecies, 1);
                        pDynamicSpecies.Add(intermedSpecies);
                        lastIntermedSpecies = intermedSpecies;
                    }
                    else
                    {
                        reaction.addProduct(product, 1);
                    }

                    pReactions.Add(reaction);
                }
            }
            else
            {
                Reaction delayedReaction = new Reaction(Model.INTERNAL_SYMBOL_PREFIX + reactionName + "___delayed_reaction");
                delayedReaction.addReactant(intermedSpecies, 1);
                delayedReaction.addProduct(product, 1);
                pReactions.Add(delayedReaction);
                delayedReaction.setRate(rate);
                double delay = 0.0;
                bool isMultistep;
                if (numSteps > 0)
                {
                    delay = (numSteps - 1) / rate;
                    isMultistep = true;
                }
                else
                {
                    delay = pReaction.Delay;
                    isMultistep = false;
                }

                DelayedReactionSolver solver = new DelayedReactionSolver((Species) reactant.Clone(), (Species) intermedSpecies.Clone(), delay, rate, isMultistep, pReactions.Count - 1, pIsStochasticSimulator);
                pDelayedReactionSolvers.Add(solver);
            }
        }
    }
}