using System;
using distlib.randomvariates;

namespace distlib.samplers
{
    public class RandLibSampler : DistributionSampler
    {
        public RandomVariateGenerator VariateGenerator { get; private set; }

        public static DistributionSampler CreateRandLibSampler(RandomVariateGenerator generator)
        {
            var sampler = new RandLibSampler {VariateGenerator = generator};

            return sampler;
        }

        private RandLibSampler()
        {
        }

        public double GenerateUniformOO()
        {
            return VariateGenerator.GenerateUniformOO();
        }

        public double GenerateUniformOO(double min, double max)
        {
            return min + (VariateGenerator.GenerateUniformOO()*(max - min));
        }

        public double GenerateUniformOC()
        {
            return VariateGenerator.GenerateUniformOC();
        }

        public double GenerateUniformOC(double min, double max)
        {
            // Use the algorithm below rather than
            // min + (VariateGenerator.GenerateUniformOC()*(max - min))
            // to ensure that floating point precision issues don't
            // prevent returning max.
            double sample = VariateGenerator.GenerateUniformOC();
            double oneMinus = 1.0 - sample;
            double delta = max - min;
            double offset = oneMinus*delta;
            double result = max - offset;

            return result;
        }

        public double GenerateUniformCO()
        {
            return VariateGenerator.GenerateUniformCO();
        }

        public double GenerateUniformCO(double min, double max)
        {
            return min + (VariateGenerator.GenerateUniformCO()*(max - min));
        }

        public double GenerateUniformCC()
        {
            return VariateGenerator.GenerateUniformCC();
        }

        public double GenerateUniformCC(double min, double max)
        {
            // Use the algorithm below rather than
            // min + (VariateGenerator.GenerateUniformCC()*(max - min))
            // to ensure that floating point precision issues don't
            // prevent returning max.
            double sample = VariateGenerator.GenerateUniformCC();

            // Floating point precision issues may prevent sample = 0.0
            // from causing min to be returned.
            if (sample == 0.0)
            {
                return min;
            }

            double oneMinus = 1.0 - sample;
            double delta = max - min;
            double offset = oneMinus * delta;
            double result = max - offset;

            return result;
        }

        /*
         * AHRENS, J.H. AND DIETER, U.                  
         * EXTENSIONS OF FORSYTHE'S METHOD FOR RANDOM SAMPLING FROM THE NORMAL DISTRIBUTION.
         * MATH. COMPUT., 27,124 (OCT. 1973), 927 - 937.
         * 
         * Algorithm FL for m=5
        */
        public double StandardNormal()
        {
            double uStar;
            double a;
            double w;
            double t;

            bool finished = false;

            //  1 Generate u. Store the first bit of u as a sign s. Left-shift u by 1 bit (u = 2u - s).
            double u;
            int s;
            do
            {
                u = VariateGenerator.GenerateUniformOO();
                s = (u >= 0.5) ? 1 : 0;
                u += u - s;
            } while (u == 0.0);

            //  2 Shift u to the left by m bits (u = 2^m*u), i = [u]. If i == 0 goto 9
            u *= 32;
            var i = (int)u;

            if (i != 0)
            {
                //  3 (Start center) Set uStar = u - i and a = a[i]
                uStar = u - i;
                a = SnA[i];

                //  4 If uStar > t[i] set w = (uStar - t[i])*h[i] and go to 17
                if (uStar > SnT[i])
                {
                    w = (uStar - SnT[i]) * SnH[i];
                }
                else
                {
                    Step5(a, i, VariateGenerator.GenerateUniformOO(), out w, out t);

                    do
                    {
                        //  6 If uStar > t go to 17
                        if (uStar > t)
                        {
                            finished = true;
                        }
                        else
                        {
                            //  7 Generate u. If uStar < u generate uStar, go to 4
                            u = VariateGenerator.GenerateUniformOO();
                            if (uStar < u)
                            {
                                uStar = VariateGenerator.GenerateUniformOO();

                                //  4 If uStar > t[i] set w = (uStar - t[i])*h[i] and go to 17
                                if (uStar > SnT[i])
                                {
                                    w = (uStar - SnT[i]) * SnH[i];
                                    finished = true;
                                }
                                else
                                {
                                    Step5(a, i, VariateGenerator.GenerateUniformOO(), out w, out t);
                                }
                            }
                            else
                            {
                                //  8 t = u, generate uStar, go to 6
                                t = u;
                                uStar = VariateGenerator.GenerateUniformOO();
                            }
                        }
                    } while (!finished);
                }
            }
            else
            {
                //  9 (Start tail) i = m + 1, a = a2[m]
                i = 6;          // m == 5
                a = SnA[32];    // 1 based indexing

                do
                {
                    // 10 u = 2u, if u >= 1 to to 12
                    u *= 2;
                    if (u < 1.0)
                    {
                        // 11 a = a + d[i], i = i + 1, goto 10
                        a += SnD[i];
                        i++;
                    }
                } while (u < 1.0);

                // 12 u = u - 1
                u -= 1.0;

                Step13(a, i, u, out t, out w);

                do
                {
                    // 14 Generate uStar, if uStar > t go to 17
                    uStar = VariateGenerator.GenerateUniformOO();
                    if (uStar > t)
                    {
                        finished = true;
                    }
                    else
                    {
                        // 15 Generate u, if uStar < u generate u and go to 13
                        u = VariateGenerator.GenerateUniformOO();
                        if (uStar < u)
                        {
                            u = VariateGenerator.GenerateUniformOO();
                            Step13(a, i, u, out t, out w);
                        }
                        else
                        {
                            // 16 t = u, go to 14
                            t = u;
                        }
                    }
                } while (!finished);
            }

            // 17 y = a + w, if s == 0 return x = y if s = 1 return x = -y
            double y = a + w;
            double norm = (s == 0) ? y : -y;

            return norm;
        }

