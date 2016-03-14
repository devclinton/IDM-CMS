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
namespace org.systemsbiology.chem
{

	/// <summary> Represents a named, well-mixed reaction compartment,
	/// which has a numeric volume.  In Model objects constructed using
	/// the {@link ModelBuilderCommandLanguage}, typically only a default
	/// Compartment object (with
	/// unit volume) is used, with the actual volume being absorbed
	/// into the values for the reaction parameters.  However,
	/// Model objects constructed from SBML using the
	/// {@link org.systemsbiology.chem.sbml.ModelBuilderMarkupLanguage}
	/// are special, in that the symbol evaluator implicitly makes
	/// use of the Compartment volume when evaluating a Species symbol
	/// that appears in a [code]kinticLaw[/code] formula.
	///
	/// </summary>
	/// <author>  Stephen Ramsey
	/// </author>
	public sealed class Compartment:SymbolValue, System.ICloneable
	{
		public System.String Name
		{
			get
			{
				return (mName);
			}

		}
		private System.String mName;
		public const double DEFAULT_VOLUME = 1.0;

		public void  setVolume(double pVolume)
		{
			setValue(new Value(pVolume));
		}

		public void  setVolume(Expression pVolume)
		{
			setValue(new Value(pVolume));
		}

		/// <summary> Creates a compartment.  The compartment name may not contain
		/// the NAME_DELIMITER string, which is a single colon character.
		/// </summary>
		public Compartment(System.String pName, double pVolume):base(pName)
		{
			mName = pName;
			setVolume(pVolume);
		}

		public Compartment(SymbolValue pSymbolValue):base(pSymbolValue)
		{
			mName = pSymbolValue.Symbol.Name;
		}

		public Compartment(System.String pName):this(pName, DEFAULT_VOLUME)
		{
		}

		public bool equals(Compartment pCompartment)
		{
			return (mName.Equals(pCompartment.mName) && base.equals(pCompartment));
		}

		public override System.String ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("Compartment: ");
			sb.Append(Name);
			sb.Append(" [Value: ");
			sb.Append(getValue().ToString());
			sb.Append("]");
			return (sb.ToString());
		}

		public override System.Object Clone()
		{
			Compartment compartment = new Compartment(mName);
			compartment.setValue((Value) getValue().Clone());
			return (compartment);
		}
	}
}