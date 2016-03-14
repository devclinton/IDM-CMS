using System;
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

	/// <summary> A container class for a [code]boolean[/code] native data type.
	/// This class allows you to alter the [code]boolean[/code] variable
	/// that it contains; it is a fully mutable object.  The purpose
	/// of this class is to provide a mechanism to use [code]boolean[/code]
	/// values as values of a [code]HashMap[/code], while allowing those
	/// values to be mutable as well; this cannot be done with the standard
	/// Java class [code]Boolean[/code], which is immutable.
	///
	/// </summary>
	/// <seealso cref="MutableInteger">
	/// </seealso>
	/// <seealso cref="MutableDouble">
	///
	/// </seealso>
	/// <author>  Stephen Ramsey
	/// </author>
	public sealed class MutableBoolean : System.ICloneable
	{
		public bool Value
		{
			get
			{
				return (mBoolean);
			}

			set
			{
				mBoolean = value;
			}

		}
		private bool mBoolean;

		public bool booleanValue()
		{
			return (mBoolean);
		}

		public MutableBoolean(bool pBoolean)
		{
			Value = pBoolean;
		}

		public System.Object Clone()
		{
			MutableBoolean md = new MutableBoolean(mBoolean);
			return (md);
		}

		public override System.String ToString()
		{
			return (System.Convert.ToString(mBoolean));
		}
	}
}