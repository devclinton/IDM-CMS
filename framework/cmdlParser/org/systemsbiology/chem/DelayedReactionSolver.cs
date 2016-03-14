/*
* Copyright (C) 2003 by Institute for Systems Biology,
* Seattle, Washington, USA.  All rights reserved.
*
* This source code is distributed under the GNU Lesser
* General Public License, the text of which is available at:
*   http://www.gnu.org/copyleft/lesser.html
*/
using System;
using System.Diagnostics;
using org.systemsbiology.data;
using PriorityQueue = org.systemsbiology.data.PriorityQueue;
using Queue = org.systemsbiology.data.Queue;
using org.systemsbiology.math;
using org.systemsbiology.util;
using cern.jet.random;
namespace org.systemsbiology.chem
{

    /// <summary> Used to simulate a chemical reaction containing a specified delay
    /// time.  The reactant is immediately converted to a (hidden)
    /// "intermediate species".  The reaction converting the intermediate
    /// species to the product species occurs after the specified delay.
    /// This class is used by subclasses of the {@link Simulator} class.
    /// The application developer will rarely need to work directly with
    /// an instance of this class.
    ///
    /// </summary>
    /// <author>  Stephen Ramsey
    /// </author>

    public sealed class DelayedReactionSolver
    {
        //UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'AnonymousClassAbstractComparator' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
        private class AnonymousClassAbstractComparator:AbstractComparator
        {
            public AnonymousClassAbstractComparator(DelayedReactionSolver enclosingInstance)
            {
                InitBlock(enclosingInstance);
            }

            private void  InitBlock(DelayedReactionSolver enclosingInstance)
            {
                this.enclosingInstance = enclosingInstance;
            }

            private DelayedReactionSolver enclosingInstance;

            public DelayedReactionSolver Enclosing_Instance
            {
                get
                {
                    return enclosingInstance;
                }

            }

            public override int compare(System.Object p1, System.Object p2)
            {
                //J return (MutableDouble.compare((MutableDouble) p1, (MutableDouble) p2));
                return (double)p1 == (double)p2 ? 1 : 0;
            }
        }
        public double Rate
        {
            get
            {
                return (mRate);
            }

        }
        public double Delay
        {
            get
            {
                return (mDelay);
            }

        }
        public Species IntermedSpecies
        {
            get
            {
                return (mIntermedSpecies);
            }

        }
        public int NumHistoryBins
        {
            get
            {
                return (mNumTimePoints);
            }

            set
            {
                if (value < MIN_NUM_HISTORY_BINS)
                {
                    throw new System.ArgumentException("invalid history bin size, must be at least " + MIN_NUM_HISTORY_BINS);
                }

                mNumTimePoints = value;
                mTimeResolution = LAMBDA_MAX * mDelay / ((double) value);

                if (mIsStochasticSimulator)
                {
                    mReactionTimesDoublePool.Clear();
                    for (int ctr = 0; ctr < value; ++ctr)
                    {
                        //J mReactionTimesDoublePool.Add(new MutableDouble(0.0));
                        mReactionTimesDoublePool.Add(0.0);
                    }
                }
                else
                {
                    mReactantHistory.initialize(mNumTimePoints);
                }
            }

        }
        internal int ReactionIndex
        {
            get
            {
                return (mReactionIndex);
            }

        }
        private const double LAMBDA_MAX = 1.1;
        public const int MIN_NUM_HISTORY_BINS = 10;
        public const int DEFAULT_NUM_HISTORY_BINS = 400;

        private Species mReactant;
        private Species mIntermedSpecies;
        private double mRate;
        private double mTimeResolution;
        private bool mFirstTimePoint;
        private bool mIsMultistep;

        // used only for stochastic simulations
        private Queue mReactionTimes;

        private System.Collections.ArrayList mReactionTimesDoublePool;

        // used only for deterministic simulation
        private SlidingWindowTimeSeriesQueue mReactantHistory;
        private int mNumTimePoints;

        private bool mIsStochasticSimulator;

        private int mReactionIndex;
        private double mDelay;

        public override System.String ToString()
        {
            return (mIntermedSpecies.Name);
        }

        public DelayedReactionSolver(Species pReactant, Species pIntermedSpecies, double pDelay, double pRate, bool pIsMultistep, int pReactionIndex, bool pIsStochasticSimulator)
        {
            Debug.Assert(pDelay > 0.0, "invalid delay");
            mDelay = pDelay;

            mReactant = pReactant;
            mIntermedSpecies = pIntermedSpecies;

            mRate = pRate;

            mFirstTimePoint = true;

            mIsMultistep = pIsMultistep;
            mReactionIndex = pReactionIndex;

            mIsStochasticSimulator = pIsStochasticSimulator;

            if (mIsStochasticSimulator)
            {
                if (mIsMultistep)
                {
                    mReactionTimes = new PriorityQueue(new AnonymousClassAbstractComparator(this));
                }
                else
                {
                    mReactionTimes = new ListQueue();
                }

                mReactionTimesDoublePool = new System.Collections.ArrayList();
                mReactantHistory = null;
            }
            else
            {
                mReactantHistory = new SlidingWindowTimeSeriesQueue(1);
                mReactionTimes = null;
                mReactionTimesDoublePool = null;
            }

            NumHistoryBins = DEFAULT_NUM_HISTORY_BINS;
        }