        private static void Step5(double a, int i, double u, out double w, out double t)
        {
            //  5 Generate u. w = u*(a[i+1] - a), t = (w/2 + a)*w
            w = u * (SnA[i + 1] - a);
            t = (0.5 * w + a) * w;
        }

        private static void Step13(double a, int i, double u, out double t, out double w)
        {
            // 13 w = u*d[i], t = (w/2 + a)*w
            w = u * SnD[i];
            t = (0.5 * w + a) * w;
        }

        static readonly double[] SnA = {
           -1.0,
            0.0,       3.917609e-2, 7.841241e-2, 0.11777,   0.1573107, 0.1970991, 0.2372021, 0.2776904,
            0.3186394, 0.36013,     0.4022501,   0.4450965, 0.4887764, 0.5334097, 0.5791322, 0.626099,
            0.6744898, 0.7245144,   0.7764218,   0.8305109, 0.8871466, 0.9467818, 1.00999,   1.077516,
            1.150349,  1.229859,    1.318011,    1.417797,  1.534121, 1.67594,    1.862732,  2.153875
        };

        static readonly double[] SnT = {
           -1.0,
            7.673828E-4, 2.30687E-3,  3.860618E-3, 5.438454E-3, 7.0507E-3,   8.708396E-3,
            1.042357E-2, 1.220953E-2, 1.408125E-2, 1.605579E-2, 1.81529E-2,  2.039573E-2,
            2.281177E-2, 2.543407E-2, 2.830296E-2, 3.146822E-2, 3.499233E-2, 3.895483E-2,
            4.345878E-2, 4.864035E-2, 5.468334E-2, 6.184222E-2, 7.047983E-2, 8.113195E-2,
            9.462444E-2, 0.1123001,   0.136498,    0.1716886,   0.2276241,   0.330498,
            0.5847031
        };

        static readonly double[] SnH = {
            -1.0,
            3.920617E-2, 3.932705E-2, 3.951E-2,    3.975703E-2, 4.007093E-2, 4.045533E-2,
            4.091481E-2, 4.145507E-2, 4.208311E-2, 4.280748E-2, 4.363863E-2, 4.458932E-2,
            4.567523E-2, 4.691571E-2, 4.833487E-2, 4.996298E-2, 5.183859E-2, 5.401138E-2,
            5.654656E-2, 5.95313E-2,  6.308489E-2, 6.737503E-2, 7.264544E-2, 7.926471E-2,
            8.781922E-2, 9.930398E-2, 0.11556,     0.1404344,   0.1836142,   0.2790016,
            0.7010474
        };

