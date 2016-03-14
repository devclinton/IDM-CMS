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

	/// <summary> Arithmetic functions.</summary>
	public class Arithmetic:Constants
	{
		// for method stirlingCorrection(...)
		private static readonly double[] stirlingCorrection_Renamed_Field = new double[]{0.0, 8.106146679532726e-02, 4.134069595540929e-02, 2.767792568499834e-02, 2.079067210376509e-02, 1.664469118982119e-02, 1.387612882307075e-02, 1.189670994589177e-02, 1.041126526197209e-02, 9.255462182712733e-03, 8.330563433362871e-03, 7.573675487951841e-03, 6.942840107209530e-03, 6.408994188004207e-03, 5.951370112758848e-03, 5.554733551962801e-03, 5.207655919609640e-03, 4.901395948434738e-03, 4.629153749334029e-03, 4.385560249232324e-03, 4.166319691996922e-03, 3.967954218640860e-03, 3.787618068444430e-03, 3.622960224683090e-03, 3.472021382978770e-03, 3.333155636728090e-03, 3.204970228055040e-03, 3.086278682608780e-03, 2.976063983550410e-03, 2.873449362352470e-03, 2.777674929752690e-03};

		// for method logFactorial(...)
		// log(k!) for k = 0, ..., 29
		protected internal static readonly double[] logFactorials = new double[]{0.00000000000000000, 0.00000000000000000, 0.69314718055994531, 1.79175946922805500, 3.17805383034794562, 4.78749174278204599, 6.57925121201010100, 8.52516136106541430, 10.60460290274525023, 12.80182748008146961, 15.10441257307551530, 17.50230784587388584, 19.98721449566188615, 22.55216385312342289, 25.19122118273868150, 27.89927138384089157, 30.67186010608067280, 33.50507345013688888, 36.39544520803305358, 39.33988418719949404, 42.33561646075348503, 45.38013889847690803, 48.47118135183522388, 51.60667556776437357, 54.78472939811231919, 58.00360522298051994, 61.26170176100200198, 64.55753862700633106, 67.88974313718153498, 71.25703896716800901};

		// k! for k = 0, ..., 20
		protected internal static readonly long[] longFactorials = new long[]{1L, 1L, 2L, 6L, 24L, 120L, 720L, 5040L, 40320L, 362880L, 3628800L, 39916800L, 479001600L, 6227020800L, 87178291200L, 1307674368000L, 20922789888000L, 355687428096000L, 6402373705728000L, 121645100408832000L, 2432902008176640000L};

		// k! for k = 21, ..., 170
		protected internal static readonly double[] doubleFactorials = new double[]{5.109094217170944e19, 1.1240007277776077e21, 2.585201673888498e22, 6.204484017332394e23, 1.5511210043330984e25, 4.032914611266057e26, 1.0888869450418352e28, 3.048883446117138e29, 8.841761993739701e30, 2.652528598121911e32, 8.222838654177924e33, 2.6313083693369355e35, 8.68331761881189e36, 2.952327990396041e38, 1.0333147966386144e40, 3.719933267899013e41, 1.3763753091226346e43, 5.23022617466601e44, 2.0397882081197447e46, 8.15915283247898e47, 3.34525266131638e49, 1.4050061177528801e51, 6.041526306337384e52, 2.6582715747884495e54, 1.196222208654802e56, 5.502622159812089e57, 2.5862324151116827e59, 1.2413915592536068e61, 6.082818640342679e62, 3.0414093201713376e64, 1.5511187532873816e66, 8.06581751709439e67, 4.274883284060024e69, 2.308436973392413e71, 1.2696403353658264e73, 7.109985878048632e74, 4.052691950487723e76, 2.350561331282879e78, 1.386831185456898e80, 8.32098711274139e81, 5.075802138772246e83, 3.146997326038794e85, 1.9826083154044396e87, 1.2688693218588414e89, 8.247650592082472e90, 5.443449390774432e92, 3.6471110918188705e94, 2.48003554243683e96, 1.7112245242814127e98, 1.1978571669969892e100, 8.504785885678624e101, 6.123445837688612e103, 4.470115461512686e105, 3.307885441519387e107, 2.4809140811395404e109, 1.8854947016660506e111, 1.451830920282859e113, 1.1324281178206295e115, 8.94618213078298e116, 7.15694570462638e118, 5.797126020747369e120, 4.7536433370128435e122, 3.94552396972066e124, 3.314240134565354e126, 2.8171041143805494e128, 2.4227095383672744e130, 2.107757298379527e132, 1.854826422573984e134, 1.6507955160908465e136, 1.4857159644817605e138, 1.3520015276784033e140, 1.2438414054641305e142, 1.156772507081641e144, 1.0873661566567426e146, 1.0329978488239061e148, 9.916779348709491e149, 9.619275968248216e151, 9.426890448883248e153, 9.332621544394415e155, 9.332621544394418e157, 9.42594775983836e159, 9.614466715035125e161, 9.902900716486178e163, 1.0299016745145631e166, 1.0813967582402912e168, 1.1462805637347086e170,
			1.2265202031961373e172, 1.324641819451829e174, 1.4438595832024942e176, 1.5882455415227423e178, 1.7629525510902457e180, 1.974506857221075e182, 2.2311927486598138e184, 2.543559733472186e186, 2.925093693493014e188, 3.393108684451899e190, 3.96993716080872e192, 4.6845258497542896e194, 5.574585761207606e196, 6.689502913449135e198, 8.094298525273444e200, 9.875044200833601e202, 1.2146304367025332e205, 1.506141741511141e207, 1.882677176888926e209, 2.3721732428800483e211, 3.0126600184576624e213, 3.856204823625808e215, 4.974504222477287e217, 6.466855489220473e219, 8.471580690878813e221, 1.1182486511960037e224, 1.4872707060906847e226, 1.99294274616152e228, 2.690472707318049e230, 3.6590428819525483e232, 5.0128887482749884e234, 6.917786472619482e236, 9.615723196941089e238, 1.3462012475717523e241, 1.8981437590761713e243, 2.6953641378881633e245, 3.8543707171800694e247, 5.550293832739308e249, 8.047926057471989e251, 1.1749972043909107e254, 1.72724589045464e256, 2.5563239178728637e258, 3.8089226376305687e260, 5.7133839564458575e262, 8.627209774233244e264, 1.3113358856834527e267, 2.0063439050956838e269, 3.0897696138473515e271, 4.789142901463393e273, 7.471062926282892e275, 1.1729568794264134e278, 1.8532718694937346e280, 2.946702272495036e282, 4.714723635992061e284, 7.590705053947223e286, 1.2296942187394494e289, 2.0044015765453032e291, 3.287218585534299e293, 5.423910666131583e295, 9.003691705778434e297, 1.5036165148649983e300, 2.5260757449731988e302, 4.2690680090047056e304, 7.257415615308004e306};

		/// <summary> Makes this class non instantiable, but still let's others inherit from it.</summary>
		protected internal Arithmetic()
		{
		}
		/// <summary> Efficiently returns the binomial coefficient, often also referred to as "n over k" or "n choose k".
		/// The binomial coefficient is defined as [tt](n * n-1 * ... * n-k+1 ) / ( 1 * 2 * ... * k )[/tt].
		/// [ul]
		/// [li]k<0[tt]: [tt]0[/tt].
		/// [li]k==0[tt]: [tt]1[/tt].
		/// [li]k==1[tt]: [tt]n[/tt].
		/// [li]else: [tt](n * n-1 * ... * n-k+1 ) / ( 1 * 2 * ... * k )[/tt].
		/// [/ul]
		/// </summary>
		/// <returns> the binomial coefficient.
		/// </returns>
		public static double binomial(double n, long k)
		{
			if (k < 0)
				return 0;
			if (k == 0)
				return 1;
			if (k == 1)
				return n;

			// binomial(n,k) = (n * n-1 * ... * n-k+1 ) / ( 1 * 2 * ... * k )
			double a = n - k + 1;
			double b = 1;
			double binomial = 1;
			for (long i = k; i-- > 0; )
			{
				binomial *= (a++) / (b++);
			}
			return binomial;
		}
		/// <summary> Efficiently returns the binomial coefficient, often also referred to as "n over k" or "n choose k".
		/// The binomial coefficient is defined as
		/// [ul]
		/// [li]k<0[tt]: [tt]0[/tt].
		/// [li]k==0 || k==n[tt]: [tt]1[/tt].
		/// [li]k==1 || k==n-1[tt]: [tt]n[/tt].
		/// [li]else: [tt](n * n-1 * ... * n-k+1 ) / ( 1 * 2 * ... * k )[/tt].
		/// [/ul]
		/// </summary>
		/// <returns> the binomial coefficient.
		/// </returns>
		public static double binomial(long n, long k)
		{
			if (k < 0)
				return 0;
			if (k == 0 || k == n)
				return 1;
			if (k == 1 || k == n - 1)
				return n;

			// try quick version and see whether we get numeric overflows.
			// factorial(..) is O(1); requires no loop; only a table lookup.
			if (n > k)
			{
				int max = longFactorials.Length + doubleFactorials.Length;
				if (n < max)
				{
					// if (n! < inf && k! < inf)
					double n_fac = factorial((int) n);
					double k_fac = factorial((int) k);
					double n_minus_k_fac = factorial((int) (n - k));
					double nk = n_minus_k_fac * k_fac;
					if (nk != System.Double.PositiveInfinity)
					{
						// no numeric overflow?
						// now this is completely safe and accurate
						return n_fac / nk;
					}
				}
				if (k > n / 2)
					k = n - k; // quicker
			}

			// binomial(n,k) = (n * n-1 * ... * n-k+1 ) / ( 1 * 2 * ... * k )
			long a = n - k + 1;
			long b = 1;
			double binomial = 1;
			for (long i = k; i-- > 0; )
			{
				//UPGRD_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
				binomial *= ((double) (a++)) / (b++);
			}
			return binomial;
		}
		/// <summary> Returns the smallest [code]long &gt;= value[/code].
		/// [dt]Examples: [code]1.0 -> 1, 1.2 -> 2, 1.9 -> 2[/code].
		/// This method is safer than using (long) Math.ceil(value), because of possible rounding error.
		/// </summary>
		public static long ceil(double value_Renamed)
		{
			//UPGRADE_TODO: Method 'java.lang.Math.round' was converted to 'System.Math.Round' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javalangMathround_double'"
			return (long) System.Math.Round(System.Math.Ceiling(value_Renamed));
		}
		/// <summary> Evaluates the series of Chebyshev polynomials Ti at argument x/2.
		/// The series is given by
		/// [pre]
		/// N-1
		/// - '
		/// y  =   >   coef[i] T (x/2)
		/// -            i
		/// i=0
		/// [/pre]
		/// Coefficients are stored in reverse order, i.e. the zero
		/// order term is last in the array.  Note N is the number of
		/// coefficients, not the order.
		/// [p]
		/// If coefficients are for the interval a to b, x must
		/// have been transformed to x -> 2(2x - b - a)/(b-a) before
		/// entering the routine.  This maps x from (a, b) to (-1, 1),
		/// over which the Chebyshev polynomials are defined.
		/// [p]
		/// If the coefficients are for the inverted interval, in
		/// which (a, b) is mapped to (1/b, 1/a), the transformation
		/// required is x -> 2(2ab/x - b - a)/(b-a).  If b is infinity,
		/// this becomes x -> 4a/x - 1.
		/// [p]
		/// SPEED:
		/// [p]
		/// Taking advantage of the recurrence properties of the
		/// Chebyshev polynomials, the routine requires one more
		/// addition per loop than evaluating a nested polynomial of
		/// the same degree.
		///
		/// </summary>
		/// <param name="x">argument to the polynomial.
		/// </param>
		/// <param name="coef">the coefficients of the polynomial.
		/// </param>
		/// <param name="N">the number of coefficients.
		/// </param>
		public static double chbevl(double x, double[] coef, int N)
		{
			double b0, b1, b2;

			int p = 0;
			int i;

			b0 = coef[p++];
			b1 = 0.0;
			i = N - 1;

			do
			{
				b2 = b1;
				b1 = b0;
				b0 = x * b1 - b2 + coef[p++];
			}
			while (--i > 0);

			return (0.5 * (b0 - b2));
		}
		/// <summary> Instantly returns the factorial [tt]k![/tt].</summary>
		/// <param name="k">must hold [tt]k &gt;= 0[/tt].
		/// </param>
		static public double factorial(int k)
		{
			if (k < 0)
				throw new System.ArgumentException();

			int length1 = longFactorials.Length;
			if (k < length1)
				return longFactorials[k];

			int length2 = doubleFactorials.Length;
			if (k < length1 + length2)
				return doubleFactorials[k - length1];
			else
				return System.Double.PositiveInfinity;
		}
		/// <summary> Returns the largest [code]long &lt;= value[/code].
		/// [dt]Examples: [code]
		/// 1.0 -> 1, 1.2 -> 1, 1.9 -> 1 [dt]
		/// 2.0 -> 2, 2.2 -> 2, 2.9 -> 2 [/code][dt]
		/// This method is safer than using (long) Math.floor(value), because of possible rounding error.
		/// </summary>
		public static long floor(double value_Renamed)
		{
			//UPGRADE_TODO: Method 'java.lang.Math.round' was converted to 'System.Math.Round' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javalangMathround_double'"
			return (long) System.Math.Round(System.Math.Floor(value_Renamed));
		}
		/// <summary> Returns [tt]log[sub]base[/sub]value[/tt].</summary>
		public static double log(double base_Renamed, double value_Renamed)
		{
			return System.Math.Log(value_Renamed) / System.Math.Log(base_Renamed);
		}
		/// <summary> Returns [tt]log[sub]10[/sub]value[/tt].</summary>
		static public double log10(double value_Renamed)
		{
			// 1.0 / Math.log(10) == 0.43429448190325176
			return System.Math.Log(value_Renamed) * 0.43429448190325176;
		}
		/// <summary> Returns [tt]log[sub]2[/sub]value[/tt].</summary>
		static public double log2(double value_Renamed)
		{
			// 1.0 / Math.log(2) == 1.4426950408889634
			return System.Math.Log(value_Renamed) * 1.4426950408889634;
		}
		/// <summary> Returns [tt]log(k!)[/tt].
		/// Tries to avoid overflows.
		/// For [tt]k<30[/tt] simply looks up a table in O(1).
		/// For [tt]k>=30[/tt] uses stirlings approximation.
		/// </summary>
		/// <param name="k">must hold [tt]k &gt;= 0[/tt].
		/// </param>
		public static double logFactorial(int k)
		{
			if (k >= 30)
			{
				double r, rr;
				double C0 = 9.18938533204672742e-01;
				double C1 = 8.33333333333333333e-02;
				double C3 = - 2.77777777777777778e-03;
				double C5 = 7.93650793650793651e-04;
				double C7 = - 5.95238095238095238e-04;

				r = 1.0 / (double) k;
				rr = r * r;
				return (k + 0.5) * System.Math.Log(k) - k + C0 + r * (C1 + rr * (C3 + rr * (C5 + rr * C7)));
			}
			else
				return logFactorials[k];
		}
		/// <summary> Instantly returns the factorial [tt]k![/tt].</summary>
		/// <param name="k">must hold [tt]k &gt;= 0 && k &lt; 21[/tt].
		/// </param>
		static public long longFactorial(int k)
		{
			if (k < 0)
				throw new System.ArgumentException("Negative k");

			if (k < longFactorials.Length)
				return longFactorials[k];
			throw new System.ArgumentException("Overflow");
		}
		/// <summary> Returns the StirlingCorrection.
		/// [p]
		/// Correction term of the Stirling approximation for [tt]log(k!)[/tt]
		/// (series in 1/k, or table values for small k)
		/// with int parameter k.
		/// [p]
		/// [tt]
		/// log k! = (k + 1/2)log(k + 1) - (k + 1) + (1/2)log(2Pi) +
		/// stirlingCorrection(k + 1)
		/// [p]
		/// log k! = (k + 1/2)log(k)     -  k      + (1/2)log(2Pi) +
		/// stirlingCorrection(k)
		/// [/tt]
		/// </summary>
		public static double stirlingCorrection(int k)
		{
			double C1 = 8.33333333333333333e-02; //  +1/12
			double C3 = - 2.77777777777777778e-03; //  -1/360
			double C5 = 7.93650793650793651e-04; //  +1/1260
			double C7 = - 5.95238095238095238e-04; //  -1/1680

			double r, rr;

			if (k > 30)
			{
				r = 1.0 / (double) k;
				rr = r * r;
				return r * (C1 + rr * (C3 + rr * (C5 + rr * C7)));
			}
			else
				return stirlingCorrection_Renamed_Field[k];
		}
	}
}