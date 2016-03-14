using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using compartments;
using compartments.cmdl;
using compartments.emod.utils;
using distlib.randomvariates;

namespace cmsunittests
{
    [TestFixture, Description("Pseudo-random number generator tests")]
    class PrngTests : AssertionHelper
    {
        private MethodInfo _resetMethodinfo;

        [TestFixtureSetUp]
        public void Init()
        {
            _resetMethodinfo = typeof (RNGFactory).GetMethod("Reset", BindingFlags.NonPublic | BindingFlags.Static);
        }

        [Test, Description("RNGFactory test - VANILLA")]
        public void RNGFactoryVanilla()
        {
            const string prngText = "vanilla";
            var prng = GetRngFromFactory(prngText);
            Assert.True(prng is DotNetVariateGenerator);
            Console.WriteLine("{0}() PASSED", MethodBase.GetCurrentMethod().Name);
        }

        [Test, Description("RNGFactory test - RANDLIB")]
        public void RNGFactoryRandLib()
        {
            const string prngText = "RandLib";
            var prng = GetRngFromFactory(prngText);
            Assert.True(prng is RandLibVariateGenerator);
            Console.WriteLine("{0}() PASSED", MethodBase.GetCurrentMethod().Name);
        }

        [Test, Description("RNGFactory test - pseudoDES")]
        public void RNGFactoryPseudoDes()
        {
            const string prngText = "pseudoDES";
            var prng = GetRngFromFactory(prngText);
            Assert.True(prng is PseudoDesVariateGenerator);
            Console.WriteLine("{0}() PASSED", MethodBase.GetCurrentMethod().Name);
        }

        [Test, Description("RNGFactory test - AESCOUNTER")]
        public void RNGFactoryAesCounter()
        {
            if (!AesCounterVariateGenerator.IsSupported)
                Assert.Ignore("CPU doesn't support AES instructions");

            const string prngText = "AESCOUNTER";
            var prng = GetRngFromFactory(prngText);
            Assert.True(prng is AesCounterVariateGenerator);
            Console.WriteLine("{0}() PASSED", MethodBase.GetCurrentMethod().Name);
        }

        private RandomVariateGenerator GetRngFromFactory(string prngText)
        {
            SetCurrentPrng(prngText);
            var prng = RNGFactory.GetRNG();
            return prng;
        }

        private void SetCurrentPrng(string prng)
        {
            const string configTemplate = "{\"RNG\":{\"type\":\"%PRNG%\"}}";
            string configText = configTemplate.Replace("%PRNG%", prng);
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configText);
            _resetMethodinfo.Invoke(null, null);
        }

        [Test, Description("SIR with Vanilla PRNG")]
        public void SirWithVanillaPrng()
        {
            const string prng = "VANILLA";
            var passed = ExecuteSirModel(prng);
            Assert.True(passed);
            Console.WriteLine("{0}() PASSED", MethodBase.GetCurrentMethod().Name);
        }

        [Test, Description("SIR with RandLib PRNG")]
        public void SirWithRandLibPrng()
        {
            const string prng = "RANDLIB";
            var passed = ExecuteSirModel(prng);
            Assert.True(passed);
            Console.WriteLine("{0}() PASSED", MethodBase.GetCurrentMethod().Name);
        }

        [Test, Description("SIR with PseudoDES PRNG")]
        public void SirWithPseudoDesPrng()
        {
            const string prng = "PseudoDES";
            var passed = ExecuteSirModel(prng);
            Assert.True(passed);
            Console.WriteLine("{0}() PASSED", MethodBase.GetCurrentMethod().Name);
        }

        [Test, Description("SIR with AesCounter PRNG")]
        public void SirWithAesCounterPrng()
        {
            if (!AesCounterVariateGenerator.IsSupported)
                Assert.Ignore("CPU doesn't support AES instructions");

            const string prng = "AesCounter";
            var passed = ExecuteSirModel(prng);
            Assert.True(passed);
            Console.WriteLine("{0}() PASSED", MethodBase.GetCurrentMethod().Name);
        }

