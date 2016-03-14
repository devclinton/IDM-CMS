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

	/// <summary> Polynomial functions.</summary>
	public class Polynomial:Constants
	{
		/// <summary> Makes this class non instantiable, but still let's others inherit from it.</summary>
		protected internal Polynomial()
		{
		}
		/// <summary> Evaluates the given polynomial of degree [tt]N[/tt] at [tt]x[/tt], assuming coefficient of N is 1.0.
		/// Otherwise same as [tt]polevl()[/tt].
		/// [pre]
		/// 2          N
		/// y  =  C  + C x + C x  +...+ C x
		/// 0    1     2          N
		///
		/// where C  = 1 and hence is omitted from the array.
		/// N
		///
		/// Coefficients are stored in reverse order:
		///
		/// coef[0] = C  , ..., coef[N-1] = C  .
		/// N-1                   0
		///
		/// Calling arguments are otherwise the same as polevl().
		/// [/pre]
		/// In the interest of speed, there are no checks for out of bounds arithmetic.
		///
		/// </summary>
		/// <param name="x">argument to the polynomial.
		/// </param>
		/// <param name="coef">the coefficients of the polynomial.
		/// </param>
		/// <param name="N">the degree of the polynomial.
		/// </param>
		public static double p1evl(double x, double[] coef, int N)
		{
			double ans;

			ans = x + coef[0];

			for (int i = 1; i < N; i++)
			{
				ans = ans * x + coef[i];
			}

			return ans;
		}
		/// <summary> Evaluates the given polynomial of degree [tt]N[/tt] at [tt]x[/tt].
		/// [pre]
		/// 2          N
		/// y  =  C  + C x + C x  +...+ C x
		/// 0    1     2          N
		///
		/// Coefficients are stored in reverse order:
		///
		/// coef[0] = C  , ..., coef[N] = C  .
		/// N                   0
		/// [/pre]
		/// In the interest of speed, there are no checks for out of bounds arithmetic.
		///
		/// </summary>
		/// <param name="x">argument to the polynomial.
		/// </param>
		/// <param name="coef">the coefficients of the polynomial.
		/// </param>
		/// <param name="N">the degree of the polynomial.
		/// </param>
		public static double polevl(double x, double[] coef, int N)
		{
			double ans;
			ans = coef[0];

			for (int i = 1; i <= N; i++)
				ans = ans * x + coef[i];

			return ans;
		}
	}
}