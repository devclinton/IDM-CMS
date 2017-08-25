using System;
using System.Collections.Generic;
using System.Linq;
using compartments.emod.utils;
using matfilelib;

namespace compartments.solvers.solverbase
{
    static class MatlabSupport
    {
        public static MatlabOutputOptions GetMatlabOutputOptions(string prefix)
        {
            bool writeMatFile = Configuration.CurrentConfiguration.GetParameterWithDefault("output.writematfile", false);
            bool compressOutput = Configuration.CurrentConfiguration.GetParameterWithDefault("output.compress", true);
            bool useNewFormat = Configuration.CurrentConfiguration.GetParameterWithDefault("output.newmatformat", false);

            var matlabOptions = new MatlabOutputOptions { Filename = prefix + ".mat", CompressOutput = compressOutput, WriteMatFile = writeMatFile, UseNewFormat = useNewFormat };

            return matlabOptions;
        }

        internal static void WriteMatFile(SolverBase.Trajectories trajectories, MatlabOutputOptions outputOptions)
        {
            var dotMatFile = new DotMatFile();

            int observableCount = trajectories.Keys.Count;
            int traceCount = trajectories.Values.First().Length;
            int sampleCount = trajectories.Values.First()[0].Length;

            Dictionary<string, MatrixElement> structure;

            if (!outputOptions.UseNewFormat)
            {
                /*
                 * version     = string
                 * observables = #observables rows x 1 column cell matrix
                 * sampletimes = 1 row x #samples columns matrix
                 * data        = (#observables x #realizations) rows x #samples columns matrix
                */
                var cellMatrix = new MatlabCell(new[] { observableCount, 1 });
                foreach (var observable in trajectories.Keys)
                {
                    cellMatrix.Contents.Add(new MatlabString(observable.Name));
                }

                structure = new Dictionary<string, MatrixElement>(4)
                {
                    {"version",     new MatlabString(VersionString)},
                    {"observables", cellMatrix},
                    {"sampletimes", new MatlabMatrix(trajectories.SampleTimes, new[] {1, sampleCount})},
                    {"data",        new MatlabMatrix(TrajectoryData(trajectories), new[] {observableCount*traceCount, sampleCount})}
                };
            }
            else
            {
                /*
                 * version = string
                 * sampletimes = 1 row x #samples columns matrix
                 * observable1 = #realizations rows x #samples columns matrix
                 * ...
                 * observable2 = #realizations rows x #samples columns matrix
                 * observableN = #realizations rows x #samples columns matrix
                */
                int elementCount = 2 + observableCount;
                structure = new Dictionary<string, MatrixElement>(elementCount)
                {
                    {"version",     new MatlabString(VersionString)},
                    {"sampletimes", new MatlabMatrix(trajectories.SampleTimes, new[] {1, sampleCount})}
                };

                foreach (var key in trajectories.Keys)
                {
                    structure.Add(key.Name, MatrixForObservable(trajectories[key]));
                }
            }

            var structMatrix = new MatlabStructure(structure);
            dotMatFile["data"] = structMatrix;
            dotMatFile.WriteToDisk(outputOptions.Filename, outputOptions.CompressOutput);
        }

        private static string VersionString => $"FrameworkVersion,\"{VersionInfo.Version}\",\"{VersionInfo.Description}\"";

        private static IEnumerable<int> TrajectoryData(SolverBase.Trajectories trajectories)
        {
            int rows = trajectories.Values.First().Length;
            int columns = trajectories.Values.First()[0].Length;

            for (int iSample = 0; iSample < columns; iSample++)
            {
                foreach (Observable o in trajectories.Keys)
                {
                    float[][] runs = trajectories[o];
                    for (int iRun = 0; iRun < rows; iRun++)
                    {
                        yield return (int)runs[iRun][iSample];
                    }
                }
            }
        }

