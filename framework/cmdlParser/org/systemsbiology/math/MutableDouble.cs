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

	/// <summary> A container class for a [code]double[/code] native data type.
	/// This class allows you to alter the [code]double[/code] variable
	/// that it contains; it is a fully mutable object.  The purpose
	/// of this class is to provide a mechanism to use [code]double[/code]
	/// values as values of a [code]HashMap[/code], while allowing those
	/// values to be mutable as well; this cannot be done with the standard
	/// Java class [code]Double[/code], which is immutable.
	///
	/// </summary>
	/// <seealso cref="MutableBoolean">
	/// </seealso>
	/// <seealso cref="MutableInteger">
	///
	/// </seealso>
	/// <author>  Stephen Ramsey
	/// </author>
	public sealed class MutableDouble : System.ICloneable
	{
		public double Value
		{
			get
			{
				return (mDouble);
			}

			set
			{
				mDouble = value;
			}

		}
		internal double mDouble;

		public double doubleValue()
		{
			return (mDouble);
		}

		public MutableDouble(double pDouble)
		{
			Value = pDouble;
		}

		public static int compare(MutableDouble p1, MutableDouble p2)
		{
			return (Double.compare(p1.mDouble, p2.mDouble));
		}

		public System.Object Clone()
		{
			MutableDouble md = new MutableDouble(mDouble);
			return (md);
		}

		public override System.String ToString()
		{
			return (System.Convert.ToString(mDouble));
		}
	}
}