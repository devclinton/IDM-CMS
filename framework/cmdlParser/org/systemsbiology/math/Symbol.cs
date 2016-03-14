using System;
using System.Diagnostics;
namespace org.systemsbiology.math
{
	/*
	* Copyright (C) 2003 by Institute for Systems Biology,
	* Seattle, Washington, USA.  All rights reserved.
	*
	* This source code is distributed under the GNU Lesser
	* General Public License, the text of which is available at:
	*   http://www.gnu.org/copyleft/lesser.html
	*/


	/// <summary> Contains a string identifier and an (optional)
	/// array index.  Used by the {@link SymbolValue} class.
	/// The array index can be used instead of the string identifier,
	/// in order to find the symbol's value.
	///
	/// </summary>
	/// <author>  Stephen Ramsey
	/// </author>
	public sealed class Symbol : System.ICloneable
	{
		public System.String Name
		{
			get
			{
				return (mSymbolName);
			}

		}
		public int ArrayIndex
		{
			get
			{
				return (mArrayIndex);
			}

			set
			{
				mArrayIndex = value;
			}

		}
		public double[] DoubleArray
		{
			get
			{
				return (mDoubleArray);
			}

		}
		public Value[] ValueArray
		{
			get
			{
				return (mValueArray);
			}

		}
		internal System.String mSymbolName;
		// for performance reasons, this field is set to public:
		internal int mArrayIndex;
		internal double[] mDoubleArray;
		internal Value[] mValueArray;

		public const int NULL_ARRAY_INDEX = - 1;

		public Symbol(System.String pSymbolName)
		{
			mSymbolName = pSymbolName;
			clearIndexInfo();
		}

		public bool hasArrayIndex()
		{
			return (NULL_ARRAY_INDEX != mArrayIndex);
		}

		public void  setArray(double[] pArray)
		{
			mDoubleArray = pArray;
			mValueArray = null;
		}

		public void  setArray(Value[] pArray)
		{
			mDoubleArray = null;
			mValueArray = pArray;
		}

		public bool equals(Symbol pSymbol)
		{
			return (mSymbolName.Equals(pSymbol.mSymbolName) && mArrayIndex == pSymbol.mArrayIndex);
		}

		public System.Object Clone()
		{
			Symbol newSymbol = new Symbol(mSymbolName);
			return (newSymbol);
		}

		public void  copyIndexInfo(Symbol pSymbol)
		{
			Debug.Assert(pSymbol.Name.Equals(mSymbolName), "inconsistent symbol names");
			mArrayIndex = pSymbol.mArrayIndex;
			mDoubleArray = pSymbol.mDoubleArray;
			mValueArray = pSymbol.mValueArray;
		}

		public void  clearIndexInfo()
		{
			mArrayIndex = NULL_ARRAY_INDEX;
			mDoubleArray = null;
			mValueArray = null;
		}

		public override System.String ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			//UPGRD_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
			sb.Append(Name + "; index: " + mArrayIndex + "; array: " + mDoubleArray);
			return (sb.ToString());
		}
	}
}