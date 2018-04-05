/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;

namespace distlib.randomvariates
{
    public class DotNetVariateGenerator : RandomVariateGenerator
    {
        private readonly Random _random;

        private DotNetVariateGenerator(int seed = 1968)
        {
            _random = new Random(seed);
        }

        public static RandomVariateGenerator CreateDotNetVariateGenerator(uint[] seedData = null)
        {
            int seed = 1968;

            if ((seedData != null) && (seedData.Length > 0))
            {
                seed = (int) seedData[0];
            }

            var generator = new DotNetVariateGenerator(seed);

            return generator;
        }

        public double GenerateUniformOO()
        {
            double next;

            // 0 <= NextDouble() < 1.0
            do
            {
                next = _random.NextDouble();
            } while (next == 0.0);

            return next;
        }

        public double GenerateUniformOC()
        {
            return (double)_random.Next(1, 0x01000001) / 0x01000000;
        }

        public double GenerateUniformCO()
        {
            return (double)_random.Next(0, 0x01000000) / 0x01000000;
        }

        public double GenerateUniformCC()
        {
            return (double)_random.Next(0, 0x01000001) / 0x01000000;
        }

        public override string ToString()
        {
            return string.Format(".Net Random Variate Generator ({0})", _random);
        }
    }
}
