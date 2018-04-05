/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using NUnit.Framework;
using compartments.emod.utils;

namespace cmsunittests.emod.utils
{
    [TestFixture]
    class DynamicHistogramTests : AssertionHelper
    {
        [Test]
        public void DynamicHistogramBinCountCtor()
        {
            const int binCount = 8;
            var histogram = new DynamicHistogram(binCount);

            Assert.AreEqual(0, histogram.SampleCount);
            Assert.AreEqual(binCount, histogram.BinCount);
        }

        [Test]
        public void DynamicHistogramVectorCtor()
        {
            const int binCount = 8;
            var bins = new long[binCount];
            var histogram = new DynamicHistogram(bins);

            Assert.AreEqual(0, histogram.SampleCount);
            Assert.AreEqual(binCount, histogram.BinCount);

            histogram.AddSample(1968);
            histogram.AddSample(2012);
            CollectionAssert.AreEqual(bins, histogram);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "count must be > 0", MatchType = MessageMatch.Contains)]
        public void DynamicHistogramCtorBadBinCount()
        {
            #pragma warning disable 168 // unused local variable
            var unused = new DynamicHistogram(-5);
            #pragma warning restore 168
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage = "must not be null", MatchType = MessageMatch.Contains)]
        public void DynamicHistogramCtorNullVector()
        {
            #pragma warning disable 168 // unused local variable
            var unused = new DynamicHistogram(null);
            #pragma warning restore 168
        }

        [Test]
        public void DynamicHistogramAddSample()
        {
            const int binCount = 8;
            var histogram = new DynamicHistogram(binCount);

            const long lowSample = 1968;
            histogram.AddSample(lowSample);
            const long highSample = 2012;
            histogram.AddSample(highSample);
            // Histogram lower bound is closed, therefore less OR equal
            Assert.LessOrEqual(histogram.LowerBound, lowSample);
            // Histogram upper bound is open, therefore strictly greater
            Assert.Greater(histogram.UpperBound, highSample);

            Assert.AreEqual(2, histogram.SampleCount);
            Assert.AreEqual(histogram.LowerBound + histogram.Width * histogram.BinCount, histogram.UpperBound);
        }
    }
}
