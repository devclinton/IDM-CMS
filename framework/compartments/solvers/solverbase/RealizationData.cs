using System;
using System.Collections.Generic;

namespace compartments.solvers.solverbase
{
    public class RealizationData
    {
        // JSON format/schema:
        // {
        //     "FrameworkVersion" : framework version string,
        //     "BuildDescription" : executable build description,
        //     "ObservablesCount" : #observables,
        //     "Runs" : #runs,
        //     "Samples" : #samples/run,
        //     "ObservableNames" : [
        //         "observable1",
        //         "observable2",
        //         "...",
        //         "observableO",
        //     ],
        //     "ChannelTitles" : [
        //         "title1",
        //         "title2",
        //         "...",
        //         "titleOxR"
        //     ],
        //     "SampleTimes" : [
        //         t0,
        //         t1,
        //         ...,
        //         tS,
        //     ],
        //     "ChannelData" : [
        //         [ o1r1.1, o1r1.2, o1r1.3, o1r1.4, o1r1.5, ..., o1r1.S ],
        //         [ o1r2.1, o1r2.2, o1r2.3, o1r2.4, o1r2.5, ..., o1r2.S ],
        //         [ ... ],
        //         [ o1rR.1, o1rR.2, o1rR.3, o1rR.4, o1rR.5, ..., o1rR.S ],
        //         [ o2r1.1, o2r1.2, o2r1.3, o2r1.4, o2r1.5, ..., o2r1.S ],
        //         [ o2r2.1, o2r2.2, o2r2.3, o2r2.4, o2r2.5, ..., o2r2.S ],
        //         [ ... ],
        //         [ o2rR.1, o2rR.2, o2rR.3, o2rR.4, o2rR.5, ..., o2rR.S ],
        //         [ ... ],
        //         [ oOr1.1, oOr1.2, oOr1.3, oOr1.4, oOr1.5, ..., oOr1.S ],
        //         [ oOr2.1, oOr2.2, oOr2.3, oOr2.4, oOr2.5, ..., oOr2.S ],
        //         [ ... ],
        //         [ oOrR.1, oOrR.2, oOrR.3, oOrR.4, oOrR.5, ..., oOrR.S ]
        //     ]
        // }
        public string FrameworkVersion;
        public string BuildDescription;
        public int Runs;
        public int Samples;
        public int ObservablesCount { get { return ObservableNames.Length; } }
        public string[] ObservableNames;
        public string[] ChannelTitles;
        public double[] SampleTimes;
        public double[][] ChannelData;

        public RealizationData(string version, string description, int runCount, int sampleCount, string[] observableNames, double[] sampleTimes, bool includeChannelTitles)
        {
            ValidateArguments(version, description, runCount, sampleCount, observableNames);

            FrameworkVersion = version;
            BuildDescription = description;
            Runs             = runCount;
            Samples          = sampleCount;
            ObservableNames  = observableNames;
            ChannelTitles    = includeChannelTitles ? ConstructChannelTitles(runCount, observableNames) : null;
            SampleTimes      = sampleTimes;
            ChannelData      = new double[ObservablesCount * runCount][];
        }

        private string[] ConstructChannelTitles(int runCount, IEnumerable<string> observableNames)
        {
            var channelTitles = new string[ObservablesCount*runCount];

            int iTitle = 0;
            foreach (var name in observableNames)
            {
                for (int run = 0; run < runCount; run++)
                {
                    // Use double '{' and '}' to differentiate the literal characters
                    // from the beginning and end of an argument placeholder.
                    channelTitles[iTitle++] = string.Format("{0}{{{1}}}", name, run);
                }
            }

            return channelTitles;
        }

        private static void ValidateArguments(string version, string description, int runCount, int sampleCount, string[] observableNames)
        {
            if (version == null)
                throw new ArgumentNullException("version", "RealizationData must have a version string.");

            if (version.Length < 1)
                throw new ArgumentException("RealizationData must have a version string.", "version");

            if (description == null)
                throw new ArgumentNullException("description", "RealizationData must have a version description string.");

            if (description.Length < 1)
                throw new ArgumentException("RealizationData must have a version description string.", "description");

            if (runCount < 1)
                throw new ArgumentException("RealizationData must have one or more realizations.", "runCount");

            if (sampleCount < 1)
                throw new ArgumentException("RealizationData must have one or more samples.", "sampleCount");

            if (observableNames == null)
                throw new ArgumentNullException("observableNames", "RealizationData must have observable names.");

            if (observableNames.Length < 1)
                throw new ArgumentException("RealizationData must have one or more observables.", "observableCount");
        }
    }
}
