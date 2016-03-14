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

using org.systemsbiology.math;
using org.systemsbiology.util;

namespace org.systemsbiology.chem
{

    /// <summary> Represents a chemical process that can take
    /// place, transforming zero or more distinct reactant
    /// {@link Species} into zero or more distinct product
    /// {@link Species}.  Each species that participates in a Reaction
    /// has an integer stoichiometry, specifying the number of molecules
    /// of that species that are consumed or producted in the reaction.
    /// Reactions are typically constructed, and
    /// then added to a {@link Model} object.  Zero reactants [b]and[/b]
    /// zero product species is a degenerate case that
    /// is not allowed.  A Species that participates in a Reaction
    /// is described internally using a {@link ReactionParticipant}
    /// object that specifies the Species and stoichiometry.
    /// Reactions are typically defined with a floating-point rate,
    /// which defines the reaction parameter.  The average rate at
    /// which the reaction is occurring is the product of the reaction
    /// parameter, and the number of distinct combinations of reactant
    /// molecules.  Alternatively, a reaction may have its rate defined
    /// in terms of an {@link org.systemsbiology.math.Expression}, which
    /// is an algebraic expression involving various {@link org.systemsbiology.math.Symbol}
    /// names.  Such symbol names may represent {@link Parameter},
    /// {@link Compartment}, or {@link Species} objects.  Note that
    /// in this case (using an Expression to define the reaction rate), the
    /// expression is simply evaluated to obtain the rate of the reaction;
    /// there is no post-multiplication by the number of reactant combinations.
    ///
    /// </summary>
    /// <author>  Stephen Ramsey
    /// </author>
    public sealed class Reaction:SymbolValue, System.ICloneable
    {
        public System.Collections.ICollection Parameters
        {
            get
            {
                return (mLocalSymbolsValuesMap.Values);
            }

        }
        public int NumSteps
        {
            get
            {
                return (mNumSteps);
            }

            set
            {
                if (value < 1)
                {
                    throw new System.ArgumentException("the number of steps for a reaction must be greater than or equal to one");
                }

                mNumSteps = value;
            }

        }
        public double Delay
        {
            get
            {
                return (mDelay);
            }

            set
            {
                if (value < 0.0)
                {
                    throw new System.ArgumentException("the delay time is invalid");
                }
                mDelay = value;
            }

        }
        public int NumReactants
        {
            get
            {
                return (mReactantsMap.Values.Count);
            }

        }
        public int NumProducts
        {
            get
            {
                return (mProductsMap.Values.Count);
            }

        }
        public int NumLocalSymbols
        {
            get
            {
                return (mLocalSymbolsValuesMap.Values.Count);
            }

        }
        internal SymbolValue[] LocalSymbolValues
        {
            get
            {
                int numSymbolValues = mLocalSymbolsValuesMap.Count;
                SymbolValue[] retArray = new SymbolValue[numSymbolValues];

                SymbolValue[] dummyArray = new SymbolValue[0];
                SymbolValue[] origArray = (SymbolValue[]) SupportClass.ICollectionSupport.ToArray(mLocalSymbolsValuesMap.Values, dummyArray);
                for (int j = 0; j < numSymbolValues; ++j)
                {
                    retArray[j] = (SymbolValue) origArray[j].Clone();
                }
                return (retArray);
            }

        }

        public Dictionary<String, ReactionParticipant> ReactantsMap
        {
            get
            {
                return (mReactantsMap);
            }

        }

        public Dictionary<String, ReactionParticipant> ProductsMap
        {
            get
            {
                return (mProductsMap);
            }

        }

        public System.String Name
        {
            get
            {
                return (mName);
            }

        }

        internal Expression RateExpression
        {
            get
            {
                Expression retVal = null;

                Value rateValue = Rate;

                if (rateValue.IsExpression)
                {
                    retVal = rateValue.ExpressionValue;
                }
                else
                {
                    System.Collections.IEnumerator reactantsIter = mReactantsMap.Values.GetEnumerator();

                    System.Text.StringBuilder expBuf = new System.Text.StringBuilder();

                    bool firstReactant = true;

                    while (reactantsIter.MoveNext())
                    {

                        ReactionParticipant participant = (ReactionParticipant) reactantsIter.Current;
                        Species species = participant.mSpecies;
                        int stoic = participant.mStoichiometry;
                        System.String speciesName = species.Name;
                        if (!firstReactant)
                        {
                            expBuf.Append("*");
                        }
                        else
                        {
                            firstReactant = false;
                        }
                        if (stoic > 1)
                        {
                            expBuf.Append(speciesName + "^" + stoic);
                        }
                        else
                        {
                            expBuf.Append(speciesName);
                        }
                    }


                    double rateVal = rateValue.getValue();
                    Expression rateExp = new Expression(rateVal);
                    if (expBuf.Length > 0)
                    {
                        retVal = Expression.multiply(rateExp, new Expression(expBuf.ToString()));
                    }
                    else
                    {
                        retVal = rateExp;
                    }
                }
                return (retVal);
            }

        }

