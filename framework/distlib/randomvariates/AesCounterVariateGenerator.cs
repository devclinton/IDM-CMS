using System;
using distlib.hidden;

namespace distlib.randomvariates
{
    public class AesCounterVariateGenerator : RandomVariateGenerator
    {
        private readonly AesCounter _aesCounter;

        private const float Two24Inv = 0.000000059604644775390625f;                         // 1/2^24 = 1/0x0100 0000
        private const double Two52Inv = 0.00000000000000022204460492503130808472633361816;  // 1/2^52 = 1/0x0010 0000 0000 0000

        private AesCounterVariateGenerator(uint seed = 0, uint index = 0)
        {
            _aesCounter = new AesCounter(seed, index);
        }

        public static RandomVariateGenerator CreateAesCounterVariateGenerator(uint[] seedData = null)
        {
            uint seed = 0;
            uint index = 0;

            if (seedData != null)
            {
                if (seedData.Length > 0)
                {
                    seed = seedData[0];
                    if (seedData.Length > 1)
                    {
                        index = seedData[1];
                    }
                }
            }

            var generator = new AesCounterVariateGenerator(seed, index);

            return generator;
        }

        public double GenerateUniformOO()
        {
            return _aesCounter.NextVariate();
        }

        public double GenerateUniformOC()
        {
            UInt64 bits = _aesCounter.Next64Bits(); // 0 <= bits <= 0xFFFF FFFF FFFF FFFF
            bits >>= 12;                            // 0 <= bits <= 0x000F FFFF FFFF FFFF
            bits += 1;                              // 0  < bits <= 0x0010 0000 0000 0000

            return bits * Two52Inv;                 // 0.0 < return <= 1.0
        }

        public double GenerateUniformCO()
        {
            UInt64 bits = _aesCounter.Next64Bits(); // 0 <= bits <= 0xFFFF FFFF FFFF FFFF
            bits >>= 12;                            // 0 <= bits <= 0x000F FFFF FFFF FFFF

            return bits * Two52Inv;                 // 0.0 <= return < 1.0
        }

        /* Note on the choice of constant 0x8000 0000 0000 07FF: all bit values from 0 to 7FF
         * (2048 possibilities) will become 0 before being returned. Similarly, all bit
         * values from 800 to FFF (another 2048 possibilities) will become 1 before being
         * returned, etc. Thus, we want to choose any of 2048 bit patterns to eventually
         * become 0x0100 0000 0000 0000 before being returned. Allowing values up to
         * 0x8000 0000 0000 07FF enables this.
         */
        public double GenerateUniformCC()
        {
            UInt64 bits;
            do
            {
                bits = _aesCounter.Next64Bits();    // 0 <= bits <= 0xFFFF FFFF FFFF FFFF
            } while (bits > 0x80000000000007FF);    // 0 <= bits <= 0x8000 0000 0000 07FF

            bits >>= 11;                            // 0 <= bits <= 0x0010 0000 0000 0000

            return bits * Two52Inv;                 // 0.0 <= return <= 1.0
        }

        public static bool IsSupported { get { return AesCounter.IsSupported; } }

        public override string ToString()
        {
            return string.Format("AesCounter Random Variate Generator ({0})", _aesCounter);
        }
    }
}
