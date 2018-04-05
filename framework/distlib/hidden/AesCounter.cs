using System;
using System.Runtime.InteropServices;

namespace distlib.hidden
{
    public class AesCounter
    {
        private readonly IntPtr _nativeObject;
        private const int CacheCount = 131072; // 128K doubles is 1MB /* 262144;  // 256K floats/uints is 1MB */
        private readonly double[] _doubleCache;
        private int _doubleCacheIndex;
        private readonly UInt64[] _bitCache;
        private int _bitCacheIndex;

        [DllImport("PrngLib.dll")]
        private static extern UInt32 CpuSupportsAesInstructions();

        [DllImport("PrngLib.dll")]
        private static extern IntPtr CreateAesCounterPrng(ulong seed);

        [DllImport("PrngLib.dll")]
        private static extern void GetDoubles(IntPtr aesPrng, double[] doubles, uint cDoubles);

        [DllImport("PrngLib.dll")]
        private static extern void GetLongs(IntPtr aesPrng, UInt64[] uints, uint count);

        public AesCounter(uint generatorSeed, uint coreIndex)
        {
            if (!IsSupported)
                throw new ApplicationException("CPU doesn't support AES instruction set.");

            _doubleCache      = new double[CacheCount];
            _doubleCacheIndex = 0;
            _bitCache         = new UInt64[CacheCount];
            _bitCacheIndex    = 0;
            var longSeed      = ((ulong) generatorSeed << 32) + coreIndex;
            _nativeObject     = CreateAesCounterPrng(longSeed);

            if (_nativeObject == IntPtr.Zero)
                throw new ApplicationException("Couldn't instantiate AesCounter pseudo-random number generator.");
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

        public static bool IsSupported { get { return CpuSupportsAesInstructions() != 0; } }

        public override string ToString()
        {
            return "AesCounter PRNG";
        }
    }
}
