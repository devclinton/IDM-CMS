/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using distlib;
using distlib.randomvariates;
using distlib.samplers;

namespace cmsunittests.distlib
{
    class RandLibSamplerTests : AssertionHelper
    {
        private const int UniformCount = 64 * 1048576;  // 64 * 2^20
        private const float UniformMin = float.MinValue/3.0f;
        private const float UniformMax = float.MaxValue/2.0f;

        private const int SampleCount = 128*1048576;    // 128 * 2^20

        private DistributionSampler GetRandLibSampler()
        {
            DistributionSampler sampler = RandLibSampler.CreateRandLibSampler(RandLibVariateGenerator.CreateRandLibVariateGenerator(new uint[] { 20130110, 19680201 }));

            return sampler;
        }

        private DistributionSampler GetAesCounterSampler()
        {
            DistributionSampler sampler = RandLibSampler.CreateRandLibSampler(AesCounterVariateGenerator.CreateAesCounterVariateGenerator(new uint[] { 20130110, 19680201 }));

            return sampler;
        }

        private delegate float Continuous();

        private delegate int Discrete();

        [Test]
        public void TestUniformOpenOpen()
        {
            DistributionSampler sampler = GetRandLibSampler();

            bool zero;
            bool one;
            float sample;
            bool inBounds = TestUniformSampler(sampler.GenerateUniformOO, 0.0f, 1.0f, out zero, out one, out sample);

            Assert.IsTrue(inBounds, string.Format("RandLibSampler.UniformOO() failed with value {0}.", sample));
            Assert.IsFalse(zero, "RandLibSampler.UniformOO() returned 0.0");
            Assert.IsFalse(one, "RandLibSampler.UniformOO() returned 1.0");
        }

        private static bool TestUniformSampler(Continuous sampleFunction, float minValue, float maxValue, out bool min, out bool max, out float sample)
        {
            bool inBounds = true;
            min = false;
            max = false;
            sample = float.NaN;

            for (int i = 0; i < UniformCount; i++)
            {
                sample = sampleFunction();
                if ((sample < minValue) || (sample > maxValue))
                {
                    inBounds = false;
                    break;
                }

                if (!min && (sample == minValue))
                {
                    min = true;
                }
                else if (!max && (sample == maxValue))
                {
                    max = true;
                }
            }

            return inBounds;
        }

        [Test]
        public void TestUniformOpenClosed()
        {
            DistributionSampler sampler = GetRandLibSampler();
            bool zero;
            bool one;
            float sample;
            bool inBounds = TestUniformSampler(sampler.GenerateUniformOC, 0.0f, 1.0f, out zero, out one, out sample);

            Assert.IsTrue(inBounds, string.Format("RandLibSampler.UniformOC() failed with value {0}.", sample));
            Assert.IsFalse(zero, "RandLibSampler.UniformOC() generated 0.0");
            Assert.IsTrue(one, "RandLibSampler.UniformOC() failed to generate 1.0");
        }

        [Test]
        public void TestUniformClosedOpen()
        {
            DistributionSampler sampler = GetRandLibSampler();
            bool zero;
            bool one;
            float sample;
            bool inBounds = TestUniformSampler(sampler.GenerateUniformCO, 0.0f, 1.0f, out zero, out one, out sample);

            Assert.IsTrue(inBounds, string.Format("RandLibSampler.UniformCO() failed with value {0}.", sample));
            Assert.IsTrue(zero, "RandLibSampler.UniformCO() failed to generate 0.0");
            Assert.IsFalse(one, "RandLibSampler.UniformCO() generated 1.0");
        }

        [Test]
        public void TestUniformClosedClosed()
        {
            DistributionSampler sampler = GetRandLibSampler();
            bool zero;
            bool one;
            float sample;
            bool inBounds = TestUniformSampler(sampler.GenerateUniformCC, 0.0f, 1.0f, out zero, out one, out sample);

            Assert.IsTrue(inBounds, string.Format("RandLibSampler.UniformCC() failed with value {0}.", sample));
            Assert.IsTrue(zero, "RandLibSampler.UniformCC() failed to generate 0.0");
            Assert.IsTrue(one, "RandLibSampler.UniformCC() failed to generate 1.0");
        }

