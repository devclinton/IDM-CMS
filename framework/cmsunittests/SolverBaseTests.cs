using System;
using compartments.solvers.solverbase;
using NUnit.Framework;

namespace cmsunittests
{
    [TestFixture]
    class SolverBaseTests : AssertionHelper
    {
        private const string _version = "version";
        private const string _description = "description";
        private const int _numRealizations = 10;
        private const int _numSamples = 20;
        private readonly float[] _sampleTimes = new[] { 0.0f, 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f, 10.0f, 11.0f, 12.0f, 13.0f, 14.0f, 15.0f, 16.0f, 17.0f, 18.0f, 19.0f };
        private readonly string[] _observableNames = new[] { "S", "E", "I", "R" };

        [Test]
        public void RealizationDataCtorTest()
        {
            int numObservables = _observableNames.Length;
            var realizationData = new RealizationData(_version, _description,
                                                      _numRealizations, _numSamples,
                                                      _observableNames, _sampleTimes, true);

            Assert.AreEqual(_version, realizationData.FrameworkVersion);
            Assert.AreEqual(_description, realizationData.BuildDescription);
            Assert.AreEqual(_numRealizations, realizationData.Runs);
            Assert.AreEqual(_numSamples, realizationData.Samples);
            Assert.AreEqual(numObservables, realizationData.ObservablesCount);
            CollectionAssert.AreEqual(_observableNames, realizationData.ObservableNames);

            Assert.AreEqual(numObservables * _numRealizations, realizationData.ChannelTitles.Length);
            Assert.AreEqual("S{0}", realizationData.ChannelTitles[0]);
            Assert.AreEqual("R{9}", realizationData.ChannelTitles[realizationData.ChannelTitles.Length - 1]);

            Assert.AreEqual(numObservables * _numRealizations, realizationData.ChannelData.Length);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage = "must have a version string", MatchType = MessageMatch.Contains)]
        public void RealizationDataNullVersion()
        {
            var realizationData = new RealizationData(null, _description, _numRealizations, _numSamples, _observableNames, _sampleTimes, true);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "must have a version string", MatchType = MessageMatch.Contains)]
        public void RealizationDataEmptyVersion()
        {
            var realizationData = new RealizationData(String.Empty, _description, _numRealizations, _numSamples, _observableNames, _sampleTimes, true);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage = "must have a version description string", MatchType = MessageMatch.Contains)]
        public void RealizationDataNullDescription()
        {
            var realizationData = new RealizationData(_version, null, _numRealizations, _numSamples, _observableNames, _sampleTimes, true);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "must have a version description string", MatchType = MessageMatch.Contains)]
        public void RealizationDataEmptyDescription()
        {
            var realizationData = new RealizationData(_version, String.Empty, _numRealizations, _numSamples, _observableNames, _sampleTimes, true);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "must have one or more realizations", MatchType = MessageMatch.Contains)]
        public void RealizationDataZeroRuns()
        {
            var realizationData = new RealizationData(_version, _description, 0, _numSamples, _observableNames, _sampleTimes, true);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "must have one or more samples", MatchType = MessageMatch.Contains)]
        public void RealizationDataZeroSamples()
        {
            var realizationData = new RealizationData(_version, _description, _numRealizations, 0, _observableNames, _sampleTimes, true);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage = "must have observable names", MatchType = MessageMatch.Contains)]
        public void RealizationDataNullObservableNames()
        {
            var realizationData = new RealizationData(_version, _description, _numRealizations, _numSamples, null, _sampleTimes, true);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "must have one or more observables", MatchType = MessageMatch.Contains)]
        public void RealizationDataZeroObservables()
        {
            var realizationData = new RealizationData(_version, _description, _numRealizations, _numSamples, new string[0], _sampleTimes, true);
        }
    }
}
