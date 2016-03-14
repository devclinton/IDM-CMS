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

        public float GenerateUniformOO()
        {
            return VariateGenerator.GenerateUniformOO();
        }

        public float GenerateUniformOO(float min, float max)
        {
            return min + (VariateGenerator.GenerateUniformOO()*(max - min));
        }

        public float GenerateUniformOC()
        {
            return VariateGenerator.GenerateUniformOC();
        }

        public float GenerateUniformOC(float min, float max)
        {
            // Use the algorithm below rather than
            // min + (VariateGenerator.GenerateUniformOC()*(max - min))
            // to ensure that floating point precision issues don't
            // prevent returning max.
            float sample = VariateGenerator.GenerateUniformOC();
            float oneMinus = 1.0f - sample;
            float delta = max - min;
            float offset = oneMinus*delta;
            float result = max - offset;

            return result;
        }

        public float GenerateUniformCO()
        {
            return VariateGenerator.GenerateUniformCO();
        }

        public float GenerateUniformCO(float min, float max)
        {
            return min + (VariateGenerator.GenerateUniformCO()*(max - min));
        }

        public float GenerateUniformCC()
        {
            return VariateGenerator.GenerateUniformCC();
        }

        public float GenerateUniformCC(float min, float max)
        {
            // Use the algorithm below rather than
            // min + (VariateGenerator.GenerateUniformCC()*(max - min))
            // to ensure that floating point precision issues don't
            // prevent returning max.
            float sample = VariateGenerator.GenerateUniformCC();

            // Floating point precision issues may prevent sample = 0.0f
            // from causing min to be returned.
            if (sample == 0.0f)
            {
                return min;
            }

            float oneMinus = 1.0f - sample;
            float delta = max - min;
            float offset = oneMinus * delta;
            float result = max - offset;

            return result;
        }

        /*
         * AHRENS, J.H. AND DIETER, U.                  
         * EXTENSIONS OF FORSYTHE'S METHOD FOR RANDOM SAMPLING FROM THE NORMAL DISTRIBUTION.
         * MATH. COMPUT., 27,124 (OCT. 1973), 927 - 937.
         * 
         * Algorithm FL for m=5
        */
        public float StandardNormal()
        {
            float uStar;
            float a;
            float w;
            float t;

            bool finished = false;

            //  1 Generate u. Store the first bit of u as a sign s. Left-shift u by 1 bit (u = 2u - s).
            float u;
            int s;
            do
            {
                u = VariateGenerator.GenerateUniformOO();
                s = (u >= 0.5f) ? 1 : 0;
                u += u - s;
            } while (u == 0.0f);

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
                    if (u < 1.0f)
                    {
                        // 11 a = a + d[i], i = i + 1, goto 10
                        a += SnD[i];
                        i++;
                    }
                } while (u < 1.0f);

                // 12 u = u - 1
                u -= 1.0f;

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
            float y = a + w;
            float norm = (s == 0) ? y : -y;
            return norm;
        }

        private static void Step5(float a, int i, float u, out float w, out float t)
        {
            //  5 Generate u. w = u*(a[i+1] - a), t = (w/2 + a)*w
            w = u * (SnA[i + 1] - a);
            t = (0.5f * w + a) * w;
        }

        private static void Step13(float a, int i, float u, out float t, out float w)
        {
            // 13 w = u*d[i], t = (w/2 + a)*w
            w = u * SnD[i];
            t = (0.5f * w + a) * w;
        }

        static readonly float[] SnA = {
           -1.0f,
            0.0f,       3.917609e-2f, 7.841241e-2f, 0.11777f,   0.1573107f, 0.1970991f, 0.2372021f, 0.2776904f,
            0.3186394f, 0.36013f,     0.4022501f,   0.4450965f, 0.4887764f, 0.5334097f, 0.5791322f, 0.626099f,
            0.6744898f, 0.7245144f,   0.7764218f,   0.8305109f, 0.8871466f, 0.9467818f, 1.00999f,   1.077516f,
            1.150349f,  1.229859f,    1.318011f,    1.417797f,  1.534121f, 1.67594f,    1.862732f,  2.153875f
        };

        static readonly float[] SnT = {
           -1.0f,
            7.673828E-4f, 2.30687E-3f,  3.860618E-3f, 5.438454E-3f, 7.0507E-3f,   8.708396E-3f,
            1.042357E-2f, 1.220953E-2f, 1.408125E-2f, 1.605579E-2f, 1.81529E-2f,  2.039573E-2f,
            2.281177E-2f, 2.543407E-2f, 2.830296E-2f, 3.146822E-2f, 3.499233E-2f, 3.895483E-2f,
            4.345878E-2f, 4.864035E-2f, 5.468334E-2f, 6.184222E-2f, 7.047983E-2f, 8.113195E-2f,
            9.462444E-2f, 0.1123001f,   0.136498f,    0.1716886f,   0.2276241f,   0.330498f,
            0.5847031f
        };

        static readonly float[] SnH = {
            -1.0f,
            3.920617E-2f, 3.932705E-2f, 3.951E-2f,    3.975703E-2f, 4.007093E-2f, 4.045533E-2f,
            4.091481E-2f, 4.145507E-2f, 4.208311E-2f, 4.280748E-2f, 4.363863E-2f, 4.458932E-2f,
            4.567523E-2f, 4.691571E-2f, 4.833487E-2f, 4.996298E-2f, 5.183859E-2f, 5.401138E-2f,
            5.654656E-2f, 5.95313E-2f,  6.308489E-2f, 6.737503E-2f, 7.264544E-2f, 7.926471E-2f,
            8.781922E-2f, 9.930398E-2f, 0.11556f,     0.1404344f,   0.1836142f,   0.2790016f,
            0.7010474f
        };

        private static readonly float[] SnD =
            {
               -1.0f,
                0.67448975019607f, 0.47585963017993f, 0.38377116397654f, 0.32861132306910f,
                0.29114282663980f, 0.26368462217502f, 0.24250845238097f, 0.22556744380930f,
                0.21163416577204f, 0.19992426749317f, 0.18991075842246f, 0.18122518100691f,
                0.17360140038056f, 0.16684190866667f, 0.16079672918053f, 0.15534971747692f,
                0.15040938382813f, 0.14590257684509f, 0.14177003276856f, 0.13796317369537f,
                0.13444176150074f, 0.13117215026483f, 0.12812596512583f, 0.12527909006226f,
                0.12261088288608f, 0.12010355965651f, 0.11774170701949f, 0.11551189226063f,
                0.11340234879117f, 0.11140272044119f, 0.10950385201710f, 0.10769761656476f,
                0.10597677198479f, 0.10433484129317f, 0.10276601206127f, 0.10126505151402f,
                0.09982723448906f, 0.09844828202068f, 0.09712430874765f, 0.09585177768776f,
                0.09462746119186f, 0.09344840710526f, 0.09231190933664f, 0.09121548217294f,
                0.09015683778986f, 0.08913386650005f, 0.08814461935364f
            };


        public float GenerateNormal(float mean, float variance)
        {
            float sample = variance*StandardNormal() + mean;

            return sample;
        }

        /*
         * AHRENS, J.H. AND DIETER, U.
         * COMPUTER GENERATION OF POISSON DEVIATES FROM MODIFIED NORMAL DISTRIBUTIONS.
         * ACM TRANS. MATH. SOFTWARE, 8,2 (JUNE 1982), 163 - 179.
         * 
         * Algorithm PD
         */
        private const float OneOverSqrt2Pi = 0.398942280f;

        public int GeneratePoisson(float mu)
        {
            int k = -1;

            if (mu >= 10.0f)
            {
                // Case A
                var s = (float)Math.Sqrt(mu);
                float d = 6.0f * mu * mu;
                var ell = (int)(mu - 1.1484f);    // equivalent to floor() since mu >= 10

                // Step N
                float T = StandardNormal();
                float g = mu + s * T;

                float u = 0.0f;

                if (g >= 0.0f)
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
                float omega = OneOverSqrt2Pi / s;
                float b1 = (1.0f / 24.0f) / mu;
                float b2 = (3.0f / 10.0f) * b1 * b1;
                float c3 = (1.0f / 7.0f) * b1 * b2;
                float c2 = b2 - 15.0f * c3;
                float c1 = b1 - 6.0f * b2 + 45.0f * c3;
                float c0 = 1.0f - b1 + 3.0f * b2 - 15.0f * c3;
                float c = 0.1069f / mu;

                float px;
                float py;
                float fx;
                float fy;

                if (g >= 0.0f)
                {
                    ProcedureF(mu, k, s, omega, c3, c2, c1, c0, out px, out py, out fx, out fy);

                    // Step Q
                    if ((fy * (1.0f - u)) <= (py * Math.Exp(px - fx)))
                    {
                        return k;
                    }
                }

                float e;
                do
                {
                    // Step E
                    do
                    {
                        e = StandardExponential();
                        u = VariateGenerator.GenerateUniformOO();
                        u += u - 1.0f;
                        T = 1.8f + e * Math.Sign(u);
                    } while (T <= -0.6744f);

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
                var p = (float)Math.Exp(-mu);
                float q = p;
                float p0 = p;
                var pk = new float[35];

                do
                {
                    // Step U
                    float u = VariateGenerator.GenerateUniformOO();
                    k = 0;

                    if (u <= p0)
                    {
                        return k;
                    }

                    // Step T
                    if (ell > 0)
                    {
                        int j = u <= 0.458f ? 1 : Math.Min(ell, m);
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

        private static void ProcedureF(float mu, int k, float s, float omega, float c3, float c2,
                                       float c1, float c0, out float px, out float py, out float fx, out float fy)
        {
            // Procedure F, Part 1
            if (k < 10)
            {
                px = -mu;
                py = (float)(Math.Pow(mu, k) / Factorial(k));
            }
            else
            {
                float delta = (1.0f / 12.0f) / k;
                delta -= 4.8f * delta * delta * delta;
                float v = (mu - k) / k;
                if (Math.Abs(v) > 0.25)
                {
                    px = (float)(k * Math.Log(1.0f + v) - (mu - k) - delta);
                }
                else
                {
                    float sumAv = 0.0f;
                    for (int i = 1; i <= PoissonA.Length; i++)
                    {
                        sumAv += (float)(PoissonA[i - 1] * Math.Pow(v, i));
                    }
                    px = k * v * v * sumAv - delta;
                }
                py = OneOverSqrt2Pi / (float)Math.Sqrt(k);
            }

            // Procedure F, Part 2
            float x = (k - mu + 0.5f) / s;
            float xSquared = x * x;
            fx = -0.5f * xSquared;    // Note, there's an error in the original paper which omits the minus sign.
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

        static readonly float[] PoissonA =
            {
                -0.49999999f,
                 0.33333328f,
                -0.25000678f,
                 0.20001178f,
                -0.16612694f,
                 0.14218783f,
                -0.13847944f,
                 0.12500596f
            };

        /*
         * AHRENS, J.H. AND DIETER, U.
         * COMPUTER METHODS FOR SAMPLING FROM THE EXPONENTIAL AND NORMAL DISTRIBUTIONS.
         * COMM. ACM, 15,10 (OCT. 1972), 873 - 882.
         */
        public float StandardExponential()
        {
            const float ln2 = 0.69314718f;  // ln(2)
            float x;

            // 1
            float a = 0.0f;
            float u = VariateGenerator.GenerateUniformOO();

            // 2
            while (u < 0.5f)
            {
                // 3
                a += ln2;
                u += u;
            }

            // 4
            u += (u - 1.0f);
            if (u <= ln2)
            {
                // 5
                x = a + u;
            }
            else
            {
                int i = 1;
                float uStar = VariateGenerator.GenerateUniformOO();
                float umin = uStar;

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
        private static readonly float[] ExponentialQ =
            {
                0.693147181f,
                0.933373688f,
                0.988877796f,
                0.998495925f,
                0.999829281f,
                0.999983316f,
                0.999998569f,
                0.999999891f,
                0.999999992f,
                1.0f
            };

        public float GenerateExponential(float mean)
        {
            float sample = StandardExponential() * mean;

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
        public float StandardGamma(float shape)
        {
            float sGamma = -1.0f;
            bool finished = false;

            if (shape >= 1.0f)  // Algorithm 'GD'
            {
                // 1
                float s2 = shape - 0.5f;
                var s = (float)Math.Sqrt(s2);
                float d = 5.65685425f - 12.0f * s;

                // 2
                float T = StandardNormal();
                float x = s + 0.5f * T;

                if (T >= 0.0f)
                {
                    sGamma = x * x;
                }
                else
                {
                    // 3
                    float u = VariateGenerator.GenerateUniformOO();
                    if ((d * u) <= (T * T * T))
                    {
                        sGamma = x * x;
                    }
                    else
                    {
                        // 4
                        float q0 = 0.0f;
                        float b;
                        float sigma;
                        float c;

                        float oneOverShape = 1.0f / shape;
                        for (int k = GammaQ.Length - 1; k >= 0; k--)
                        {
                            q0 += GammaQ[k];
                            q0 *= oneOverShape;
                        }

                        if (shape <= 3.686f)
                        {
                            b = 0.463f + s + 0.178f * s2;   // + 0.178f * s2 -OR- - 0.178f * s2 ?
                            sigma = 1.235f;
                            c = 0.195f / s - 0.079f + 0.16f * s;
                        }
                        else if (shape <= 13.022f)
                        {
                            b = 1.654f + 0.0076f * s2;
                            sigma = 1.68f / s + 0.275f;
                            c = 0.062f / s + 0.024f;
                        }
                        else
                        {
                            b = 1.77f;
                            sigma = 0.75f;
                            c = 0.1515f / s;
                        }

                        // 5
                        float v;
                        float q;
                        if (x > 0.0f)
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
                            float e = StandardExponential();
                            u = VariateGenerator.GenerateUniformOO();
                            u += u - 1.0f;
                            T = b + e * sigma * Math.Sign(u);

                            // 9
                            if (T > -0.71874483771719f)
                            {
                                // 10
                                v = T / (s + s);
                                q = GammaStep6(v, q0, s, T, s2);

                                // 11
                                if (q > 0.0f)
                                {
                                    double temp1 = c * Math.Abs(u);
                                    double temp2;
                                    if (q > 0.5f)
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
                                        x = s + 0.5f * T;
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
                float b = 1.0f + 0.36788794412f * shape;
                do
                {
                    float p = b * VariateGenerator.GenerateUniformOO();
                    if (p <= 1.0f)
                    {
                        var x = (float)(Math.Exp(Math.Log(p) / shape));
                        if (Math.Log(VariateGenerator.GenerateUniformOO()) <= -x)
                        {
                            sGamma = x;
                            finished = true;
                        }
                    }
                    else
                    {
                        float x = -(float)Math.Log((b - p) / shape);
                        if (Math.Log(VariateGenerator.GenerateUniformOO()) <= ((shape - 1.0f) * Math.Log(x)))
                        {
                            sGamma = x;
                            finished = true;
                        }
                    }
                } while (!finished);
            }

            return sGamma;
        }

        private static float GammaStep6(float v, float q0, float s, float T, float s2)
        {
            float q;

            if (Math.Abs(v) > 0.25)
            {
                q = q0 - s * T + 0.25f * T * T + (s2 + s2) * (float)Math.Log(1 + v);
            }
            else
            {
                float sum = 0.0f;
                for (int k = 1; k <= GammaA.Length; k++)
                {
                    sum += (float)(GammaA[k - 1] * Math.Pow(v, k));
                }

                q = q0 + 0.5f * T * T * sum;
            }

            return q;
        }

        private static readonly float[] GammaA =
            {
                0.3333333f,
                -0.2500030f,
                0.2000062f,
                -0.1662921f,
                0.1423657f,
                -0.1367177f,
                0.1233795f
            };

        private static readonly float[] GammaE =
            {
                1.0f,
                0.4999897f,
                0.1668290f,
                0.0407753f,
                0.0102930f
            };

        private static readonly float[] GammaQ =
            {
                0.04166669f,
                0.02083148f,
                0.00801191f,
                0.00144121f,
                -0.00007388f,
                0.00024511f,
                0.00024240f
            };

        public float GenerateGamma(float shape, float scale)
        {
            float stdGamma = StandardGamma(shape);
            float gamma = stdGamma / scale;

            return gamma;
        }

        /*
         *     Kachitvichyanukul, V. and Schmeiser, B. W.
         *     Binomial Random Variate Generation.
         *     Communications of the ACM, 31, 2
         *     (February, 1988) 216.
        */
        public int GenerateBinomial(int trials, float probabilitySuccess)
        {
            int successes;
            float pPrime = Math.Min(probabilitySuccess, 1.0f - probabilitySuccess);

            if ((trials * pPrime) < 30.0f)
            {
                // Algorithm BU
                // Step 1
                float q = 1.0f - pPrime;
                float s = pPrime / q;
                float a = (trials + 1.0f) * s;
                var qn = (float)Math.Pow(q, trials);
                float r = qn;

                // Step 2
                // float uSave = VariateGenerator.GenerateUniformOO();
                // float u = uSave;
                float u = VariateGenerator.GenerateUniformOO();
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
                float r = Math.Min(probabilitySuccess, 1.0f - probabilitySuccess);
                float q = 1.0f - r;
                float fM = trials * r + r;
                var m = (int)(fM); // floor(fM)
                float p1 = (float)(Math.Floor(2.195 * Math.Sqrt(trials * r * q) - 4.6 * q)) + 0.5f;
                float xM = m + 0.5f;
                float xL = xM - p1;
                float xR = xM + p1;
                float c = 0.134f + 20.5f / (15.3f + m);
                float a = (fM - xL) / (fM - xL * r);
                float lambdaL = a * (1.0f + 0.5f * a);
                a = (xR - fM) / (xR * q);
                float lambdaR = a * (1.0f + 0.5f * a);
                float p2 = p1 * (1.0f + 2.0f * c);
                float p3 = p2 + c / lambdaL;
                float p4 = p3 + c / lambdaR;

                // This value is never used, the initialization makes the compiler happy.
                // Note that y is first read in Step 5, accept reject. We only encounter this code
                // when acceptReject is true in which case we have set y appropriately.
                int y = 0;

                // Step 1
                bool finished = false;
                do
                {
                    float u = VariateGenerator.GenerateUniformOO() * p4;
                    float v = VariateGenerator.GenerateUniformOO();

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
                            float x = xL + (u - p1) / c;
                            v = v * c + 1.0f - Math.Abs(m - x + 0.5f) / p1;
                            if (v <= 1)
                            {
                                y = (int)Math.Floor(x);
                                acceptReject = true;
                            }
                        }

                        // Step 5
                        if (acceptReject)
                        {
                            float k = Math.Abs(y - m);
                            if ((k > 20) && (k < (0.5f * trials * r * q - 1.0f)))
                            {
                                // Step 5.2
                                float rho = (k / (trials * r * q)) * ((k * (k / 3 + 0.625f) + 0.166667f) / (trials * r * q) + 0.5f);
                                float t = -0.5f * k * k / (trials * r * q);
                                var logV = (float)Math.Log(v);

                                if (logV < (t - rho))
                                {
                                    finished = true;
                                }
                                else
                                {
                                    if (logV <= (t + rho))
                                    {
                                        // Step 5.3
                                        float x1 = y + 1.0f;
                                        float f1 = m + 1.0f;
                                        float z = trials + 1 - m;
                                        float w = trials - y + 1.0f;
                                        float x2 = x1 * x1;
                                        float f2 = f1 * f1;
                                        float z2 = z * z;
                                        float w2 = w * w;

                                        float test = xM * (float)Math.Log(f1 / x1) + (trials - m + 0.5f) * (float)Math.Log(z / w) +
                                                     (y - m) * (float)Math.Log(w * r / (x1 * q)) +
                                                     (13860.0f - (462.0f - (132.0f - (99.0f - 140.0f / f2) / f2) / f2) / f2) / f1 /
                                                     166320.0f +
                                                     (13860.0f - (462.0f - (132.0f - (99.0f - 140.0f / z2) / z2) / z2) / z2) / z /
                                                     166320.0f +
                                                     (13860.0f - (462.0f - (132.0f - (99.0f - 140.0f / x2) / x2) / x2) / x2) / x1 /
                                                     166320.0f +
                                                     (13860.0f - (462.0f - (132.0f - (99.0f - 140.0f / w2) / w2) / w2) / w2) / w /
                                                     166320.0f;

                                        finished = (logV <= test);
                                    }
                                }
                            }
                            else
                            {
                                // Step 5.1
                                float s = r / q;
                                a = s * (trials + 1);
                                float f = 1.0f;

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

            int binomial = (probabilitySuccess <= 0.5f) ? successes : trials - successes;

            return binomial;
        }

        /*
         * Algorithm from page 559 of Devroye, Luc
         * Non-Uniform Random Variate Generation.  Springer-Verlag, New York, 1986.
         */
        public void GenerateMultinomial(int totalEvents, float[] probabilityVector, int[] events)
        {
            int remainingEvents = totalEvents;
            for (int i = 0; i < events.Length; i++)
            {
                events[i] = 0;
            }

            float remainingProbability = 1.0f;
            for (int i = 0; i < events.Length - 1; i++)
            {
                float probability = probabilityVector[i] / remainingProbability;
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