        private static readonly double[] SnD =
            {
               -1.0,
                0.67448975019607, 0.47585963017993, 0.38377116397654, 0.32861132306910,
                0.29114282663980, 0.26368462217502, 0.24250845238097, 0.22556744380930,
                0.21163416577204, 0.19992426749317, 0.18991075842246, 0.18122518100691,
                0.17360140038056, 0.16684190866667, 0.16079672918053, 0.15534971747692,
                0.15040938382813, 0.14590257684509, 0.14177003276856, 0.13796317369537,
                0.13444176150074, 0.13117215026483, 0.12812596512583, 0.12527909006226,
                0.12261088288608, 0.12010355965651, 0.11774170701949, 0.11551189226063,
                0.11340234879117, 0.11140272044119, 0.10950385201710, 0.10769761656476,
                0.10597677198479, 0.10433484129317, 0.10276601206127, 0.10126505151402,
                0.09982723448906, 0.09844828202068, 0.09712430874765, 0.09585177768776,
                0.09462746119186, 0.09344840710526, 0.09231190933664, 0.09121548217294,
                0.09015683778986, 0.08913386650005, 0.08814461935364
            };


        public double GenerateNormal(double mean, double variance)
        {
            double sample = variance * StandardNormal() + mean;

            return sample;
        }

        /*
         * AHRENS, J.H. AND DIETER, U.
         * COMPUTER GENERATION OF POISSON DEVIATES FROM MODIFIED NORMAL DISTRIBUTIONS.
         * ACM TRANS. MATH. SOFTWARE, 8,2 (JUNE 1982), 163 - 179.
         * 
         * Algorithm PD
         */
        private const double OneOverSqrt2Pi = 0.39894228040143267793994605993438; // (float)0.398942280f;

        public int GeneratePoisson(double mu)
        {
            int k = -1;

            if (mu >= 10.0)
            {
                // Case A
                var s = Math.Sqrt(mu);
                double d = 6.0 * mu * mu;
                var ell = (int)(mu - 1.1484);    // equivalent to floor() since mu >= 10

                // Step N
                double T = StandardNormal();
                double g = mu + s * T;

                double u = 0.0;

                if (g >= 0.0)
                {
                    k = (int)g;    // equivalent to floor() since G >= 0

                    // Step I
                    if (k >= ell)
                    {
                        return k;
                    }

                    // Step S
                    u = VariateGenerator.GenerateUniformOO();
                    if ((d * u) >= ((mu - k) * (mu - k) * (mu - k)))
                    {
                        return k;
                    }
                }

                // Step P
                double omega = OneOverSqrt2Pi / s;
                double b1 = (1.0 / 24.0) / mu;
                double b2 = (3.0 / 10.0) * b1 * b1;
                double c3 = (1.0 / 7.0) * b1 * b2;
                double c2 = b2 - 15.0 * c3;
                double c1 = b1 - 6.0 * b2 + 45.0 * c3;
                double c0 = 1.0 - b1 + 3.0 * b2 - 15.0 * c3;
                double c = 0.1069 / mu;

                double px;
                double py;
                double fx;
                double fy;

                if (g >= 0.0)
                {
                    ProcedureF(mu, k, s, omega, c3, c2, c1, c0, out px, out py, out fx, out fy);

                    // Step Q
                    if ((fy * (1.0 - u)) <= (py * Math.Exp(px - fx)))
                    {
                        return k;
                    }
                }

                double e;
                do
                {
                    // Step E
                    do
                    {
                        e = StandardExponential();
                        u = VariateGenerator.GenerateUniformOO();
                        u += u - 1.0;
                        T = 1.8 + e * Math.Sign(u);
                    } while (T <= -0.6744);

                    k = (int)(mu + s * T);

                    ProcedureF(mu, k, s, omega, c3, c2, c1, c0, out px, out py, out fx, out fy);

                    // Step H
                } while ((c * Math.Abs(u)) > (py * Math.Exp(px + e) - fy * Math.Exp(fx + e)));
            }
            else
            {
                // Case B
                int m = Math.Max(1, (int)mu);
                int ell = 0;
                var p = Math.Exp(-mu);
                double q = p;
                double p0 = p;
                var pk = new double[35];

                do
                {
                    // Step U
                    double u = VariateGenerator.GenerateUniformOO();
                    k = 0;

                    if (u <= p0)
                    {
                        return k;
                    }

                    // Step T
                    if (ell > 0)
                    {
                        int j = u <= 0.458 ? 1 : Math.Min(ell, m);
                        for (k = j; k <= ell; k++)
                        {
                            if (u <= pk[k - 1])
                            {
                                return k;
                            }
                        }
                    }
                    else
                    {
                        // Step C
                        for (k = ell + 1; k <= 35; k++)
                        {
                            p = p * mu / k;
                            q += p;
                            pk[k - 1] = q;
                            if (u <= q)
                            {
                                return k;
                            }
                        }

                        ell = 35;
                    }
                } while (true);
            }

            return k;
        }

