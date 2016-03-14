/*
Copyright © 1999 CERN - European Organization for Nuclear Research.
Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose
is hereby granted without fee, provided that the above copyright notice appear in all copies and
that both that copyright notice and this permission notice appear in supporting documentation.
CERN makes no representations about the suitability of this software for any purpose.
It is provided "as is" without expressed or implied warranty.*/
using System;
using Polynomial = cern.jet.math.Polynomial;
namespace cern.jet.stat
{
	/// <summary> Gamma and Beta functions.
	/// [p]
	/// [b]Implementation:[/b]
	/// [dt]
	/// Some code taken and adapted from the [A HREF="http://www.sci.usq.edu.au/staff/leighb/graph/Top.html"]Java 2D Graph Package 2.4[/A],
	/// which in turn is a port from the [A HREF="http://people.ne.mediaone.net/moshier/index.html#Cephes"]Cephes 2.2[/A] Math Library (C).
	/// Most Cephes code (missing from the 2D Graph Package) directly ported.
	///
	/// </summary>
	/// <author>  wolfgang.hoschek@cern.ch
	/// </author>
	/// <version>  0.9, 22-Jun-99
	/// </version>
	public class Gamma:cern.jet.math.Constants
	{
		/// <summary> Makes this class non instantiable, but still let's others inherit from it.</summary>
		protected internal Gamma()
		{
		}
		/// <summary> Returns the beta function of the arguments.
		/// [pre]
		/// -     -
		/// | (a) | (b)
		/// beta( a, b )  =  -----------.
		/// -
		/// | (a+b)
		/// [/pre]
		/// </summary>
		static public double beta(double a, double b)
		{
			double y;

			y = a + b;
			y = gamma(y);
			if (y == 0.0)
				return 1.0;

			if (a > b)
			{
				y = gamma(a) / y;
				y *= gamma(b);
			}
			else
			{
				y = gamma(b) / y;
				y *= gamma(a);
			}

			return (y);
		}
		/// <summary> Returns the Gamma function of the argument.</summary>
		static public double gamma(double x)
		{

			double[] P = new double[]{1.60119522476751861407e-4, 1.19135147006586384913e-3, 1.04213797561761569935e-2, 4.76367800457137231464e-2, 2.07448227648435975150e-1, 4.94214826801497100753e-1, 9.99999999999999996796e-1};
			double[] Q = new double[]{- 2.31581873324120129819e-5, 5.39605580493303397842e-4, - 4.45641913851797240494e-3, 1.18139785222060435552e-2, 3.58236398605498653373e-2, - 2.34591795718243348568e-1, 7.14304917030273074085e-2, 1.00000000000000000320e0};
			//double MAXGAM = 171.624376956302725;
			//double LOGPI  = 1.14472988584940017414;

			double p, z;
			double q = System.Math.Abs(x);

			if (q > 33.0)
			{
				if (x < 0.0)
				{
					p = System.Math.Floor(q);
					if (p == q)
						throw new System.ArithmeticException("gamma: overflow");
					z = q - p;
					if (z > 0.5)
					{
						p += 1.0;
						z = q - p;
					}
					z = q * System.Math.Sin(System.Math.PI * z);
					if (z == 0.0)
						throw new System.ArithmeticException("gamma: overflow");
					z = System.Math.Abs(z);
					z = System.Math.PI / (z * stirlingFormula(q));

					return - z;
				}
				else
				{
					return stirlingFormula(x);
				}
			}

			z = 1.0;
			while (x >= 3.0)
			{
				x -= 1.0;
				z *= x;
			}

			while (x < 0.0)
			{
				if (x == 0.0)
				{
					throw new System.ArithmeticException("gamma: singular");
				}
				else if (x > - 1e-9)
				{
					return (z / ((1.0 + 0.5772156649015329 * x) * x));
				}
				z /= x;
				x += 1.0;
			}

			while (x < 2.0)
			{
				if (x == 0.0)
				{
					throw new System.ArithmeticException("gamma: singular");
				}
				else if (x < 1e-9)
				{
					return (z / ((1.0 + 0.5772156649015329 * x) * x));
				}
				z /= x;
				x += 1.0;
			}

			if ((x == 2.0) || (x == 3.0))
				return z;

			x -= 2.0;
			p = Polynomial.polevl(x, P, 6);
			q = Polynomial.polevl(x, Q, 7);
			return z * p / q;
		}
		/// <summary> Returns the Incomplete Beta Function evaluated from zero to [tt]xx[/tt]; formerly named [tt]ibeta[/tt].
		///
		/// </summary>
		/// <param name="aa">the alpha parameter of the beta distribution.
		/// </param>
		/// <param name="bb">the beta parameter of the beta distribution.
		/// </param>
		/// <param name="xx">the integration end point.
		/// </param>
		public static double incompleteBeta(double aa, double bb, double xx)
		{
			double a, b, t, x, xc, w, y;
			bool flag;

			if (aa <= 0.0 || bb <= 0.0)
				throw new System.ArithmeticException("ibeta: Domain error!");

			if ((xx <= 0.0) || (xx >= 1.0))
			{
				if (xx == 0.0)
					return 0.0;
				if (xx == 1.0)
					return 1.0;
				throw new System.ArithmeticException("ibeta: Domain error!");
			}

			flag = false;
			if ((bb * xx) <= 1.0 && xx <= 0.95)
			{
				t = powerSeries(aa, bb, xx);
				return t;
			}

			w = 1.0 - xx;

			/* Reverse a and b if x is greater than the mean. */
			if (xx > (aa / (aa + bb)))
			{
				flag = true;
				a = bb;
				b = aa;
				xc = xx;
				x = w;
			}
			else
			{
				a = aa;
				b = bb;
				xc = w;
				x = xx;
			}

			if (flag && (b * x) <= 1.0 && x <= 0.95)
			{
				t = powerSeries(a, b, x);
				if (t <= MACHEP)
					t = 1.0 - MACHEP;
				else
					t = 1.0 - t;
				return t;
			}

			/* Choose expansion for better convergence. */
			y = x * (a + b - 2.0) - (a - 1.0);
			if (y < 0.0)
				w = incompleteBetaFraction1(a, b, x);
			else
				w = incompleteBetaFraction2(a, b, x) / xc;

			/* Multiply w by the factor
			a      b   _             _     _
			x  (1-x)   | (a+b) / ( a | (a) | (b) ) .   */

			y = a * System.Math.Log(x);
			t = b * System.Math.Log(xc);
			if ((a + b) < MAXGAM && System.Math.Abs(y) < MAXLOG && System.Math.Abs(t) < MAXLOG)
			{
				t = System.Math.Pow(xc, b);
				t *= System.Math.Pow(x, a);
				t /= a;
				t *= w;
				t *= gamma(a + b) / (gamma(a) * gamma(b));
				if (flag)
				{
					if (t <= MACHEP)
						t = 1.0 - MACHEP;
					else
						t = 1.0 - t;
				}
				return t;
			}
			/* Resort to logarithms.  */
			y += t + logGamma(a + b) - logGamma(a) - logGamma(b);
			y += System.Math.Log(w / a);
			if (y < MINLOG)
				t = 0.0;
			else
				t = System.Math.Exp(y);

			if (flag)
			{
				if (t <= MACHEP)
					t = 1.0 - MACHEP;
				else
					t = 1.0 - t;
			}
			return t;
		}
		/// <summary> Continued fraction expansion #1 for incomplete beta integral; formerly named [tt]incbcf[/tt].</summary>
		internal static double incompleteBetaFraction1(double a, double b, double x)
		{
			double xk, pk, pkm1, pkm2, qk, qkm1, qkm2;
			double k1, k2, k3, k4, k5, k6, k7, k8;
			double r, t, ans, thresh;
			int n;

			k1 = a;
			k2 = a + b;
			k3 = a;
			k4 = a + 1.0;
			k5 = 1.0;
			k6 = b - 1.0;
			k7 = k4;
			k8 = a + 2.0;

			pkm2 = 0.0;
			qkm2 = 1.0;
			pkm1 = 1.0;
			qkm1 = 1.0;
			ans = 1.0;
			r = 1.0;
			n = 0;
			thresh = 3.0 * MACHEP;
			do
			{
				xk = (- (x * k1 * k2)) / (k3 * k4);
				pk = pkm1 + pkm2 * xk;
				qk = qkm1 + qkm2 * xk;
				pkm2 = pkm1;
				pkm1 = pk;
				qkm2 = qkm1;
				qkm1 = qk;

				xk = (x * k5 * k6) / (k7 * k8);
				pk = pkm1 + pkm2 * xk;
				qk = qkm1 + qkm2 * xk;
				pkm2 = pkm1;
				pkm1 = pk;
				qkm2 = qkm1;
				qkm1 = qk;

				if (qk != 0)
					r = pk / qk;
				if (r != 0)
				{
					t = System.Math.Abs((ans - r) / r);
					ans = r;
				}
				else
					t = 1.0;

				if (t < thresh)
					return ans;

				k1 += 1.0;
				k2 += 1.0;
				k3 += 2.0;
				k4 += 2.0;
				k5 += 1.0;
				k6 -= 1.0;
				k7 += 2.0;
				k8 += 2.0;

				if ((System.Math.Abs(qk) + System.Math.Abs(pk)) > big)
				{
					pkm2 *= biginv;
					pkm1 *= biginv;
					qkm2 *= biginv;
					qkm1 *= biginv;
				}
				if ((System.Math.Abs(qk) < biginv) || (System.Math.Abs(pk) < biginv))
				{
					pkm2 *= big;
					pkm1 *= big;
					qkm2 *= big;
					qkm1 *= big;
				}
			}
			while (++n < 300);

			return ans;
		}
		/// <summary> Continued fraction expansion #2 for incomplete beta integral; formerly named [tt]incbd[/tt].</summary>
		internal static double incompleteBetaFraction2(double a, double b, double x)
		{
			double xk, pk, pkm1, pkm2, qk, qkm1, qkm2;
			double k1, k2, k3, k4, k5, k6, k7, k8;
			double r, t, ans, z, thresh;
			int n;

			k1 = a;
			k2 = b - 1.0;
			k3 = a;
			k4 = a + 1.0;
			k5 = 1.0;
			k6 = a + b;
			k7 = a + 1.0; ;
			k8 = a + 2.0;

			pkm2 = 0.0;
			qkm2 = 1.0;
			pkm1 = 1.0;
			qkm1 = 1.0;
			z = x / (1.0 - x);
			ans = 1.0;
			r = 1.0;
			n = 0;
			thresh = 3.0 * MACHEP;
			do
			{
				xk = (- (z * k1 * k2)) / (k3 * k4);
				pk = pkm1 + pkm2 * xk;
				qk = qkm1 + qkm2 * xk;
				pkm2 = pkm1;
				pkm1 = pk;
				qkm2 = qkm1;
				qkm1 = qk;

				xk = (z * k5 * k6) / (k7 * k8);
				pk = pkm1 + pkm2 * xk;
				qk = qkm1 + qkm2 * xk;
				pkm2 = pkm1;
				pkm1 = pk;
				qkm2 = qkm1;
				qkm1 = qk;

				if (qk != 0)
					r = pk / qk;
				if (r != 0)
				{
					t = System.Math.Abs((ans - r) / r);
					ans = r;
				}
				else
					t = 1.0;

				if (t < thresh)
					return ans;

				k1 += 1.0;
				k2 -= 1.0;
				k3 += 2.0;
				k4 += 2.0;
				k5 += 1.0;
				k6 += 1.0;
				k7 += 2.0;
				k8 += 2.0;

				if ((System.Math.Abs(qk) + System.Math.Abs(pk)) > big)
				{
					pkm2 *= biginv;
					pkm1 *= biginv;
					qkm2 *= biginv;
					qkm1 *= biginv;
				}
				if ((System.Math.Abs(qk) < biginv) || (System.Math.Abs(pk) < biginv))
				{
					pkm2 *= big;
					pkm1 *= big;
					qkm2 *= big;
					qkm1 *= big;
				}
			}
			while (++n < 300);

			return ans;
		}
		/// <summary> Returns the Incomplete Gamma function; formerly named [tt]igamma[/tt].</summary>
		/// <param name="a">the parameter of the gamma distribution.
		/// </param>
		/// <param name="x">the integration end point.
		/// </param>
		static public double incompleteGamma(double a, double x)
		{


			double ans, ax, c, r;

			if (x <= 0 || a <= 0)
				return 0.0;

			if (x > 1.0 && x > a)
				return 1.0 - incompleteGammaComplement(a, x);

			/* Compute  x**a * exp(-x) / gamma(a)  */
			ax = a * System.Math.Log(x) - x - logGamma(a);
			if (ax < - MAXLOG)
				return (0.0);

			ax = System.Math.Exp(ax);

			/* power series */
			r = a;
			c = 1.0;
			ans = 1.0;

			do
			{
				r += 1.0;
				c *= x / r;
				ans += c;
			}
			while (c / ans > MACHEP);

			return (ans * ax / a);
		}
		/// <summary> Returns the Complemented Incomplete Gamma function; formerly named [tt]igamc[/tt].</summary>
		/// <param name="a">the parameter of the gamma distribution.
		/// </param>
		/// <param name="x">the integration start point.
		/// </param>
		static public double incompleteGammaComplement(double a, double x)
		{
			double ans, ax, c, yc, r, t, y, z;
			double pk, pkm1, pkm2, qk, qkm1, qkm2;

			if (x <= 0 || a <= 0)
				return 1.0;

			if (x < 1.0 || x < a)
				return 1.0 - incompleteGamma(a, x);

			ax = a * System.Math.Log(x) - x - logGamma(a);
			if (ax < - MAXLOG)
				return 0.0;

			ax = System.Math.Exp(ax);

			/* continued fraction */
			y = 1.0 - a;
			z = x + y + 1.0;
			c = 0.0;
			pkm2 = 1.0;
			qkm2 = x;
			pkm1 = x + 1.0;
			qkm1 = z * x;
			ans = pkm1 / qkm1;

			do
			{
				c += 1.0;
				y += 1.0;
				z += 2.0;
				yc = y * c;
				pk = pkm1 * z - pkm2 * yc;
				qk = qkm1 * z - qkm2 * yc;
				if (qk != 0)
				{
					r = pk / qk;
					t = System.Math.Abs((ans - r) / r);
					ans = r;
				}
				else
					t = 1.0;

				pkm2 = pkm1;
				pkm1 = pk;
				qkm2 = qkm1;
				qkm1 = qk;
				if (System.Math.Abs(pk) > big)
				{
					pkm2 *= biginv;
					pkm1 *= biginv;
					qkm2 *= biginv;
					qkm1 *= biginv;
				}
			}
			while (t > MACHEP);

			return ans * ax;
		}
		/// <summary> Returns the natural logarithm of the gamma function; formerly named [tt]lgamma[/tt].</summary>
		public static double logGamma(double x)
		{
			double p, q, w, z;

			double[] A = new double[]{8.11614167470508450300e-4, - 5.95061904284301438324e-4, 7.93650340457716943945e-4, - 2.77777777730099687205e-3, 8.33333333333331927722e-2};
			double[] B = new double[]{- 1.37825152569120859100e3, - 3.88016315134637840924e4, - 3.31612992738871184744e5, - 1.16237097492762307383e6, - 1.72173700820839662146e6, - 8.53555664245765465627e5};
			double[] C = new double[]{- 3.51815701436523470549e2, - 1.70642106651881159223e4, - 2.20528590553854454839e5, - 1.13933444367982507207e6, - 2.53252307177582951285e6, - 2.01889141433532773231e6};

			if (x < - 34.0)
			{
				q = - x;
				w = logGamma(q);
				p = System.Math.Floor(q);
				if (p == q)
					throw new System.ArithmeticException("lgam: Overflow");
				z = q - p;
				if (z > 0.5)
				{
					p += 1.0;
					z = p - q;
				}
				z = q * System.Math.Sin(System.Math.PI * z);
				if (z == 0.0)
					throw new System.ArithmeticException("lgamma: Overflow");
				z = LOGPI - System.Math.Log(z) - w;
				return z;
			}

			if (x < 13.0)
			{
				z = 1.0;
				while (x >= 3.0)
				{
					x -= 1.0;
					z *= x;
				}
				while (x < 2.0)
				{
					if (x == 0.0)
						throw new System.ArithmeticException("lgamma: Overflow");
					z /= x;
					x += 1.0;
				}
				if (z < 0.0)
					z = - z;
				if (x == 2.0)
					return System.Math.Log(z);
				x -= 2.0;
				p = x * Polynomial.polevl(x, B, 5) / Polynomial.p1evl(x, C, 6);
				return (System.Math.Log(z) + p);
			}

			if (x > 2.556348e305)
				throw new System.ArithmeticException("lgamma: Overflow");

			q = (x - 0.5) * System.Math.Log(x) - x + 0.91893853320467274178;
			//if( x > 1.0e8 ) return( q );
			if (x > 1.0e8)
				return (q);

			p = 1.0 / (x * x);
			if (x >= 1000.0)
				q += ((7.9365079365079365079365e-4 * p - 2.7777777777777777777778e-3) * p + 0.0833333333333333333333) / x;
			else
				q += Polynomial.polevl(p, A, 4) / x;
			return q;
		}
		/// <summary> Power series for incomplete beta integral; formerly named [tt]pseries[/tt].
		/// Use when b*x is small and x not too close to 1.
		/// </summary>
		internal static double powerSeries(double a, double b, double x)
		{
			double s, t, u, v, n, t1, z, ai;

			ai = 1.0 / a;
			u = (1.0 - b) * x;
			v = u / (a + 1.0);
			t1 = v;
			t = u;
			n = 2.0;
			s = 0.0;
			z = MACHEP * ai;
			while (System.Math.Abs(v) > z)
			{
				u = (n - b) * x / n;
				t *= u;
				v = t / (a + n);
				s += v;
				n += 1.0;
			}
			s += t1;
			s += ai;

			u = a * System.Math.Log(x);
			if ((a + b) < MAXGAM && System.Math.Abs(u) < MAXLOG)
			{
				t = Gamma.gamma(a + b) / (Gamma.gamma(a) * Gamma.gamma(b));
				s = s * t * System.Math.Pow(x, a);
			}
			else
			{
				t = Gamma.logGamma(a + b) - Gamma.logGamma(a) - Gamma.logGamma(b) + u + System.Math.Log(s);
				if (t < MINLOG)
					s = 0.0;
				else
					s = System.Math.Exp(t);
			}
			return s;
		}
		/// <summary> Returns the Gamma function computed by Stirling's formula; formerly named [tt]stirf[/tt].
		/// The polynomial STIR is valid for 33 <= x <= 172.
		/// </summary>
		internal static double stirlingFormula(double x)
		{
			double[] STIR = new double[]{7.87311395793093628397e-4, - 2.29549961613378126380e-4, - 2.68132617805781232825e-3, 3.47222221605458667310e-3, 8.33333333333482257126e-2};
			double MAXSTIR = 143.01608;

			double w = 1.0 / x;
			double y = System.Math.Exp(x);

			w = 1.0 + w * Polynomial.polevl(w, STIR, 4);

			if (x > MAXSTIR)
			{
				/* Avoid overflow in Math.pow() */
				double v = System.Math.Pow(x, 0.5 * x - 0.25);
				y = v * (v / y);
			}
			else
			{
				y = System.Math.Pow(x, x - 0.5) / y;
			}
			y = SQTPI * y * w;
			return y;
		}
	}
}