        private System.String mName;

        private Dictionary<String, ReactionParticipant> mReactantsMap;
        private Dictionary<String, ReactionParticipant> mProductsMap;
        private Dictionary<String, SymbolValue> mLocalSymbolsValuesMap;

        private int mNumSteps;
        private double mDelay;

        private const bool DEFAULT_REACTANT_DYNAMIC = true;

        public override System.Object Clone()
        {
            Reaction reaction = new Reaction(mName);
            reaction.Rate = (Value)Rate.Clone();
            reaction.mReactantsMap = mReactantsMap;
            reaction.mProductsMap = mProductsMap;
            reaction.mLocalSymbolsValuesMap = mLocalSymbolsValuesMap;
            reaction.mNumSteps = mNumSteps;
            reaction.mDelay = mDelay;
            return (reaction);
        }

        public Reaction(System.String pName):base(pName)
        {
            mName = pName;

            mReactantsMap = new Dictionary<String, ReactionParticipant>();
            mProductsMap = new Dictionary<String, ReactionParticipant>();
            mLocalSymbolsValuesMap = new Dictionary<String, SymbolValue>();

            mNumSteps = 1;
            mDelay = 0.0;
        }

        internal bool containsReactant(System.String pReactantName)
        {
            //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
            //return (null != mReactantsMap[pReactantName]);
            return mReactantsMap.ContainsKey(pReactantName);
        }

        public void  addParameter(Parameter pParameter)
        {
            pParameter.addSymbolToMap(mLocalSymbolsValuesMap, pParameter.SymbolName);
        }

        public bool hasLocalSymbols()
        {
            //UPGRADE_TODO: Method 'java.util.HashMap.keySet' was converted to 'SupportClass.HashSetSupport' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapkeySet'"
            return (new SupportClass.HashSetSupport(mLocalSymbolsValuesMap.Keys).Count > 0);
        }

        public int getNumParticipants(ReactionParticipant.Type pParticipantType)
        {
            int numParticipants = 0;
            if (pParticipantType.Equals(ReactionParticipant.Type.REACTANT))
            {
                numParticipants = mReactantsMap.Values.Count;
            }
            else
            {
                numParticipants = mProductsMap.Values.Count;
            }
            return (numParticipants);
        }


        internal static Species getIndexedSpecies(Species pSpecies, System.Collections.Hashtable pSymbolMap, Species[] pDynamicSymbolValues, SymbolValue[] pNonDynamicSymbolValues)
        {
            System.String speciesName = pSpecies.Name;
            //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
            Symbol extSymbol = (Symbol) pSymbolMap[speciesName];
            int extSpeciesIndex = extSymbol.ArrayIndex;
            Species species = null;
            if (null != extSymbol.DoubleArray)
            {
                species = pDynamicSymbolValues[extSpeciesIndex];
            }
            else
            {
                species = (Species) pNonDynamicSymbolValues[extSpeciesIndex];
            }
            return (species);
        }


        public void  constructSpeciesArrays(Species[] pSpeciesArray, int[] pStoichiometryArray, bool[] pDynamicArray, ReactionParticipant.Type pParticipantType)
        {
            constructSpeciesArrays(pSpeciesArray, pStoichiometryArray, pDynamicArray, null, null, null, pParticipantType);
        }