        private static void ProcedureF(double mu, int k, double s, double omega, double c3, double c2,
                                       double c1, double c0, out double px, out double py, out double fx, out double fy)
        {
            // Procedure F, Part 1
            if (k < 10)
            {
                px = -mu;
                py = Math.Pow(mu, k) / Factorial(k);
            }
            else
            {
                double delta = (1.0 / 12.0) / k;
                delta -= 4.8 * delta * delta * delta;
                double v = (mu - k) / k;
                if (Math.Abs(v) > 0.25)
                {
                    px = (k * Math.Log(1.0 + v) - (mu - k) - delta);
                }
                else
                {
                    double sumAv = 0.0;
                    for (int i = 1; i <= PoissonA.Length; i++)
                    {
                        sumAv += (PoissonA[i - 1] * Math.Pow(v, i));
                    }
                    px = k * v * v * sumAv - delta;
                }
                py = OneOverSqrt2Pi / Math.Sqrt(k);
            }

            // Procedure F, Part 2
            double x = (k - mu + 0.5) / s;
            double xSquared = x * x;
            fx = -0.5 * xSquared;   // Note, there's an error in the original paper which omits the minus sign.
            fy = omega * (((c3 * xSquared + c2) * xSquared + c1) * xSquared + c0);
        }

        private static readonly long[] Factorials =
            {
                            1,
                            1,
                            2,
                            6,
                           24,
                          120,
                          720,
                         5040,
                        40320,
                       362880,
                      3628800,
                     39916800,
                    479001600,
                   6227020800,
                  87178291200,
                1307674368000
            };

        internal static long Factorial(int i)
        {
            long fact;
            int length = Factorials.Length;

            if (i < length)
            {
                fact = Factorials[i];
            }
            else
            {
                fact = Factorials[length - 1];
                while (i >= length)
                {
                    fact *= i--;
                }
            }

            return fact;
        }

        static readonly double[] PoissonA =
            {
                -0.49999999,
                 0.33333328,
                -0.25000678,
                 0.20001178,
                -0.16612694,
                 0.14218783,
                -0.13847944,
                 0.12500596
            };

        /*
         * AHRENS, J.H. AND DIETER, U.
         * COMPUTER METHODS FOR SAMPLING FROM THE EXPONENTIAL AND NORMAL DISTRIBUTIONS.
         * COMM. ACM, 15,10 (OCT. 1972), 873 - 882.
         */
        public double StandardExponential()
        {
            const double ln2 = 0.69314718055994530941723212145818;// (float)0.69314718f;
            double x;

            // 1
            double a = 0.0;
            double u = VariateGenerator.GenerateUniformOO();

            // 2
            while (u < 0.5)
            {
                // 3
                a += ln2;
                u += u;
            }

            // 4
            u += (u - 1.0);
            if (u <= ln2)
            {
                // 5
                x = a + u;
            }
            else
            {
                int i = 1;
                double uStar = VariateGenerator.GenerateUniformOO();
                double umin = uStar;

                do
                {
                    uStar = VariateGenerator.GenerateUniformOO();
                    if (uStar < umin)
                    {
                        umin = uStar;
                    }

                    // 8
                } while (u > ExponentialQ[i++]);

                // 9
                // if (u <= q[1])
                {
                    x = a + umin * ln2;   // q[1]
                }
            }

            return x;
        }

        // q[i] = SUM(POW(LN(2),i)/i!)
        private static readonly double[] ExponentialQ =
            {
                0.693147181,
                0.933373688,
                0.988877796,
                0.998495925,
                0.999829281,
                0.999983316,
                0.999998569,
                0.999999891,
                0.999999992,
                1.0
            };

        public double GenerateExponential(double mean)
        {
            double sample = StandardExponential() * mean;

            return sample;
        }

