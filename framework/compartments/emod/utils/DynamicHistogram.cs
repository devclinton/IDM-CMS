using System;
using System.Collections;
using System.Collections.Generic;

namespace compartments.emod.utils
{
    public class DynamicHistogram : IEnumerable<long>
    {
        public long SampleCount { get; protected set; }
        public long LowerBound { get; protected set; }
        public long Width { get; protected set; }
        public long UpperBound { get; protected set; }
        public readonly int BinCount;

        private float _inverseWidth;
        private readonly long[] _bins;

        public long this[int index]
        {
            get { return _bins[index]; }
        }

        public DynamicHistogram(int binCount)
        {
            if (binCount < 1)
            {
                throw new ArgumentException("Bin count must be > 0.", "binCount");
            }

            _bins    = new long[binCount];
            BinCount = binCount;
            Initialize();
        }

        public DynamicHistogram(long[] bins)
        {
            if (bins == null)
            {
                throw new ArgumentNullException("bins", "Bins array must not be null.");
            }

            _bins    = bins;
            BinCount = bins.Length;
            Initialize();
        }

        private void Initialize()
        {
            SampleCount   = 0;
            LowerBound    = 0;
            Width         = 1;
            _inverseWidth = 1.0f;
            UpperBound    = BinCount; // LowerBound + Width * BinCount
        }

        public void AddSample(long sampleValue)
        {
            SampleCount++;
            EnsureBoundsForSample(sampleValue);
            _bins[BinIndexForSample(sampleValue)]++;
        }

        private int BinIndexForSample(long sampleValue)
        {
            return (int) ((sampleValue - LowerBound)*_inverseWidth);
        }

        private void EnsureBoundsForSample(long sampleValue)
        {
            if ((LowerBound > sampleValue) || (sampleValue >= UpperBound))
            {
                long lower = FindSmallestSampleValue(sampleValue);
                long upper = FindMinimumUpperBound(sampleValue);
                RebuildForLowAndHigh(lower, upper);
            }
        }

        private long FindSmallestSampleValue(long sampleValue)
        {
            long lower = sampleValue;
            if (LowerBound < lower)
            {
                for (int i = 0; i < BinCount; i++)
                {
                    if (_bins[i] != 0)
                    {
                        lower = Math.Min(lower, LowerBound + i*Width);
                        break;
                    }
                }
            }
            return lower;
        }

        private long FindMinimumUpperBound(long sampleValue)
        {
            long upper = sampleValue + 1;
            if (UpperBound > upper)
            {
                for (int i = BinCount; i-- > 0;)
                {
                    if (_bins[i] != 0)
                    {
                        upper = Math.Max(upper, LowerBound + (i + 1)*Width);
                        break;
                    }
                }
            }
            return upper;
        }

        private void RebuildForLowAndHigh(long low, long high)
        {
            long newWidth      = Width;
            var newLowerBound  = (long) (Math.Floor((double)low/Width)*Width);
            long newUpperBound = newLowerBound + BinCount*Width;

            while (high > newUpperBound)
            {
                newWidth *= 2;
                newLowerBound = (long)(Math.Floor((double)low / newWidth) * newWidth);
                newUpperBound = newLowerBound + BinCount * newWidth;
            }

            UpdateLowerBoundAndWidth(newLowerBound, newWidth);
        }

        private void UpdateLowerBoundAndWidth(long newLowerBound, long newWidth)
        {
            long oldWidth = Width;
            long oldLowerBound = LowerBound;

            LowerBound    = newLowerBound;
            Width         = newWidth;
            _inverseWidth = 1.0f / newWidth;
            UpperBound    = newLowerBound + BinCount*newWidth;

            var newBins           = new long[BinCount];

            for (int i = 0; i < BinCount; i++)
            {
                if (_bins[i] != 0)
                {
                    long value = oldLowerBound + i*oldWidth;
                    newBins[BinIndexForSample(value)] += _bins[i];
                }
            }

            newBins.CopyTo(_bins, 0);
        }

        public void Clear()
        {
            SampleCount   = 0;
            LowerBound    = 0;
            UpperBound    = BinCount;
            Width         = 1;
            _inverseWidth = 1.0f;

            for (int i = 0; i < BinCount; i++)
            {
                _bins[i] = 0;
            }
        }

        public IEnumerator<long> GetEnumerator()
        {
            return (new List<long>(_bins)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