        internal void  addReactant(SymbolEvaluatorChem pSymbolEvaluator)
        {
            //J MutableDouble newReactionTime = null;
            double newReactionTime = 0.0;
            double relTime;
            if (mIsMultistep)
            {
                relTime = Gamma.staticNextDouble(mRate * mDelay, mRate);
            }
            else
            {
                relTime = mDelay;
            }
            double reactionTime = pSymbolEvaluator.Time + relTime;

            System.Collections.ArrayList reactionTimesDoublePool = mReactionTimesDoublePool;

            if (reactionTimesDoublePool.Count > 0)
            {
                //J newReactionTime = (MutableDouble)reactionTimesDoublePool[reactionTimesDoublePool.Count - 1];
                newReactionTime = (double)reactionTimesDoublePool[reactionTimesDoublePool.Count - 1];
                reactionTimesDoublePool.RemoveAt(reactionTimesDoublePool.Count - 1);
                //J newReactionTime.Value = reactionTime;
                newReactionTime = reactionTime;
            }
            else
            {
                //J newReactionTime = new MutableDouble(reactionTime);
                newReactionTime = reactionTime;
            }

            mReactionTimes.add(newReactionTime);
        }


        internal double pollNextReactionTime()
        {
            //J MutableDouble reactionTime = (MutableDouble)mReactionTimes.getNext();
            double reactionTime = (double)mReactionTimes.getNext();
/*J
            if (null == reactionTime)
            {
                throw new System.SystemException("no molecules are in the multistep reaction queue");
            }
*/
            //J double nextReactionTime = reactionTime.Value;
            //J reactionTime.Value = 0.0;
            double nextReactionTime = reactionTime;
            reactionTime = 0.0;
            mReactionTimesDoublePool.Insert(mReactionTimesDoublePool.Count, reactionTime);
            return (nextReactionTime);
        }

        // used for stochastic simulator
        internal bool canHaveReaction()
        {
            return (null != mReactionTimes.peekNext());
        }

        // used for stochastic simulator
        internal double peekNextReactionTime()
        {
            //J MutableDouble reactionTime = (MutableDouble)mReactionTimes.peekNext();
            double reactionTime = (double)mReactionTimes.peekNext();
/*J
            if (null == reactionTime)
            {
                throw new System.SystemException("no molecules are in the multistep reaction queue");
            }
*/
            //J return (reactionTime.Value);
            return reactionTime;
        }

        internal double getEstimatedAverageFutureRate(SymbolEvaluator pSymbolEvaluator)
        {
            double rate = pSymbolEvaluator.getValue(mIntermedSpecies.Symbol) / mDelay;
            return (rate);
        }

        internal void  clear()
        {
            if (mIsStochasticSimulator)
            {
                while (null != mReactionTimes.peekNext())
                {
                    //J MutableDouble reactionTime = (MutableDouble)mReactionTimes.getNext();
                    //J reactionTime.Value = 0.0;
                    double reactionTime = (double)mReactionTimes.getNext();
                    reactionTime = 0.0;
                    mReactionTimesDoublePool.Insert(mReactionTimesDoublePool.Count, reactionTime);
                }
            }
            else
            {
                mReactantHistory.clear();
                mFirstTimePoint = true;
            }
        }

        public void  update(SymbolEvaluator pSymbolEvaluator, double pTime)
        {
            SlidingWindowTimeSeriesQueue reactantHistory = mReactantHistory;
            if (!mFirstTimePoint)
            {
                double lastTime = reactantHistory.LastTimePoint;
                bool gotValue = false;
                double reactantValue = 0.0;
                double intermedSpeciesValue = 0.0;
                while (pTime - lastTime > mTimeResolution)
                {
                    if (!gotValue)
                    {
                        reactantValue = pSymbolEvaluator.getValue(mReactant.Symbol);
                        intermedSpeciesValue = pSymbolEvaluator.getValue(mIntermedSpecies.Symbol);
                    }
                    lastTime += mTimeResolution;

                    Debug.Assert(reactantValue >= 0.0, "invalid value");
                    reactantHistory.insertPoint(lastTime, reactantValue);

                    Debug.Assert(intermedSpeciesValue >= 0.0, "invalid value");
                }
            }
            else
            {
                double reactantValue = pSymbolEvaluator.getValue(mReactant.Symbol);
                Debug.Assert(reactantValue >= 0.0, "invalid value");

                double intermedSpeciesValue = pSymbolEvaluator.getValue(mIntermedSpecies.Symbol);
                Debug.Assert(intermedSpeciesValue >= 0.0, "invalid value");

                reactantHistory.insertPoint(pTime, reactantValue);

                mFirstTimePoint = false;
            }
        }

