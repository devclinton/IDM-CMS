/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using System.Text.RegularExpressions;
using NUnit.Framework;
using compartments;
using compartments.emod.interfaces;
using compartments.emod.utils;
using compartments.emodl;
using compartments.solvers;

namespace cmsunittests
{
    [TestFixture, Description("Utility Classes Tests")]
    class UtilsTests : AssertionHelper
    {
        private const string _sirModel = "(import (rnrs) (emodl cmslib))(start-model \"SIR\")(species S 990)(species I 10)(species R)(observe Susceptible S)(observe Infectious I)(observe Recovered R)(reaction Infect (S) (I) (* 0.0005 S I))(reaction Recover (I) (R) (* 0.143 I))(end-model)";
        [Test]
        public void RuntimeTest()
        {
            //Verify we are running on a 64-bit Environment
            var version = Environment.Version;
            Console.WriteLine("CLR Runtime Version: {0} ({1}-bit)", version, (IntPtr.Size * 8));
            Assert.AreEqual(64, IntPtr.Size * 8);
        }

        [Test]
        [Ignore("simulation results tests not yet implemented.")]
        public void SimulationResults()
        {
            Expect(false);
        }

        [Test]
        public void SolverFactoryTest()
        {
            var model = EmodlLoader.LoadEMODLModel(_sirModel);
            ISolver solver = SolverFactory.CreateSolver("SSA", model, 1, 2.0, 3);
            Assert.IsTrue(solver is Gillespie);
            solver = SolverFactory.CreateSolver("FIRST", model, 1, 2.0, 3);
            Assert.IsTrue(solver is GillespieFirstReaction);
            solver = SolverFactory.CreateSolver("NEXT", model, 1, 2.0, 3);
            Assert.IsTrue(solver is GibsonBruck);
            solver = SolverFactory.CreateSolver("HYBRID", model, 1, 2.0, 3);
            Assert.IsTrue(solver is HybridSSA);
            solver = SolverFactory.CreateSolver("TAU", model, 1, 2.0, 3);
            Assert.IsTrue(solver is TauLeaping);
            solver = SolverFactory.CreateSolver("MID", model, 1, 2.0, 3);
            Assert.IsTrue(solver is MidPoint);
            solver = SolverFactory.CreateSolver("R", model, 1, 2.0, 3);
            Assert.IsTrue(solver is RLeaping);
            solver = SolverFactory.CreateSolver("RFAST", model, 1, 2.0, 3);
            Assert.IsTrue(solver is RLeapingFast);
            solver = SolverFactory.CreateSolver("BLEAP", model, 1, 2.0, 3);
            Assert.IsTrue(solver is BLeaping);
            solver = SolverFactory.CreateSolver("TSSA", model, 1, 2.0, 3);
            Assert.IsTrue(solver is TransportSSA);
            solver = SolverFactory.CreateSolver("DFSP", model, 1, 2.0, 3);
            Assert.IsTrue(solver is DFSP);
            solver = SolverFactory.CreateSolver("OTSSA", model, 1, 2.0, 3);
            Assert.IsTrue(solver is OptimalTransportSSA);
/* Fractional diffusion solver doesn't handle single node models gracefully.
            solver = SolverFactory.CreateSolver("LEVY", model, 1, 2.0, 3);
            Assert.IsTrue(solver is FractionalDiffusion);
*/
/* Exit times solver doesn't handle missing target condition gracefully.
            solver = SolverFactory.CreateSolver("EXITTIMES", model, 1, 2.0, 3);
            Assert.IsTrue(solver is ExitTimes);
*/
/* dwSSA solver automatically starts solving
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString("{\"dwSSA\":{\"re_name\":\"I\",\"re_val\":500}}");
            solver = SolverFactory.CreateSolver("DWSSA", model, 1, 2.0, 3);
            Assert.IsTrue(solver is dwSSA);
*/
        }

        [Test]
        public void VersionInfoTest()
        {
            var regEx = new Regex(@"[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+");
            Assert.IsTrue(regEx.IsMatch(VersionInfo.Version));
            Assert.IsTrue(VersionInfo.Description.Contains("CMS"));
        }
    }
}
