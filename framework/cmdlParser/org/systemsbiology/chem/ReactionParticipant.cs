using System;
namespace org.systemsbiology.chem
{
	/*
	* Copyright (C) 2003 by Institute for Systems Biology,
	* Seattle, Washington, USA.  All rights reserved.
	*
	* This source code is distributed under the GNU Lesser
	* General Public License, the text of which is available at:
	*   http://www.gnu.org/copyleft/lesser.html
	*/
	public sealed class ReactionParticipant : System.IComparable
	{
		public int Stoichiometry
		{
			get
			{
				return (mStoichiometry);
			}

		}
		public bool Dynamic
		{
			get
			{
				return (mDynamic);
			}

		}
		public Species Species
		{
			get
			{
				return (mSpecies);
			}

		}
		internal Species mSpecies;
		internal int mStoichiometry;
		internal bool mDynamic;

		public sealed class Type
		{
			private System.String mName;
			internal Type(System.String pName)
			{
				mName = pName;
			}

			public override System.String ToString()
			{
				return (mName);
			}

			public static readonly Type REACTANT = new Type("reactant");
			public static readonly Type PRODUCT = new Type("product");
		}

		public ReactionParticipant(Species pSpecies, int pStoichiometry, bool pDynamic)
		{
            if (pSpecies.getValue().IsExpression && pDynamic == true)
			{
				throw new System.ArgumentException("attempt to use a species with a population expression, as a dynamic participant of a reaction");
			}
			if (pStoichiometry < 1)
			{
				throw new System.ArgumentException("illegal stoichiometry value: " + pStoichiometry);
			}

			mStoichiometry = pStoichiometry;
			mSpecies = pSpecies;
			mDynamic = pDynamic;
		}

		public bool equals(ReactionParticipant pReactionParticipant)
		{
			return (mSpecies.equals(pReactionParticipant.mSpecies) && mStoichiometry == pReactionParticipant.mStoichiometry && mDynamic == pReactionParticipant.mDynamic);
		}

		public int CompareTo(System.Object pReactionParticipant)
		{
			return (String.CompareOrdinal(mSpecies.Name, ((ReactionParticipant) pReactionParticipant).mSpecies.Name));
		}

		public override System.String ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("ReactionParticipant[");
			sb.Append(mSpecies.ToString());
			sb.Append(", Stoichiometry: ");
			sb.Append(mStoichiometry);
			sb.Append(", Dynamic: ");
			sb.Append(mDynamic);
			sb.Append("]");
			return (sb.ToString());
		}
	}
}