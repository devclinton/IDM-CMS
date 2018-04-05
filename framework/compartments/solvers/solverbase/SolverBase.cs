/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using compartments.emod;
using compartments.emod.interfaces;
using compartments.emod.perf;
using compartments.emod.utils;
using distlib.randomvariates;

namespace compartments.solvers.solverbase
{
    public abstract class SolverBase : ISolver
    {
        protected internal class Trajectories : Dictionary<Observable, double[][]>
        {
            public double[] SampleTimes { get; private set; }

            public Trajectories(int sampleCount)
            {
                SampleTimes = new double[sampleCount];
            }

            internal SamplingParameters RecordObservables(IEnumerable<Observable> observables, SamplingParameters samplingParams, double nextReactionTime, double duration)
            {
                if (samplingParams.SampleCount > 1)
                {
                    if (nextReactionTime > duration)
                        throw new ArgumentException("nextReactionTime must be <= realization duration.");

                    double reportTime = duration * samplingParams.CurrentSample / (samplingParams.SampleCount - 1);
                    while (reportTime < nextReactionTime)
                    {
                        // ReSharper disable PossibleMultipleEnumeration
                        RecordObservables(observables, samplingParams.CurrentRealization, samplingParams.CurrentSample, reportTime);
                        // ReSharper restore PossibleMultipleEnumeration

                        samplingParams.CurrentSample++;
                        reportTime = duration * samplingParams.CurrentSample / (samplingParams.SampleCount - 1);
                    }
                }

                return samplingParams;
            }

            internal void RecordObservables(IEnumerable<Observable> observables, int realizationIndex, int sampleIndex, double sampleTime)
            {
                SampleTimes[sampleIndex] = sampleTime;
                foreach (var observable in observables)
                {
                    this[observable][realizationIndex][sampleIndex] = observable.Value;
                }
            }

            internal string[] GetTrajectoryLabels()
            {
                var labels = new string[Keys.Count * Values.First().Length];
                int iLabel = 0;
                var name = new StringBuilder();

                foreach (Observable o in Keys)
                {
                    for (int run = 0; run < this[o].Length; run++)
                    {
                        name.Append(o.Name);
                        name.Append('{'); name.Append(run); name.Append('}');
                        labels[iLabel++] = name.ToString();
                        name.Clear();
                    }
                }

                return labels;
            }
        }

        protected internal class SamplingParameters
        {
            public int RealizationCount { set; get; }
            public int CurrentRealization { set; get; }
            public int SampleCount { set; get; }
            public int CurrentSample { set; get; }
        }

        public class InvalidTimeStepException : ApplicationException {}

        // ReSharper disable InconsistentNaming
        protected ModelInfo modelInfo;
        protected IModel model;
        protected RandomVariateGenerator rng;

        protected SamplingParameters SamplingParams;

        protected double duration;

        private double _currentTime;
        protected Trajectories trajectories;

        protected Stopwatch stopWatch;

        private Parameter _time;
        protected Queue<TriggeredEvent> untriggeredEvents;
        protected Queue<TriggeredEvent> triggeredEvents;
        protected PriorityQueue<ScheduledEvent> scheduledEvents;

        protected long reactionsFiredInCurrentRealization;
        protected PerformanceMeasurementConfigurationParameters perfConfig;
        protected PerformanceMeasurements perfMeasurements;
        // ReSharper restore InconsistentNaming

        protected SolverBase(ModelInfo modelInfo, double duration, int repeats, int samples, IModelBuilder modelBuilder = null)
        {
            if (modelBuilder == null)
            {
                modelBuilder = new ModelBuilder();
            }

            this.modelInfo = modelInfo;
            this.duration = duration;                     
            SamplingParams = new SamplingParameters { RealizationCount = repeats, SampleCount = samples };

            rng = RNGFactory.GetRNG();

            _currentTime = 0.0;

            model = modelBuilder.BuildModel(modelInfo);
            _time = model.Parameters.First(p => p.Name == "time");
            untriggeredEvents = null;
            triggeredEvents = new Queue<TriggeredEvent>();
            scheduledEvents = model.ScheduledEvents.Count > 0 ? new PriorityQueue<ScheduledEvent>(model.ScheduledEvents.Count) : null;
            trajectories = AllocateRecordingArrays(model.Observables, SamplingParams);

            InitializePerformanceMeasurements();

            stopWatch = new Stopwatch();
        }

        protected static Trajectories AllocateRecordingArrays(IEnumerable<Observable> observables, SamplingParameters samplingParams)
        {
            int numRealizations = samplingParams.RealizationCount;
            int numSamples      = samplingParams.SampleCount;

            var trajectories = new Trajectories(numSamples);

            foreach (Observable o in observables)
            {
                var runs = new double[numRealizations][];
                for (int i = 0; i < numRealizations; i++)
                    runs[i] = new double[numSamples];
                trajectories.Add(o, runs);
            }

            return trajectories;
        }

        private void InitializePerformanceMeasurements()
        {
            perfConfig = new PerformanceMeasurementConfigurationParameters(Configuration.CurrentConfiguration, duration);
            if (perfConfig.Enabled)
            {
                perfMeasurements = new PerformanceMeasurements(VersionInfo.Version, VersionInfo.Description, perfConfig);
            }
        }

