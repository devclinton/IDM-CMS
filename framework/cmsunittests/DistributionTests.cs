using System;
using NUnit.Framework;
using compartments;
using compartments.emod.distributions;
using compartments.emod.interfaces;
using compartments.emod.utils;
// ReSharper disable UnusedVariable
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace cmsunittests
{
    [TestFixture, Description("Distribution function tests")]
    class DistributionTests : AssertionHelper
    {
        const int NumberOfSamples = (int)1.0e6;

        private bool IsUniformDistribution()
        {
            const float minimum = 0.0f;
            const float maximum = 1.0f;

            IValue uniform = new Uniform(minimum, maximum);
            float sum = 0.0f;
            var values = new float[NumberOfSamples];

            for (int i = 0; i < NumberOfSamples; i++)
            {
                float value = uniform.Value;
                sum += value;
                values[i] = value;
            }

            float actualMean = sum / NumberOfSamples;
            const float expectedMean = (maximum + minimum) / 2;
            float accumulator = 0.0f;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (float value in values)
            {
                float difference = value - actualMean;
                accumulator += (difference * difference);
            }

            float actualVariance = accumulator / NumberOfSamples;
            const float expectedVariance = (maximum - minimum) * (maximum - minimum) / 12.0f;

            Console.WriteLine("Uniform Distribution Test:");
            Console.WriteLine("MinimumOperator: {0} MaximumOperator:   {1}", minimum, maximum);
            Console.WriteLine("Expected mean:   {0} Expected variance: {1}", expectedMean, expectedVariance);
            Console.WriteLine("Actual mean:     {0} Actual variance:   {1}", actualMean, actualVariance);

            // x = 2.4 means < 1/1000 chance of false negative
            const double x = 2.4;
            double limit = x * Math.Sqrt(expectedVariance) / Math.Sqrt(NumberOfSamples);
            Console.WriteLine("99.9% limit: {0}", limit);
            Console.WriteLine("Actual:      {0}", Math.Abs(actualMean - expectedMean));
            Console.WriteLine();

            bool success = Math.Abs(actualMean - expectedMean) <= limit;

            return success;
        }

        [Test]
        [Ignore, Description("This test is disabled until it is redesigned.")]
        public void TestUniform()
        {
            int numOfFailures = 0;
            const int numOfTimesToRun = 1000;
            const int maxExpectedFailures = 15;

            for (int i = 0; i < numOfTimesToRun; i++)
            {
                Console.WriteLine("Test Run Number {0}", (i + 1));
                //if (IsNormalDistribution(mu, sigma) == false)
                if(IsUniformDistribution() == false)
                {
                    numOfFailures++;
                    Console.WriteLine("This failed {0} times.", numOfFailures);
                }
                //Reset the seed for the next run
                //Common.SetSeed(DateTime.Now.Millisecond, DateTime.Now.AddSeconds(10).Millisecond);
            }
            Console.WriteLine("End of Test. This failed {0} times.", numOfFailures);
            Expect(numOfFailures <= maxExpectedFailures);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "must be", MatchType = MessageMatch.Contains)]
        public void TestInvalidUniform()
        {
            Console.WriteLine("Testing invalid Uniform(128f, -128f) - minimum <= maximum");
            #pragma warning disable 168
            IValue uniform = new Uniform(128f, -128f);
            #pragma warning restore 168
            Console.WriteLine("Invalid Uniform(128f, -128f) FAILED.");
        }

        [Test]
        [Ignore, Description("This test is disabled until it is redesigned.")]
        public void TestNormal()
        {
            const float mu = 0.0f;
            const float sigma = 1.0f;
            int numOfFailures = 0;
            const int numOfTimesToRun = 1000;
            const int maxExpectedFailures = 15;

            for (int i = 0; i < numOfTimesToRun; i++)
            {
                Console.WriteLine("Test Run Number {0}", (i + 1));

                // Reset the seed for the next run
                Configuration.CurrentConfiguration =
                    Configuration.ConfigurationFromString($"{{\"prng_seed\":{DateTime.Now.Millisecond},\"prng_index\":{i}}}");
                RNGFactory.Reset();

                if (IsNormalDistribution(mu, sigma) == false)
                {
                    numOfFailures++;
                    Console.WriteLine("This failed {0} times.", numOfFailures);
                }
            }
            Console.WriteLine("End of Test. This failed {0} times.", numOfFailures);
            Expect(numOfFailures <= maxExpectedFailures);
        }

        private bool IsNormalDistribution(float mu, float sigma)
        {
            IValue normal = new Normal(mu, sigma*sigma);

            float computedMean = 0.0f;

            for (int i = 0; i < NumberOfSamples; i++)
            {
                
                float value = normal.Value;
                computedMean += value;
            }

            computedMean = Math.Abs(computedMean/NumberOfSamples);

            const float alpha = 2.4f;
            float criterion = (float)Math.Pow(NumberOfSamples, -0.5)*sigma*alpha;

            Console.WriteLine("Abs. Computed Mean: {0}", computedMean); 
            Console.WriteLine("Criterion: {0}", criterion); 
            Console.WriteLine("Abs. Difference: {0}", Math.Abs(computedMean - criterion));

            bool success = Math.Abs(computedMean - mu) <= criterion;

            return success;
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "must be", MatchType = MessageMatch.Contains)]
        public void TestInvalidNormal1()
        {
            Console.WriteLine("Testing invalid Normal(10.0f, 0.0f)");
            #pragma warning disable 168
            IValue normal = new Normal(10.0f, 0.0f);
            #pragma warning restore 168
            Console.WriteLine("Invalid Normal(10.0f, 0.0f) FAILED.");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "must be", MatchType = MessageMatch.Contains)]
        public void TestInvalidNormal2()
        {
            Console.WriteLine("Testing invalid Normal(10.0f, -2.0f)");
            #pragma warning disable 168
            IValue normal = new Normal(10.0f, -2.0f);
            #pragma warning restore 168
            Console.WriteLine("Invalid Normal(10.0f, -2.0f) FAILED.");
        }

        [Test]
        public void TestEmpiricalFromFile()
        {
            Console.WriteLine("Testing Empirical.FromFile()");
            Empirical empirical = Empirical.FromFile("resources\\americanages.txt");

            Console.WriteLine("Checking bin count.");
            Expect(empirical.BinCount == 16);

            var edges = new[] { 0.0f, 5.0f, 10.0f, 15.0f, 20.0f, 25.0f, 30.0f, 35.0f, 40.0f, 45.0f, 50.0f, 55.0f, 60.0f, 65.0f, 75.0f, 85.0f, 100.0f };
            Console.WriteLine("Checking bin edges (#).");
            Expect(empirical.BinEdges.Length == edges.Length);
            Console.WriteLine("Checking bin edges (values).");
            for (int iEdge = 0; iEdge < edges.Length; iEdge++)
                Expect(empirical.BinEdges[iEdge] == edges[iEdge]);

            var probabilities = new[] {
                0.068139579f,
                0.073020819f,
                0.072944671f,
                0.071844401f,
                0.067382041f,
                0.068870283f,
                0.072882992f,
                0.080687295f,
                0.079745843f,
                0.071396427f,
                0.062488581f,
                0.047861576f,
                0.038395993f,
                0.065350307f,
                0.043924148f,
                0.015065044f
            };
            Console.WriteLine("Checking bin values (probabilities).");
            for (int iBin = 0; iBin < empirical.BinCount; iBin++)
                Expect(empirical[iBin] == probabilities[iBin]);

            Console.WriteLine("Empirical.FromFile() passed.");
        }
    }
}
