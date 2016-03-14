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
            var results = compartments.Program.ExecuteModel(modelInfo, "SSA", 10.0f, 1000, 4);
            foreach (var vector in results.Data)
            {
                Assert.AreEqual(42.0f, vector[0]);
            }
        }

        [Test]
        public void UniformPopulationTest()
        {
            const string distribution = "uniform 21 64";
            float mean;
            float stdDev;
            RunPopulationTest(distribution, out mean, out stdDev);
            Assert.IsTrue((40.0f <= mean) && (mean <= 44.0f));
            Assert.IsTrue((11.0f <= stdDev) && (stdDev <= 14.0f));
        }

        private static void RunPopulationTest(string distribution, out float mean, out float stdDev)
        {
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(string.Format("{{\"prng_seed\":{0},\"prng_index\":{1}}}", DateTime.Now.Millisecond, DateTime.Now.DayOfYear));
            RNGFactory.Reset();
            const string modelTemplate = "(import (rnrs) (emodl cmslib)) (start-model \"fixedPopulation\") (species S ({0})) (species I 1) (observe susceptible S) (reaction infect (S) (I) (* S I)) (end-model)";
            var modelInfo = compartments.emodl.EmodlLoader.LoadEMODLModel(string.Format(modelTemplate, distribution));
            var results = compartments.Program.ExecuteModel(modelInfo, "SSA", 10.0f, 1000, 4);
            float sum = results.Data.Sum(vector => vector[0]);
            float sumSqr = results.Data.Sum(vector => (vector[0]*vector[0]));
            float n = results.Data.Length;
            mean = sum/n;
            stdDev = (float) Math.Sqrt((sumSqr - 2*mean*sum + n*mean*mean)/n);
            Console.WriteLine("initial population (mean):      {0}", mean);
            Console.WriteLine("initial population (std. dev.): {0}", stdDev);
        }

        [Test]
        public void NormalPopulationTest()
        {
            const string distribution = "normal 42 4";
            float mean;
            float stdDev;
            RunPopulationTest(distribution, out mean, out stdDev);
            Assert.IsTrue((40.0f <= mean) && (mean <= 44.0f));
            Assert.IsTrue((3.5f <= stdDev) && (stdDev <= 4.5f));
        }
    }
}