        internal void  constructSpeciesArrays(Species[] pSpeciesArray, int[] pStoichiometryArray, bool[] pDynamicArray, Species[] pDynamicSymbolValues, SymbolValue[] pNonDynamicSymbolValues, System.Collections.Hashtable pSymbolMap, ReactionParticipant.Type pParticipantType)
        {
            System.Collections.ICollection speciesColl = null;
            if (pParticipantType.Equals(ReactionParticipant.Type.REACTANT))
            {
                speciesColl = mReactantsMap.Values;
            }
            else
            {
                speciesColl = mProductsMap.Values;
            }
            int numSpecies = speciesColl.Count;
            if (pSpeciesArray.Length < numSpecies)
            {
                throw new System.ArgumentException("insufficient array size");
            }
            if (pStoichiometryArray.Length < numSpecies)
            {
                throw new System.ArgumentException("insufficient array size");
            }
            System.Collections.IEnumerator speciesIter = speciesColl.GetEnumerator();

            int reactantCtr = 0;

            while (speciesIter.MoveNext())
            {

                ReactionParticipant participant = (ReactionParticipant) speciesIter.Current;
                Species species = participant.mSpecies;
                if (null != pSymbolMap)
                {
                    species = getIndexedSpecies(species, pSymbolMap, pDynamicSymbolValues, pNonDynamicSymbolValues);
                }
                else
                {
                    // do nothing
                }
                pSpeciesArray[reactantCtr] = species;
                pDynamicArray[reactantCtr] = participant.mDynamic;
                int stoic = participant.mStoichiometry;
                pStoichiometryArray[reactantCtr] = stoic;

                reactantCtr++;
            }
        }

        public bool equals(Reaction pReaction)
        {
            return (mName.Equals(pReaction.mName) && SupportClass.EqualsSupport.Equals((System.Collections.ICollection) mReactantsMap, pReaction.mReactantsMap) && SupportClass.EqualsSupport.Equals((System.Collections.ICollection) mProductsMap, pReaction.mProductsMap) && base.equals(pReaction));
        }

        public Value Rate
        {
            get { return getValue(); }
            set { setValue(value); }
        }

        public void  setRate(double pRate)
        {
            Rate = new Value(pRate);
        }

        public void  setRate(Expression pRate)
        {
            Rate = new Value(pRate);
        }


        public void addReactionParticipantToMap(ReactionParticipant pReactionParticipant, Dictionary<String, ReactionParticipant> pMap)
        {
            Species species = pReactionParticipant.Species;
            if (null == species.getValue())
            {
                throw new System.ArgumentException("species has no initial value defined");
            }
            System.String speciesSymbolName = species.Name;
            //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
/*
            ReactionParticipant reactionParticipant = (ReactionParticipant) pMap[speciesSymbolName];
            if (null != reactionParticipant)
            {
                throw new System.SystemException("Species is already defined for this reaction.  Species name: " + speciesSymbolName);
            }
*/
            if (pMap.ContainsKey(speciesSymbolName))
                throw new SystemException("Species is already defined for this reaction.  Species name: " + speciesSymbolName);

            pMap[speciesSymbolName] = pReactionParticipant;
        }

        internal void addReactant(ReactionParticipant pReactionParticipant)
        {
            addReactionParticipantToMap(pReactionParticipant, mReactantsMap);
        }

        public void addReactant(Species pSpecies, int pStoichiometry, bool pDynamic)
        {
            addReactant(new ReactionParticipant(pSpecies, pStoichiometry, pDynamic));
        }

        public void addReactant(Species pSpecies, int pStoichiometry)
        {
            addReactant(new ReactionParticipant(pSpecies, pStoichiometry, DEFAULT_REACTANT_DYNAMIC));
        }


        internal void addProduct(ReactionParticipant pReactionParticipant)
        {
            addReactionParticipantToMap(pReactionParticipant, mProductsMap);
        }

        public void  addProduct(Species pSpecies, int pStoichiometry)
        {
            bool dynamic = true;
            addProduct(new ReactionParticipant(pSpecies, pStoichiometry, dynamic));
        }

        public void  addProduct(Species pSpecies, int pStoichiometry, bool pDynamic)
        {
            addProduct(new ReactionParticipant(pSpecies, pStoichiometry, pDynamic));
        }

        public void  addSpecies(Species pSpecies, int pStoichiometry, bool pDynamic, ReactionParticipant.Type pParticipantType)
        {
            if (pParticipantType.Equals(ReactionParticipant.Type.REACTANT))
            {
                addReactant(pSpecies, pStoichiometry, pDynamic);
            }
            else if (pParticipantType.Equals(ReactionParticipant.Type.PRODUCT))
            {
                addProduct(pSpecies, pStoichiometry, pDynamic);
            }
            else
            {
                throw new System.ArgumentException("unknown reaction participant type: " + pParticipantType);
            }
        }

        public override System.String ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            // ---------------------------------------------------------------------
            // FOR DEBUGGING PURPOSES:
            sb.Append("Reaction: ");

            //UPGRADE_TODO: Method 'java.util.HashMap.keySet' was converted to 'SupportClass.HashSetSupport' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapkeySet'"
            System.Collections.IEnumerator reactantsIter = new SupportClass.HashSetSupport(mReactantsMap.Keys).GetEnumerator();
            sb.Append(Name + ", ");

