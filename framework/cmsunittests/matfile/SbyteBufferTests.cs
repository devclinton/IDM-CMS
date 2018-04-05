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
    class SbyteBufferTests
    {
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void NullBufferTest()
        {
            sbyte[] data = null;
            var matrix = new MatlabMatrix(data);
        }

        [Test]
        public void EmptyBufferTest()
        {
            var matfile = new DotMatFile();
            var data = new sbyte[0];
            var matrix = new MatlabMatrix(data);
            matfile["emptySbytes"] = matrix;
            matfile.WriteToDisk("emptySbytes.mat");
        }

        [Test]
        public void SimpleBufferTest()
        {
            var matfile = new DotMatFile();
            var data = new sbyte[] { 0, -1, 2, -4, 8, -16, 32, -64, 127, -128 };
            var matrix = new MatlabMatrix(data);
            matfile["simpleSbytes"] = matrix;
            matfile.WriteToDisk("simpleSbytes.mat");
        }

        [Test]
        public void TwoDeeBufferTest()
        {
            var matfile = new DotMatFile();
            var data = new sbyte[] { 1, -2, 3, -4, 5, -6, 7, -8 };
            var matrix = new MatlabMatrix(data, new[] { 2, 4 });
            matfile["twodSbytes"] = matrix;
            matfile.WriteToDisk("twodSbytes.mat");
        }

        [Test]
        public void ThreeDeeBufferTest()
        {
            var matfile = new DotMatFile();
            var data = new sbyte[] { 0, -1, 2, -4, 6, -5, 3, -7 };
            var matrix = new MatlabMatrix(data, new[] { 2, 2, 2 });
            matfile["threedSbytes"] = matrix;
            matfile.WriteToDisk("threedSbytes.mat");
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ShortBufferTest()
        {
            var data = new sbyte[] { 1, -2, 3, -4, 5, -6, 7, -8 };
            var matrix = new MatlabMatrix(data, new[] { 3, 3 });
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void LongBufferTest()
        {
            var data = new sbyte[] { 1, -2, 3, -4, 5, -6, 7, -8, 9, -10, 11, -12, 13, -14, 15 };
            var matrix = new MatlabMatrix(data, new[] { 3, 3 });
        }
    }
}