using System;
using System.Linq;
using NUnit.Framework;
using compartments;
using compartments.emod.utils;

namespace cmsunittests
{
    [TestFixture]
    class InitialPopulationTests
    {
        [Test]
        public void FixedPopulationTest()
        {
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString("{}");
            RNGFactory.Reset();
            var modelInfo = compartments.emodl.EmodlLoader.LoadEMODLModel("(import (rnrs) (emodl cmslib)) (start-model \"fixedPopulation\") (species S 42) (species I 1) (observe susceptible S) (reaction infect (S) (I) (* S I)) (end-model)");
            var results = compartments.Program.ExecuteModel(modelInfo, "SSA", 10.0, 1000, 4);
            foreach (var vector in results.Data)
            {
                Assert.AreEqual(42.0, vector[0]);
            }
        }

        [Test]
        public void UniformPopulationTest()
        {
            const string distribution = "uniform 21 64";
            double mean;
            double stdDev;
            RunPopulationTest(distribution, out mean, out stdDev);
            Assert.IsTrue((40.0 <= mean) && (mean <= 44.0));
            Assert.IsTrue((11.0 <= stdDev) && (stdDev <= 14.0));
        }

        private static void RunPopulationTest(string distribution, out double mean, out double stdDev)
        {
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(string.Format("{{\"prng_seed\":{0},\"prng_index\":{1}}}", DateTime.Now.Millisecond, DateTime.Now.DayOfYear));
            RNGFactory.Reset();
            const string modelTemplate = "(import (rnrs) (emodl cmslib)) (start-model \"fixedPopulation\") (species S ({0})) (species I 1) (observe susceptible S) (reaction infect (S) (I) (* S I)) (end-model)";
            var modelInfo = compartments.emodl.EmodlLoader.LoadEMODLModel(string.Format(modelTemplate, distribution));
            var results = compartments.Program.ExecuteModel(modelInfo, "SSA", 10.0, 1000, 4);
            double sum = results.Data.Sum(vector => vector[0]);
            double sumSqr = results.Data.Sum(vector => (vector[0]*vector[0]));
            double n = results.Data.Length;
            mean = sum/n;
            stdDev = Math.Sqrt((sumSqr - 2*mean*sum + n*mean*mean)/n);
            Console.WriteLine("initial population (mean):      {0}", mean);
            Console.WriteLine("initial population (std. dev.): {0}", stdDev);
        }

        [Test]
        public void NormalPopulationTest()
        {
            const string distribution = "normal 42 4";
            double mean;
            double stdDev;
            RunPopulationTest(distribution, out mean, out stdDev);
            Assert.IsTrue((40.0 <= mean) && (mean <= 44.0));
            Assert.IsTrue((3.5 <= stdDev) && (stdDev <= 4.5));
        }
    }
}