            while (reactantsIter.MoveNext())
            {

                System.String reactant = (System.String) reactantsIter.Current;
                //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
                ReactionParticipant participant = (ReactionParticipant) mReactantsMap[reactant];
                int stoic = participant.mStoichiometry;
                bool dynamic = participant.mDynamic;
                for (int i = 0; i < stoic; ++i)
                {
                    if (!dynamic)
                    {
                        sb.Append("$");
                    }
                    sb.Append(reactant);
                    if (i < stoic - 1)
                    {
                        sb.Append(" + ");
                    }
                }

                if (reactantsIter.MoveNext())
                {
                    sb.Append(" + ");
                }
            }
            sb.Append(" -> ");
            //UPGRADE_TODO: Method 'java.util.HashMap.keySet' was converted to 'SupportClass.HashSetSupport' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapkeySet'"
            System.Collections.IEnumerator productsIter = new SupportClass.HashSetSupport(ProductsMap.Keys).GetEnumerator();

            while (productsIter.MoveNext())
            {

                System.String product = (System.String) productsIter.Current;
                //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
                ReactionParticipant participant = (ReactionParticipant) mProductsMap[product];
                int stoic = participant.mStoichiometry;
                for (int i = 0; i < stoic; ++i)
                {
                    sb.Append(product);
                    if (i < stoic - 1)
                    {
                        sb.Append(" + ");
                    }
                }

                if (productsIter.MoveNext())
                {
                    sb.Append(" + ");
                }
            }
            sb.Append(", ");
            sb.Append(" [Rate: ");
            sb.Append(Rate.ToString());
            sb.Append("]");
            return (sb.ToString());
        }



        private void addSymbolsFromReactionSpeciesMapToGlobalSymbolMap(Dictionary<String, ReactionParticipant> pReactionSpeciesMap, Dictionary<String, SymbolValue> pSymbolMap, ReservedSymbolMapper pReservedSymbolMapper)
        {
            System.Collections.ICollection speciesCollection = pReactionSpeciesMap.Values;
            System.Collections.IEnumerator speciesIter = speciesCollection.GetEnumerator();

            while (speciesIter.MoveNext())
            {

                ReactionParticipant reactionParticipant = (ReactionParticipant) speciesIter.Current;
                Species species = reactionParticipant.Species;
                species.addSymbolsToGlobalSymbolMap(pSymbolMap, pReservedSymbolMapper);
            }
        }


        internal void addSymbolsToGlobalSymbolMap(Dictionary<String, SymbolValue> pSymbolMap, ReservedSymbolMapper pReservedSymbolMapper)
        {
            addSymbolToMap(pSymbolMap, mName, pReservedSymbolMapper);
            addSymbolsFromReactionSpeciesMapToGlobalSymbolMap(ReactantsMap, pSymbolMap, pReservedSymbolMapper);
            addSymbolsFromReactionSpeciesMapToGlobalSymbolMap(ProductsMap, pSymbolMap, pReservedSymbolMapper);
        }


        private void addDynamicSpeciesFromReactionSpeciesMapToGlobalSpeciesMap(Dictionary<String, ReactionParticipant> pReactionSpecies, Dictionary<String, SymbolValue> pDynamicSpecies, ReservedSymbolMapper pReservedSymbolMapper)
        {
            System.Collections.ICollection speciesCollection = pReactionSpecies.Values;
            System.Collections.IEnumerator speciesIter = speciesCollection.GetEnumerator();

            while (speciesIter.MoveNext())
            {

                ReactionParticipant reactionParticipant = (ReactionParticipant) speciesIter.Current;
                if (reactionParticipant.Dynamic)
                {
                    Species species = reactionParticipant.Species;
                    System.String speciesSymbolName = species.Symbol.Name;
                    species.addSymbolToMap(pDynamicSpecies, speciesSymbolName, pReservedSymbolMapper);
                }
            }
        }

        internal void addDynamicSpeciesToGlobalSpeciesMap(Dictionary<String, SymbolValue> pDynamicSpecies, ReservedSymbolMapper pReservedSymbolMapper)
        {
            addDynamicSpeciesFromReactionSpeciesMapToGlobalSpeciesMap(ReactantsMap, pDynamicSpecies, pReservedSymbolMapper);
            addDynamicSpeciesFromReactionSpeciesMapToGlobalSpeciesMap(ProductsMap, pDynamicSpecies, pReservedSymbolMapper);
        }
    }
}