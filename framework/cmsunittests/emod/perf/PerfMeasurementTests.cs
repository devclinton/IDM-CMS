/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using NUnit.Framework;
using compartments;
using compartments.emod.perf;
using compartments.emod.utils;

namespace cmsunittests.emod.perf
{
    [TestFixture]
    class PerfMeasurementTests : AssertionHelper
    {
        private const int TestRealizations = 16;
        private const double TestDuration  = 42.0;

        [Test]
        public void PerfMeasurementConfigParamsCtorDefaults()
        {
            var config = Configuration.ConfigurationFromString("{}");
            var configParams = new PerformanceMeasurementConfigurationParameters(config, TestDuration);

            Assert.IsFalse(configParams.Enabled);
            Assert.AreEqual(TestDuration, configParams.SimulationDuration);
            Assert.AreEqual(1, configParams.LogCount);
            Assert.AreEqual(16, configParams.HistogramBins);
            Assert.IsTrue(configParams.RecordRealizationCpuTime);
            Assert.IsTrue(configParams.RecordSolverStepTicks);
            Assert.IsTrue(configParams.RecordSolverSteps);
            Assert.IsTrue(configParams.RecordReactionFirings);
            Assert.AreEqual(PerformanceMeasurementConfigurationParameters.LoggingFileFormat.JSON, configParams.LogFileFormat);
            Assert.AreEqual("perflog", configParams.LogFilenamePrefix);
            Assert.IsFalse(configParams.CompressOutput);
            Assert.AreEqual("perflog.json", configParams.WorkingFilename);
        }

        [Test]
        public void PerfMeasurementConfigParamsCtorFromConfig()
        {
            const int logCount = 42;
            const int histogramBins = 20;
            const PerformanceMeasurementConfigurationParameters.LoggingFileFormat logFileFormat = PerformanceMeasurementConfigurationParameters.LoggingFileFormat.CSV;
            const string logFilenamePrefix = "logfile";
            var configString = BuildPerformanceMeasurementConfigurationString(enabled: true,
                                                                              simulationDuration: TestDuration,
                                                                              logCount: logCount,
                                                                              histogramBins: histogramBins,
                                                                              recordRealizationCpuTime: false,
                                                                              recordSolverStepTicks: false,
                                                                              recordSolverSteps: false,
                                                                              recordReactionFirings: false,
                                                                              logFileFormat: logFileFormat,
                                                                              logFilenamePrefix: logFilenamePrefix,
                                                                              compressOutput: true);
            var config = Configuration.ConfigurationFromString(configString);
            var configParams = new PerformanceMeasurementConfigurationParameters(config, TestDuration);

            Assert.IsTrue(configParams.Enabled);
            Assert.AreEqual(TestDuration, configParams.SimulationDuration);
            Assert.AreEqual(logCount, configParams.LogCount);
            Assert.AreEqual(histogramBins, configParams.HistogramBins);
            Assert.IsFalse(configParams.RecordRealizationCpuTime);
            Assert.IsFalse(configParams.RecordSolverStepTicks);
            Assert.IsFalse(configParams.RecordSolverSteps);
            Assert.IsFalse(configParams.RecordReactionFirings);
            Assert.AreEqual(logFileFormat, configParams.LogFileFormat);
            Assert.AreEqual(logFilenamePrefix, configParams.LogFilenamePrefix);
            Assert.IsTrue(configParams.CompressOutput);
            Assert.AreEqual(logFilenamePrefix + ".csv.gz", configParams.WorkingFilename);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "must be > 0", MatchType = MessageMatch.Contains)]
        public void PerfMeasurementConfigParamsBadDurationArgument()
        {
            var config = Configuration.ConfigurationFromString("{}");
            #pragma warning disable 168
            var unused = new PerformanceMeasurementConfigurationParameters(config, -2.0);
            #pragma warning restore 168
        }

        [Test]
        [ExpectedException(typeof(PerformanceMeasurementConfigurationParameters.PerformanceConfigurationException), ExpectedMessage = "should be > 0", MatchType = MessageMatch.Contains)]
        public void PerfMeasurementConfigParamsBadDurationInConfig()
        {
            string configString = BuildPerformanceMeasurementConfigurationString(simulationDuration: -2.0);
            var config = Configuration.ConfigurationFromString(configString);
            #pragma warning disable 168
            var unused= new PerformanceMeasurementConfigurationParameters(config, TestDuration);
            #pragma warning restore 168
        }