        private bool ExecuteSirModel(string prng)
        {
            SetCurrentPrng(prng);
            var modelInfo = CmdlLoader.LoadCMDLFile("resources\\sir.cmdl");
            const int repeats = 128;
            const int samples = 100;
            var solver = SolverFactory.CreateSolver("SSA", modelInfo, repeats, 548.0f, samples);
            solver.Solve();
            var tempFilename = Path.GetTempFileName();
            var trajectoryData = solver.GetTrajectoryData();
            var means = new float[3][];
            var speciesMin = new[] {float.MaxValue, float.MaxValue, float.MaxValue};
            var speciesMax = new float[3];

            for (int species = 0; species < 3; species++)
            {
                means[species] = new float[samples];
                for (int trajectory = 0; trajectory < repeats; trajectory++)
                {
                    for (int sample = 0; sample < samples; sample++)
                    {
                        means[species][sample] += trajectoryData[species*repeats + trajectory][sample];
                    }
                }

                for (int sample = 0; sample < samples; sample++)
                {
                    var mean = means[species][sample]/repeats;
                    speciesMin[species] = Math.Min(speciesMin[species], mean);
                    speciesMax[species] = Math.Max(speciesMax[species], mean);
                    means[species][sample] = mean;
                }
            }

            const float minSmin = 19.0f;
            const float maxSmin = 27.0f;
            const float maxS    = 990.0f;
            const float minI    = 10.0f;
            const float minImax = 551.0f;
            const float maxImax = 571.0f;
            const float minR    = 0.0f;
            const float minRmax = 859.0f;
            const float maxRmax = 879.0f;

            Console.WriteLine("Susceptible: min = {0}, max = {1} (limits- {2} <= min <= {3}, max == {4})",
                speciesMin[0], speciesMax[0], minSmin, maxSmin, maxS);
            Console.WriteLine("Infectious:  min = {0}, max = {1} (limits- min == {2}, {3} <= max <= {4})",
                speciesMin[1], speciesMax[1], minI, minImax, maxImax);
            Console.WriteLine("Recovered:   min = {0}, max = {1} (limits- min == {2}, {3} <= max <= {4})",
                speciesMin[2], speciesMax[2], minR, minRmax, maxRmax);

            // ReSharper disable CompareOfFloatsByEqualityOperator
            var susceptibleOkay = (minSmin <= speciesMin[0]) && (speciesMin[0] <= maxSmin) && (speciesMax[0] == maxS);
            var infectiousOkay  = (speciesMin[1] == minI) && (minImax <= speciesMax[1]) && (speciesMax[1] <= maxImax);
            var recoveredOkay   = (speciesMin[2] == minR) && (minRmax <= speciesMax[2]) && (speciesMax[2] <= maxRmax);
            // ReSharper restore CompareOfFloatsByEqualityOperator

            if (!(susceptibleOkay && infectiousOkay && recoveredOkay))
            {
                solver.OutputData(tempFilename);
                Console.WriteLine("Wrote SIR simulation data (with {0} PRNG) to {1}", prng, tempFilename);
            }
            else
            {
                File.Delete(tempFilename);
            }

            var passed = susceptibleOkay && infectiousOkay && recoveredOkay;
            return passed;
        }

        [Test]
        public void RNGFactoryDefaultTest()
        {
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString("{}");
            _resetMethodinfo.Invoke(null, null);

            RandomVariateGenerator rng = RNGFactory.GetRNG();

            Console.WriteLine("Testing RNGFactory default value...");
            if (AesCounterVariateGenerator.IsSupported)
            {
                Expect(rng is AesCounterVariateGenerator);
            }
            else
            {
                Expect(rng is PseudoDesVariateGenerator);
            }
            Console.WriteLine("{0}() PASSED", MethodBase.GetCurrentMethod().Name);
        }

        [Test]
        [ExpectedException(typeof(TypeInitializationException))]
        public void RNGFactoryExceptionTest()
        {
            Console.WriteLine("Testing RNGFactory exception...");

            const string configString = "{\"RNG\":{\"type\":\"busted\",\"seed\":1968}}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configString);
            MethodInfo reset = typeof(RNGFactory).GetMethod("Reset", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static);
            try
            {
                reset.Invoke(null, null);
            }
            catch (Exception ex)
            {
                if (ex.InnerException is ArgumentException)
                {
                    Console.WriteLine("Caught TargetInvocationException with InnerException == ArgumentException");
                    throw new TypeInitializationException("RNGFactory.GeneratorType", null);
                }

                Console.WriteLine("Caught exception: {0}", ex.Message);
                throw ex.InnerException;
            }

            Console.WriteLine("{0}() FAILED", MethodBase.GetCurrentMethod().Name);
        }
    }
}
