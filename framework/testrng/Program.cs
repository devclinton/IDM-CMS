/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using RngLib;

namespace TestRNG
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                int samples = Int32.Parse(args[0]);

                for (int iArg = 1; iArg < args.Length; iArg++)
                {
                    switch (args[iArg].ToLower())
                    {
                        case "beta":
                            {
                                var beta = new float[samples];
                                for (int i = 0; i < beta.Length; i++)
                                    beta[i] = RNG.GenerateBeta(0.5f, 0.5f);
                                ArrayToCSV(beta);

                                for (int i = 0; i < beta.Length; i++)
                                    beta[i] = RNG.GenerateBeta(5.0f, 1.0f);
                                ArrayToCSV(beta);

                                for (int i = 0; i < beta.Length; i++)
                                    beta[i] = RNG.GenerateBeta(1.0f, 3.0f);
                                ArrayToCSV(beta);

                                for (int i = 0; i < beta.Length; i++)
                                    beta[i] = RNG.GenerateBeta(2.0f, 2.0f);
                                ArrayToCSV(beta);

                                for (int i = 0; i < beta.Length; i++)
                                    beta[i] = RNG.GenerateBeta(2.0f, 5.0f);
                                ArrayToCSV(beta);
                            }
                            break;

                        case "bin":
                        case "binomial":
                            {
                                var binomial = new int[samples];
                                for (int i = 0; i < binomial.Length; i++)
                                    binomial[i] = RNG.GenerateBinomial(20, 0.1f);
                                ArrayToCSV(binomial);

                                for (int i = 0; i < binomial.Length; i++)
                                    binomial[i] = RNG.GenerateBinomial(20, 0.5f);
                                ArrayToCSV(binomial);

                                for (int i = 0; i < binomial.Length; i++)
                                    binomial[i] = RNG.GenerateBinomial(20, 0.8f);
                                ArrayToCSV(binomial);
                            }
                            break;

                        case "chi":
                        case "chisquare":
                            {
                                var chi = new float[samples];
                                for (int i = 0; i < chi.Length; i++)
                                    chi[i] = RNG.GenerateChiSquare(1.0f);
                                ArrayToCSV(chi);

                                for (int i = 0; i < chi.Length; i++)
                                    chi[i] = RNG.GenerateChiSquare(2.0f);
                                ArrayToCSV(chi);

                                for (int i = 0; i < chi.Length; i++)
                                    chi[i] = RNG.GenerateChiSquare(3.0f);
                                ArrayToCSV(chi);

                                for (int i = 0; i < chi.Length; i++)
                                    chi[i] = RNG.GenerateChiSquare(4.0f);
                                ArrayToCSV(chi);

                                for (int i = 0; i < chi.Length; i++)
                                    chi[i] = RNG.GenerateChiSquare(5.0f);
                                ArrayToCSV(chi);
                            }
                            break;

                        case "exp":
                        case "exponential":
                            {
                                var exp = new float[samples];
                                for (int i = 0; i < exp.Length; i++)
                                    exp[i] = RNG.GenerateExponential(0.5f);
                                ArrayToCSV(exp);

                                for (int i = 0; i < exp.Length; i++)
                                    exp[i] = RNG.GenerateExponential(1.0f);
                                ArrayToCSV(exp);

                                for (int i = 0; i < exp.Length; i++)
                                    exp[i] = RNG.GenerateExponential(1.5f);
                                ArrayToCSV(exp);
                            }
                            break;

                        case "f":
                        case "eff":
                            {
                                var f = new float[samples];
                                for (int i = 0; i < f.Length; i++)
                                    f[i] = RNG.GenerateF(1.0f, 1.0f);
                                ArrayToCSV(f);

                                for (int i = 0; i < f.Length; i++)
                                    f[i] = RNG.GenerateF(2.0f, 1.0f);
                                ArrayToCSV(f);

                                for (int i = 0; i < f.Length; i++)
                                    f[i] = RNG.GenerateF(5.0f, 2.0f);
                                ArrayToCSV(f);

                                for (int i = 0; i < f.Length; i++)
                                    f[i] = RNG.GenerateF(100.0f, 1.0f);
                                ArrayToCSV(f);

                                for (int i = 0; i < f.Length; i++)
                                    f[i] = RNG.GenerateF(100.0f, 100.0f);
                                ArrayToCSV(f);
                            }
                            break;

                        case "gamma":
                            {
                                var gamma = new float[samples];
                                for (int i = 0; i < gamma.Length; i++)
                                    gamma[i] = RNG.GenerateGamma(1.0f, 1.0f);
                                ArrayToCSV(gamma);

                                for (int i = 0; i < gamma.Length; i++)
                                    gamma[i] = RNG.GenerateGamma(2.0f, 1.0f);
                                ArrayToCSV(gamma);

                                for (int i = 0; i < gamma.Length; i++)
                                    gamma[i] = RNG.GenerateGamma(2.0f, 2.0f);
                                ArrayToCSV(gamma);

                                for (int i = 0; i < gamma.Length; i++)
                                    gamma[i] = RNG.GenerateGamma(2.0f, 3.0f);
                                ArrayToCSV(gamma);

                                for (int i = 0; i < gamma.Length; i++)
                                    gamma[i] = RNG.GenerateGamma(1.0f, 5.0f);
                                ArrayToCSV(gamma);

                                for (int i = 0; i < gamma.Length; i++)
                                    gamma[i] = RNG.GenerateGamma(0.5f, 9.0f);
                                ArrayToCSV(gamma);
                            }
                            break;

                        case "norm":
                        case "normal":
                            {
                                var normal = new float[samples];
                                for (int i = 0; i < normal.Length; i++)
                                    normal[i] = RNG.GenerateNormal(0.0f, 0.2f);
                                ArrayToCSV(normal);

                                for (int i = 0; i < normal.Length; i++)
                                    normal[i] = RNG.GenerateNormal(0.0f, 1.0f);
                                ArrayToCSV(normal);

                                for (int i = 0; i < normal.Length; i++)
                                    normal[i] = RNG.GenerateNormal(0.0f, 5.0f);
                                ArrayToCSV(normal);

                                for (int i = 0; i < normal.Length; i++)
                                    normal[i] = RNG.GenerateNormal(-2.0f, 0.5f);
                                ArrayToCSV(normal);
                            }
                            break;

                        case "poisson":
                            {
                                var poisson = new int[samples];
                                for (int i = 0; i < poisson.Length; i++)
                                    poisson[i] = RNG.GeneratePoisson(1.0f);
                                ArrayToCSV(poisson);

                                for (int i = 0; i < poisson.Length; i++)
                                    poisson[i] = RNG.GeneratePoisson(4.0f);
                                ArrayToCSV(poisson);

                                for (int i = 0; i < poisson.Length; i++)
                                    poisson[i] = RNG.GeneratePoisson(10.0f);
                                ArrayToCSV(poisson);
                            }
                            break;

                        case "uniform":
                            {
                                var uniform = new float[samples];
                                for (int i = 0; i < uniform.Length; i++)
                                    uniform[i] = RNG.GenerateUniform(-2.0f, 3.0f);
                                ArrayToCSV(uniform);

                                for (int i = 0; i < uniform.Length; i++)
                                    uniform[i] = RNG.GenerateUniform(1.0f, 10.0f);
                                ArrayToCSV(uniform);

                                for (int i = 0; i < uniform.Length; i++)
                                    uniform[i] = RNG.GenerateUniform(0.0f, 2.0f);
                                ArrayToCSV(uniform);
                            }
                            break;

                        default:
                            Console.Error.WriteLine("Unknown (or unimplemented) distribution: '" + args[iArg] + "'.");
                            break;
                    }
                }
            }
            else
            {
                Console.Error.WriteLine("Usage: TestRNG #samples [beta] [bin[omial]] [chi[square]] [exp[onential]] [f] [gamma] [norm[al]] [poisson] [uniform]");
            }
        }

        static void ArrayToCSV(int[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (i > 0)
                    Console.Write(",");
                Console.Write(data[i]);
            }
            Console.WriteLine();
        }

        static void ArrayToCSV(float[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (i > 0)
                    Console.Write(",");
                Console.Write(data[i]);
            }
            Console.WriteLine();
        }
    }
}
