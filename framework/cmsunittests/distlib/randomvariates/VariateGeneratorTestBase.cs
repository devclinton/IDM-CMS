/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using NUnit.Framework;
using distlib.randomvariates;

namespace cmsunittests.distlib.randomvariates
{
    class VariateGeneratorTestBase : AssertionHelper
    {
        private const int UniformCount = 64 * 1048576;  // 4 * 2^24

        protected static float TestUniformOpenOpen(RandomVariateGenerator generator, out bool inBounds)
        {
            float sample = Single.NaN;
            inBounds = true;

            for (int i = 0; i < UniformCount; i++)
            {
                sample = generator.GenerateUniformOO();
                if ((sample <= 0.0f) || (sample >= 1.0f))
                {
                    inBounds = false;
                    break;
                }
            }

            return sample;
        }

        protected static float TestUniformOpenClosed(RandomVariateGenerator generator, out bool inBounds, out bool one)
        {
            float sample = Single.NaN;
            inBounds = true;
            one = false;

            for (int i = 0; i < UniformCount; i++)
            {
                sample = generator.GenerateUniformOC();
                if ((sample <= 0.0f) || (sample > 1.0f))
                {
                    inBounds = false;
                    break;
                }

                if (!one && (sample == 1.0f))
                {
                    one = true;
                }
            }

            return sample;
        }

        protected static float TestUniformClosedOpen(RandomVariateGenerator generator, out bool inBounds, out bool zero)
        {
            float sample = Single.NaN;
            inBounds = true;
            zero = false;

            for (int i = 0; i < UniformCount; i++)
            {
                sample = generator.GenerateUniformCO();
                if ((sample < 0.0f) || (sample >= 1.0f))
                {
                    inBounds = false;
                    break;
                }

                if (!zero && (sample == 0.0f))
                {
                    zero = true;
                }
            }

            return sample;
        }

        protected static float TestUniformClosedClosed(RandomVariateGenerator generator, out bool inBounds, out bool zero, out bool one)
        {
            float sample = Single.NaN;
            inBounds = true;
            zero = false;
            one = false;

            for (int i = 0; i < UniformCount; i++)
            {
                sample = generator.GenerateUniformCC();
                if ((sample < 0.0f) || (sample > 1.0f))
                {
                    inBounds = false;
                    break;
                }

                if (!zero && (sample == 0.0f))
                {
                    zero = true;
                }
                else if (!one && (sample == 1.0f))
                {
                    one = true;
                }

            }
            return sample;
        }
    }
}