        public double computeRate(SymbolEvaluator pSymbolEvaluator)
        {
            if (!mIsStochasticSimulator)
            {
                if (mIsMultistep)
                {
                    return (computeRateMultistep(pSymbolEvaluator));
                }
                else
                {
                    return (computeRateDelay(pSymbolEvaluator));
                }
            }
            else
            {
                return (0.0);
            }
        }

        private double computeRateMultistep(SymbolEvaluator pSymbolEvaluator)
        {
            double prodRate = 0.0;

            SymbolEvaluatorChem symbolEvaluator = (SymbolEvaluatorChem) pSymbolEvaluator;

            double currentTime = symbolEvaluator.Time;

            SlidingWindowTimeSeriesQueue reactantSpeciesHistory = mReactantHistory;

            double intermedSpeciesValue = symbolEvaluator.getValue(mIntermedSpecies.Symbol);
            double generatedAux = reactantSpeciesHistory.MinTime;
            if (intermedSpeciesValue > 0.0)
            {
                prodRate = computeIntegral(reactantSpeciesHistory, mTimeResolution, mNumTimePoints, mDelay, mRate, currentTime);
            }
            else
            {
                // do nothing; rate of production is zero
            }

            return (prodRate);
        }

        // used for deterministic simulator
        private double computeRateDelay(SymbolEvaluator pSymbolEvaluator)
        {
            double prodRate = 0.0;

            SymbolEvaluatorChem symbolEvaluator = (SymbolEvaluatorChem) pSymbolEvaluator;

            double currentTime = symbolEvaluator.Time;

            SlidingWindowTimeSeriesQueue reactantSpeciesHistory = mReactantHistory;

            double intermedSpeciesValue = symbolEvaluator.getValue(mIntermedSpecies.Symbol);
            double minTime = reactantSpeciesHistory.MinTime;
            double peakTimeRel = mDelay;
            double peakTime = currentTime - peakTimeRel;

            if (intermedSpeciesValue > 0.0 && peakTime >= minTime)
            {
                double peakValue = 0.0;

                double peakIndexDouble = (peakTime - minTime) / mTimeResolution;
                double peakIndexDoubleFloor = System.Math.Floor(peakIndexDouble);
                //UPGRD_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
                int peakIndex = (int) peakIndexDouble;
                if (peakIndexDouble > peakIndexDoubleFloor)
                {
                    double valueLeft = reactantSpeciesHistory.getValue(peakIndex);
                    peakValue = valueLeft + ((peakIndexDouble - peakIndexDoubleFloor) * (reactantSpeciesHistory.getValue(peakIndex + 1) - valueLeft));
                }
                else
                {
                    peakValue = reactantSpeciesHistory.getValue(peakIndex);
                }

                prodRate = mRate * peakValue;
            }
            else
            {
                // do nothing; rate of production is zero
            }

            return (prodRate);
        }

        // keeping this around for historical purposes
        private static double computeIntegral(SlidingWindowTimeSeriesQueue history, double timeResolution, int numTimePoints, double delay, double rate, double currentTime)
        {
            double value_Renamed = 0.0;
            double prodRate = 0.0;
            double numStepsCorrected = delay * rate;
            int numPoints = history.NumStoredPoints;

            double sqrtTwoPiNumStepsCorrected = System.Math.Sqrt(2.0 * System.Math.PI * numStepsCorrected);

            for (int ctr = numPoints; --ctr >= 0; )
            {
                value_Renamed = timeResolution * computeIntegrandValue(history, ctr, rate, rate * rate, sqrtTwoPiNumStepsCorrected, numStepsCorrected, currentTime);

                if (ctr == 0 || ctr == numTimePoints - 1)
                {
                    prodRate += value_Renamed / 3.0;
                }
                else if ((ctr % 2) == 1)
                {
                    prodRate += 2.0 * value_Renamed / 3.0;
                }
                else
                {
                    prodRate += 4.0 * value_Renamed / 3.0;
                }
            }
            return (prodRate);
        }

        private static double computeIntegrandValue(SlidingWindowTimeSeriesQueue pReactantHistory, int pTimePointIndex, double pRate, double pRateSquared, double pSqrtTwoPiNumStepsCorrected, double numStepsCorrected, double pCurrentTime)
        {
            double reactantValue = pReactantHistory.getValue(pTimePointIndex);
            double timePoint = pReactantHistory.getTimePoint(pTimePointIndex);
            Debug.Assert(pCurrentTime >= timePoint, "time point is in the future");
            double rate = pRate;
            double lambda = rate * (pCurrentTime - timePoint);
            double retVal = reactantValue * pRateSquared * System.Math.Pow(lambda * System.Math.E / numStepsCorrected, numStepsCorrected) / (System.Math.Exp(lambda) * pSqrtTwoPiNumStepsCorrected);
            return (retVal);
        }
    }
}