        [Test]
        public void UniformOpenOpenMinMax()
        {
            DistributionSampler sampler = GetRandLibSampler();
            bool min;
            bool max;
            float sample;
            bool inBounds = TestUniformSampler(() => sampler.GenerateUniformOO(UniformMin, UniformMax), UniformMin, UniformMax, out min, out max, out sample);

            Assert.IsTrue(inBounds, string.Format("RandLibSampler.UniformOO(min,max) failed with value {0}.", sample));
            Assert.IsFalse(min, string.Format("RandLibSampler.UniformOO(min,max) generated {0}", min));
            Assert.IsFalse(max, string.Format("RandLibSampler.UniformOO(min,max) generated {0}", max));
        }

        [Test]
        public void UniformOpenClosedMinMax()
        {
            DistributionSampler sampler = GetRandLibSampler();
            bool min;
            bool max;
            float sample;
            bool inBounds = TestUniformSampler(() => sampler.GenerateUniformOC(UniformMin, UniformMax), UniformMin, UniformMax, out min, out max, out sample);

            Assert.IsTrue(inBounds, string.Format("RandLibSampler.UniformOC(min,max) failed with value {0}.", sample));
            Assert.IsFalse(min, string.Format("RandLibSampler.UniformOC(min,max) generated {0}", min));
            Assert.IsTrue(max, string.Format("RandLibSampler.UniformOC(min,max) failed to generate {0}", max));
        }

        [Test]
        public void UniformClosedOpenMinMax()
        {
            DistributionSampler sampler = GetRandLibSampler();
            bool min;
            bool max;
            float sample;
            bool inBounds = TestUniformSampler(() => sampler.GenerateUniformCO(UniformMin, UniformMax), UniformMin, UniformMax, out min, out max, out sample);

            Assert.IsTrue(inBounds, string.Format("RandLibSampler.UniformCO(min,max) failed with value {0}.", sample));
            Assert.IsTrue(min, string.Format("RandLibSampler.UniformCO(min,max) failed to generate {0}", min));
            Assert.IsFalse(max, string.Format("RandLibSampler.UniformCO(min,max) generated {0}", max));
        }

        [Test]
        public void UniformClosedClosedMinMax()
        {
            DistributionSampler sampler = GetRandLibSampler();
            bool min;
            bool max;
            float sample;
            bool inBounds = TestUniformSampler(() => sampler.GenerateUniformCC(UniformMin, UniformMax), UniformMin, UniformMax, out min, out max, out sample);

            Assert.IsTrue(inBounds, string.Format("RandLibSampler.UniformCC(min,max) failed with value {0}.", sample));
            Assert.IsTrue(min, string.Format("RandLibSampler.UniformCC(min,max) failed to generate {0}", min));
            Assert.IsTrue(max, string.Format("RandLibSampler.UniformCC(min,max) failed to generate {0}", max));
        }

        [Test]
        public void TestStandardNormal()
        {
            float[] distribution = LoadDistributionProfile<float>("distlib\\distributions\\StandardNormal.txt");
            int binCount = distribution.Length;
            DistributionSampler sampler = GetAesCounterSampler();
            var bins = new int[binCount];

            for (int i = 0; i < SampleCount; i++)
            {
                float sample = sampler.StandardNormal();
                int index = (int) (Math.Floor(sample*10.0f)) + (binCount/2);
                if ((index >= 0) && (index < binCount))
                    bins[index]++;
            }

            bool passed = ValidateDistribution(distribution, bins, 0.03f);

            Assert.IsTrue(passed);
        }

        [Test]
        public void TestGenerateNormal()
        {
            float[] distribution = LoadDistributionProfile<float>("distlib\\distributions\\GeneratedNormal.txt");
            int binCount = distribution.Length;
            DistributionSampler sampler = GetAesCounterSampler();
            var bins = new int[binCount];

            for (int i = 0; i < SampleCount; i++)
            {
                float sample = sampler.GenerateNormal(-2.0f, 0.5f);
                int index = (int) (Math.Floor(sample*10.0f)) + 40;
                if ((index >= 0) && (index < binCount))
                {
                    bins[index]++;
                }
            }

            bool passed = ValidateDistribution(distribution, bins, 0.025f);

            Assert.IsTrue(passed);
        }

        [Test]
        public void TestGeneratePoisson4()
        {
            int[] distribution = LoadDistributionProfile<int>("distlib\\distributions\\PoissonLambda4.txt");
            int binCount = distribution.Length;
            DistributionSampler sampler = GetAesCounterSampler();
            int[] bins = SampleDiscrete(() => sampler.GeneratePoisson(4.0f), binCount);

            bool passed = true;
            for (int i = 0; i < binCount; i++)
            {
                float error = Math.Abs(distribution[i] - bins[i]) / (float)SampleCount;
                if (error > 0.000147834420f)
                {
                    passed = false;
                }
                Console.WriteLine("{0},{1}", distribution[i], bins[i]);
            }

            Assert.IsTrue(passed);
        }

