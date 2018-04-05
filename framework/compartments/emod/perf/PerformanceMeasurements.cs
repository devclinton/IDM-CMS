/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using System.Diagnostics;
using compartments.emod.utils;

namespace compartments.emod.perf
{
    public class PerformanceMeasurements
    {
        protected const string Version = "1.0";

        public string FormatVersion { get { return Version; } }
        public string FrameworkVersion { get; protected set; }
        public string FrameworkDescription { get; protected set; }
        public double SimulationDuration { get; protected set; }
        public int FrameCount { get; protected set; }
        public int HistogramBins { get; protected set; }
        public long TotalTimeTicks { get; protected set; }
        public long TickFrequency { get { return Stopwatch.Frequency; } }
        public double TotalTimeMs { get { return 1000.0 * TotalTimeTicks / Stopwatch.Frequency; } }
        public long MeasurementTimeTicks { get; protected set; }
        public DynamicHistogram RealizationTimes { get; protected set; }
        public DynamicHistogram[] SolverSteps { get; protected set; }
        public DynamicHistogram[] StepTicks { get; protected set; }
        public DynamicHistogram[] ReactionFirings { get; protected set; }

        private int _currentSample;
        private readonly Stopwatch _stopwatch;
        private double _simTimeOfLastLog;
        private long _ticksAtLastLog;
        private long _ticksAtRealizationStart;
        private int _accumulatedSolverSteps;
        private long _accumulatedStepTicks;
        private int _accumulatedReactionFirings;

        public PerformanceMeasurements(string version, string description, PerformanceMeasurementConfigurationParameters parameters)
        {
            ValidateArguments(version, description);

            FrameworkVersion     = version;
            FrameworkDescription = description;

            SimulationDuration = parameters.SimulationDuration;
            FrameCount         = parameters.LogCount;
            HistogramBins      = parameters.HistogramBins;

            RealizationTimes = new DynamicHistogram(HistogramBins);
            SolverSteps      = new DynamicHistogram[FrameCount];
            StepTicks        = new DynamicHistogram[FrameCount];
            ReactionFirings  = new DynamicHistogram[FrameCount];

            for (int iFrame = 0; iFrame < FrameCount; iFrame++)
            {
                SolverSteps[iFrame]     = new DynamicHistogram(HistogramBins);
                StepTicks[iFrame]       = new DynamicHistogram(HistogramBins);
                ReactionFirings[iFrame] = new DynamicHistogram(HistogramBins);
            }

            MeasurementTimeTicks = 0;
            _stopwatch           = new Stopwatch();
        }

        private static void ValidateArguments(string version, string description)
        {
            if (version == null)
                throw new ArgumentNullException("version", "Version string should not be null.");

            if (version == String.Empty)
                throw new ArgumentException("Version string should not be empty.", "version");

            if (description == null)
                throw new ArgumentNullException("description", "Description string should not be null.");

            if (description == String.Empty)
                throw new ArgumentException("Description string should not be empty.", "description");
        }

        public void StartMeasurement()
        {
            _stopwatch.Reset();
        }

        public void StartRealization()
        {
            _stopwatch.Start();
            _currentSample           = 0;
            _simTimeOfLastLog        = 0.0;
            _ticksAtRealizationStart = _stopwatch.ElapsedTicks;
            _ticksAtLastLog          = _stopwatch.ElapsedTicks;

            _accumulatedSolverSteps     = 0;
            _accumulatedStepTicks       = 0;
            _accumulatedReactionFirings = 0;
        }

        public void LogStep(int reactionFirings, double currentSimulationTime)
        {
            ValidateStepArguments(reactionFirings, currentSimulationTime);

            long startTicks = _stopwatch.ElapsedTicks;

            _accumulatedSolverSteps++;

            int firingsRemainingToAllocate = reactionFirings;
            long ticksRemainingToAllocate  = _stopwatch.ElapsedTicks - _ticksAtLastLog;

            while (_simTimeOfLastLog < currentSimulationTime)
            {
                double remainingSimTime = currentSimulationTime - _simTimeOfLastLog;
                double loggingBinEndTime = (_currentSample + 1) * SimulationDuration / FrameCount;
                double simTimeAttributableToCurrentBin = Math.Min(currentSimulationTime, loggingBinEndTime) - _simTimeOfLastLog;
                var firingsToAttribute = (int)Math.Ceiling(firingsRemainingToAllocate * simTimeAttributableToCurrentBin / remainingSimTime);
                _accumulatedReactionFirings += firingsToAttribute;
                firingsRemainingToAllocate -= firingsToAttribute;
                var ticksToAttribute = (long)Math.Ceiling(ticksRemainingToAllocate*simTimeAttributableToCurrentBin/remainingSimTime);
                _accumulatedStepTicks += ticksToAttribute;
                ticksRemainingToAllocate -= ticksToAttribute;

                if (currentSimulationTime < loggingBinEndTime)
                {
                    _simTimeOfLastLog = currentSimulationTime;
                }
                else
                {
                    StepTicks[_currentSample].AddSample(_accumulatedStepTicks);
                    SolverSteps[_currentSample].AddSample(_accumulatedSolverSteps);
                    ReactionFirings[_currentSample].AddSample(_accumulatedReactionFirings);

                    _accumulatedStepTicks       = 0;
                    _accumulatedSolverSteps     = 0;
                    _accumulatedReactionFirings = 0;

                    _simTimeOfLastLog = loggingBinEndTime;
                    _currentSample++;
                }
            }

            _ticksAtLastLog = _stopwatch.ElapsedTicks;
            MeasurementTimeTicks += _stopwatch.ElapsedTicks - startTicks;
        }

        private void ValidateStepArguments(int reactionFirings, double currentSimulationTime)
        {
            if (reactionFirings < 0)
                throw new ArgumentException("Zero or more reactions should fire each step.", "reactionFirings");

            if (currentSimulationTime > SimulationDuration)
                throw new ArgumentException("Simulation time at logging should not be greater than simulation duration.",
                                            "currentSimulationTime");
        }

        public void EndRealization()
        {
            _stopwatch.Stop();
            RealizationTimes.AddSample(_stopwatch.ElapsedTicks - _ticksAtRealizationStart);
        }

        public void EndMeasurement()
        {
            _stopwatch.Stop();
            TotalTimeTicks = _stopwatch.ElapsedTicks;
        }
    }
}
