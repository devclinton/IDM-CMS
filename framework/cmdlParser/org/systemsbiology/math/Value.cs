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

    /// <summary> Class that can represent a floating point value,
    /// or an expression representing a floating point value.
    /// An object of this class always contains either an
    /// {@link Expression} object or a {@link MutableDouble} object.
    ///
    /// </summary>
    /// <author>  Stephen Ramsey
    /// </author>
    public sealed class Value : System.ICloneable
    {
        public Expression ExpressionValue
        {
            get
            {
                return (mExpressionValue);
            }

        }
        /// <summary> Returns true if the object has an {@link Expression} object
        /// stored within it, or false otherwise.
        /// </summary>
        public bool IsExpression
        {
            get
            {
                return (null != mExpressionValue);
            }

        }
        private Expression mExpressionValue;
        private bool mExpressionValueCached;
        //J private MutableDouble mNumericValue;
        private double mNumericValue;

        /// <summary> Constructs a {@link Value} composed of the
        /// specified {@link Expression}.
        /// </summary>
        public Value(Expression pExpressionValue)
        {
            setValue(pExpressionValue);
        }

        /// <summary> Constructs a {@link Value} composed of the
        /// specified floating-point value.
        /// </summary>
        public Value(double pNumericValue)
        {
            setValue(pNumericValue);
        }

        /// <summary> Stores the specified floating-point value.</summary>
        public void  setValue(double pValue)
        {
/*J
            if (null == mNumericValue)
            {
                //J mNumericValue = new MutableDouble(pValue);
                mNumericValue = pValue;
            }
            else
            {
                //J mNumericValue.Value = pValue;
                mNumericValue = pValue;
            }
*/
            mNumericValue = pValue;
            mExpressionValue = null;
            mExpressionValueCached = false;
        }

        /// <summary> Stores the specified {@link Expression}.
        /// If this object was constructed using a
        /// floating-point value, an IllegalStateException is thrown.
        /// </summary>
        public void  setValue(Expression pExpressionValue)
        {
            if (null == pExpressionValue)
            {
                throw new System.ArgumentException("null expression object");
            }
            mExpressionValue = pExpressionValue;
            mExpressionValueCached = false;
            //J mNumericValue = new MutableDouble(0.0);
            mNumericValue = 0.0;
        }

        /// <summary> Returns the floating-point value defined for this object.
        /// If the object instead has an {@link Expression} stored
        /// within it, an IllegalStateException is thrown.
        /// </summary>
        public double getValue()
        {
            if (null == mExpressionValue)
            {
                //J return (mNumericValue.doubleValue());
                return mNumericValue;
            }
            else
            {
                throw new System.SystemException("this symbol value is an expression; must provide a SymbolValueMap");
            }
        }

        /// <summary> If this object contains an {@link Expression}, computes
        /// the value of the Expression using the supplied
        /// {@link SymbolEvaluator}; otherwise it returns the
        /// floating-point value stored in the internal
        /// MutableDouble object within this object.
        /// </summary>
        public double getValueWithCaching(SymbolEvaluator pSymbolValueMap)
        {
            Expression expression = mExpressionValue;
            if (null != expression)
            {
                if (mExpressionValueCached)
                {
                    //J return (mNumericValue.mDouble);
                    return mNumericValue;
                }
                else
                {
                    double value_Renamed = expression.computeValue(pSymbolValueMap);
                    mExpressionValueCached = true;
                    //J mNumericValue.Value = value_Renamed;
                    mNumericValue = value_Renamed;
                    return value_Renamed;
                }
            }
            else
            {
                //J return (mNumericValue.mDouble);
                return mNumericValue;
            }
        }

        public double getValue(SymbolEvaluator pSymbolValueMap)
        {
            Expression expression = mExpressionValue;
            if (null != expression)
            {
                double value_Renamed = expression.computeValue(pSymbolValueMap);
                return (value_Renamed);
            }
            else
            {
                //J return (mNumericValue.mDouble);
                return mNumericValue;
            }
        }

        /// <summary> Return a string representation of the
        /// {@link Expression} object stored in this class.
        /// If no expression object is stored within this class,
        /// an IllegalStateException is thrown.
        /// </summary>
        public System.String getExpressionString()
        {
            if (!IsExpression)
            {
                throw new System.SystemException("Value object does not have an Expression defined");
            }
            return (mExpressionValue.ToString());
        }

        public System.String getExpressionString(Expression.SymbolPrinter pSymbolPrinter)
        {
            if (!IsExpression)
            {
                throw new System.SystemException("Value object does not have an Expression defined");
            }
            return (mExpressionValue.toString(pSymbolPrinter));
        }

        public override System.String ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (IsExpression)
            {
                sb.Append("\"");
                sb.Append(mExpressionValue.ToString());
                sb.Append("\"");
            }
            else
            {
                sb.Append(mNumericValue);
            }
            return (sb.ToString());
        }

        public void  clearExpressionValueCache()
        {
            mExpressionValueCached = false;
        }

        public System.Object Clone()
        {
            Value value_Renamed = null;

            if (null != mExpressionValue)
            {
                value_Renamed = new Value((Expression) mExpressionValue.Clone());
            }
            else
            {
                //J value_Renamed = new Value(mNumericValue.doubleValue());
                value_Renamed = new Value(mNumericValue);
            }

            return (value_Renamed);
        }
    }
}