        public double CurrentTime
        {
            get { return _currentTime; }
            set
            {
                _currentTime = value;
                _time.Value = _currentTime;
            }
        }

        public virtual void Solve()
        {
            int progressReportThreshold = 0;

            StartPerformanceMeasurementForEnsemble();
            stopWatch.Start();

            int numRealizations = SamplingParams.RealizationCount;
            for (int curRealization = 0; curRealization < numRealizations; curRealization++)
            {
                progressReportThreshold = ReportProgressToConsole(numRealizations, progressReportThreshold, curRealization);

                StartRealization();
                SolveOnce();
            }

            stopWatch.Stop();
            EndPerformanceMeasurementForEnsemble();

            Console.WriteLine("Running time for ensemble of {0} realizations: {1} ", SamplingParams.RealizationCount, stopWatch.Elapsed);
        }

        protected void StartPerformanceMeasurementForEnsemble()
        {
            if (perfConfig.Enabled)
            {
                perfMeasurements.StartMeasurement();
            }
        }

        protected void EndPerformanceMeasurementForEnsemble()
        {
            if (perfConfig.Enabled)
            {
                perfMeasurements.EndMeasurement();
                IPerformanceDataWriter dataWriter = PerformanceDataWriterFactory.GetDataWriter(perfConfig);
                dataWriter.WritePerformanceMeasurements(perfMeasurements, perfConfig);
            }
        }

        protected int ReportProgressToConsole(int numRealizations, int progressReportThreshold, int curRealization)
        {
            SamplingParams.CurrentRealization = curRealization;

            int progress = 100 * curRealization / numRealizations;
            if (progress > progressReportThreshold)
            {
                var format = string.Format("{{0,3}}% ({{1,{0}}} trajectories calculated)", Math.Ceiling(Math.Log10(numRealizations)));
                Console.WriteLine(format, progress, curRealization);
                progressReportThreshold = progress;
            }

            return progressReportThreshold;
        }

        protected virtual void StartRealization()
        {
            CurrentTime = 0.0; // use property so the time symbol is also updated
            SamplingParams.CurrentSample = 0;
            reactionsFiredInCurrentRealization = 0;

            ResetModelState();
        }

        protected virtual void ResetModelState()
        {
            foreach (Parameter p in model.Parameters)
                p.Value = p.Info.Value;

            foreach (Species s in model.Species)
                s.Reset();

            untriggeredEvents = new Queue<TriggeredEvent>(model.TriggeredEvents);
            triggeredEvents.Clear();

            if (scheduledEvents != null)
            {
                scheduledEvents.Clear();
                foreach (ScheduledEvent evt in model.ScheduledEvents)
                {
                    evt.Time = evt.Info.Time;
                    scheduledEvents.Add(evt.Time, evt);
                }
            }
        }

        protected virtual void SolveOnce()
        {
            StartPerformanceMeasurementForRealization();

            while (CurrentTime < duration)
            {
                long reactionFiringCountBeforeStep = reactionsFiredInCurrentRealization;

                StepOnce();

                LogRealizationStepForPerformance(reactionFiringCountBeforeStep);
            }

            EndPerformanceMeasurementForRealization();

            trajectories.RecordObservables(model.Observables, SamplingParams.CurrentRealization, SamplingParams.CurrentSample++, duration);

            if (SamplingParams.CurrentSample < SamplingParams.SampleCount)
                Console.Error.WriteLine("Finished realization without final sampling.");
        }

        private void StartPerformanceMeasurementForRealization()
        {
            if (perfConfig.Enabled)
            {
                perfMeasurements.StartRealization();
            }
        }

        private void LogRealizationStepForPerformance(long currentReactionFiringCount)
        {
            if ((CurrentTime < duration) && perfConfig.Enabled)
            {
                var reactionsFired = (int)(reactionsFiredInCurrentRealization - currentReactionFiringCount);
                perfMeasurements.LogStep(reactionsFired, CurrentTime);
            }
        }

        private void EndPerformanceMeasurementForRealization()
        {
            if (perfConfig.Enabled)
            {
                perfMeasurements.EndRealization();
            }
        }

        protected virtual void StepOnce()
        {
            double timeOfNextEvent = ExecuteScheduledEvents(duration);

            double newTau = CalculateProposedTau(timeOfNextEvent);

            if (newTau > duration)
                throw new InvalidTimeStepException();

            CurrentTime = newTau;
            SamplingParams = trajectories.RecordObservables(model.Observables, SamplingParams, CurrentTime, duration);

            if (CurrentTime < duration) 
            {
                ExecuteReactions();
                UpdateTriggeredEvents();
            }
        }

        private double ExecuteScheduledEvents(double tauLimit)
        {
            if (scheduledEvents != null)
            {
                while (scheduledEvents.First.Priority <= CurrentTime)
                {
                    double eventTime;
                    ScheduledEvent nextEvent;
                    scheduledEvents.Top(out eventTime, out nextEvent);
                    nextEvent.Fire();
                    eventTime = nextEvent.Interval > 0 ? eventTime + nextEvent.Interval : double.PositiveInfinity;
                    scheduledEvents.UpdateIndex(eventTime, nextEvent);
                }

                tauLimit = Math.Min(tauLimit, scheduledEvents.First.Priority);
            }

            return tauLimit;
        }

