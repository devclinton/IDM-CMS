/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System.IO;
using NUnit.Framework;
using compartments;

namespace cmsunittests
{
    [TestFixture]
    class ExecutionParameterTests : AssertionHelper
    {
        [Test, Description("ExecutionParameters constructor test")]
        public void ExecutionParametersConstructor()
        {
            var ep = new ExecutionParameters();
            Assert.AreEqual(string.Empty, ep.ConfigFileName);
            Assert.AreEqual(100.0, ep.Duration);
            Assert.AreEqual(string.Empty, ep.ModelFileName);
            Assert.AreEqual(1, ep.Repeats);
            Assert.AreEqual(100, ep.Samples);
            Assert.AreEqual("SSA", ep.SolverName);
            Assert.AreEqual(".", ep.WorkingDirectory);
            Assert.False(ep.WriteModelFile);
        }

        [Test, Description("ExecuationParameters ToString() test")]
        public void ExecutionParametersToString()
        {
            var ep = new ExecutionParameters();
            const string expectedString = "Model:    \r\nSolver:   SSA\r\nConfig:   \r\nDuration: 100\r\nSamples:  100\r\nRuns:     1\r\nWorking Directory: .\r\n";
            Assert.AreEqual(expectedString, ep.ToString());
        }
    }
}
