using System;
using compartments.solvers.solverbase;
using NUnit.Framework;

namespace cmsunittests
{
    [TestFixture]
    class SolverBaseTests : AssertionHelper
    {
        private const string Version = "version";
        private const string Description = "description";
        private const int NumRealizations = 10;
        private const int NumSamples = 20;
        private readonly double[] _sampleTimes = { 0.0, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, 12.0, 13.0, 14.0, 15.0, 16.0, 17.0, 18.0, 19.0 };
        private readonly string[] _observableNames = { "S", "E", "I", "R" };

        [Test]
        public void RealizationDataCtorTest()
        {
            int numObservables = _observableNames.Length;
            var realizationData = new RealizationData(Version, Description,
                                                      NumRealizations, NumSamples,
                                                      _observableNames, _sampleTimes, true);

            Assert.AreEqual(Version, realizationData.FrameworkVersion);
            Assert.AreEqual(Description, realizationData.BuildDescription);
            Assert.AreEqual(NumRealizations, realizationData.Runs);
            Assert.AreEqual(NumSamples, realizationData.Samples);
            Assert.AreEqual(numObservables, realizationData.ObservablesCount);
            CollectionAssert.AreEqual(_observableNames, realizationData.ObservableNames);

            Assert.AreEqual(numObservables * NumRealizations, realizationData.ChannelTitles.Length);
            Assert.AreEqual("S{0}", realizationData.ChannelTitles[0]);
            Assert.AreEqual("R{9}", realizationData.ChannelTitles[realizationData.ChannelTitles.Length - 1]);

            Assert.AreEqual(numObservables * NumRealizations, realizationData.ChannelData.Length);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage = "must have a version string", MatchType = MessageMatch.Contains)]
        public void RealizationDataNullVersion()
        {
            var _ = new RealizationData(null, Description, NumRealizations, NumSamples, _observableNames, _sampleTimes, true);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "must have a version string", MatchType = MessageMatch.Contains)]
        public void RealizationDataEmptyVersion()
        {
            var _ = new RealizationData(String.Empty, Description, NumRealizations, NumSamples, _observableNames, _sampleTimes, true);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage = "must have a version description string", MatchType = MessageMatch.Contains)]
        public void RealizationDataNullDescription()
        {
            var _ = new RealizationData(Version, null, NumRealizations, NumSamples, _observableNames, _sampleTimes, true);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "must have a version description string", MatchType = MessageMatch.Contains)]
        public void RealizationDataEmptyDescription()
        {
            var _ = new RealizationData(Version, String.Empty, NumRealizations, NumSamples, _observableNames, _sampleTimes, true);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "must have one or more realizations", MatchType = MessageMatch.Contains)]
        public void RealizationDataZeroRuns()
        {
            var _ = new RealizationData(Version, Description, 0, NumSamples, _observableNames, _sampleTimes, true);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "must have one or more samples", MatchType = MessageMatch.Contains)]
        public void RealizationDataZeroSamples()
        {
            var _ = new RealizationData(Version, Description, NumRealizations, 0, _observableNames, _sampleTimes, true);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage = "must have observable names", MatchType = MessageMatch.Contains)]
        public void RealizationDataNullObservableNames()
        {
            var _ = new RealizationData(Version, Description, NumRealizations, NumSamples, null, _sampleTimes, true);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "must have one or more observables", MatchType = MessageMatch.Contains)]
        public void RealizationDataZeroObservables()
        {
            var _ = new RealizationData(Version, Description, NumRealizations, NumSamples, new string[0], _sampleTimes, true);
        }
    }
}