        protected abstract double CalculateProposedTau(double tauLimit);
        protected abstract void ExecuteReactions();

        protected virtual void UpdateTriggeredEvents()
        {
            /* Cache the _current_ count of triggered events because we may add
             * to this queue but don't want to consider the new entries this time around.
             *
             * Consider each untriggered event to see if its condition is now satisfied.
             * If the event condition _is_ now satisfied,
             *     execute the statements of the event
             *     If the event is repeating,
             *         put it on the triggered queue until it's re-latched
             *     else,
             *         do nothing, we're done with this event for this run
             * else,
             *     put it back on the untriggered queue
             * 
             * Consider each previously triggered event (but not any newly triggered
             * events) to see if its condition is no longer true and it is re-latched.
             * If the event condition _is not_ now satisfied,
             *     put it back on the untriggered queue
             * else,
             *     put it back on the triggered queue
             */

            int cTriggered = triggeredEvents.Count;

            int cUntriggered = untriggeredEvents.Count;
            for (int iEvent = 0; iEvent < cUntriggered; iEvent++)
            {
                TriggeredEvent evt = untriggeredEvents.Dequeue();
                if (evt.Value)
                {
                    evt.Fire();
                    if (evt.Repeats)
                    {
                        triggeredEvents.Enqueue(evt);
                    }
                }
                else
                {
                    untriggeredEvents.Enqueue(evt);
                }
            }

            for (int iEvent = 0; iEvent < cTriggered; iEvent++)
            {
                TriggeredEvent evt = triggeredEvents.Dequeue();
                if (!evt.Value)
                {
                    untriggeredEvents.Enqueue(evt);
                }
                else
                {
                    triggeredEvents.Enqueue(evt);
                }
            }
        }

        protected double UpdateAndSumRates(IEnumerable<Reaction> reactions, IList<double> rates)
        {
            double a0  = 0.0;
            int index = 0;

            foreach (Reaction r in reactions)
            {
                double aj = r.Rate;

                if (double.IsNaN(aj))
                {
                    var message = $"Reaction propensity evaluated to NaN ('{r.Name}')";
                    Console.Error.WriteLine(message);
                    throw new ApplicationException(message);
                }

                if (aj < 0)
                {
                    var message = $"Reaction propensity evaluated to negative ('{r.Name}')";
                    Console.Error.WriteLine(message);
                    throw new ApplicationException(message);
                }

                if (double.IsInfinity(aj))
                {
                    var message = $"Reaction propensity evaluated to infinity ('{r.Name}')";
                    Console.Error.WriteLine(message);
                    throw new ApplicationException(message);
                }

                a0 += aj;
                rates[index++] = aj;
            }

            return a0;
        }

        protected int GetReactionIndex(IList<double> rates, double threshold)
        {
            int mu = 0;
            double cummulativeSum = 0.0;

            for (int i = 0; i < rates.Count; i++)
            {
                cummulativeSum += rates[i];
                if ((threshold <= cummulativeSum) && (rates[i] > 0.0))
                {
                    mu = i;
                    break;
                }
            }

            return mu;
        }

        protected virtual void FireReaction(Reaction reaction)
        {
            FireReaction(reaction, 1);
        }

        protected virtual void FireReaction(Reaction reaction, int delta)
        {
            foreach (Species species in reaction.Reactants)
                species.Decrement(delta);

            foreach (Species species in reaction.Products)
                species.Increment(delta);

            reactionsFiredInCurrentRealization += delta;
        }

        public virtual void OutputData(string prefix)
        {
            var csvOptions = CsvSupport.GetCsvOutputOptions(prefix);
            if (csvOptions.WriteCsvFile)
            {
                CsvSupport.WriteCsvFile(trajectories, csvOptions);
            }

            var jsonOptions = JsonSupport.GetJsonOutputOptions(prefix);
            if (jsonOptions.WriteJsonFile)
            {
                JsonSupport.WriteJsonFile(trajectories, jsonOptions);
            }

            var matlabOptions = MatlabSupport.GetMatlabOutputOptions(prefix);
            if (matlabOptions.WriteMatFile)
            {
                MatlabSupport.WriteMatFile(trajectories, matlabOptions);
            }
        }

        public virtual string[] GetTrajectoryLabels()
        {
            return trajectories.GetTrajectoryLabels();
        }

        public virtual double[][] GetTrajectoryData()
        {
            int cObservables = trajectories.Values.Count;
            int cRuns = trajectories.Values.First().Length;
            var data = new double[cObservables * cRuns][];

            int iTrajectory = 0;
            foreach (Observable o in trajectories.Keys)
            {
                double[][] runs = trajectories[o];
                for (int iRun = 0; iRun < runs.Length; iRun++, iTrajectory++)
                {
                    data[iTrajectory] = runs[iRun];
                }
            }

            return data;
        }
    }
}
