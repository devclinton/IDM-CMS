using System;
using NUnit.Framework;
using distlib.hidden;

namespace cmsunittests.distlib.hidden
{
    class AesCounterTests
    {
        [Test]
        public void ConstructorTest()
        {
            var aesCounter = new AesCounter(20130116, 19680201);
            Assert.AreNotEqual(IntPtr.Zero, UtilityFunctions.GetField<IntPtr>(aesCounter, "_nativeObject"));
            Assert.IsNotNull(UtilityFunctions.GetField<float[]>(aesCounter, "_floatCache"));
            Assert.Greater(UtilityFunctions.GetField<float[]>(aesCounter, "_floatCache").Length, 0);
            Assert.AreEqual(0, UtilityFunctions.GetField<int>(aesCounter, "_floatCacheIndex"));
            Assert.IsNotNull(UtilityFunctions.GetField<uint[]>(aesCounter, "_bitCache"));
            Assert.Greater(UtilityFunctions.GetField<uint[]>(aesCounter, "_bitCache").Length, 0);
            Assert.AreEqual(0, UtilityFunctions.GetField<int>(aesCounter, "_bitCacheIndex"));
        }

        [Test]
        public void FillFloatCacheTest()
        {
            var aesCounter = new AesCounter(20130116, 19680201);
            UtilityFunctions.CallMethod(aesCounter, "FillFloatCache");
            var currentCache = UtilityFunctions.GetField<float[]>(aesCounter, "_floatCache");
            var cacheCopy = new float[currentCache.Length];
            currentCache.CopyTo(cacheCopy, 0);
            UtilityFunctions.CallMethod(aesCounter, "FillFloatCache");
            for (int i = 0; i < currentCache.Length; i++)
            {
                Assert.AreNotEqual(cacheCopy[i], currentCache[i]);
            }
        }

        [Test]
        public void FillBitCacheTest()
        {
            var aesCounter = new AesCounter(20130116, 19680201);
            UtilityFunctions.CallMethod(aesCounter, "FillBitCache");
            var currentCache = UtilityFunctions.GetField<uint[]>(aesCounter, "_bitCache");
            var cacheCopy = new uint[currentCache.Length];
            currentCache.CopyTo(cacheCopy, 0);
            UtilityFunctions.CallMethod(aesCounter, "FillBitCache");
            for (int i = 0; i < currentCache.Length; i++)
            {
                Assert.AreNotEqual(cacheCopy[i], currentCache[i]);
            }
        }

        [Test]
        public void ToStringTest()
        {
            var aesCounter = new AesCounter(20130116, 19680201);
            Assert.AreEqual("AesCounter PRNG", aesCounter.ToString());
        }
    }
}
