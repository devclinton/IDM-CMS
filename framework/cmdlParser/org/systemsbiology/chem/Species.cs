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

namespace org.systemsbiology.chem
{

	/// <summary> Represents a distinct, named chemical species.  Must
	/// reside in a {@link Compartment}.  The species name
	/// does not have to be globally unique; two species can
	/// have the same name, if they reside in a different
	/// compartment.  The "symbol name" is constructed using
	/// the species name and the compartment name, and it
	/// will be different between the two species.  A species
	/// object has a value associated with it, by virtue of
	/// the superclass {@link org.systemsbiology.math.SymbolValue}.
	/// This value represents the amount of the species present,
	/// in molecules.
	///
	/// </summary>
	/// <author>  Stephen Ramsey
	/// </author>
	public sealed class Species:SymbolValue, System.ICloneable
	{
		public System.String Name
		{
			get
			{
				return (mName);
			}

		}
		public Compartment Compartment
		{
			get
			{
				return (mCompartment);
			}

		}
		private System.String mName; // species name; does not have to be globally unique
		private Compartment mCompartment;

		public Species(System.String pName, Compartment pCompartment):base(pName)
		{
			mName = pName;
			mCompartment = pCompartment;
		}

		public Species(SymbolValue pSymbolValue, Compartment pCompartment):base(pSymbolValue)
		{
			mName = pSymbolValue.Symbol.Name;
			mCompartment = pCompartment;
		}

		public void  setSpeciesPopulation(double pSpeciesPopulation)
		{
			setValue(new Value(pSpeciesPopulation));
		}

		public void  setSpeciesPopulation(Expression pSpeciesPopulation)
		{
			setValue(new Value(pSpeciesPopulation));
		}


        internal void addSymbolsToGlobalSymbolMap(Dictionary<String, SymbolValue> pSymbolMap, ReservedSymbolMapper pReservedSymbolMapper)
		{
			addSymbolToMap(pSymbolMap, Name, pReservedSymbolMapper);
			Compartment compartment = Compartment;
			compartment.addSymbolToMap(pSymbolMap, compartment.Name, pReservedSymbolMapper);
		}

		public bool equals(Species pSpecies)
		{
			return (mName.Equals(pSpecies.mName) && base.equals(pSpecies) && mCompartment.equals(pSpecies.mCompartment));
		}

		public override System.Object Clone()
		{
			Species species = new Species(mName, mCompartment);
			species.setValue((Value) getValue().Clone());
			return (species);
		}

		public override System.String ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("Species: ");
			sb.Append(Name);
			sb.Append(" [Value: ");
			sb.Append(getValue().ToString());
			sb.Append(", Compartment: ");
			sb.Append(Compartment.Name);
			sb.Append("]");
			return (sb.ToString());
		}
	}
}