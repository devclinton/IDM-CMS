/*
* Copyright (C) 2003 by Institute for Systems Biology,
* Seattle, Washington, USA.  All rights reserved.
*
* This source code is distributed under the GNU Lesser
* General Public License, the text of which is available at:
*   http://www.gnu.org/copyleft/lesser.html
*/
using System;
using DataNotFoundException = org.systemsbiology.util.DataNotFoundException;
namespace org.systemsbiology.math
{

    /// <summary> An implementation of the {@link SymbolEvaluator} abstract
    /// class based on a HashMap.  The string name in the
    /// {@link Symbol} object is used as the key into the HashMap,
    /// and the associated {@link Value} object is the value in the map.
    ///
    /// </summary>
    /// <author>  Stephen Ramsey
    /// </author>
    public class SymbolEvaluatorHashMap:SymbolEvaluator
    {
        protected internal System.Collections.Hashtable mSymbolMap;

        public SymbolEvaluatorHashMap(System.Collections.Hashtable pSymbolMap):base()
        {
            mSymbolMap = pSymbolMap;
        }

        //    public void setSymbolsMap(HashMap pSymbolMap)
        //    {
        //        mSymbolMap = pSymbolMap;
        //    }

        protected internal virtual double getValue(System.String pSymbolName)
        {
            double value_Renamed = 0.0;

/*
            //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
            SymbolValue symbolValue = (SymbolValue) mSymbolMap[pSymbolName];

            if (null != symbolValue)
            {
                Value valueObj = symbolValue.getValue();
                if (null == valueObj)
                {
                    throw new System.SystemException("no value was assigned for symbol: " + symbolValue.Symbol.Name);
                }
                value_Renamed = symbolValue.getValue().getValue(this);
            }
            else
            {
                throw new DataNotFoundException("unable to evaluate symbol: " + pSymbolName);
            }
*/
            if (mSymbolMap.ContainsKey(pSymbolName))
            {
                SymbolValue symbolValue = (SymbolValue)mSymbolMap[pSymbolName];
                Value valueObj = symbolValue.getValue();

                if (null == valueObj)
                {
                    throw new System.SystemException("no value was assigned for symbol: " + symbolValue.Symbol.Name);
                }

                value_Renamed = symbolValue.getValue().getValue(this);
            }
            else
            {
                throw new DataNotFoundException("unable to evaluate symbol: " + pSymbolName);
            }

            return (value_Renamed);
        }

        public override double getUnindexedValue(Symbol pSymbol)
        {
            return (getValue(pSymbol.Name));
        }

        public override bool hasValue(Symbol pSymbol)
        {
            System.String symbolName = pSymbol.Name;
            //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
/*
            SymbolValue symbolValue = (SymbolValue) mSymbolMap[symbolName];
            return (null != symbolValue);
*/
            return mSymbolMap.ContainsKey(symbolName);
        }

        public override Expression getExpressionValue(Symbol pSymbol)
        {
            System.String symbolName = pSymbol.Name;
            //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
            SymbolValue symbolValue = (SymbolValue) mSymbolMap[symbolName];
            if (null == symbolValue)
            {
                throw new DataNotFoundException("unable to find symbol in symbol map, symbol is \"" + symbolName + "\"");
            }
            Value value_Renamed = symbolValue.getValue();
            Expression retVal = null;
            if (null != value_Renamed)
            {
                if (value_Renamed.IsExpression)
                {
                    retVal = value_Renamed.ExpressionValue;
                }
            }
            return (retVal);
        }
        //UPGRADE_TODO: The following method was automatically generated and it must be implemented in order to preserve the class logic. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1232'"
        override public System.Object Clone()
        {
            return null;
        }
    }
}