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
            const double minimum = 0.0;
            const double maximum = 1.0;

            IValue uniform = new Uniform(minimum, maximum);
            double sum = 0.0;
            var values = new double[NumberOfSamples];

            for (int i = 0; i < NumberOfSamples; i++)
            {
                double value = uniform.Value;
                sum += value;
                values[i] = value;
            }

            double actualMean = sum / NumberOfSamples;
            const double expectedMean = (maximum + minimum) / 2;
            double accumulator = 0.0;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (double value in values)
            {
                double difference = value - actualMean;
                accumulator += (difference * difference);
            }

            double actualVariance = accumulator / NumberOfSamples;
            const double expectedVariance = (maximum - minimum) * (maximum - minimum) / 12.0;

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
            Console.WriteLine("Testing invalid Uniform(128, -128f) - minimum <= maximum");
            #pragma warning disable 168
            IValue uniform = new Uniform(128, -128f);
            #pragma warning restore 168
            Console.WriteLine("Invalid Uniform(128, -128f) FAILED.");
        }

        [Test]
        [Ignore, Description("This test is disabled until it is redesigned.")]
        public void TestNormal()
        {
            const double mu = 0.0;
            const double sigma = 1.0;
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

        private bool IsNormalDistribution(double mu, double sigma)
        {
            IValue normal = new Normal(mu, sigma*sigma);

            double computedMean = 0.0;

            for (int i = 0; i < NumberOfSamples; i++)
            {

                double value = normal.Value;
                computedMean += value;
            }

            computedMean = Math.Abs(computedMean/NumberOfSamples);

            const double alpha = 2.4;
            double criterion = Math.Pow(NumberOfSamples, -0.5)*sigma*alpha;

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
            Console.WriteLine("Testing invalid Normal(10.0, 0.0)");
            #pragma warning disable 168
            IValue normal = new Normal(10.0, 0.0);
            #pragma warning restore 168
            Console.WriteLine("Invalid Normal(10.0, 0.0) FAILED.");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "must be", MatchType = MessageMatch.Contains)]
        public void TestInvalidNormal2()
        {
            Console.WriteLine("Testing invalid Normal(10.0, -2.0)");
            #pragma warning disable 168
            IValue normal = new Normal(10.0, -2.0);
            #pragma warning restore 168
            Console.WriteLine("Invalid Normal(10.0, -2.0) FAILED.");
        }

        [Test]
        public void TestEmpiricalFromFile()
        {
            Console.WriteLine("Testing Empirical.FromFile()");
            Empirical empirical = Empirical.FromFile("resources\\americanages.txt");

            Console.WriteLine("Checking bin count.");
            Expect(empirical.BinCount == 16);

            var edges = new[] { 0.0, 5.0, 10.0, 15.0, 20.0, 25.0, 30.0, 35.0, 40.0, 45.0, 50.0, 55.0, 60.0, 65.0, 75.0, 85.0, 100.0 };
            Console.WriteLine("Checking bin edges (#).");
            Expect(empirical.BinEdges.Length == edges.Length);
            Console.WriteLine("Checking bin edges (values).");
            for (int iEdge = 0; iEdge < edges.Length; iEdge++)
                Expect(empirical.BinEdges[iEdge] == edges[iEdge]);

            var probabilities = new[] {
                0.068139579,
                0.073020819,
                0.072944671,
                0.071844401,
                0.067382041,
                0.068870283,
                0.072882992,
                0.080687295,
                0.079745843,
                0.071396427,
                0.062488581,
                0.047861576,
                0.038395993,
                0.065350307,
                0.043924148,
                0.015065044
            };
            Console.WriteLine("Checking bin values (probabilities).");
            for (int iBin = 0; iBin < empirical.BinCount; iBin++)
                Expect(empirical[iBin] == probabilities[iBin]);

            Console.WriteLine("Empirical.FromFile() passed.");
        }
    }
}
