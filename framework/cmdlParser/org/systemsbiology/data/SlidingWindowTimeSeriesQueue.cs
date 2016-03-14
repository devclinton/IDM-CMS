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

using org.systemsbiology.math;

namespace org.systemsbiology.data
{

	/// <summary> Implements a queue of ordered pairs of floating-point
	/// values.  The first element of the ordered pair is the time,
	/// and the second element of the ordered pair is the value
	/// of some variable at that time.  When the queue fills up,
	/// it start overwriting itself, discarding the oldest point
	/// first.  Therefore it is a FIFO (first-in, first-out) queue.
	/// The temporal ordering of the timestamps is not enforced.
	///
	/// </summary>
	/// <author>  Stephen Ramsey
	/// </author>
	public class SlidingWindowTimeSeriesQueue
	{
		virtual public double TimeLastNonzeroValue
		{
			get
			{
				if (!mHasNonzeroValue)
				{
					throw new System.SystemException("there is no nonzero value in the history");
				}

				return (mTimeLastNonzeroValue);
			}

		}
		virtual public double LastTimePoint
		{
			get
			{
				return (mLastTime);
			}

		}
		virtual public int NumStoredPoints
		{
			get
			{
				return (mNumStoredPoints);
			}

		}
		virtual public double AverageValue
		{
			get
			{
				return (mAverageValue);
			}

		}
		private double ExactAverageValue
		{
			get
			{
				int numPoints = mNumStoredPoints;
				double avg = 0.0;
				for (int ctr = 0; ctr < numPoints; ++ctr)
				{
					avg += getValue(ctr);
				}
				avg /= ((double) numPoints);
				return (avg);
			}

		}
		virtual public double MinTime
		{
			get
			{
				return (mTimePoints[mMinIndex]);
			}

		}
		virtual public double[] TimePoints
		{
			get
			{
				return (mTimePoints);
			}

		}
		virtual public double[] Values
		{
			get
			{
				return (mValues);
			}

		}
		private int mNumTimePoints;
		private int mQueueIndex;
		private double[] mTimePoints;
		private double[] mValues;
		private int mNumStoredPoints;
		private int mMinIndex;
		private double mLastTime;
		private double mAverageValue;

		private double mTimeLastNonzeroValue;
		private bool mHasNonzeroValue;
		private int mCounterForRecomputeAverage;

		public SlidingWindowTimeSeriesQueue(int pNumTimePoints)
		{
			initialize(pNumTimePoints);
		}

		public virtual void  initialize(int pNumTimePoints)
		{
			Debug.Assert(pNumTimePoints > 0, "invalid number of time points");

			mTimePoints = new double[pNumTimePoints];
			mValues = new double[pNumTimePoints];
			mNumTimePoints = pNumTimePoints;
			clear();
		}

		public virtual double getValue(int pIndex)
		{
			return (mValues[getInternalIndex(pIndex)]);
		}

		public virtual void  clear()
		{
			mQueueIndex = 0;
			DoubleVector.zeroElements(mTimePoints);
			DoubleVector.zeroElements(mValues);
			mNumStoredPoints = 0;
			mMinIndex = 0;
			mLastTime = 0.0;
			mAverageValue = 0.0;

			mTimeLastNonzeroValue = 0.0;
			mHasNonzeroValue = false;
			mCounterForRecomputeAverage = 0;
		}

		public virtual bool hasNonzeroValue()
		{
			return (mHasNonzeroValue);
		}

		public virtual double getTimePoint(int pIndex)
		{
			return (mTimePoints[getInternalIndex(pIndex)]);
		}

		public virtual void  insertPoint(double pTime, double pValue)
		{
			Debug.Assert(pValue >= 0.0, "invalid value in history");
			mLastTime = pTime;
			int queueIndex = mQueueIndex;
			int numTimePoints = mNumTimePoints;

			double newAverage = mAverageValue * mNumStoredPoints;

			double lastValue = 0.0;

			if (mNumStoredPoints < numTimePoints)
			{
				if (mNumStoredPoints == 0)
				{
					mMinIndex = mQueueIndex;
				}

				++mNumStoredPoints;
			}
			else
			{
				// we are about to overwrite the min time point;
				int nextIndex = queueIndex + 1;
				if (nextIndex >= numTimePoints)
				{
					nextIndex -= numTimePoints;
				}

				lastValue = mValues[queueIndex];

				mMinIndex = nextIndex;
			}

			mTimePoints[queueIndex] = pTime;
			mValues[queueIndex] = pValue;
			if (queueIndex < mNumTimePoints - 1)
			{
				mQueueIndex++;
			}
			else
			{
				mQueueIndex = 0;
			}

			if (pValue > 0.0)
			{
				// need to update the mTimeLastNonzeroValue
				mTimeLastNonzeroValue = pTime;
				mHasNonzeroValue = true;
			}
			else
			{
				if (mHasNonzeroValue)
				{
					if (mTimeLastNonzeroValue < mTimePoints[mMinIndex])
					{
						mHasNonzeroValue = false;
						mTimeLastNonzeroValue = 0.0;
					}
				}
			}

			++mCounterForRecomputeAverage;
			if (mCounterForRecomputeAverage <= mNumTimePoints)
			{
				newAverage = (System.Math.Abs(newAverage - lastValue) + pValue) / mNumStoredPoints;
				mAverageValue = newAverage;
			}
			else
			{
				mAverageValue = ExactAverageValue;
				mCounterForRecomputeAverage = 0;
			}
			Debug.Assert(newAverage >= 0.0, "invalid average value (negative); lastValue: " + lastValue + "; numStoredPoints: " + mNumStoredPoints + "; newAverage: " + newAverage + "; pValue: " + pValue);
		}

		private int getInternalIndex(int pExternalIndex)
		{
			Debug.Assert(pExternalIndex < mNumTimePoints, "invalid external index");
			Debug.Assert(pExternalIndex >= 0, "invalid external index");

			int tempIndex = 0;
			if (mNumStoredPoints >= mNumTimePoints)
			{
				tempIndex = mQueueIndex + pExternalIndex;
				if (tempIndex >= mNumTimePoints)
				{
					tempIndex -= mNumTimePoints;
				}
			}
			else
			{
				if (pExternalIndex >= mNumStoredPoints)
				{
					throw new System.SystemException("no data point has yet been stored for that index; num stored points is " + mNumStoredPoints + " and requested index is " + pExternalIndex);
				}
				tempIndex = pExternalIndex;
			}

			return (tempIndex);
		}
	}
}