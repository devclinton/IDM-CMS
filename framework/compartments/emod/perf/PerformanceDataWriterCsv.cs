using System.Collections.Generic;
using System.IO;
using compartments.emod.utils;

namespace compartments.emod.perf
{
    public class PerformanceDataWriterCsv : PerformanceDataWriterBase
    {
        protected override void SerializeMeasurementsToStream(PerformanceMeasurements measurements, TextWriter textWriter)
        {
            SerializeHeaderToStream(measurements, textWriter);
            SerializeHistogramToStream(measurements.RealizationTimes, textWriter);
            SerializeHistogramsToStream(measurements.SolverSteps, textWriter);
            SerializeHistogramsToStream(measurements.StepTicks, textWriter);
            SerializeHistogramsToStream(measurements.ReactionFirings, textWriter);
        }

        private static void SerializeHeaderToStream(PerformanceMeasurements measurements, TextWriter textWriter)
        {
            textWriter.WriteLine("FormatVersion,\"{0}\"", measurements.FormatVersion);
            textWriter.WriteLine("FrameworkVersion,\"{0}\"", measurements.FrameworkVersion);
            textWriter.WriteLine("FrameworkDescription,\"{0}\"", measurements.FrameworkDescription);
            textWriter.WriteLine("SimulationDuration,{0}", measurements.SimulationDuration);
            textWriter.WriteLine("LogCount,{0}", measurements.FrameCount);
            textWriter.WriteLine("HistogramBins,{0}", measurements.HistogramBins);
            textWriter.WriteLine("TotalTimeTicks,{0}", measurements.TotalTimeTicks);
            textWriter.WriteLine("TickFrequency,{0}", measurements.TickFrequency);
            textWriter.WriteLine("TotalTimeMs,{0}", measurements.TotalTimeMs);
            textWriter.WriteLine("MeasurementTimeTicks,{0}", measurements.MeasurementTimeTicks);
        }

        private static void SerializeHistogramToStream(DynamicHistogram histogram, TextWriter textWriter)
        {
            textWriter.Write("{0},", histogram.LowerBound);
            textWriter.Write("{0},", histogram.Width);
            textWriter.Write("{0},", histogram.BinCount);

            textWriter.Write(histogram[0]);
            for (int i = 1; i < histogram.BinCount; i++)
            {
                textWriter.Write(",{0}", histogram[i]);
            }
            textWriter.WriteLine();
        }

        private static void SerializeHistogramsToStream(IEnumerable<DynamicHistogram> histograms, TextWriter textWriter)
        {
            foreach (var histogram in histograms)
            {
                SerializeHistogramToStream(histogram, textWriter);
            }
        }
    }
}
