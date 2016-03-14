using System;
using NUnit.Framework;
using distlib.hidden;

namespace cmsunittests.distlib.hidden
{
    class PseudoDesTests : AssertionHelper
    {
        [Test]
        public void ConstructorTest()
        {
            var pseudoDes = new PseudoDes(20130116, 19680201);
            Assert.AreNotEqual(IntPtr.Zero, UtilityFunctions.GetField<IntPtr>(pseudoDes, "_nativeObject"));
            Assert.IsNotNull(UtilityFunctions.GetField<float[]>(pseudoDes, "_floatCache"));
            Assert.Greater(UtilityFunctions.GetField<float[]>(pseudoDes, "_floatCache").Length, 0);
            Assert.AreEqual(0, UtilityFunctions.GetField<int>(pseudoDes, "_floatCacheIndex"));
            Assert.IsNotNull(UtilityFunctions.GetField<uint[]>(pseudoDes, "_bitCache"));
            Assert.Greater(UtilityFunctions.GetField<uint[]>(pseudoDes, "_bitCache").Length, 0);
            Assert.AreEqual(0, UtilityFunctions.GetField<int>(pseudoDes, "_bitCacheIndex"));
        }

        [Test]
        public void FillFloatCacheTest()
        {
            var pseudoDes = new PseudoDes(20130116, 19680201);
            UtilityFunctions.CallMethod(pseudoDes, "FillFloatCache");
            var currentCache = UtilityFunctions.GetField<float[]>(pseudoDes, "_floatCache");
            var cacheCopy = new float[currentCache.Length];
            currentCache.CopyTo(cacheCopy, 0);
            UtilityFunctions.CallMethod(pseudoDes, "FillFloatCache");
            for (int i = 0; i < currentCache.Length; i++)
            {
                Assert.AreNotEqual(cacheCopy[i], currentCache[i]);
            }
        }

        [Test]
        public void FillBitCacheTest()
        {
            var pseudoDes = new PseudoDes(20130116, 19680201);
            UtilityFunctions.CallMethod(pseudoDes, "FillBitCache");
            var currentCache = UtilityFunctions.GetField<uint[]>(pseudoDes, "_bitCache");
            var cacheCopy = new uint[currentCache.Length];
            currentCache.CopyTo(cacheCopy, 0);
            UtilityFunctions.CallMethod(pseudoDes, "FillBitCache");
            for (int i = 0; i < currentCache.Length; i++)
            {
                Assert.AreNotEqual(cacheCopy[i], currentCache[i]);
            }
        }

        [Test]
        public void ToStringTest()
        {
            var pseudoDes = new PseudoDes(20130116, 19680201);
            Assert.AreEqual("PseudoDes PRNG", pseudoDes.ToString());
        }
    }
}
