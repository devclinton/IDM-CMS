/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using NUnit.Framework;
using distlib.hidden;

namespace cmsunittests.distlib.hidden
{
    class RandLibTests : AssertionHelper
    {
        [Test]
        public void ConstructorDefaultsTest()
        {
            var randlib = new RandLib();

            Assert.AreEqual(1234567890, UtilityFunctions.GetField<int>(randlib, "_CgOne"));
            Assert.AreEqual(123456789, UtilityFunctions.GetField<int>(randlib, "_CgTwo"));
        }

        [Test]
        public void ConstructorTest()
        {
            int s1 = 20130116;
            int s2 = 19680201;
            var randlib = new RandLib(s1, s2);

            Assert.AreEqual(s1, UtilityFunctions.GetField<int>(randlib, "_CgOne"));
            Assert.AreEqual(s2, UtilityFunctions.GetField<int>(randlib, "_CgTwo"));
        }

        [Test]
        public void ToStringTest()
        {
            var randlib = new RandLib();
            Assert.AreEqual("RandLib PRNG", randlib.ToString());
        }
    }
}