        [Test]
        public void TestGeneratePoisson10()
        {
            int[] distribution = LoadDistributionProfile<int>("distlib\\distributions\\PoissonLambda10.txt");
            int binCount = distribution.Length;
            DistributionSampler sampler = GetAesCounterSampler();
            int[] bins = SampleDiscrete(() => sampler.GeneratePoisson(10.0f), binCount);

            bool passed = true;
            for (int i = 0; i < binCount; i++)
            {
                float error = Math.Abs(distribution[i] - bins[i]) / (float)SampleCount;
                if (error > 0.002502352f)
                {
                    passed = false;
                }
                Console.WriteLine("{0},{1}", distribution[i], bins[i]);
            }

            Assert.IsTrue(passed);
        }

        [Test]
        public void TestStandardExponential()
        {
            float[] distribution = LoadDistributionProfile<float>("distlib\\distributions\\StandardExponential.txt");
            DistributionSampler sampler = GetAesCounterSampler();
            int[] bins = SampleContinuous(sampler.StandardExponential, distribution.Length);
            bool passed = ValidateDistribution(distribution, bins, 0.01f);
            Assert.IsTrue(passed);
        }

        [Test]
        public void TestGenerateExponential()
        {
            float[] distribution = LoadDistributionProfile<float>("distlib\\distributions\\GeneratedExponential.txt");
            DistributionSampler sampler = GetAesCounterSampler();
            int[] bins = SampleContinuous(() => sampler.GenerateExponential(0.5f), distribution.Length);
            bool passed = ValidateDistribution(distribution, bins, 0.025f);
            Assert.IsTrue(passed);
        }

        [Test]
        [Ignore, Description("This test is disabled until we measure distribution matches more robustly.")]
        public void TestStandardGammaPt5()
        {
            float[] distribution = LoadDistributionProfile<float>("distlib\\distributions\\StandardGammaShapePt5.txt");
            int binCount = distribution.Length;
            DistributionSampler sampler = GetAesCounterSampler();
            int[] bins = SampleContinuous(() => sampler.StandardGamma(0.5f), binCount);
            bool passed = ValidateDistribution(distribution, bins, 0.015f);
            Assert.IsTrue(passed);
        }

        [Test]
        public void TestStandardGamma1()
        {
            float[] distribution = LoadDistributionProfile<float>("distlib\\distributions\\StandardGammaShape1.txt");
            int binCount = distribution.Length;
            DistributionSampler sampler = GetAesCounterSampler();
            int[] bins = SampleContinuous(() => sampler.StandardGamma(1.0f), binCount);
            bool passed = ValidateDistribution(distribution, bins, 0.02f);
            Assert.IsTrue(passed);
        }

        [Test]
        public void TestStandardGamma2()
        {
            float[] distribution = LoadDistributionProfile<float>("distlib\\distributions\\StandardGammaShape2.txt");
            int binCount = distribution.Length;
            DistributionSampler sampler = GetAesCounterSampler();
            int[] bins = SampleContinuous(() => sampler.StandardGamma(2.0f), binCount);
            bool passed = ValidateDistribution(distribution, bins, 0.02f);
            Assert.IsTrue(passed);
        }

        [Test]
        public void TestStandardGamma3()
        {
            float[] distribution = LoadDistributionProfile<float>("distlib\\distributions\\StandardGammaShape3.txt");
            int binCount = distribution.Length;
            DistributionSampler sampler = GetAesCounterSampler();
            int[] bins = SampleContinuous(() => sampler.StandardGamma(3.0f), binCount);
            bool passed = ValidateDistribution(distribution, bins, 0.015f);
            Assert.IsTrue(passed);
        }

        [Test]
        public void TestStandardGamma5()
        {
            float[] distribution = LoadDistributionProfile<float>("distlib\\distributions\\StandardGammaShape5.txt");
            int binCount = distribution.Length;
            DistributionSampler sampler = GetAesCounterSampler();
            int[] bins = SampleContinuous(() => sampler.StandardGamma(5.0f), binCount);
            bool passed = ValidateDistribution(distribution, bins, 0.01f, 4);
            Assert.IsTrue(passed);
        }

