using System.IO;
using System.IO.Compression;
using System.Linq;
using Newtonsoft.Json;
using compartments.emod.utils;

namespace compartments.solvers.solverbase
{
    static class JsonSupport
    {
        internal static JsonOutputOptions GetJsonOutputOptions(string prefix)
        {
            bool writeJsonFile = Configuration.CurrentConfiguration.GetParameterWithDefault("output.writejson", false);
            bool compressOutput = Configuration.CurrentConfiguration.GetParameterWithDefault("output.compress", false);
            bool channelTitles = Configuration.CurrentConfiguration.GetParameterWithDefault("output.channeltitles", false);

            var jsonOptions = new JsonOutputOptions { Filename = prefix + ".json", CompressOutput = compressOutput, WriteJsonFile = writeJsonFile, ChannelTitles = channelTitles };

            return jsonOptions;
        }

        internal static void WriteJsonFile(SolverBase.Trajectories trajectories, JsonOutputOptions outputOptions)
        {
            var jsonSerializer = new JsonSerializer();

            StreamWriter streamWriter;
            if (outputOptions.CompressOutput)
            {
                var fs = File.Open(outputOptions.Filename + ".gz", FileMode.OpenOrCreate, FileAccess.Write);
                var gz = new GZipStream(fs, CompressionMode.Compress);
                streamWriter = new StreamWriter(gz);
            }
            else
            {
                streamWriter = new StreamWriter(outputOptions.Filename);
            }

            using (streamWriter)
            using (var jsonWriter = new JsonTextWriter(streamWriter))
            {
                jsonWriter.Formatting = Formatting.Indented;
                jsonWriter.Indentation = 1;
                jsonWriter.IndentChar = '\t';

                int runCount = trajectories.Values.First().Length;
                int sampleCount = trajectories.Values.First()[0].Length;

                var realizationData = new RealizationData(VersionInfo.Version,
                                                          VersionInfo.Description,
                                                          runCount,
                                                          sampleCount, trajectories.Keys.Select(o => o.Name).ToArray(),
                                                          trajectories.SampleTimes,
                                                          outputOptions.ChannelTitles);

                int channelIndex = 0;
                foreach (Observable o in trajectories.Keys)
                {
                    for (int runIndex = 0; runIndex < trajectories[o].Length; runIndex++)
                    {
                        realizationData.ChannelData[channelIndex++] = trajectories[o][runIndex];
                    }
                }

                jsonSerializer.Serialize(jsonWriter, realizationData);
            }
        }
    }
}