        private static MatrixElement MatrixForObservable(float[][] observableData)
        {
            MatrixElement matrix = null;

            bool isIntegral;
            float minimum;
            float maximum;

            CharacterizeData(observableData, out isIntegral, out minimum, out maximum);

            int traceCount = observableData.Length;
            int sampleCount = observableData[0].Length;

            // floating point
            if (isIntegral)
            {
                // signed
                if (minimum < 0.0f)
                {
                    if ((minimum >= SByte.MinValue) && (maximum <= SByte.MaxValue))
                    {
                        matrix = new MatlabMatrix(ObservableAsInt8(observableData), new[] { traceCount, sampleCount });
                    }
                    else if ((minimum >= Int16.MinValue) && (maximum <= Int16.MaxValue))
                    {
                        matrix = new MatlabMatrix(ObservableAsShort(observableData), new[] { traceCount, sampleCount });
                    }
                    else if ((minimum >= Int32.MinValue) && (maximum <= Int32.MaxValue))
                    {
                        matrix = new MatlabMatrix(ObservableAsInt(observableData), new[] { traceCount, sampleCount });
                    }
                }
                else // unsigned
                {
                    if (maximum <= Byte.MaxValue)
                    {
                        matrix = new MatlabMatrix(ObservableAsByte(observableData), new[] { traceCount, sampleCount });
                    }
                    else if (maximum <= UInt16.MaxValue)
                    {
                        matrix = new MatlabMatrix(ObservableAsUshort(observableData), new[] { traceCount, sampleCount });
                    }
                    else if (maximum <= UInt32.MaxValue)
                    {
                        matrix = new MatlabMatrix(ObservableAsUint(observableData), new[] { traceCount, sampleCount });
                    }
                }
            }

            return matrix ?? new MatlabMatrix(ObservableAsFloat(observableData), new[] { traceCount, sampleCount });
        }

        private static void CharacterizeData(IEnumerable<float[]> data, out bool isIntegral, out float minimum, out float maximum)
        {
            isIntegral = true;
            minimum = Single.MaxValue;
            maximum = Single.MinValue;

            foreach (var vector in data)
            {
                foreach (var element in vector)
                {
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if ((int)element != element)
                    {
                        isIntegral = false;
                        return;
                    }

                    minimum = Math.Min(minimum, element);
                    maximum = Math.Max(maximum, element);
                }
            }
        }

        private static IEnumerable<float> ObservableAsFloat(float[][] runs)
        {
            for (int iSample = 0; iSample < runs[0].Length; iSample++)
            {
                foreach (float[] trace in runs)
                {
                    yield return trace[iSample];
                }
            }
        }

        private static IEnumerable<SByte> ObservableAsInt8(float[][] runs)
        {
            for (int iSample = 0; iSample < runs[0].Length; iSample++)
            {
                foreach (float[] trace in runs)
                {
                    yield return (SByte)trace[iSample];
                }
            }
        }

        private static IEnumerable<short> ObservableAsShort(float[][] runs)
        {
            for (int iSample = 0; iSample < runs[0].Length; iSample++)
            {
                foreach (float[] trace in runs)
                {
                    yield return (short)trace[iSample];
                }
            }
        }

        private static IEnumerable<int> ObservableAsInt(float[][] runs)
        {
            for (int iSample = 0; iSample < runs[0].Length; iSample++)
            {
                foreach (float[] trace in runs)
                {
                    yield return (int)trace[iSample];
                }
            }
        }

        private static IEnumerable<byte> ObservableAsByte(float[][] runs)
        {
            for (int iSample = 0; iSample < runs[0].Length; iSample++)
            {
                foreach (float[] trace in runs)
                {
                    yield return (byte)trace[iSample];
                }
            }
        }

        private static IEnumerable<ushort> ObservableAsUshort(float[][] runs)
        {
            for (int iSample = 0; iSample < runs[0].Length; iSample++)
            {
                foreach (float[] trace in runs)
                {
                    yield return (ushort)trace[iSample];
                }
            }
        }

        private static IEnumerable<uint> ObservableAsUint(float[][] runs)
        {
            for (int iSample = 0; iSample < runs[0].Length; iSample++)
            {
                foreach (float[] trace in runs)
                {
                    yield return (uint)trace[iSample];
                }
            }
        }
    }
}
