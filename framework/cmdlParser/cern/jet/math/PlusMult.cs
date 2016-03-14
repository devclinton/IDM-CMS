/*
Copyright © 1999 CERN - European Organization for Nuclear Research.
Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose
is hereby granted without fee, provided that the above copyright notice appear in all copies and
that both that copyright notice and this permission notice appear in supporting documentation.
CERN makes no representations about the suitability of this software for any purpose.
It is provided "as is" without expressed or implied warranty.*/
using System;
namespace cern.jet.math
{

	/// <summary> Only for performance tuning of compute intensive linear algebraic computations.
	/// Constructs functions that return one of
	/// [ul]
	/// [li][tt]a + b*constant[/tt]
	/// [li][tt]a - b*constant[/tt]
	/// [li][tt]a + b/constant[/tt]
	/// [li][tt]a - b/constant[/tt]
	/// [/ul]
	/// [tt]a[/tt] and [tt]b[/tt] are variables, [tt]constant[/tt] is fixed, but for performance reasons publicly accessible.
	/// Intended to be passed to [tt]matrix.assign(otherMatrix,function)[/tt] methods.
	/// </summary>
	public sealed class PlusMult : cern.colt.function.DoubleDoubleFunction
	{
		/// <summary> Public read/write access to avoid frequent object construction.</summary>
		public double multiplicator;
		/// <summary> Insert the method's description here.
		/// Creation date: (8/10/99 19:12:09)
		/// </summary>
		internal PlusMult(double multiplicator)
		{
			this.multiplicator = multiplicator;
		}
		/// <summary> Returns the result of the function evaluation.</summary>
		public double apply(double a, double b)
		{
			return a + b * multiplicator;
		}
		/// <summary> [tt]a - b/constant[/tt].</summary>
		public static PlusMult minusDiv(double constant)
		{
			return new PlusMult((- 1) / constant);
		}
		/// <summary> [tt]a - b*constant[/tt].</summary>
		public static PlusMult minusMult(double constant)
		{
			return new PlusMult(- constant);
		}
		/// <summary> [tt]a + b/constant[/tt].</summary>
		public static PlusMult plusDiv(double constant)
		{
			return new PlusMult(1 / constant);
		}
		/// <summary> [tt]a + b*constant[/tt].</summary>
		public static PlusMult plusMult(double constant)
		{
			return new PlusMult(constant);
		}
	}
}