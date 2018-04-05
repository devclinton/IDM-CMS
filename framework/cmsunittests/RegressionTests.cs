/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using compartments;
using compartments.emod;
using compartments.emodl;
using compartments.solvers;
using NUnit.Framework;

namespace cmsunittests
{
    [TestFixture, Description("Regression Tests")]
    class RegressionTests : AssertionHelper
    {
        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "Reaction propensity evaluated to negative ('exposure')", MatchType = MessageMatch.Contains)]
        public void TestCatchNegativePropensityGillespie()
        {
            const string configString = @"{""solver"":""SSA""}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configString);

            ModelInfo modelInfo = EmodlLoader.LoadEMODLFile("resources\\negativepropensity.emodl");
            const double duration = 6.28318531;
            const int repeats = 42;
            const int samples = 100;
            var solver = new Gillespie(modelInfo, duration, repeats, samples);

            solver.Solve();

            Assert.Fail("Execution should fail on negative reaction propensity.");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "Reaction propensity evaluated to negative ('exposure')", MatchType = MessageMatch.Contains)]
        public void TestCatchNegativePropensityFirstReaction()
        {
            const string configString = @"{""solver"":""First""}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configString);

            ModelInfo modelInfo = EmodlLoader.LoadEMODLFile("resources\\negativepropensity.emodl");
            const double duration = 6.28318531;
            const int repeats = 42;
            const int samples = 100;
            var solver = new GillespieFirstReaction(modelInfo, duration, repeats, samples);

            solver.Solve();

            Assert.Fail("Execution should fail on negative reaction propensity.");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "Reaction propensity evaluated to negative ('exposure')", MatchType = MessageMatch.Contains)]
        public void TestCatchNegativePropensityNextReaction()
        {
            const string configString = @"{""solver"":""NextReaction""}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configString);

            ModelInfo modelInfo = EmodlLoader.LoadEMODLFile("resources\\negativepropensity.emodl");
            const double duration = 6.28318531;
            const int repeats = 42;
            const int samples = 100;
            var solver = new GibsonBruck(modelInfo, duration, repeats, samples);

            solver.Solve();

            Assert.Fail("Execution should fail on negative reaction propensity.");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "Reaction propensity evaluated to negative ('exposure')", MatchType = MessageMatch.Contains)]
        public void TestCatchNegativePropensityBleaping()
        {
            const string configString = @"{""solver"":""B""}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configString);

            ModelInfo modelInfo = EmodlLoader.LoadEMODLFile("resources\\negativepropensity.emodl");
            const double duration = 6.28318531;
            const int repeats = 42;
            const int samples = 100;
            var solver = new BLeaping(modelInfo, duration, repeats, samples);

            solver.Solve();

            Assert.Fail("Execution should fail on negative reaction propensity.");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "Reaction propensity evaluated to negative ('exposure')", MatchType = MessageMatch.Contains)]
        public void TestCatchNegativePropensityHybridSsa()
        {
            const string configString = @"{""solver"":""Hybrid""}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configString);

            ModelInfo modelInfo = EmodlLoader.LoadEMODLFile("resources\\negativepropensity.emodl");
            const double duration = 6.28318531;
            const int repeats = 42;
            const int samples = 100;
            var solver = new HybridSSA(modelInfo, duration, repeats, samples);

            solver.Solve();

            Assert.Fail("Execution should fail on negative reaction propensity.");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "Reaction propensity evaluated to negative ('exposure')", MatchType = MessageMatch.Contains)]
        public void TestCatchNegativePropensityMidpoint()
        {
            const string configString = @"{""solver"":""MidPoint""}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configString);

            ModelInfo modelInfo = EmodlLoader.LoadEMODLFile("resources\\negativepropensity.emodl");
            const double duration = 6.28318531;
            const int repeats = 42;
            const int samples = 100;
            var solver = new MidPoint(modelInfo, duration, repeats, samples);

            solver.Solve();

            Assert.Fail("Execution should fail on negative reaction propensity.");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "Reaction propensity evaluated to negative ('exposure')", MatchType = MessageMatch.Contains)]
        public void TestCatchNegativePropensityRleaping()
        {
            const string configString = @"{""solver"":""RLeaping""}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configString);

            ModelInfo modelInfo = EmodlLoader.LoadEMODLFile("resources\\negativepropensity.emodl");
            const double duration = 6.28318531;
            const int repeats = 42;
            const int samples = 100;
            var solver = new RLeaping(modelInfo, duration, repeats, samples);

            solver.Solve();

            Assert.Fail("Execution should fail on negative reaction propensity.");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "Reaction propensity evaluated to negative ('exposure')", MatchType = MessageMatch.Contains)]
        public void TestCatchNegativePropensityRleapingFast()
        {
            const string configString = @"{""solver"":""RLeapingFast""}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configString);

            ModelInfo modelInfo = EmodlLoader.LoadEMODLFile("resources\\negativepropensity.emodl");
            const double duration = 6.28318531;
            const int repeats = 42;
            const int samples = 100;
            var solver = new RLeapingFast(modelInfo, duration, repeats, samples);

            solver.Solve();

            Assert.Fail("Execution should fail on negative reaction propensity.");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "Reaction propensity evaluated to negative ('exposure')", MatchType = MessageMatch.Contains)]
        public void TestCatchNegativePropensityTauLeaping()
        {
            const string configString = @"{""solver"":""TauLeaping""}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configString);

            ModelInfo modelInfo = EmodlLoader.LoadEMODLFile("resources\\negativepropensity.emodl");
            const double duration = 6.28318531;
            const int repeats = 42;
            const int samples = 100;
            var solver = new TauLeaping(modelInfo, duration, repeats, samples);

            solver.Solve();

            Assert.Fail("Execution should fail on negative reaction propensity.");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "Reaction propensity evaluated to infinity ('exposure')", MatchType = MessageMatch.Contains)]
        public void TestCatchInfinitePropensityGillespie()
        {
            const string configString = @"{""solver"":""SSA""}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configString);

            ModelInfo modelInfo = EmodlLoader.LoadEMODLFile("resources\\infinitepropensity.emodl");
            const double duration = 6.28318531;
            const int repeats = 42;
            const int samples = 100;
            var solver = new Gillespie(modelInfo, duration, repeats, samples);

            solver.Solve();

            Assert.Fail("Execution should fail on reaction propensity evaluates to infinity.");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "Reaction propensity evaluated to infinity ('exposure')", MatchType = MessageMatch.Contains)]
        public void TestCatchInfinitePropensityFirstReaction()
        {
            const string configString = @"{""solver"":""First""}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configString);

            ModelInfo modelInfo = EmodlLoader.LoadEMODLFile("resources\\infinitepropensity.emodl");
            const double duration = 6.28318531;
            const int repeats = 42;
            const int samples = 100;
            var solver = new GillespieFirstReaction(modelInfo, duration, repeats, samples);

            solver.Solve();

            Assert.Fail("Execution should fail on reaction propensity evaluates to infinity.");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "Reaction propensity evaluated to infinity ('exposure')", MatchType = MessageMatch.Contains)]
        public void TestCatchInfinitePropensityNextReaction()
        {
            const string configString = @"{""solver"":""NextReaction""}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configString);

            ModelInfo modelInfo = EmodlLoader.LoadEMODLFile("resources\\infinitepropensity.emodl");
            const double duration = 6.28318531;
            const int repeats = 42;
            const int samples = 100;
            var solver = new GibsonBruck(modelInfo, duration, repeats, samples);

            solver.Solve();

            Assert.Fail("Execution should fail on reaction propensity evaluates to infinity.");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "Reaction propensity evaluated to infinity ('exposure')", MatchType = MessageMatch.Contains)]
        public void TestCatchInfinitePropensityBleaping()
        {
            const string configString = @"{""solver"":""B""}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configString);

            ModelInfo modelInfo = EmodlLoader.LoadEMODLFile("resources\\infinitepropensity.emodl");
            const double duration = 6.28318531;
            const int repeats = 42;
            const int samples = 100;
            var solver = new BLeaping(modelInfo, duration, repeats, samples);

            solver.Solve();

            Assert.Fail("Execution should fail on reaction propensity evaluates to infinity.");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "Reaction propensity evaluated to infinity ('exposure')", MatchType = MessageMatch.Contains)]
        public void TestCatchInfinitePropensityHybridSsa()
        {
            const string configString = @"{""solver"":""Hybrid""}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configString);

            ModelInfo modelInfo = EmodlLoader.LoadEMODLFile("resources\\infinitepropensity.emodl");
            const double duration = 6.28318531;
            const int repeats = 42;
            const int samples = 100;
            var solver = new HybridSSA(modelInfo, duration, repeats, samples);

            solver.Solve();

            Assert.Fail("Execution should fail on reaction propensity evaluates to infinity.");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "Reaction propensity evaluated to infinity ('exposure')", MatchType = MessageMatch.Contains)]
        public void TestCatchInfinitePropensityMidpoint()
        {
            const string configString = @"{""solver"":""MidPoint""}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configString);

            ModelInfo modelInfo = EmodlLoader.LoadEMODLFile("resources\\infinitepropensity.emodl");
            const double duration = 6.28318531;
            const int repeats = 42;
            const int samples = 100;
            var solver = new MidPoint(modelInfo, duration, repeats, samples);

            solver.Solve();

            Assert.Fail("Execution should fail on reaction propensity evaluates to infinity.");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "Reaction propensity evaluated to infinity ('exposure')", MatchType = MessageMatch.Contains)]
        public void TestCatchInfinitePropensityRleaping()
        {
            const string configString = @"{""solver"":""RLeaping""}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configString);

            ModelInfo modelInfo = EmodlLoader.LoadEMODLFile("resources\\infinitepropensity.emodl");
            const double duration = 6.28318531;
            const int repeats = 42;
            const int samples = 100;
            var solver = new RLeaping(modelInfo, duration, repeats, samples);

            solver.Solve();

            Assert.Fail("Execution should fail on reaction propensity evaluates to infinity.");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "Reaction propensity evaluated to infinity ('exposure')", MatchType = MessageMatch.Contains)]
        public void TestCatchInfinitePropensityRleapingFast()
        {
            const string configString = @"{""solver"":""RLeapingFast""}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configString);

            ModelInfo modelInfo = EmodlLoader.LoadEMODLFile("resources\\infinitepropensity.emodl");
            const double duration = 6.28318531;
            const int repeats = 42;
            const int samples = 100;
            var solver = new RLeapingFast(modelInfo, duration, repeats, samples);

            solver.Solve();

            Assert.Fail("Execution should fail on reaction propensity evaluates to infinity.");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "Reaction propensity evaluated to infinity ('exposure')", MatchType = MessageMatch.Contains)]
        public void TestCatchInfinitePropensityTauLeaping()
        {
            const string configString = @"{""solver"":""TauLeaping""}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configString);

            ModelInfo modelInfo = EmodlLoader.LoadEMODLFile("resources\\infinitepropensity.emodl");
            const double duration = 6.28318531;
            const int repeats = 42;
            const int samples = 100;
            var solver = new TauLeaping(modelInfo, duration, repeats, samples);

            solver.Solve();

            Assert.Fail("Execution should fail on reaction propensity evaluates to infinity.");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "Reaction propensity evaluated to NaN ('exposure')", MatchType = MessageMatch.Contains)]
        public void TestCatchNaNPropensityGillespie()
        {
            const string configString = @"{""solver"":""SSA""}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configString);

            ModelInfo modelInfo = EmodlLoader.LoadEMODLFile("resources\\nanpropensity.emodl");
            const double duration = 6.28318531;
            const int repeats = 42;
            const int samples = 100;
            var solver = new Gillespie(modelInfo, duration, repeats, samples);

            solver.Solve();

            Assert.Fail("Execution should fail on reaction propensity evaluates to NaN.");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "Reaction propensity evaluated to NaN ('exposure')", MatchType = MessageMatch.Contains)]
        public void TestCatchNaNPropensityFirstReaction()
        {
            const string configString = @"{""solver"":""First""}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configString);

            ModelInfo modelInfo = EmodlLoader.LoadEMODLFile("resources\\nanpropensity.emodl");
            const double duration = 6.28318531;
            const int repeats = 42;
            const int samples = 100;
            var solver = new GillespieFirstReaction(modelInfo, duration, repeats, samples);

            solver.Solve();

            Assert.Fail("Execution should fail on reaction propensity evaluates to NaN.");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "Reaction propensity evaluated to NaN ('exposure')", MatchType = MessageMatch.Contains)]
        public void TestCatchNaNPropensityNextReaction()
        {
            const string configString = @"{""solver"":""NextReaction""}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configString);

            ModelInfo modelInfo = EmodlLoader.LoadEMODLFile("resources\\nanpropensity.emodl");
            const double duration = 6.28318531;
            const int repeats = 42;
            const int samples = 100;
            var solver = new GibsonBruck(modelInfo, duration, repeats, samples);

            solver.Solve();

            Assert.Fail("Execution should fail on reaction propensity evaluates to NaN.");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "Reaction propensity evaluated to NaN ('exposure')", MatchType = MessageMatch.Contains)]
        public void TestCatchNaNPropensityBleaping()
        {
            const string configString = @"{""solver"":""B""}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configString);

            ModelInfo modelInfo = EmodlLoader.LoadEMODLFile("resources\\nanpropensity.emodl");
            const double duration = 6.28318531;
            const int repeats = 42;
            const int samples = 100;
            var solver = new BLeaping(modelInfo, duration, repeats, samples);

            solver.Solve();

            Assert.Fail("Execution should fail on reaction propensity evaluates to NaN.");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "Reaction propensity evaluated to NaN ('exposure')", MatchType = MessageMatch.Contains)]
        public void TestCatchNaNPropensityHybridSsa()
        {
            const string configString = @"{""solver"":""Hybrid""}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configString);

            ModelInfo modelInfo = EmodlLoader.LoadEMODLFile("resources\\nanpropensity.emodl");
            const double duration = 6.28318531;
            const int repeats = 42;
            const int samples = 100;
            var solver = new HybridSSA(modelInfo, duration, repeats, samples);

            solver.Solve();

            Assert.Fail("Execution should fail on reaction propensity evaluates to NaN.");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "Reaction propensity evaluated to NaN ('exposure')", MatchType = MessageMatch.Contains)]
        public void TestCatchNaNPropensityMidpoint()
        {
            const string configString = @"{""solver"":""MidPoint""}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configString);

            ModelInfo modelInfo = EmodlLoader.LoadEMODLFile("resources\\nanpropensity.emodl");
            const double duration = 6.28318531;
            const int repeats = 42;
            const int samples = 100;
            var solver = new MidPoint(modelInfo, duration, repeats, samples);

            solver.Solve();

            Assert.Fail("Execution should fail on reaction propensity evaluates to NaN.");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "Reaction propensity evaluated to NaN ('exposure')", MatchType = MessageMatch.Contains)]
        public void TestCatchNaNPropensityRleaping()
        {
            const string configString = @"{""solver"":""RLeaping""}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configString);

            ModelInfo modelInfo = EmodlLoader.LoadEMODLFile("resources\\nanpropensity.emodl");
            const double duration = 6.28318531;
            const int repeats = 42;
            const int samples = 100;
            var solver = new RLeaping(modelInfo, duration, repeats, samples);

            solver.Solve();

            Assert.Fail("Execution should fail on reaction propensity evaluates to NaN.");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "Reaction propensity evaluated to NaN ('exposure')", MatchType = MessageMatch.Contains)]
        public void TestCatchNaNPropensityRleapingFast()
        {
            const string configString = @"{""solver"":""RLeapingFast""}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configString);

            ModelInfo modelInfo = EmodlLoader.LoadEMODLFile("resources\\nanpropensity.emodl");
            const double duration = 6.28318531;
            const int repeats = 42;
            const int samples = 100;
            var solver = new RLeapingFast(modelInfo, duration, repeats, samples);

            solver.Solve();

            Assert.Fail("Execution should fail on reaction propensity evaluates to NaN.");
        }

        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "Reaction propensity evaluated to NaN ('exposure')", MatchType = MessageMatch.Contains)]
        public void TestCatchNaNPropensityTauLeaping()
        {
            const string configString = @"{""solver"":""TauLeaping""}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configString);

            ModelInfo modelInfo = EmodlLoader.LoadEMODLFile("resources\\nanpropensity.emodl");
            const double duration = 6.28318531;
            const int repeats = 42;
            const int samples = 100;
            var solver = new TauLeaping(modelInfo, duration, repeats, samples);

            solver.Solve();

            Assert.Fail("Execution should fail on reaction propensity evaluates to NaN.");
        }
    }
}
