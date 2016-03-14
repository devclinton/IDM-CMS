using System;
using System.Runtime.InteropServices;

namespace distlib.hidden
{
    internal class PseudoDes
    {
        private readonly IntPtr _nativeObject;
        private const int CacheCount = 262144;    // 256K floats/uints is 1MB
        private readonly float[] _floatCache;
        private int _floatCacheIndex;
        private readonly uint[] _bitCache;
        private int _bitCacheIndex;

        [DllImport("PrngLib.dll")]
        private static extern IntPtr CreatePseudoDesPrng(uint seedData);

        [DllImport("PrngLib.dll")]
        private static extern void GetFloats(IntPtr desPrng, float[] floats, uint cFloats);

        [DllImport("PrngLib.dll")]
        private static extern void GetInts(IntPtr desPrng, uint[] uints, uint count);

        public PseudoDes(uint generatorSeed, uint coreIndex)
        {
            _floatCache      = new float[CacheCount];
            _floatCacheIndex = 0;
            _bitCache        = new uint[CacheCount];
            _bitCacheIndex   = 0;
            var seedData     = ((generatorSeed << 16) + coreIndex);
            _nativeObject    = CreatePseudoDesPrng(seedData);

            if (_nativeObject == IntPtr.Zero)
                throw new ApplicationException("Couldn't instantiate PseudoDes pseudo-random number generator.");
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

        public override string ToString()
        {
            return "PseudoDes PRNG";
        }
    }
}