        [Test]
        public void TestStandardGamma14()
        {
            float[] distribution = LoadDistributionProfile<float>("distlib\\distributions\\StandardGammaShape14.txt");
            int binCount = distribution.Length;
            DistributionSampler sampler = GetAesCounterSampler();
            int[] bins = SampleContinuous(() => sampler.StandardGamma(14.0f), binCount);
            bool passed = ValidateDistribution(distribution, bins, 0.015f, 39);
            Assert.IsTrue(passed);
        }

        [Test]
        public void TestGeneratedGamma1Pt5()
        {
            float[] distribution = LoadDistributionProfile<float>("distlib\\distributions\\GeneratedGamma1Pt5.txt");
            int binCount = distribution.Length;
            DistributionSampler sampler = GetAesCounterSampler();
            int[] bins = SampleContinuous(() => sampler.GenerateGamma(1.0f, 0.5f), binCount);
            bool passed = ValidateDistribution(distribution, bins, 0.015f, 1);
            Assert.IsTrue(passed);
        }

        [Test]
        public void TestGeneratedGamma2Pt5()
        {
            float[] distribution = LoadDistributionProfile<float>("distlib\\distributions\\GeneratedGamma2Pt5.txt");
            int binCount = distribution.Length;
            DistributionSampler sampler = GetAesCounterSampler();
            int[] bins = SampleContinuous(() => sampler.GenerateGamma(2.0f, 0.5f), binCount);
            bool passed = ValidateDistribution(distribution, bins, 0.0067f);
            Assert.IsTrue(passed);
        }

        [Test]
        public void TestGeneratedGamma3Pt5()
        {
            float[] distribution = LoadDistributionProfile<float>("distlib\\distributions\\GeneratedGamma3Pt5.txt");
            int binCount = distribution.Length;
            DistributionSampler sampler = GetAesCounterSampler();
            int[] bins = SampleContinuous(() => sampler.GenerateGamma(3.0f, 0.5f), binCount);
            bool passed = ValidateDistribution(distribution, bins, 0.01f, 1);
            Assert.IsTrue(passed);
        }

        [Test]
        public void TestBinomial20Pt5()
        {
            float[] distribution = LoadDistributionProfile<float>("distlib\\distributions\\Binomial20Pt5.txt");
            DistributionSampler sampler = GetAesCounterSampler();
            int[] bins = SampleDiscrete(() => sampler.GenerateBinomial(20, 0.5f), distribution.Length);
            bool passed = ValidateDistribution(distribution, bins, 0.01f, 1);
            Assert.IsTrue(passed);
        }

        [Test]
        public void TestBinomial20Pt7()
        {
            float[] distribution = LoadDistributionProfile<float>("distlib\\distributions\\Binomial20Pt7.txt");
            DistributionSampler sampler = GetAesCounterSampler();
            int[] bins = SampleDiscrete(() => sampler.GenerateBinomial(20, 0.7f), distribution.Length);
            bool passed = ValidateDistribution(distribution, bins, 0.005f, 7);
            Assert.IsTrue(passed);
        }

        [Test]
        public void TestBinomial40Pt3()
        {
            float[] distribution = LoadDistributionProfile<float>("distlib\\distributions\\Binomial40Pt3.txt");
            DistributionSampler sampler = GetAesCounterSampler();
            int[] bins = SampleDiscrete(() => sampler.GenerateBinomial(40, 0.3f), distribution.Length);
            bool passed = ValidateDistribution(distribution, bins, 0.02f, 1);
            Assert.IsTrue(passed);
        }

        [Test]
        public void TestBinomial40Pt5()
        {
            float[] distribution = LoadDistributionProfile<float>("distlib\\distributions\\Binomial40Pt5.txt");
            DistributionSampler sampler = GetAesCounterSampler();
            int[] bins = SampleDiscrete(() => sampler.GenerateBinomial(40, 0.5f), distribution.Length);
            bool passed = ValidateDistribution(distribution, bins, 0.025f, 8);
            Assert.IsTrue(passed);
        }

        [Test]
        public void TestBinomial80Pt5()
        {
            float[] distribution = LoadDistributionProfile<float>("distlib\\distributions\\Binomial80Pt5.txt");
            DistributionSampler sampler = GetAesCounterSampler();
            int[] bins = SampleDiscrete(() => sampler.GenerateBinomial(80, 0.5f), distribution.Length);
            bool passed = ValidateDistribution(distribution, bins, 0.01f, 24);
            Assert.IsTrue(passed);
        }

