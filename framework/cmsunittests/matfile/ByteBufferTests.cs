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
    class ByteBufferTests
    {
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void NullBufferTest()
        {
            byte[] data = null;
            var matrix = new MatlabMatrix(data);
        }

        [Test]
        public void EmptyBufferTest()
        {
            var matfile = new DotMatFile();
            var data = new byte[0];
            var matrix = new MatlabMatrix(data);
            matfile["emptyBytes"] = matrix;
            matfile.WriteToDisk("emptyBytes.mat");
        }

        [Test]
        public void SimpleBufferTest()
        {
            var matfile = new DotMatFile();
            var data = new byte[] { 0, 1, 2, 4, 8, 16, 32, 64, 128, 255 };
            var matrix = new MatlabMatrix(data);
            matfile["simpleBytes"] = matrix;
            matfile.WriteToDisk("simpleBytes.mat");
        }

        [Test]
        public void TwoDeeBufferTest()
        {
            var matfile = new DotMatFile();
            byte[] data = { 1, 2, 3, 4, 5, 6, 7, 8 };
            var matrix = new MatlabMatrix(data, new [] { 2, 4 });
            matfile["twodBytes"] = matrix;
            matfile.WriteToDisk("twodBytes.mat");
        }

        [Test]
        public void ThreeDeeBufferTest()
        {
            var matfile = new DotMatFile();
            byte[] data = { 0, 1, 2, 4, 6, 5, 3, 7 };
            var matrix = new MatlabMatrix(data, new [] {2, 2, 2});
            matfile["threedBytes"] = matrix;
            matfile.WriteToDisk("threedBytes.mat");
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ShortBufferTest()
        {
            byte[] data = {1, 2, 3, 4, 5, 6, 7, 8};
            var matrix = new MatlabMatrix(data, new [] {3, 3});
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void LongBufferTest()
        {
            byte[] data = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            var matrix = new MatlabMatrix(data, new [] { 3, 3 });
        }
    }
}
