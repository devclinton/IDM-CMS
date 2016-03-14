/*
* Copyright (C) 2003 by Institute for Systems Biology,
* Seattle, Washington, USA.  All rights reserved.
*
* This source code is distributed under the GNU Lesser
* General Public License, the text of which is available at:
*   http://www.gnu.org/copyleft/lesser.html
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using org.systemsbiology.math;
using org.systemsbiology.util;
namespace org.systemsbiology.chem
{

    /// <summary> A named collection of {@link Reaction} and
    /// {@link Parameter} objects, which represents
    /// a system of interacting chemical {@link Species}.
    /// The chemical Species are contained in the Reaction
    /// objects.
    ///
    /// </summary>
    /// <author>  Stephen Ramsey
    /// </author>
    public sealed class Model
    {
        /// <summary> Associate a {@link ReservedSymbolMapper} with this Model.  This
        /// allows {@link org.systemsbiology.math.Expression expressions} that
        /// reference reserved symbols from the ReservedSymbolMapper.  For
        /// example, in order to have a model in which expressions can reference
        /// the symbol "time", you need to create an instance of
        /// {@link ReservedSymbolMapperChemCommandLanguage} and pass it to this method.
        ///
        /// </summary>
        /// <param name="pReservedSymbolMapper">
        /// </param>
        public ReservedSymbolMapper ReservedSymbolMapper
        {
            get
            {
                return (mReservedSymbolMapper);
            }

            set
            {
                mReservedSymbolMapper = value;
            }
        }

        public SymbolEvaluationPostProcessor SymbolEvaluationPostProcessor
        {
            get
            {
                return (mSymbolEvaluationPostProcessor);
            }

            set
            {
                mSymbolEvaluationPostProcessor = value;
            }
        }

        /// <summary> Sets the model name to be the string contained in [code]pName[/code].
        /// This overrides the model name passed to the constructor.
        /// </summary>
        public System.String Name
        {
            get
            {
                return (mName);
            }

            set
            {
                mName = value;
            }
        }

        internal Dictionary<String, SymbolValue> SymbolsMap
        {
            get
            {
                return (mSymbolsMap);
            }
        }

        internal Dictionary<String, SymbolValue> DynamicSymbolsMap
        {
            get
            {
                return (mDynamicSymbolsMap);
            }
        }

        internal Dictionary<String, Reaction> ReactionsMap
        {
            get
            {
                return (mReactionsMap);
            }
        }

        public System.Collections.ICollection DynamicSymbols
        {
            get
            {
                return (mDynamicSymbolsMap.Values);
            }
        }

        public System.Collections.ICollection Reactions
        {
            get
            {
                return (mReactionsMap.Values);
            }
        }

        public IList<SymbolValue> Symbols
        {
            get
            {
                return new List<SymbolValue>(mSymbolsMap.Values);
            }
        }

        public System.String[] OrderedSpeciesNamesArray
        {
            get
            {
                System.Collections.IList speciesNamesList = new System.Collections.ArrayList();
                System.Collections.IEnumerator symbolValuesIter = mSymbolsMap.Values.GetEnumerator();

                while (symbolValuesIter.MoveNext())
                {

                    SymbolValue symbolValue = (SymbolValue) symbolValuesIter.Current;
                    if (symbolValue is Species)
                    {
                        speciesNamesList.Add(symbolValue.Symbol.Name);
                    }
                }
                SupportClass.CollectionsSupport.Sort(speciesNamesList, null);
                return ((System.String[]) SupportClass.ICollectionSupport.ToArray(speciesNamesList, new System.String[0]));
            }
        }

        public System.String[] OrderedResultsSymbolNamesArray
        {
            get
            {
                System.Collections.IList symbolNamesList = new System.Collections.ArrayList();
                System.Collections.IEnumerator symbolValuesIter = mSymbolsMap.Values.GetEnumerator();
                System.String symbolName = null;

                while (symbolValuesIter.MoveNext())
                {
                    SymbolValue symbolValue = (SymbolValue) symbolValuesIter.Current;
                    if (null == symbolValue.getValue())
                    {
                        throw new System.SystemException("symbol has no value associated with it: " + symbolValue.Symbol.Name);
                    }
                    if ((symbolValue.getValue().IsExpression && !(symbolValue is Reaction)) || symbolValue is Species)
                    {
                        symbolName = symbolValue.Symbol.Name;
                        symbolNamesList.Add(symbolName);
                    }
                }
                SupportClass.CollectionsSupport.Sort(symbolNamesList, null);
                return ((System.String[]) SupportClass.ICollectionSupport.ToArray(symbolNamesList, new System.String[0]));
            }
        }

        public const System.String INTERNAL_SYMBOL_PREFIX = "___";

        private Dictionary<String, Reaction> mReactionsMap;
        private System.String mName;

        private Dictionary<String, SymbolValue> mDynamicSymbolsMap;

        private Dictionary<String, SymbolValue> mSymbolsMap;

        private Dictionary<String, SymbolValue> mParametersMap;
        public const System.String NAMESPACE_IDENTIFIER = "::";
        private SymbolEvaluationPostProcessor mSymbolEvaluationPostProcessor;
        private ReservedSymbolMapper mReservedSymbolMapper;

        public Model()
        {
            mReactionsMap                  = new Dictionary<String, Reaction>();
            mDynamicSymbolsMap             = new Dictionary<String, SymbolValue>();
            mSymbolsMap                    = new Dictionary<String, SymbolValue>();
            mParametersMap                 = new Dictionary<String, SymbolValue>();
            mSymbolEvaluationPostProcessor = null;
            mReservedSymbolMapper          = null;
            Name                           = null;
        }

        public Model(System.String pName):this()
        {
            Name = pName;
        }

        internal System.Collections.ArrayList constructReactionsList()
        {
            Reaction[] sampleArray = new Reaction[0];
            Reaction[] intArray = (Reaction[]) SupportClass.ICollectionSupport.ToArray(ReactionsMap.Values, sampleArray);
            int numReactions = intArray.Length;
            System.Collections.ArrayList reactionsList = new System.Collections.ArrayList();

            for (int reactionCtr = 0; reactionCtr < numReactions; ++reactionCtr)
            {
                reactionsList.Add((Reaction) intArray[reactionCtr].Clone());
            }

            return (reactionsList);
        }

        internal Reaction[] constructReactionsArray()
        {
            Reaction[] sampleArray = new Reaction[0];
            Reaction[] intArray = (Reaction[]) SupportClass.ICollectionSupport.ToArray(ReactionsMap.Values, sampleArray);
            int numReactions = intArray.Length;
            Reaction[] retArray = new Reaction[numReactions];
            for (int reactionCtr = 0; reactionCtr < numReactions; ++reactionCtr)
            {
                retArray[reactionCtr] = (Reaction) intArray[reactionCtr].Clone();
            }

            return (retArray);
        }

        internal System.Collections.ArrayList constructDynamicSymbolsList()
        {
            Species[] sampleArray = new Species[0];
            Species[] intArray = (Species[]) SupportClass.ICollectionSupport.ToArray(DynamicSymbolsMap.Values, sampleArray);
            int numSpecies = intArray.Length;
            System.Collections.ArrayList symbolsList = new System.Collections.ArrayList();
            for (int speciesCtr = 0; speciesCtr < numSpecies; ++speciesCtr)
            {
                symbolsList.Add((Species) intArray[speciesCtr].Clone());
            }

            return (symbolsList);
        }

        internal Species[] constructDynamicSymbolsArray()
        {
            Species[] sampleArray = new Species[0];
            Species[] intArray = (Species[]) SupportClass.ICollectionSupport.ToArray(DynamicSymbolsMap.Values, sampleArray);
            int numSpecies = intArray.Length;
            Species[] retArray = new Species[numSpecies];
            for (int speciesCtr = 0; speciesCtr < numSpecies; ++speciesCtr)
            {
                retArray[speciesCtr] = (Species) intArray[speciesCtr].Clone();
            }

            return (retArray);
        }

        internal SymbolValue[] constructGlobalNonDynamicSymbolsArray()
        {
            Dictionary<String, SymbolValue> symbolsMap = SymbolsMap;

            Dictionary<String, SymbolValue> dynamicSpeciesMap = DynamicSymbolsMap;
            //UPGRADE_TODO: Method 'java.util.HashMap.keySet' was converted to 'SupportClass.HashSetSupport' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapkeySet'"
            System.Collections.IEnumerator symbolsIter = new SupportClass.HashSetSupport(symbolsMap.Keys).GetEnumerator();
            System.Collections.ArrayList retList = new System.Collections.ArrayList();

            while (symbolsIter.MoveNext())
            {

                System.String symbolName = (System.String) symbolsIter.Current;
                //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
                //if (null == dynamicSpeciesMap[symbolName])
                if (!dynamicSpeciesMap.ContainsKey(symbolName))
                {
                    //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
                    SymbolValue symbolValue = (SymbolValue) symbolsMap[symbolName];
                    Debug.Assert(null != symbolValue.getValue(), "null value for symbol: " + symbolName);
                    SymbolValue newSymbolValue = (SymbolValue) symbolValue.Clone();
                    retList.Add(newSymbolValue);
                }
            }
            SymbolValue[] sampleArray = new SymbolValue[0];

            return ((SymbolValue[]) SupportClass.ICollectionSupport.ToArray(retList, sampleArray));
        }

        internal SymbolValue getSymbolByName(System.String pSymbolName)
        {
            //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
            return ((SymbolValue) SymbolsMap[pSymbolName]);
        }

        public void  addParameter(Parameter pParameter)
        {
            pParameter.addSymbolToMap(mSymbolsMap, pParameter.SymbolName, mReservedSymbolMapper);
            pParameter.addSymbolToMap(mParametersMap, pParameter.SymbolName, mReservedSymbolMapper);
        }

        public void  addSpecies(Species pSpecies)
        {
            pSpecies.addSymbolsToGlobalSymbolMap(mSymbolsMap, mReservedSymbolMapper);
        }

        /// <summary> It is illegal to add the reaction with a given name, twice</summary>
        public void  addReaction(Reaction pReaction)
        {
            System.String reactionName = pReaction.Name;

            //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
/*
            Reaction storedReaction = (Reaction) mReactionsMap[reactionName];
            if (null != storedReaction)
            {
                throw new System.SystemException("reaction is already added to this model: " + reactionName);
            }
            else
            {
                mReactionsMap[reactionName] = pReaction;
            }
*/
            if (mReactionsMap.ContainsKey(reactionName))
                throw new SystemException("reaction is already added to this model: " + reactionName);

            mReactionsMap[reactionName] = pReaction;

            pReaction.addDynamicSpeciesToGlobalSpeciesMap(DynamicSymbolsMap, mReservedSymbolMapper);
            pReaction.addSymbolsToGlobalSymbolMap(SymbolsMap, mReservedSymbolMapper);
        }

        public Species getSpeciesByName(System.String pSpeciesName)
        {
            //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
/*
            SymbolValue symbolValue = (SymbolValue) mSymbolsMap[pSpeciesName];
            if (null == symbolValue)
            {
                throw new DataNotFoundException("could not find species: " + pSpeciesName);
            }
*/
            if (!mSymbolsMap.ContainsKey(pSpeciesName))
                throw new DataNotFoundException("could not find species: " + pSpeciesName);

            SymbolValue symbolValue = mSymbolsMap[pSpeciesName];

            if (!(symbolValue is Species))
            {
                throw new System.ArgumentException("requested item is not a species: " + pSpeciesName);
            }

            return ((Species) symbolValue);
        }

        public bool containsDelayedOrMultistepReaction()
        {
            bool containsDelayedOrMultistepReaction = false;

            System.Collections.IEnumerator reactionIter = mReactionsMap.Values.GetEnumerator();

            while (reactionIter.MoveNext())
            {

                Reaction reaction = (Reaction) reactionIter.Current;
                if (reaction.Delay > 0.0 || reaction.NumSteps > 1)
                {
                    containsDelayedOrMultistepReaction = true;
                }
            }

            return (containsDelayedOrMultistepReaction);
        }

        public override System.String ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("Model: ");
            sb.Append(Name);
            sb.Append("\n\n");
            sb.Append("Parameters: \n");
            System.String separatorString = ",\n";
            DebugUtils.describeSortedObjectList(sb, new List<Object>(mParametersMap.Values), separatorString);
            sb.Append("\n\n");
            sb.Append("Compartments: \n");
            DebugUtils.describeSortedObjectList(sb, new List<Object>(mSymbolsMap.Values), typeof(Compartment), separatorString);
            sb.Append("\n\n");
            sb.Append("Species: \n");
            DebugUtils.describeSortedObjectList(sb, new List<Object>(mSymbolsMap.Values), typeof(Species), separatorString);
            sb.Append("\n\n");
            sb.Append("Reactions: \n");
            DebugUtils.describeSortedObjectList(sb, new List<Object>(mReactionsMap.Values), separatorString);
            return (sb.ToString());
        }
    }
}