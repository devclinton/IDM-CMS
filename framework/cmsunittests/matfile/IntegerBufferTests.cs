/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using NUnit.Framework;
using matfilelib;

namespace cmsunittests.matfile
{
    [TestFixture]
    class IntegerBufferTests
    {
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void NullBufferTest()
        {
            int[] data = null;
            var matrix = new MatlabMatrix(data);
        }

        [Test]
        public void EmptyBufferTest()
        {
            var matfile = new DotMatFile();
            var data = new int[0];
            var matrix = new MatlabMatrix(data);
            matfile["emptyIntegers"] = matrix;
            matfile.WriteToDisk("emptyIntegers.mat");
        }

        [Test]
        public void SimpleBufferTest()
        {
            var matfile = new DotMatFile();
            var data = new[] { 0, 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536, 131072, 262144, 524288, 1048576, 2097151 };
            var matrix = new MatlabMatrix(data);
            matfile["simpleIntegers"] = matrix;
            matfile.WriteToDisk("simpleIntegers.mat");
        }

        [Test]
        public void TwoDeeBufferTest()
        {
            var matfile = new DotMatFile();
            var data = new[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536, 131072, 262144, 524288, 1048576, 2097151 };
            var matrix = new MatlabMatrix(data, new[] { 2, 11 });
            matfile["twodIntegers"] = matrix;
            matfile.WriteToDisk("twodIntegers.mat");
        }

        [Test]
        public void ThreeDeeBufferTest()
        {
            var matfile = new DotMatFile();
            var data = new[] { 1000000, 1000001, 1000002, 1000004, 1000006, 1000005, 1000003, 1000007 };
            var matrix = new MatlabMatrix(data, new[] { 2, 2, 2 });
            matfile["threedIntegers"] = matrix;
            matfile.WriteToDisk("threedIntegers.mat");
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ShortBufferTest()
        {
            var data = new[] { 1000000, 1000001, 1000002, 1000004, 1000006, 1000005, 1000003, 1000007 };
            var matrix = new MatlabMatrix(data, new[] { 3, 3 });
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void LongBufferTest()
        {
            var data = new[] { 1000000, 1000001, 1000002, 1000004, 1000006, 1000005, 1000003, 1000007, 1000008, 1000009, 1000010, 1000011, 1000012, 1000013, 1000014, 1000015 };
            var matrix = new MatlabMatrix(data, new[] { 3, 3 });
        }
    }
}