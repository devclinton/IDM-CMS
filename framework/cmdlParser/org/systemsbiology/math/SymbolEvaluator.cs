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

	/*
	* Abstract class defining the interface for a class that
	* can convert a {@link Symbol} into a floating-point value.
	* An abstract class is used instead of an interface, for
	* reasons of speed.
	*
	* @author Stephen Ramsey
	*/
	public abstract class SymbolEvaluator : System.ICloneable
	{
		virtual public SymbolEvaluationPostProcessor SymbolEvaluationPostProcessor
		{
			get
			{
				return (mSymbolEvaluationPostProcessor);
			}

		}
		//UPGRADE_NOTE: The initialization of  'NULL_ARRAY_INDEX' was moved to static method 'org.systemsbiology.math.SymbolEvaluator'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1005'"
		protected internal static readonly int NULL_ARRAY_INDEX;

		private bool mUseExpressionValueCaching;
		private SymbolEvaluationPostProcessor mSymbolEvaluationPostProcessor;

		public SymbolEvaluator():this(false, null)
		{
		}

		public SymbolEvaluator(bool pUseExpressionValueCaching, SymbolEvaluationPostProcessor pSymbolEvaluationPostProcessor)
		{
			mUseExpressionValueCaching = pUseExpressionValueCaching;
			mSymbolEvaluationPostProcessor = pSymbolEvaluationPostProcessor;
		}

		protected internal double getIndexedValue(int pArrayIndex, Symbol pSymbol)
		{
			double[] doubleArray = pSymbol.mDoubleArray;
			if (null != doubleArray)
			{
				if (null == mSymbolEvaluationPostProcessor)
				{
					return (doubleArray[pArrayIndex]);
				}
				else
				{
					return (mSymbolEvaluationPostProcessor.modifyResult(pSymbol, this, doubleArray[pArrayIndex]));
				}
			}
			else
			{
				if (!mUseExpressionValueCaching)
				{
					if (null == mSymbolEvaluationPostProcessor)
					{
						return (pSymbol.mValueArray[pArrayIndex].getValue(this));
					}
					else
					{
						return (mSymbolEvaluationPostProcessor.modifyResult(pSymbol, this, pSymbol.mValueArray[pArrayIndex].getValue(this)));
					}
				}
				else
				{
					if (null == mSymbolEvaluationPostProcessor)
					{
						return (pSymbol.mValueArray[pArrayIndex].getValueWithCaching(this));
					}
					else
					{
						return (mSymbolEvaluationPostProcessor.modifyResult(pSymbol, this, pSymbol.mValueArray[pArrayIndex].getValueWithCaching(this)));
					}
				}
			}
		}

		/// <summary> Returns null if the symbol corresponds to a numeric value; or
		/// returns the Expression if the symbol corresponds to an Expression;
		/// or throws an exception if the symbol is not defined.
		/// </summary>
		public abstract Expression getExpressionValue(Symbol pSymbol);


		/// <summary> Returns the floating-point value associated with the specified
		/// {@link Symbol}.
		///
		/// </summary>
		/// <param name="pSymbol">the {@link Symbol} object for which the value is to be
		/// returned
		///
		/// </param>
		/// <returns> the floating-point value associated with the specified
		/// symbol.
		/// </returns>
		/*
		* NOTE:  the code in this function is nasty-looking and repetitive because
		* it has been optimized for speed.
		*/
		public double getValue(Symbol pSymbol)
		{
			if (NULL_ARRAY_INDEX == pSymbol.mArrayIndex)
			{
				return getUnindexedValue(pSymbol);
			}
			else
			{
				if (null != pSymbol.mDoubleArray)
				{
					if (null == mSymbolEvaluationPostProcessor)
					{
						return (pSymbol.mDoubleArray[pSymbol.mArrayIndex]);
					}
					else
					{
						return (mSymbolEvaluationPostProcessor.modifyResult(pSymbol, this, pSymbol.mDoubleArray[pSymbol.mArrayIndex]));
					}
				}
				else
				{
					if (!mUseExpressionValueCaching)
					{
						if (null == mSymbolEvaluationPostProcessor)
						{
							return (pSymbol.mValueArray[pSymbol.mArrayIndex].getValue(this));
						}
						else
						{
							return (mSymbolEvaluationPostProcessor.modifyResult(pSymbol, this, pSymbol.mValueArray[pSymbol.mArrayIndex].getValue(this)));
						}
					}
					else
					{
						if (null == mSymbolEvaluationPostProcessor)
						{
							return (pSymbol.mValueArray[pSymbol.mArrayIndex].getValueWithCaching(this));
						}
						else
						{
							return (mSymbolEvaluationPostProcessor.modifyResult(pSymbol, this, pSymbol.mValueArray[pSymbol.mArrayIndex].getValueWithCaching(this)));
						}
					}
				}
			}
		}

		/// <summary> Returns true if the object has a {@link Value} defined,
		/// or false otherwise.
		///
		/// </summary>
		/// <returns> true if the object has a {@link Value} defined,
		/// or false otherwise.
		/// </returns>
		public abstract bool hasValue(Symbol pSymbol);

		public abstract double getUnindexedValue(Symbol pSymbol);
		//UPGRADE_TODO: The following method was automatically generated and it must be implemented in order to preserve the class logic. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1232'"
		abstract public System.Object Clone();
		static SymbolEvaluator()
		{
			NULL_ARRAY_INDEX = Symbol.NULL_ARRAY_INDEX;
		}
	}
}