/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using NUnit.Framework;
using distlib.randomvariates;

namespace cmsunittests.distlib.randomvariates
{
    class DotNetGeneratorTests : VariateGeneratorTestBase
    {
        private RandomVariateGenerator CreateVariateGenerator()
        {
            RandomVariateGenerator generator = DotNetVariateGenerator.CreateDotNetVariateGenerator(new uint[] {19860201});
            return generator;
        }

        [Test]
        public void UniformOpenOpen()
        {
            RandomVariateGenerator generator = CreateVariateGenerator();
            bool inBounds;
            var sample = TestUniformOpenOpen(generator, out inBounds);

            Assert.IsTrue(inBounds, string.Format("AesCounterVariateGenerator.GenerateUniformOO() failed with value {0}.", sample));
        }

        [Test]
        public void UniformOpenClosed()
        {
            RandomVariateGenerator generator = CreateVariateGenerator();
            bool inBounds;
            bool one;
            var sample = TestUniformOpenClosed(generator, out inBounds, out one);

            Assert.IsTrue(inBounds, string.Format("AesCounterVariateGenerator.GenerateUniformOC() failed with value {0}.", sample));
            Assert.IsTrue(one, "AesCounterVariateGenerator.GenerateUniformOC() failed to generate 1.0");
        }

        [Test]
        public void UniformClosedOpen()
        {
            RandomVariateGenerator generator = CreateVariateGenerator();
            bool inBounds;
            bool zero;
            var sample = TestUniformClosedOpen(generator, out inBounds, out zero);

            Assert.IsTrue(inBounds, string.Format("AesCounterVariateGenerator.GenerateUniformCO() failed with value {0}.", sample));
            Assert.IsTrue(zero, "AesCounterVariateGenerator.GenerateUniformCO() failed to generate 0.0");
        }

        [Test]
        public void UniformClosedClosed()
        {
            RandomVariateGenerator generator = CreateVariateGenerator();
            bool inBounds;
            bool zero;
            bool one;
            var sample = TestUniformClosedClosed(generator, out inBounds, out zero, out one);

            Assert.IsTrue(inBounds, string.Format("AesCounterVariateGenerator.GenerateUniformCC() failed with value {0}.", sample));
            Assert.IsTrue(zero, "AesCounterVariateGenerator.GenerateUniformCC() failed to generate 0.0");
            Assert.IsTrue(one, "AesCounterVariateGenerator.GenerateUniformCC() failed to generate 1.0");
        }

        [Test]
        public void ToStringTest()
        {
            RandomVariateGenerator generator = CreateVariateGenerator();
            Assert.AreEqual(".Net Random Variate Generator (System.Random)", generator.ToString());
        }
    }
}