        [Test]
        [ExpectedException(typeof(PerformanceMeasurementConfigurationParameters.PerformanceConfigurationException), ExpectedMessage = "should be > 0", MatchType = MessageMatch.Contains)]
        public void PerfMeasurementConfigParamsBadLogCountInConfig()
        {
            string configString = BuildPerformanceMeasurementConfigurationString(logCount: -10);
            var config = Configuration.ConfigurationFromString(configString);
            #pragma warning disable 168
            var unused = new PerformanceMeasurementConfigurationParameters(config, TestDuration);
            #pragma warning restore 168
        }

        [Test]
        [ExpectedException(typeof(PerformanceMeasurementConfigurationParameters.PerformanceConfigurationException), ExpectedMessage = "should be > 1", MatchType = MessageMatch.Contains)]
        public void PerfMeasurementConfigParamsBadHistogramBinsInConfig()
        {
            string configString = BuildPerformanceMeasurementConfigurationString(histogramBins: -1);
            var config = Configuration.ConfigurationFromString(configString);
            #pragma warning disable 168
            var unused = new PerformanceMeasurementConfigurationParameters(config, TestDuration);
            #pragma warning restore 168
        }

        [Test]
        [ExpectedException(typeof(PerformanceMeasurementConfigurationParameters.PerformanceConfigurationException), ExpectedMessage = "should be valid and write-able", MatchType = MessageMatch.Contains)]
        public void PerfMeasurementConfigParamsBadLogFileNameInConfig()
        {
            string tempFileNameAndPath = Path.GetTempFileName();

            try
            {
                // ReSharper disable AssignNullToNotNullAttribute
                string badPath = Path.Combine(tempFileNameAndPath, Path.GetFileName(tempFileNameAndPath)).Replace(@"\", @"\\");
                // ReSharper restore AssignNullToNotNullAttribute
                string configString = BuildPerformanceMeasurementConfigurationString(logFilenamePrefix: badPath);
                var config = Configuration.ConfigurationFromString(configString);
                #pragma warning disable 168
                var unused = new PerformanceMeasurementConfigurationParameters(config, TestDuration);
                #pragma warning restore 168
            }
            finally
            {
                File.Delete(tempFileNameAndPath);
            }
        }

        [Test]
        public void PerformanceMeasurementsCtor()
        {
            const int logCount = 16;
            string configString = $"{{\"perf\":{{\"LogCount\":{logCount}}}}}";
            var config = Configuration.ConfigurationFromString(configString);
            const double duration = 48.0;
            var perfConfig = new PerformanceMeasurementConfigurationParameters(config, duration);
            var perf = new PerformanceMeasurements(VersionInfo.Version, VersionInfo.Description, perfConfig);

            const int defaultBinCount = 16;

            Assert.AreEqual("1.0", perf.FormatVersion);
            Assert.AreEqual(VersionInfo.Version, perf.FrameworkVersion);
            Assert.AreEqual(VersionInfo.Description, perf.FrameworkDescription);
            Assert.AreEqual(duration, perf.SimulationDuration);
            Assert.AreEqual(logCount, perf.FrameCount);
            Assert.AreEqual(defaultBinCount, perf.HistogramBins);
            Assert.AreEqual(defaultBinCount, perf.RealizationTimes.BinCount);
            Assert.AreEqual(logCount, perf.SolverSteps.Length);
            foreach (var histogram in perf.SolverSteps)
            {
                Assert.AreEqual(defaultBinCount, histogram.BinCount);
            }
            Assert.AreEqual(logCount, perf.StepTicks.Length);
            foreach (var histogram in perf.StepTicks)
            {
                Assert.AreEqual(defaultBinCount, histogram.BinCount);
            }
            Assert.AreEqual(logCount, perf.ReactionFirings.Length);
            foreach (var histogram in perf.ReactionFirings)
            {
                Assert.AreEqual(defaultBinCount, histogram.BinCount);
            }
        }

/*
        [Test]
        public void PerfLogging()
        {
            var configuration = BuildPerformanceMeasurementConfiguration();
            var perf = new PerformanceMeasurements(VersionInfo.Version, VersionInfo.Description, configuration);
            const int realizationCount = 16;

            perf.StartMeasurement();
            for (int realization = 0; realization < realizationCount; realization++)
            {
                perf.StartRealization();
                for (int step = 0; step < perf.FrameCount; step++)
                {
                    // Log 3/4 of the way through each interval so there is one step and one reaction
                    // per logging bin.
                    Thread.Sleep(10);
                    double tau = Math.Min((step + 0.75) * perf.SimulationDuration / perf.FrameCount, perf.SimulationDuration);
                    // Console.WriteLine("LogStep(1, {0})", tau);
                    perf.LogStep(1, tau);
//                     Assert.GreaterOrEqual(perf.SolverSteps[step].Sum(), 0, "SolverSteps should be >= 0.");
//                     Assert.GreaterOrEqual(perf.ReactionFirings[step].Sum(), 0, "ReactionFirings should be > 0.");
//                     Assert.Greater(perf.StepTicks[step].Sum(), 0, "StepTimes should be > 0.");
                }
                perf.EndRealization();
//                 Assert.Greater(perf.RealizationTimes.SampleCount, 0, "RealizationTimes should be > 0.");
            }
            perf.EndMeasurement();

//             Assert.Greater(perf.TotalTimeTicks, 0.0, "TotalTime should be > 0.");
//             Assert.GreaterOrEqual(perf.TotalTimeTicks, perf.RealizationTimes.Sum());

            WriteMeasurementsToConsole(perf);
        }
*/

/*
        private static void WriteMeasurementsToConsole(PerformanceMeasurements perf)
        {
            Console.WriteLine();
            Console.Write("SolverSteps: ");
            foreach (var vector in perf.SolverSteps)
                foreach (var count in vector)
                {
                    Console.Write("{0} ", count);
                }

            Console.WriteLine();
            Console.Write("ReactionFirings: ");
            foreach (var vector in perf.ReactionFirings)
                foreach (var count in vector)
                {
                    Console.Write("{0} ", count);
                }

            Console.WriteLine();
            Console.Write("StepTimes: ");
            foreach (var vector in perf.StepTicks)
                foreach (var count in vector)
                {
                    Console.Write("{0} ", count);
                }

            Console.WriteLine();
            Console.Write("RealizationTimes: ");
            foreach (var count in perf.RealizationTimes)
            {
                Console.Write("{0} ", count);
            }

            Console.WriteLine();
            Console.WriteLine("Tick frequency: {0}", perf.TickFrequency);
        }
*/

/*
        [Test]
        public void PerfLoggingToo()
        {
            var configuration = BuildPerformanceMeasurementConfiguration();
            var perf = GeneratePerformanceMeasurements(configuration);

            Assert.Greater(perf.TotalTimeTicks, 0.0, "TotalTime should be > 0.");

            Assert.GreaterOrEqual(perf.TotalTimeTicks, perf.RealizationTimes[0]);
            Assert.GreaterOrEqual(perf.RealizationTimes[0], perf.StepTicks[0].Sum());
            Assert.AreEqual(16, perf.SolverSteps[0].Sum());
            Assert.AreEqual(816, perf.ReactionFirings[0].Sum());

            WriteMeasurementsToConsole(perf);
        }
*/

        private static PerformanceMeasurementConfigurationParameters BuildPerformanceMeasurementConfiguration(bool enabled = true, int logCount = 16, int histogramBins = 16, PerformanceMeasurementConfigurationParameters.LoggingFileFormat logFileFormat = PerformanceMeasurementConfigurationParameters.LoggingFileFormat.JSON, String logFilenamePrefix = "perflog", bool compressOutput = false, double duration = 48.0)
        {
            var configString = BuildPerformanceMeasurementConfigurationString(enabled: enabled, logCount: logCount, histogramBins: histogramBins, logFileFormat: logFileFormat, logFilenamePrefix: logFilenamePrefix, compressOutput: compressOutput);
            var config       = Configuration.ConfigurationFromString(configString);
            var perfConfig   = new PerformanceMeasurementConfigurationParameters(config, duration);

            return perfConfig;
        }

        private static String BuildPerformanceMeasurementConfigurationString(bool enabled = true,
                                                                             double simulationDuration = 100.0,
                                                                             int logCount = 1,
                                                                             int histogramBins = 16,
                                                                             bool recordRealizationCpuTime = true,
                                                                             bool recordSolverStepTicks = true,
                                                                             bool recordSolverSteps = true,
                                                                             bool recordReactionFirings = true,
                                                                             PerformanceMeasurementConfigurationParameters.LoggingFileFormat logFileFormat = PerformanceMeasurementConfigurationParameters.LoggingFileFormat.JSON,
                                                                             string logFilenamePrefix = "perflog",
                                                                             bool compressOutput = false)
        {
            var stringBuilder = new StringBuilder();
            // Double literal '{' in format string since single '{' delimits an argument.
            stringBuilder.AppendFormat("{{\"duration\":{0},\"perf\":{{", simulationDuration);
            stringBuilder.AppendFormat("\"Enabled\":{0},", enabled.ToString(CultureInfo.InvariantCulture).ToLower());
            stringBuilder.AppendFormat("\"LogCount\":{0},", logCount);
            stringBuilder.AppendFormat("\"HistogramBins\":{0},", histogramBins);
            stringBuilder.AppendFormat("\"RecordRealizationCpuTime\":{0},", recordRealizationCpuTime.ToString(CultureInfo.InvariantCulture).ToLower());
            stringBuilder.AppendFormat("\"RecordSolverStepTicks\":{0},", recordSolverStepTicks.ToString(CultureInfo.InvariantCulture).ToLower());
            stringBuilder.AppendFormat("\"RecordSolverSteps\":{0},", recordSolverSteps.ToString(CultureInfo.InvariantCulture).ToLower());
            stringBuilder.AppendFormat("\"RecordReactionFirings\":{0},", recordReactionFirings.ToString(CultureInfo.InvariantCulture).ToLower());
            stringBuilder.AppendFormat("\"LogFileFormat\":\"{0}\",", logFileFormat);
            stringBuilder.AppendFormat("\"LogFilenamePrefix\":\"{0}\",", logFilenamePrefix.Replace(@"\", @"\\"));
            stringBuilder.AppendFormat("\"CompressOutput\":{0}", compressOutput.ToString(CultureInfo.InvariantCulture).ToLower());
            stringBuilder.Append("}}");
            return stringBuilder.ToString();
        }

        private static PerformanceMeasurements GeneratePerformanceMeasurements(PerformanceMeasurementConfigurationParameters configuration, int realizationCount)
        {
            var perf = new PerformanceMeasurements(VersionInfo.Version, VersionInfo.Description, configuration);
            const int stepCount = 16;

            perf.StartMeasurement();
            for (int realization = 0; realization < realizationCount; realization++)
            {
                double simulationTime = 0.0;
                perf.StartRealization();
                for (int step = 0; step < stepCount; step++)
                {
                    Thread.Sleep(10);
                    int reactionFiringsThisStep = (step + 1)*(stepCount - step);
                    simulationTime += perf.SimulationDuration*reactionFiringsThisStep/816;
                    simulationTime = Math.Min(simulationTime, perf.SimulationDuration);
                    Console.WriteLine("LogStep({0}, {1})", reactionFiringsThisStep, simulationTime);
                    perf.LogStep(reactionFiringsThisStep, simulationTime);
                }
                perf.EndRealization();
            }
            perf.EndMeasurement();
            return perf;
        }

/*
        [Test]
        public void PerfLoggingThree()
        {
            var configuration = BuildPerformanceMeasurementConfiguration(realizationCount: 1);
            var perf = new PerformanceMeasurements(VersionInfo.Version, VersionInfo.Description, configuration);

            int accumulatedStepCount = 0;
            int accumulatedReactionFirings = 0;

            perf.StartMeasurement();
            {
                double simulationTime = 0.0;
                perf.StartRealization();
                while (simulationTime < perf.SimulationDuration)
                {
                    Thread.Sleep(10);

                    double deltaTau = RngLib.RNG.GenerateExponential(3.0);
                    simulationTime += deltaTau;
                    simulationTime = Math.Min(simulationTime, perf.SimulationDuration);
                    int reactionFiringsThisStep = RngLib.RNG.GeneratePoisson(100 * deltaTau);
                    Console.WriteLine("LogStep({0}, {1})", reactionFiringsThisStep, simulationTime);
                    perf.LogStep(reactionFiringsThisStep, simulationTime);
                    accumulatedStepCount++;
                    if (simulationTime <= perf.SimulationDuration)
                    {
                        accumulatedReactionFirings += reactionFiringsThisStep;
                    }
                }
                perf.EndRealization();
            }
            perf.EndMeasurement();

            Assert.Greater(perf.RealizationTimes[0], 0.0, "Realization time should be > 0.");
            Assert.Greater(perf.TotalTimeTicks, 0.0, "TotalTime should be > 0.");

            Assert.GreaterOrEqual(perf.TotalTimeTicks, perf.RealizationTimes[0]);
            Assert.GreaterOrEqual(perf.RealizationTimes[0], perf.StepTicks[0].Sum());
            Assert.AreEqual(accumulatedStepCount, perf.SolverSteps[0].Sum());
            Assert.GreaterOrEqual(accumulatedReactionFirings, perf.ReactionFirings[0].Sum());

            WriteMeasurementsToConsole(perf);
        }
*/

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Zero or more reactions should fire", MatchType = MessageMatch.Contains)]
        public void PerfLoggingLogBadReactionFiringCount()
        {
            var configuration = BuildPerformanceMeasurementConfiguration();
            var perf = new PerformanceMeasurements(VersionInfo.Version, VersionInfo.Description, configuration);

            perf.StartMeasurement();
            {
                perf.StartRealization();
                {
                    perf.LogStep(-10, perf.SimulationDuration/2.0);
                }
                perf.EndRealization();
            }
            perf.EndMeasurement();
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "time at logging should not be greater", MatchType = MessageMatch.Contains)]
        public void PerfLoggingLogBadSimulationTime()
        {
            var configuration = BuildPerformanceMeasurementConfiguration();
            var perf = new PerformanceMeasurements(VersionInfo.Version, VersionInfo.Description, configuration);

            perf.StartMeasurement();
            {
                perf.StartRealization();
                {
                    perf.LogStep(1, perf.SimulationDuration + 1.0);
                }
                perf.EndRealization();
            }
            perf.EndMeasurement();
        }

        [Test]
        public void PerfLoggingWriteJsonFile()
        {
            var tempFilename = Path.GetTempFileName();
            try
            {
                var configuration = BuildPerformanceMeasurementConfiguration(logFilenamePrefix: tempFilename);
                var perf = GeneratePerformanceMeasurements(configuration, TestRealizations);
                var jsonWriter = new PerformanceDataWriterJson();
                jsonWriter.WritePerformanceMeasurements(perf, configuration);
                Console.WriteLine("Wrote performance measurements to JSON file '{0}'.", configuration.WorkingFilename);
            }
            finally
            {
                File.Delete(tempFilename);
            }
        }

        [Test]
        public void PerfLoggingWriteCompressedJsonFile()
        {
            var tempFilename = Path.GetTempFileName();
            try
            {
                var configuration = BuildPerformanceMeasurementConfiguration(logFilenamePrefix: tempFilename, compressOutput: true);
                var perf = GeneratePerformanceMeasurements(configuration, TestRealizations);
                var jsonWriter = new PerformanceDataWriterJson();
                jsonWriter.WritePerformanceMeasurements(perf, configuration);
                Console.WriteLine("Wrote performance measurements to compressed JSON file '{0}'.", configuration.WorkingFilename);
            }
            finally
            {
                File.Delete(tempFilename);
            }
        }

        [Test]
        public void PerfLoggingWriteCsvFile()
        {
            var tempFilename = Path.GetTempFileName();
            try
            {
                var configuration = BuildPerformanceMeasurementConfiguration(logFilenamePrefix: tempFilename, logFileFormat:PerformanceMeasurementConfigurationParameters.LoggingFileFormat.CSV);
                var perf = GeneratePerformanceMeasurements(configuration, TestRealizations);
                var csvWriter = new PerformanceDataWriterCsv();
                csvWriter.WritePerformanceMeasurements(perf, configuration);
                Console.WriteLine("Wrote performance measurements to CSV file '{0}'.", configuration.WorkingFilename);
            }
            finally
            {
                File.Delete(tempFilename);
            }
        }

        [Test]
        public void PerfLoggingWriteCompressedCsvFile()
        {
            var tempFilename = Path.GetTempFileName();
            try
            {
                var configuration = BuildPerformanceMeasurementConfiguration(logFilenamePrefix: tempFilename, logFileFormat:PerformanceMeasurementConfigurationParameters.LoggingFileFormat.CSV, compressOutput: true);
                var perf = GeneratePerformanceMeasurements(configuration, TestRealizations);
                var csvWriter = new PerformanceDataWriterCsv();
                csvWriter.WritePerformanceMeasurements(perf, configuration);
                Console.WriteLine("Wrote performance measurements to compressed CSV file '{0}'.", configuration.WorkingFilename);
            }
            finally
            {
                File.Delete(tempFilename);
            }
        }
    }
}
