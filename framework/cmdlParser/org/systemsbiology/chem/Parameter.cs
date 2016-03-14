/*
* Copyright (C) 2003 by Institute for Systems Biology,
* Seattle, Washington, USA.  All rights reserved.
*
* This source code is distributed under the GNU Lesser
* General Public License, the text of which is available at:
*   http://www.gnu.org/copyleft/lesser.html
*/
using System;
using SymbolValue = org.systemsbiology.math.SymbolValue;
using Value = org.systemsbiology.math.Value;
using Expression = org.systemsbiology.math.Expression;
namespace org.systemsbiology.chem
{

	public sealed class Parameter:SymbolValue, System.ICloneable
	{
		public System.String SymbolName
		{
			get
			{
				return (mName);
			}

		}
		public System.String Name
		{
			get
			{
				return (mName);
			}

		}
		private System.String mName;

		public Parameter(System.String pName, Expression pValue):base(pName)
		{
			setValue(new Value(pValue));
			mName = pName;
		}

		public Parameter(System.String pName):base(pName)
		{
			mName = pName;
		}

		public Parameter(System.String pName, double pValue):base(pName)
		{
			setValue(new Value(pValue));
			mName = pName;
		}

		public Parameter(SymbolValue pSymbolValue):base(pSymbolValue)
		{
			mName = pSymbolValue.Symbol.Name;
		}

		public void  setValue(Expression pValue)
		{
			setValue(new Value(pValue));
		}

		public void  setValue(double pValue)
		{
			setValue(new Value(pValue));
		}

		public override System.Object Clone()
		{
			Parameter newParam = new Parameter(mName);
			newParam.setValue((Value) mValue.Clone());
			return (newParam);
		}

		public override System.String ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("Parameter: ");
			sb.Append(Name);
			sb.Append(" [Value: ");
			sb.Append(getValue().ToString());
			sb.Append("]");
			return (sb.ToString());
		}
	}
}