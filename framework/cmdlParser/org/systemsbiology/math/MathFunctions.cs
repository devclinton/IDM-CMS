using System;
using System.Diagnostics;

namespace org.systemsbiology.math
{
	/*
	* Copyright (C) 2003 by Institute for Systems Biology,
	* Seattle, Washington, USA.  All rights reserved.
	* (Except those portions of code that are copyright
	* Stephen L. Moshier, as specifically indicated herein)
	*
	* This source code is distributed under the GNU Lesser
	* General Public License, the text of which is available at:
	*   http://www.gnu.org/copyleft/lesser.html
	*/

	/// <summary> This class is a collection of useful mathematical functions.</summary>
	public sealed class MathFunctions
	{
		/// <summary> Returns the factorial of an integer argument.
		///
		/// </summary>
		/// <returns> the factorial of an integer argument.
		/// </returns>
		public static long factorial(int pArg)
		{
			Debug.Assert(pArg > 0);

			long retVal = 1L;
			for (int ctr = 1; ctr <= pArg; ++ctr)
			{
				retVal *= ctr;
			}

			return (retVal);
		}

		/// <summary> Returns 0 if the argument is negative, and 1 if the
		/// argument is nonnegative.
		///
		/// </summary>
		/// <returns> 0 if the argument is negative, and 1 if the
		/// argument is nonnegative.
		/// </returns>
		public static double thetaFunction(double pArg)
		{
			double retVal = 0.0;
			if (pArg > 0.0)
			{
				retVal = 1.0;
			}
			return (retVal);
		}

		/// <summary> This function computes N choose M, for small values of M.  It is required
		/// that  N > 0, M >= 0, and M <= N.
		/// [p /]
		/// [b]WARNING:[/b] This implementation will generate an overflow
		/// or underflow for large values of M, but it will work for small values
		/// of M.
		/// </summary>
		public static double chooseFunction(long N, int M)
		{
			if (N < 0)
			{
				throw new System.ArgumentException("invalid parameter for choose function; N=" + N);
			}
			if (M < 0)
			{
				throw new System.ArgumentException("invalid parameter for choose function; M=" + M);
			}
			if (M > N)
			{
				throw new System.ArgumentException("invalid parameters for choose function; M=" + M + "; N=" + N);
			}
			double retVal = 1.0;
			for (long ctr = 0; ctr < M; ++ctr)
			{
				//UPGRD_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
				retVal *= (double) (N - ctr);
			}
			//UPGRD_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			retVal /= ((double) factorial((int) M));
			return (retVal);
		}

		public static readonly double LN10 = System.Math.Log(10.0);

		/// <summary> Returns the logarithm base 10, of the argument.</summary>
		// NOTE:  In Java 1.5, switch to using Math.log10()
		public static double log10(double pArg)
		{
			return (System.Math.Log(pArg) / LN10);
		}

		//J public static void  stats(double[] pVec, MutableDouble pMean, MutableDouble pStdDev)
		public static void  stats(double[] pVec, out double pMean, out double pStdDev)
		{
			int num = pVec.Length;
			if (num <= 1)
			{
				throw new System.ArgumentException("minimum vector length for computing statistics is 2");
			}
			double mean = 0.0;
			for (int i = 0; i < num; ++i)
			{
				mean += pVec[i];
			}
			mean /= ((double) num);
            //J pMean.Value = mean;
            pMean = mean;
            double stdev = 0.0;
			for (int i = 0; i < num; ++i)
			{
				stdev += System.Math.Pow(pVec[i] - mean, 2.0);
			}
			stdev = System.Math.Sqrt(stdev / ((double) num));
            //J pStdDev.Value = stdev;
            pStdDev = stdev;
        }

		public static double sign(double x)
		{
			double retVal = 0.0;
			if (x > 0.0)
			{
				retVal = 1.0;
			}
			else if (x < 0.0)
			{
				retVal = - 1.0;
			}
			return retVal;
		}

		public static double extendedSimpsonsRule(double[] pVals, double pXmin, double pXmax, int pNmin, int pNmax)
		{
			if (pXmax <= pXmin)
			{
				throw new System.ArgumentException("max value must exceed the min value");
			}

			if (pNmax <= pNmin)
			{
				throw new System.ArgumentException("max bin number must exceed the min bin number");
			}

			if (pNmin < 0)
			{
				throw new System.ArgumentException("min bin number must be nonnegative");
			}

			if (pNmax >= pVals.Length || pNmin >= pVals.Length)
			{
				throw new System.ArgumentException("bin range is out of range, for the data array supplied");
			}

			int numBins = pNmax - pNmin + 1;

			if (numBins < 3)
			{
				throw new System.ArgumentException("at least three bins required for using Extended Simpsons Rule");
			}

			double cumSum = 0.0;
			double fac = 0.0;

			double h = (pXmax - pXmin) / ((double) numBins);

			for (int k = numBins; --k >= 0; )
			{
				if (k == numBins - 1)
				{
					fac = 1.0 / 3.0;
				}
				else if (k == 0)
				{
					fac = 1.0 / 3.0;
				}
				else if (k % 2 == 1)
				// this means k+1 is even
				{
					fac = 4.0 / 3.0;
				}
				// this means k+1 is odd
				else
				{
					fac = 2.0 / 3.0;
				}

				cumSum += h * fac * pVals[k + pNmin];
			}

			return cumSum;
		}
	}
}