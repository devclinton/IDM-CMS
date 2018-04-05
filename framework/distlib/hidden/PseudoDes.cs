/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using System.Runtime.InteropServices;

namespace distlib.hidden
{
    internal class PseudoDes
    {
        private readonly IntPtr _nativeObject;
        private const int CacheCount = 131072;  // 128K doubles is 1MB /* 262144;    // 256K floats/uints is 1MB */
        private readonly double[] _doubleCache;
        private int _doubleCacheIndex;
        private readonly UInt64[] _bitCache;
        private int _bitCacheIndex;

        [DllImport("PrngLib.dll")]
        private static extern IntPtr CreatePseudoDesPrng(uint seedData);

        [DllImport("PrngLib.dll")]
        private static extern void GetDoubles(IntPtr desPrng, double[] doubles, uint cDoubles);

        [DllImport("PrngLib.dll")]
        private static extern void GetLongs(IntPtr desPrng, UInt64[] uints, uint count);

        public PseudoDes(uint generatorSeed, uint coreIndex)
        {
            _doubleCache      = new double[CacheCount];
            _doubleCacheIndex = 0;
            _bitCache         = new UInt64[CacheCount];
            _bitCacheIndex    = 0;
            var seedData      = ((generatorSeed << 16) + coreIndex);
            _nativeObject     = CreatePseudoDesPrng(seedData);

            if (_nativeObject == IntPtr.Zero)
                throw new ApplicationException("Couldn't instantiate PseudoDes pseudo-random number generator.");
        }

        private void FillDoubleCache()
        {
            GetDoubles(_nativeObject, _doubleCache, CacheCount);
            _doubleCacheIndex = CacheCount;
        }

        public double NextVariate()
        {
            if (_doubleCacheIndex == 0) FillDoubleCache();

            return _doubleCache[--_doubleCacheIndex];
        }

        private void FillBitCache()
        {
            GetLongs(_nativeObject, _bitCache, CacheCount);
            _bitCacheIndex = CacheCount;
        }

        public UInt64 Next64Bits()
        {
            if (_bitCacheIndex == 0) FillBitCache();

            return _bitCache[--_bitCacheIndex];
        }

        public override string ToString()
        {
            return "PseudoDes PRNG";
        }
    }
}
