using System;
using System.Runtime.InteropServices;

namespace distlib.hidden
{
    public class AesCounter
    {
        private readonly IntPtr _nativeObject;
        private const int CacheCount = 262144;  // 256K floats/uints is 1MB
        private readonly float[] _floatCache;
        private int _floatCacheIndex;
        private readonly uint[] _bitCache;
        private int _bitCacheIndex;

        [DllImport("PrngLib.dll")]
        private static extern UInt32 CpuSupportsAesInstructions();

        [DllImport("PrngLib.dll")]
        private static extern IntPtr CreateAesCounterPrng(ulong seed);

        [DllImport("PrngLib.dll")]
        private static extern void GetFloats(IntPtr aesPrng, float[] floats, uint cFloats);

        [DllImport("PrngLib.dll")]
        private static extern void GetInts(IntPtr aesPrng, uint[] uints, uint count);

        public AesCounter(uint generatorSeed, uint coreIndex)
        {
            if (!IsSupported)
                throw new ApplicationException("CPU doesn't support AES instruction set.");

            _floatCache      = new float[CacheCount];
            _floatCacheIndex = 0;
            _bitCache        = new uint[CacheCount];
            _bitCacheIndex   = 0;
            var longSeed     = ((ulong) generatorSeed << 32) + coreIndex;
            _nativeObject    = CreateAesCounterPrng(longSeed);

            if (_nativeObject == IntPtr.Zero)
                throw new ApplicationException("Couldn't instantiate AesCounter pseudo-random number generator.");
        }

        private void FillFloatCache()
        {
            GetFloats(_nativeObject, _floatCache, CacheCount);
            _floatCacheIndex = CacheCount;
        }

        public float NextVariate()
        {
            if (_floatCacheIndex == 0) FillFloatCache();

            return _floatCache[--_floatCacheIndex];
        }

        private void FillBitCache()
        {
            GetInts(_nativeObject, _bitCache, CacheCount);
            _bitCacheIndex = CacheCount;
        }

        public uint Next32Bits()
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