        /*
         * For shape >= 1
         * 
         * AHRENS, J.H. AND DIETER, U.
         * GENERATING GAMMA VARIATES BY A MODIFIED REJECTION TECHNIQUE.
         * COMM. ACM, 25,1 (JAN. 1982), 47 - 54.
         * 
         * Algorithm 'GD'
         * 
         * For 0 < shape < 1
         * 
         * AHRENS, J.H. AND DIETER, U.
         * COMPUTER METHODS FOR SAMPLING FROM GAMMA, BETA, POISSON AND BINOMIAL DISTRIBUTIONS.
         * COMPUTING, 12 (1974), 223 - 246.
         * 
         * Algorithm 'GS'
         */
        public double StandardGamma(double shape)
        {
            double sGamma = -1.0;
            bool finished = false;

            if (shape >= 1.0)  // Algorithm 'GD'
            {
                // 1
                double s2 = shape - 0.5;
                var s = Math.Sqrt(s2);
                double d = 5.65685425 - 12.0 * s;

                // 2
                double T = StandardNormal();
                double x = s + 0.5 * T;

                if (T >= 0.0)
                {
                    sGamma = x * x;
                }
                else
                {
                    // 3
                    double u = VariateGenerator.GenerateUniformOO();
                    if ((d * u) <= (T * T * T))
                    {
                        sGamma = x * x;
                    }
                    else
                    {
                        // 4
                        double q0 = 0.0;
                        double b;
                        double sigma;
                        double c;

                        double oneOverShape = 1.0 / shape;
                        for (int k = GammaQ.Length - 1; k >= 0; k--)
                        {
                            q0 += GammaQ[k];
                            q0 *= oneOverShape;
                        }

                        if (shape <= 3.686)
                        {
                            b = 0.463 + s + 0.178 * s2;   // + 0.178 * s2 -OR- - 0.178 * s2 ?
                            sigma = 1.235;
                            c = 0.195 / s - 0.079 + 0.16 * s;
                        }
                        else if (shape <= 13.022)
                        {
                            b = 1.654 + 0.0076 * s2;
                            sigma = 1.68 / s + 0.275;
                            c = 0.062 / s + 0.024;
                        }
                        else
                        {
                            b = 1.77;
                            sigma = 0.75;
                            c = 0.1515 / s;
                        }

                        // 5
                        double v;
                        double q;
                        if (x > 0.0)
                        {
                            // 6
                            v = T / (s + s);
                            q = GammaStep6(v, q0, s, T, s2);

                            // 7
                            if (Math.Log(1 - u) <= q)
                            {
                                sGamma = x * x;
                                finished = true;
                            }
                        }

                        // 8
                        while (!finished)
                        {
                            double e = StandardExponential();
                            u = VariateGenerator.GenerateUniformOO();
                            u += u - 1.0;
                            T = b + e * sigma * Math.Sign(u);

                            // 9
                            if (T > -0.71874483771719)
                            {
                                // 10
                                v = T / (s + s);
                                q = GammaStep6(v, q0, s, T, s2);

                                // 11
                                if (q > 0.0)
                                {
                                    double temp1 = c * Math.Abs(u);
                                    double temp2;
                                    if (q > 0.5)
                                    {
                                        temp2 = Math.Exp(q) - 1;
                                    }
                                    else
                                    {
                                        temp2 = 0;
                                        /*
                                        for (int k = 1; k <= GammaE.Length; k++)
                                        {
                                            temp2 += GammaE[k - 1]*Math.Pow(q, k);
                                        }
                                        */
                                        for (int k = GammaE.Length; k >= 1; k--)
                                        {
                                            temp2 += GammaE[k - 1];
                                            temp2 *= q;
                                        }
                                    }
                                    double temp3 = temp2 * Math.Exp(e - 0.5 * T * T);
                                    if (temp1 <= temp3)
                                    {
                                        // 12
                                        x = s + 0.5 * T;
                                        sGamma = x * x;
                                        finished = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else    // Algorithm 'GS'
            {
                double b = 1.0 + 0.36788794412 * shape;
                do
                {
                    double p = b * VariateGenerator.GenerateUniformOO();
                    if (p <= 1.0)
                    {
                        var x = (Math.Exp(Math.Log(p) / shape));
                        if (Math.Log(VariateGenerator.GenerateUniformOO()) <= -x)
                        {
                            sGamma = x;
                            finished = true;
                        }
                    }
                    else
                    {
                        double x = -Math.Log((b - p) / shape);
                        if (Math.Log(VariateGenerator.GenerateUniformOO()) <= ((shape - 1.0) * Math.Log(x)))
                        {
                            sGamma = x;
                            finished = true;
                        }
                    }
                } while (!finished);
            }

            return sGamma;
        }

        private static double GammaStep6(double v, double q0, double s, double T, double s2)
        {
            double q;

            if (Math.Abs(v) > 0.25)
            {
                q = q0 - s * T + 0.25 * T * T + (s2 + s2) * Math.Log(1 + v);
            }
            else
            {
                double sum = 0.0;
                for (int k = 1; k <= GammaA.Length; k++)
                {
                    sum += (GammaA[k - 1] * Math.Pow(v, k));
                }

                q = q0 + 0.5 * T * T * sum;
            }

            return q;
        }

        private static readonly double[] GammaA =
            {
                 0.3333333,
                -0.2500030,
                 0.2000062,
                -0.1662921,
                 0.1423657,
                -0.1367177,
                 0.1233795
            };

        private static readonly double[] GammaE =
            {
                1.0,
                0.4999897,
                0.1668290,
                0.0407753,
                0.0102930
            };

        private static readonly double[] GammaQ =
            {
                 0.04166669,
                 0.02083148,
                 0.00801191,
                 0.00144121,
                -0.00007388,
                 0.00024511,
                 0.00024240
            };

        public double GenerateGamma(double shape, double scale)
        {
            double stdGamma = StandardGamma(shape);
            double gamma = stdGamma / scale;

            return gamma;
        }

        /*
         *     Kachitvichyanukul, V. and Schmeiser, B. W.
         *     Binomial Random Variate Generation.
         *     Communications of the ACM, 31, 2
         *     (February, 1988) 216.
        */
        public int GenerateBinomial(int trials, double probabilitySuccess)
        {
            int successes;
            double pPrime = Math.Min(probabilitySuccess, 1.0 - probabilitySuccess);

            if ((trials * pPrime) < 30.0)
            {
                // Algorithm BU
                // Step 1
                double q = 1.0 - pPrime;
                double s = pPrime / q;
                double a = (trials + 1.0) * s;
                var qn = Math.Pow(q, trials);
                double r = qn;

                // Step 2
                // double uSave = VariateGenerator.GenerateUniformOO();
                // double u = uSave;
                double u = VariateGenerator.GenerateUniformOO();
                int x = 0;

                // Step 3
                while (u > r)
                {
                    if (x < trials)
                    {
                        u -= r;
                        x += 1;
                        r *= ((a / x) - s);
                    }
                    else
                    {
                        x = 0;
                        r = qn;
                        u = VariateGenerator.GenerateUniformOO();
                    }
                }

                successes = x;
            }
            else
            {
                // Algorithm BTPE
                // Step 0
                double r = Math.Min(probabilitySuccess, 1.0 - probabilitySuccess);
                double q = 1.0 - r;
                double fM = trials * r + r;
                var m = (int)(fM); // floor(fM)
                double p1 = (Math.Floor(2.195 * Math.Sqrt(trials * r * q) - 4.6 * q)) + 0.5;
                double xM = m + 0.5;
                double xL = xM - p1;
                double xR = xM + p1;
                double c = 0.134 + 20.5 / (15.3 + m);
                double a = (fM - xL) / (fM - xL * r);
                double lambdaL = a * (1.0 + 0.5 * a);
                a = (xR - fM) / (xR * q);
                double lambdaR = a * (1.0 + 0.5 * a);
                double p2 = p1 * (1.0 + 2.0 * c);
                double p3 = p2 + c / lambdaL;
                double p4 = p3 + c / lambdaR;

                // This value is never used, the initialization makes the compiler happy.
                // Note that y is first read in Step 5, accept reject. We only encounter this code
                // when acceptReject is true in which case we have set y appropriately.
                int y = 0;

                // Step 1
                bool finished = false;
                do
                {
                    double u = VariateGenerator.GenerateUniformOO() * p4;
                    double v = VariateGenerator.GenerateUniformOO();

                    bool acceptReject = false;

                    if (u <= p1)
                    {
                        y = (int)Math.Floor(xM - p1 * v + u);
                        finished = true;
                    }
                    else
                    {
                        // Step 2
                        if (u > p2)
                        {
                            // Step 3
                            if (u > p3)
                            {
                                // Step 4
                                y = (int)Math.Floor(xR - Math.Log(v) / lambdaR);
                                if (y <= trials)
                                {
                                    v = v * (u - p3) * lambdaR;
                                    acceptReject = true;
                                }
                            }
                            else
                            {
                                y = (int)Math.Floor(xL + Math.Log(v) / lambdaL);
                                if (y >= 0)
                                {
                                    v = v * (u - p2) * lambdaL;
                                    acceptReject = true;
                                }
                            }
                        }
                        else
                        {
                            double x = xL + (u - p1) / c;
                            v = v * c + 1.0 - Math.Abs(m - x + 0.5) / p1;
                            if (v <= 1)
                            {
                                y = (int)Math.Floor(x);
                                acceptReject = true;
                            }
                        }

                        // Step 5
                        if (acceptReject)
                        {
                            double k = Math.Abs(y - m);
                            if ((k > 20) && (k < (0.5 * trials * r * q - 1.0)))
                            {
                                // Step 5.2
                                double rho = (k / (trials * r * q)) * ((k * (k / 3 + 0.625) + 0.166667) / (trials * r * q) + 0.5);
                                double t = -0.5 * k * k / (trials * r * q);
                                var logV = Math.Log(v);

                                if (logV < (t - rho))
                                {
                                    finished = true;
                                }
                                else
                                {
                                    if (logV <= (t + rho))
                                    {
                                        // Step 5.3
                                        double x1 = y + 1.0;
                                        double f1 = m + 1.0;
                                        double z = trials + 1 - m;
                                        double w = trials - y + 1.0;
                                        double x2 = x1 * x1;
                                        double f2 = f1 * f1;
                                        double z2 = z * z;
                                        double w2 = w * w;

                                        double test = xM * Math.Log(f1 / x1) + (trials - m + 0.5) * Math.Log(z / w) +
                                                      (y - m) * Math.Log(w * r / (x1 * q)) +
                                                      (13860.0 - (462.0 - (132.0 - (99.0 - 140.0 / f2) / f2) / f2) / f2) / f1 /
                                                      166320.0 +
                                                      (13860.0 - (462.0 - (132.0 - (99.0 - 140.0 / z2) / z2) / z2) / z2) / z /
                                                      166320.0 +
                                                      (13860.0 - (462.0 - (132.0 - (99.0 - 140.0 / x2) / x2) / x2) / x2) / x1 /
                                                      166320.0 +
                                                      (13860.0 - (462.0 - (132.0 - (99.0 - 140.0 / w2) / w2) / w2) / w2) / w /
                                                      166320.0;

                                        finished = (logV <= test);
                                    }
                                }
                            }
                            else
                            {
                                // Step 5.1
                                double s = r / q;
                                a = s * (trials + 1);
                                double f = 1.0;

                                if (m < y)
                                {
                                    for (int i = m; i++ < y; )
                                    {
                                        f = f * (a / i - s);
                                    }
                                }
                                else if (m > y)
                                {
                                    for (int i = y; i++ < m; )
                                    {
                                        f = f / (a / i - s);
                                    }
                                }

                                finished = (v <= f);
                            }
                        }
                    }
                } while (!finished);

                // Step 6
                successes = y;
            }

            int binomial = (probabilitySuccess <= 0.5) ? successes : trials - successes;

            return binomial;
        }

        /*
         * Algorithm from page 559 of Devroye, Luc
         * Non-Uniform Random Variate Generation.  Springer-Verlag, New York, 1986.
         */
        public void GenerateMultinomial(int totalEvents, double[] probabilityVector, int[] events)
        {
            int remainingEvents = totalEvents;
            for (int i = 0; i < events.Length; i++)
            {
                events[i] = 0;
            }

            double remainingProbability = 1.0;
            for (int i = 0; i < events.Length - 1; i++)
            {
                double probability = probabilityVector[i] / remainingProbability;
                events[i] = GenerateBinomial(remainingEvents, probability);
                remainingEvents -= events[i];
                if (remainingEvents <= 0)
                {
                    return;
                }
                remainingProbability -= probabilityVector[i];
            }

            events[events.Length - 1] = remainingEvents;
        }

        public override string ToString()
        {
            return string.Format("RandLib Distribution Sampler ({0})", VariateGenerator);
        }
    }
}