        [Test]
        public void TestMultinomial3From3()
        {
            float[] distribution = LoadDistributionProfile<float>("distlib\\distributions\\Multinomial3From3.txt");
            DistributionSampler sampler = GetAesCounterSampler();

            const int nEvents = 3;
            const int nCategories = 3;
            var bins = SampleMultinomial(nCategories, nEvents, distribution.Length, sampler);

            bool passed = ValidateDistribution(distribution, bins, 0.01f);
            Assert.IsTrue(passed);
        }

        [Test]
        public void TestMultinomial4From4()
        {
            float[] distribution = LoadDistributionProfile<float>("distlib\\distributions\\Multinomial4From4.txt");
            DistributionSampler sampler = GetAesCounterSampler();

            const int nEvents = 4;
            const int nCategories = 4;
            var bins = SampleMultinomial(nCategories, nEvents, distribution.Length, sampler);

            bool passed = ValidateDistribution(distribution, bins, 0.01f);
            Assert.IsTrue(passed);
        }

        private static int[] SampleMultinomial(int nCategories, int nEvents, int binCount, DistributionSampler sampler)
        {
            int nTotal = nCategories*(nCategories + 1)/2;
            var probabilityVector = new float[nCategories];
            for (int i = 0; i < nCategories; i++)
            {
                probabilityVector[i] = (float) (i + 1)/nTotal;
            }

            var bins = new int[binCount];
            var events = new int[nCategories];
            for (int i = 0; i < SampleCount; i++)
            {
                sampler.GenerateMultinomial(nEvents, probabilityVector, events);
                int index = 0;
                for (int j = nCategories - 1; j >= 0; j--)
                {
                    index *= (nEvents + 1);
                    index += events[j];
                }
                bins[index]++;
            }

            return bins;
        }

        private static int[] SampleContinuous(Continuous sampleFunction, int binCount)
        {
            var bins = new int[binCount];

            for (int i = 0; i < SampleCount; i++)
            {
                float sample = sampleFunction();
                var index = (int) (sample*10);
                if (index < binCount)
                {
                    bins[index]++;
                }
            }

            return bins;
        }

        private static int[] SampleDiscrete(Discrete sampleFunction, int binCount)
        {
            var bins = new int[binCount];

            for (int i = 0; i < SampleCount; i++)
            {
                int sample = sampleFunction();
                if (sample < binCount)
                {
                    bins[sample]++;
                }
            }

            return bins;
        }

        private static bool ValidateDistribution(float[] distribution, int[] bins,
                                                 float errorLimit, int startingIndex = 0)
        {
            var failures = new List<int>();
            bool passed  = true;

            for (int i = startingIndex; i < bins.Length; i++)
            {
                float expected = distribution[i]*SampleCount;
                float error = Math.Abs(expected - bins[i])/expected;
                if (error > errorLimit)
                {
                    passed = false;
                    failures.Add(i);
                }
                float density = ((float) bins[i])/SampleCount;
                Console.WriteLine("{0},{1}", distribution[i], density);
            }

            foreach (int index in failures)
            {
                Console.WriteLine("[{0}]: ({1} -{2})/{1} > {3}", index, distribution[index] * SampleCount, bins[index], errorLimit);
            }

            return passed;
        }

        private T[] LoadDistributionProfile<T>(string filename)
        {
            var data = new List<T>();
            MethodInfo parse = typeof (T).GetMethod("Parse", new[] {typeof (string)});

            using (var sr = new StreamReader(filename))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    int iComment = line.IndexOf("//", StringComparison.Ordinal);
                    if (iComment >= 0)
                    {
                        line = line.Remove(iComment);
                    }
                    if (line.Length > 0)
                    {
                        var value = (T) parse.Invoke(null, new object[] {line});
                        data.Add(value);
                    }
                }
            }

            return data.ToArray();
        }

        [Test]
        public void SmallFactorialTest()
        {
            long fact = RandLibSampler.Factorial(10);
            Assert.AreEqual(2*3*4*5*6*7*8*9*10, fact);
        }

        [Test]
        public void LargeFactorialTest()
        {
            long fact = RandLibSampler.Factorial(20);
            Assert.AreEqual(2432902008176640000, fact);
        }

        [Test]
        public void AesCounterToStringTest()
        {
            DistributionSampler sampler = GetAesCounterSampler();
            Assert.AreEqual("RandLib Distribution Sampler (AesCounter Random Variate Generator (AesCounter PRNG))", sampler.ToString());
        }

        [Test]
        public void RandLibToStringTest()
        {
            DistributionSampler sampler = GetRandLibSampler();
            Assert.AreEqual("RandLib Distribution Sampler (RandLib Random Variate Generator (RandLib PRNG))", sampler.ToString());
        }
    }
}
