/*
* Copyright (C) 2003 by Institute for Systems Biology,
* Seattle, Washington, USA.  All rights reserved.
*
* This source code is distributed under the GNU Lesser
* General Public License, the text of which is available at:
*   http://www.gnu.org/copyleft/lesser.html
*/
using System;
using org.systemsbiology.math;
using org.systemsbiology.util;
namespace org.systemsbiology.chem
{

    /// <summary> Symbol Evaluator class used for chemical simulations.
    ///
    /// </summary>
    /// <author>  Stephen Ramsey
    /// </author>
    public sealed class SymbolEvaluatorChem:SymbolEvaluator
    {
        public ReservedSymbolMapper ReservedSymbolMapper
        {
            get
            {
                return (mReservedSymbolMapper);
            }

        }

        public System.Collections.Hashtable LocalSymbolsMap
        {
            set
            {
                mLocalSymbolsMap = value;
            }

        }
        public double Time
        {
            get
            {
                return (mTime);
            }

            set
            {
                mTime = value;
            }

        }

        internal System.Collections.Hashtable SymbolsMap
        {
            get
            {
                return (mSymbolsMap);
            }

        }

        private System.Collections.Hashtable mSymbolsMap;
        private double mTime;

        private System.Collections.Hashtable mLocalSymbolsMap;
        private ReservedSymbolMapper mReservedSymbolMapper;


        public SymbolEvaluatorChem(bool pUseExpressionValueCaching, SymbolEvaluationPostProcessor pSymbolEvaluationPostProcessor, ReservedSymbolMapper pReservedSymbolMapper, System.Collections.Hashtable pSymbolsMap):base(pUseExpressionValueCaching, pSymbolEvaluationPostProcessor)
        {
            mReservedSymbolMapper = pReservedSymbolMapper;
            mSymbolsMap = pSymbolsMap;
        }

        public override Expression getExpressionValue(Symbol pSymbol)
        {
            Expression retVal = null;
            System.String symbolName = pSymbol.Name;
            //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
/*
            Symbol symbol = (Symbol) mSymbolsMap[symbolName];
            if (null == symbol)
            {
                //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
                symbol = (Symbol) mLocalSymbolsMap[symbolName];
                if (null == symbol)
                {
                    throw new DataNotFoundException("unable to find symbol in symbol map, symbol is \"" + symbolName + "\"");
                }
            }
*/
            Symbol symbol;
            if (!mSymbolsMap.Contains(symbolName))
            {
                if (!mLocalSymbolsMap.ContainsKey(symbolName))
                    throw new DataNotFoundException("unable to find symbol in symbol map, symbol is \"" + symbolName + "\"");

                symbol = (Symbol)mLocalSymbolsMap[symbolName];
            }

            symbol = (Symbol)mSymbolsMap[symbolName];

            Value[] valueArray = symbol.ValueArray;
            int arrayIndex = symbol.ArrayIndex;
            if (NULL_ARRAY_INDEX == arrayIndex)
            {
                throw new System.SystemException("null array index for symbol \"" + symbolName + "\"");
            }
            if (null != valueArray)
            {
                Value value_Renamed = valueArray[arrayIndex];
                if (null == value_Renamed)
                {
                    throw new System.SystemException("unexpected null value for symbol: " + symbolName);
                }
                if (value_Renamed.IsExpression)
                {
                    retVal = value_Renamed.ExpressionValue;
                }
            }
            return (retVal);
        }

        public override double getUnindexedValue(Symbol pSymbol)
        {

            if (NULL_ARRAY_INDEX != pSymbol.ArrayIndex)
            {
                throw new System.SystemException("getUnindexedValue() was called on symbol with non-null array index: " + pSymbol.Name);
            }

            if (null != mReservedSymbolMapper)
            {
                if (mReservedSymbolMapper.isReservedSymbol(pSymbol))
                {
                    return (mReservedSymbolMapper.getReservedSymbolValue(pSymbol, this));
                }
            }

            System.String symbolName = pSymbol.Name;
            Symbol indexedSymbol = null;

            if (null != mLocalSymbolsMap)
            {
                //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
                //indexedSymbol = (Symbol) mLocalSymbolsMap[symbolName];
                if (mLocalSymbolsMap.ContainsKey(symbolName))
                    indexedSymbol = (Symbol)mLocalSymbolsMap[symbolName];
            }

            if (null == indexedSymbol)
            {
                //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
                //indexedSymbol = (Symbol) mSymbolsMap[symbolName];
                if (mSymbolsMap.ContainsKey(symbolName))
                    indexedSymbol = (Symbol)mSymbolsMap[symbolName];
            }

            if (null == indexedSymbol)
            {
                throw new DataNotFoundException("unable to obtain value for symbol: " + symbolName);
            }

            pSymbol.copyIndexInfo(indexedSymbol);

            return (getValue(pSymbol));
        }

        public override bool hasValue(Symbol pSymbol)
        {
            bool hasValue = false;

            if (((mReservedSymbolMapper != null) && mReservedSymbolMapper.isReservedSymbol(pSymbol)) || mSymbolsMap.ContainsKey(pSymbol.Name))
                hasValue = true;

/*
            if (null != mReservedSymbolMapper)
            {
                if (mReservedSymbolMapper.isReservedSymbol(pSymbol))
                {
                    hasValue = true;
                }
            }

            if (!hasValue)
            {
                //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
/ *
                if (null != mSymbolsMap[pSymbol.Name])
                {
                    hasValue = true;
                }
* /
                hasValue = mSymbolsMap.ContainsKey(pSymbol.Name);
            }
*/

            return (hasValue);
        }

        public Symbol getSymbol(System.String pSymbolName)
        {
            //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
            return ((Symbol) mSymbolsMap[pSymbolName]);
        }
        //UPGRADE_TODO: The following method was automatically generated and it must be implemented in order to preserve the class logic. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1232'"
        override public System.Object Clone()
        {
            throw new NotImplementedException();
        }
    }
}