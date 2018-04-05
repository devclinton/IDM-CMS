/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using compartments.emod.utils;

namespace compartments.solvers.solverbase
{
    static class CsvSupport
    {
        internal static CsvOutputOptions GetCsvOutputOptions(string prefix)
        {
            bool writeCsvFile = Configuration.CurrentConfiguration.GetParameterWithDefault("output.writecsv", true);
            bool compressOutput = Configuration.CurrentConfiguration.GetParameterWithDefault("output.compress", false);
            bool writeRealizationIndex = Configuration.CurrentConfiguration.GetParameterWithDefault("output.writerealizationindex", true);

            var csvOptions = new CsvOutputOptions
                {
                    Filename = prefix + ".csv",
                    CompressOutput = compressOutput,
                    WriteCsvFile = writeCsvFile,
                    WriteRealizationIndex = writeRealizationIndex
                };

            return csvOptions;
        }

        internal static void WriteCsvFile(SolverBase.Trajectories trajectories, CsvOutputOptions outputOptions)
        {
            TextWriter output;
            if (outputOptions.CompressOutput)
            {
                var fileStream = File.Open(outputOptions.Filename + ".gz", FileMode.OpenOrCreate, FileAccess.Write);
                var gZipStream = new GZipStream(fileStream, CompressionMode.Compress);
                output = new StreamWriter(gZipStream);
            }
            else
            {
                output = new StreamWriter(outputOptions.Filename);
            }

            using (output)
            {
                WriteVersionInfo(output);
                WriteSampleTimes(trajectories, output);
                WriteTrajectoryData(trajectories, outputOptions, output);
            }
        }

        private static void WriteVersionInfo(TextWriter output)
        {
            if (GetPrivateStaticField("_writeVersionInfo", typeof(CsvOutputOptions)))
            {
                output.WriteLine("FrameworkVersion,\"{0}\",\"{1}\"", VersionInfo.Version, VersionInfo.Description);
            }
        }

        private static void WriteSampleTimes(SolverBase.Trajectories trajectories, TextWriter output)
        {
            if (GetPrivateStaticField("_writeSampleTimes", typeof(CsvOutputOptions)))
            {
                WriteRowHeader("sampletimes", output);
                WriteRowData(trajectories.SampleTimes, output);
            }
        }

        private static void WriteRowHeader(string header, TextWriter output)
        {
            output.Write("{0},", header);
        }

        private static void WriteRowData(IEnumerable<double> data, TextWriter output)
        {
            bool first = true;
            foreach (var value in data)
            {
                if (!first)
                {
                    output.Write(",");
                }
                output.Write(value);
                first = false;
            }

            output.WriteLine();
        }

        private static void WriteTrajectoryData(SolverBase.Trajectories trajectories, CsvOutputOptions outputOptions, TextWriter output)
        {
            bool writeObservableInfo = GetPrivateStaticField("_writeObservableInfo", typeof (CsvOutputOptions));

            foreach (Observable o in trajectories.Keys)
            {
                double[][] observableTrajectories = trajectories[o];
                var header = new StringBuilder();
                for (int runIndex = 0; runIndex < observableTrajectories.Length; runIndex++)
                {
                    if (writeObservableInfo)
                    {
                        BuildRowHeader(o, runIndex, outputOptions, header);
                        WriteRowHeader(header.ToString(), output);
                    }
                    WriteRowData(observableTrajectories[runIndex], output);
                }
            }
        }

        private static void BuildRowHeader(Observable o, int runIndex, CsvOutputOptions outputOptions, StringBuilder header)
        {
            header.Clear();
            header.Append(o.Name);
            if (outputOptions.WriteRealizationIndex)
            {
                header.AppendFormat("{{{0}}}", runIndex);
            }
        }

        private static bool GetPrivateStaticField(string fieldName, Type sourceType)
        {
            var fieldInfo = sourceType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            // ReSharper disable once PossibleNullReferenceException
            return (bool) fieldInfo.GetValue(null);
        }
    }
}
