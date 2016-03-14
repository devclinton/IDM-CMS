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

namespace org.systemsbiology.math
{

    /// <summary> Represents a {@link Symbol} and an associated
    /// {@link Value}.  It is the base class for many
    /// other classes such as {@link org.systemsbiology.chem.Species}.
    ///
    /// </summary>
    /// <author>  Stephen Ramsey
    /// </author>
    public class SymbolValue : System.IComparable, System.ICloneable
    {
        /// <summary> Accessor for the {@link Symbol} object stored in
        /// this object.
        /// </summary>
        virtual public Symbol Symbol
        {
            get
            {
                return (mSymbol);
            }

        }
        protected internal Value mValue;
        protected internal Symbol mSymbol;

        /// <summary> Constructs a SymbolValue using the
        /// specified string symbol name.  The {@link Value}.
        /// object is set to null.
        /// </summary>
        public SymbolValue(System.String pSymbolName):this(pSymbolName, null)
        {
        }

        public SymbolValue(System.String pSymbolName, double pSymbolValue):this(pSymbolName, new Value(pSymbolValue))
        {
        }


        /// <summary> The copy constructor.</summary>
        public SymbolValue(SymbolValue pSymbolValue)
        {
            mValue = pSymbolValue.mValue;
            mSymbol = pSymbolValue.mSymbol;
        }

        /// <summary> Constructs a SymbolValue using the specified
        /// string symbol name, and the specified {@link Value}
        /// object.
        /// </summary>
        public SymbolValue(System.String pSymbolName, Value pValue)
        {
            mSymbol = new Symbol(pSymbolName);
            setValue(pValue);
        }

        /// <summary> Adds itself to the specified HashMap, using the string symbol
        /// name specified as [code]pSymbolName[/code].
        /// </summary>

        public void addSymbolToMap(Dictionary<String, SymbolValue> pMap, System.String pSymbolName)
        {
            //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
/*
            SymbolValue foundObject = (SymbolValue) pMap[pSymbolName];
            if (null != foundObject)
            {
                if (!foundObject.equals(this))
                {
                    throw new System.SystemException("inconsistent object found with identical symbol names: " + pSymbolName + "; " + foundObject.Symbol.Name);
                }
            }
            else
            {
                pMap[pSymbolName] = this;
            }
*/
            if (pMap.ContainsKey(pSymbolName))
            {
                SymbolValue foundObject = (SymbolValue)pMap[pSymbolName];

                if (!foundObject.equals(this))
                {
                    throw new System.SystemException("inconsistent object found with identical symbol names: " + pSymbolName + "; " + foundObject.Symbol.Name);
                }
            }
            else
            {
                pMap[pSymbolName] = this;
            }
        }


        public void  addSymbolToMap(Dictionary<String, SymbolValue> pMap, System.String pSymbolName, ReservedSymbolMapper pReservedSymbolMapper)
        {
            if (null != pReservedSymbolMapper)
            {
                if (pReservedSymbolMapper.isReservedSymbol(Symbol))
                {
                    throw new System.ArgumentException("reserved symbol used: \"" + pSymbolName + "\"");
                }
            }
            addSymbolToMap(pMap, pSymbolName);
        }

        public bool equals(SymbolValue pSymbolValue)
        {
            return (mValue.Equals(pSymbolValue.mValue) && mSymbol.equals(pSymbolValue.mSymbol));
        }

        /// <summary> Accessor for the {@link Value} object stored in
        /// this object.
        /// </summary>
        public Value getValue()
        {
            return (mValue);
        }

        public virtual void  setValue(Value pValue)
        {
            mValue = pValue;
        }

        public virtual int CompareTo(System.Object pObject)
        {
            return (String.CompareOrdinal(mSymbol.Name, ((SymbolValue) pObject).mSymbol.Name));
        }

        public virtual System.Object Clone()
        {
            SymbolValue retObj = new SymbolValue(mSymbol.Name, (Value) mValue.Clone());
            return (retObj);
        }
    }
}