using distlib.hidden;

namespace distlib.randomvariates
{
    public class AesCounterVariateGenerator : RandomVariateGenerator
    {
        private readonly AesCounter _aesCounter;

        // 1/2^24 = 1/0x0100 0000
        private const float Two24Inv = 0.000000059604644775390625f;

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

        public float GenerateUniformOO()
        {
            return _aesCounter.NextVariate();
        }

        public float GenerateUniformOC()
        {
            uint bits = _aesCounter.Next32Bits();   // 0 <= bits <= 0xFFFF FFFF
            bits >>= 8;                             // 0 <= bits <= 0x00FF FFFF
            bits += 1;                              // 0  < bits <= 0x0100 0000

            return bits * Two24Inv;
        }

        public float GenerateUniformCO()
        {
            uint bits = _aesCounter.Next32Bits();   // 0 <= bits <= 0xFFFF FFFF
            bits >>= 8;                             // 0 <= bits <= 0x00FF FFFF

            return bits * Two24Inv;
        }

        public float GenerateUniformCC()
        {
            uint bits;
            do
            {
                bits = _aesCounter.Next32Bits();    // 0 <= bits <= 0xFFFF FFFF
            } while (bits > 0x8000007F);            // 0 <= bits <= 0x8000 007F

            bits >>= 7;                             // 0 <= bits <= 0x0100 0000

            return bits * Two24Inv;
        }

        public static bool IsSupported { get { return AesCounter.IsSupported; } }

        public override string ToString()
        {
            return string.Format("AesCounter Random Variate Generator ({0})", _aesCounter);
        }
    }
}
