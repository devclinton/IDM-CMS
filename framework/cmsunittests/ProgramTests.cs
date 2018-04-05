/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using System.IO;
using compartments;

using NUnit.Framework;
using compartments.emod;
using compartments.emod.utils;

namespace cmsunittests
{
    [TestFixture]
    class ProgramTests : AssertionHelper
    {
        [Test]
        public void ProcessArgumentsValid()
        {
            ExecutionParameters executionParameters;
            var args = new[] { "-model", "resources\\foo.emodl", "-cfg", "resources\\bar.cfg" };
            Assert.IsTrue(compartments.Program.ProcessArguments(args, out executionParameters));
            Assert.AreEqual("resources\\bar.cfg", executionParameters.ConfigFileName);
            Assert.AreEqual(314159.0, executionParameters.Duration);
            Assert.AreEqual("resources\\foo.emodl", executionParameters.ModelFileName);
            Assert.AreEqual(2012, executionParameters.Repeats);
            Assert.AreEqual(271828, executionParameters.Samples);
            Assert.AreEqual("SSA", executionParameters.SolverName);
            Assert.AreEqual(".", executionParameters.WorkingDirectory);
        }

        [Test]
        public void ProcessArgumentsInvalid()
        {
            ExecutionParameters executionParameters;
            var args = new[] {"-cfg", "bar.cfg", "-d", Path.GetTempPath()};
            Assert.IsFalse(compartments.Program.ProcessArguments(args, out executionParameters));
        }

        [Test, ExpectedException(typeof(NotSupportedException))]
        public void LoadModelSbml()
        {
            /*ModelInfo modelInfo =*/ compartments.Program.LoadModel("model.sbml");
            // Previous line should have thrown an exception.
        }

        [Test, ExpectedException(typeof(NotSupportedException))]
        public void LoadModelXml()
        {
            /*ModelInfo modelInfo =*/ compartments.Program.LoadModel("model.xml");
            // Previous line should have thrown an exception.
        }

        [Test, ExpectedException(typeof(NotSupportedException))]
        public void LoadModelBngl()
        {
            /*ModelInfo modelInfo =*/ compartments.Program.LoadModel("model.bngl");
            // Previous line should have thrown an exception.
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void LoadModelUnknown()
        {
            /*ModelInfo modelInfo =*/ compartments.Program.LoadModel("model.foo");
            // Previous line should have thrown an exception.
        }

        [Test]
        public void TestMain()
        {
            compartments.Program.Main(new[] { "-model", "resources\\testmodel.emodl", "-config", "resources\\testmodel.cfg" });
            Assert.IsTrue(File.Exists("testmodel.csv"));
            Assert.IsTrue(File.Exists("testmodel.mat"));
        }

        [Test]
        public void TestRunModel()
        {
            File.Delete("testmodel.csv");
            File.Delete("testmodel.json");
            Configuration.CurrentConfiguration =
                Configuration.ConfigurationFromString("{\"output\":{\"prefix\":\"testmodel\",\"writematfile\":true}}");
            ModelInfo modelInfo = compartments.Program.LoadModel("resources\\testmodel.emodl");
            compartments.Program.RunModel(modelInfo, "SSA", 1825.0, 10, 250);
            Assert.IsTrue(File.Exists("testmodel.csv"));
            Assert.IsTrue(File.Exists("testmodel.mat"));
        }

        [Test]
        public void TestExecuteModel()
        {
            ModelInfo modelInfo = compartments.Program.LoadModel("resources\\testmodel.emodl");
            const int runs = 10;
            const int samples = 250;
            SimulationResults results = compartments.Program.ExecuteModel(modelInfo, "SSA", 1825.0, runs, samples);
            Assert.AreEqual(runs*2, results.Labels.Length);
            Assert.AreEqual("Susceptible{0}", results.Labels[0]);
            Assert.AreEqual(runs*2, results.Data.Length);
            Assert.AreEqual(samples, results.Data[0].Length);
        }
    }
}
