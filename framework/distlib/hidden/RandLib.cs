namespace distlib.hidden
{
    internal class RandLib
    {
        // V = 20, W = 30
        private const int M1 = 2147483563;  // 7FFF FFAB
        private const int M2 = 2147483399;
        private const int A1 = 40014;
        private const int A2 = 40692;
/*
        private const int A1W = 1033780774; // A1^(2^W) % M1
        private const int A2W = 1494757890; // A2^(2^W) % M2

        private enum SeedType
        {
            InitialSeed,
            NewSeed
        };
*/

        // ReSharper disable InconsistentNaming
        private int _lgOne;
        private int _lgTwo;
        private int _LgOne;
        private int _LgTwo;
        private int _CgOne;
        private int _CgTwo;
/*
        private bool _antithetic;
*/
        // ReSharper restore InconsistentNaming

        public RandLib(int s1 = 1234567890, int s2 = 123456789)
        {
            Initialize(s1, s2);
        }

/*
        private static int MultMod(int a, int s, int m)
        {
            long al = a;
            long sl = s;
            long ml = m;

            long mml = (al*sl)%ml;
            var mm = (int) mml;

            return mm;
        }
*/

        private void InitGenerator(/*SeedType where*/)
        {
/*
            switch (where)
            {
                case SeedType.InitialSeed:
*/
                    _LgOne = _lgOne;
                    _LgTwo = _lgTwo;
/*
                    break;

                case SeedType.NewSeed:
                    _LgOne = MultMod(A1W, _LgOne, M1);
                    _LgTwo = MultMod(A2W, _lgTwo, M2);
                    break;
            }
*/

            _CgOne = _LgOne;
            _CgTwo = _LgTwo;
        }

        private void SetInitialSeed(int s1, int s2)
        {
            _lgOne = s1;
            _lgTwo = s2;
            InitGenerator(/*SeedType.InitialSeed*/);
        }

/*
        public void SetSeed(int s1, int s2)
        {
            _lgOne = s1;
            _lgTwo = s2;
            InitGenerator(SeedType.InitialSeed);
        }

        public void AdvanceState(int k)
        {
            int b1 = A1;
            int b2 = A2;

            for (int i = 1; i <= k; i++)
            {
                b1 = MultMod(b1, b1, M1);
                b2 = MultMod(b2, b2, M2);
            }

            SetSeed(MultMod(b1, _CgOne, M1), MultMod(b2, _CgTwo, M2));
        }

        public void GetState(out int s1, out int s2)
        {
            s1 = _CgOne;
            s2 = _CgTwo;
        }

        public void SetAntithetic(bool isAntithetic)
        {
            _antithetic = isAntithetic;
        }
*/

        /*
        ** Returns a value 0 < z < M1
        */
        public int Random()
        {
            int s1 = _CgOne;
            int s2 = _CgTwo;
            int k = s1 / 53668;
            s1 = A1*(s1 - k*53668) - k*12211;

            if (s1 < 0)
            {
                s1 += M1;
            }

            k = s2/52774;
            s2 = A2*(s2 - k*52774) - k*3791;

            if (s2 < 0)
            {
                s2 += M2;
            }

            _CgOne = s1;
            _CgTwo = s2;
            int z = s1 - s2;

            if (z < 1)
            {
                z += (M1 - 1);
            }

/*
            if (_antithetic)
            {
                z = M1 - z;
            }
*/

            return z;
        }

        private void Initialize(int s1, int s2)
        {
/*
            _antithetic = false;
*/

            SetInitialSeed(s1, s2);
        }

        public override string ToString()
        {
            return "RandLib PRNG";
        }
    }
}
