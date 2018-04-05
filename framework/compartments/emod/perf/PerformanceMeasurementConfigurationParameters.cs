/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using System.IO;
using System.Text;

namespace compartments.emod.perf
{
    public class PerformanceMeasurementConfigurationParameters
    {
        public enum LoggingFileFormat
        {
            JSON,
            CSV
        }

        public class PerformanceConfigurationException : ArgumentException
        {
            public PerformanceConfigurationException(String message, String parameter) : base(message, parameter) {}
        }

        public bool Enabled { get; protected set; }
        public double SimulationDuration { get; protected set; }
        public int LogCount { get; protected set; }
        public int HistogramBins { get; protected set; }
        public bool RecordRealizationCpuTime { get; protected set; }
        public bool RecordSolverStepTicks { get; protected set; }
        public bool RecordSolverSteps { get; protected set; }
        public bool RecordReactionFirings { get; protected set; }
        public LoggingFileFormat LogFileFormat { get; protected set; }
        public string LogFilenamePrefix { get; protected set; }
        public bool CompressOutput { get; protected set; }

        public String WorkingFilename
        {
            get
            {
                var sb = new StringBuilder(LogFilenamePrefix);
                sb.Append('.');
                sb.Append(LogFileFormat.ToString().ToLower());
                if (CompressOutput)
                {
                    sb.Append(".gz");
                }

                return sb.ToString();
            }
        }

        public PerformanceMeasurementConfigurationParameters(Configuration config, double duration)
        {
            if (duration <= 0.0)
            {
                throw new ArgumentException("Duration must be > 0", "duration");
            }

            Enabled                  = config.GetParameterWithDefault(@"perf\Enabled", false);
            SimulationDuration       = config.GetParameterWithDefault(@"duration", duration);
            LogCount                 = config.GetParameterWithDefault(@"perf\LogCount", 1);
            HistogramBins            = config.GetParameterWithDefault(@"perf\HistogramBins", 16);
            RecordRealizationCpuTime = config.GetParameterWithDefault(@"perf\RecordRealizationCpuTime", true);
            RecordSolverStepTicks    = config.GetParameterWithDefault(@"perf\RecordSolverStepTicks", true);
            RecordSolverSteps        = config.GetParameterWithDefault(@"perf\RecordSolverSteps", true);
            RecordReactionFirings    = config.GetParameterWithDefault(@"perf\RecordReactionFirings", true);
            var logFileFormatString  = (config.GetParameterWithDefault(@"perf\LogFileFormat", "JSON")).ToUpper();
            LogFileFormat            = (LoggingFileFormat)Enum.Parse(typeof(LoggingFileFormat), logFileFormatString);
            LogFilenamePrefix        = config.GetParameterWithDefault(@"perf\LogFilenamePrefix", "perflog");
            CompressOutput           = config.GetParameterWithDefault(@"perf\CompressOutput", false);

            ValidateConfiguration();
        }

        private void ValidateConfiguration()
        {
            if (Enabled)
            {
                if (SimulationDuration <= 0.0)
                {
                    throw new PerformanceConfigurationException(
                        String.Format("SimulationDuration should be > 0 (actual - {0}).", SimulationDuration),
                        "SimulationDuration");
                }

                if (LogCount < 1)
                {
                    throw new PerformanceConfigurationException(
                        String.Format("LogCount should be > 0 (actual - {0}).", LogCount), "LogCount");
                }

                if (HistogramBins < 2)
                {
                    throw new PerformanceConfigurationException(String.Format("HistogramBins should be > 1 (actual - {0}.", HistogramBins), "HistogramBins");
                }

                var fileIsWriteable = false;
                try
                {
                    #pragma warning disable 168 // unused local variable
                    using (var test = new StreamWriter(WorkingFilename))
                    {
                        fileIsWriteable = true;
                    }
                     #pragma warning restore 168
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                }

                if (!fileIsWriteable)
                {
                    throw new PerformanceConfigurationException(
                        String.Format("LogFileName ('{0}') should be valid and write-able.", WorkingFilename), "WorkingFilename");
                }
            }
        }
    }
}
