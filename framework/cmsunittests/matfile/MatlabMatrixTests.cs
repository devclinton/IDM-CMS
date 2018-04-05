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
    class MatlabMatrixTests
    {
        [Test]
        public void DoubleDataTest()
        {
            var matfile = new DotMatFile();
            var matrix = new MatlabMatrix(new[] {1.2, -2.3, 3.4, -4.5, 5.6, -6.7, 7.8, -8.9, 9.10, -10.11});
            matfile["doubleData"] = matrix;
            matfile.WriteToDisk("doubleData.mat");
        }

        [Test]
        public void FloatDataTest()
        {
            var matfile = new DotMatFile();
            var matrix = new MatlabMatrix(new[] { 1.2f, -2.3f, 3.4f, -4.5f, 5.6f, -6.7f, 7.8f, -8.9f, 9.10f, -10.11f });
            matfile["floatData"] = matrix;
            matfile.WriteToDisk("floatData.mat");
        }

        [Test]
        public void ByteDataTest()
        {
            var matfile = new DotMatFile();
            var matrix = new MatlabMatrix(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            matfile["byteData"] = matrix;
            matfile.WriteToDisk("byteData.mat");
        }

        [Test]
        public void SbyteDataTest()
        {
            var matfile = new DotMatFile();
            var matrix = new MatlabMatrix(new sbyte[] { 1, -2, 3, -4, 5, -6, 7, -8, 9, -10 });
            matfile["sbyteData"] = matrix;
            matfile.WriteToDisk("sbyteData.mat");
        }

        [Test]
        public void ShortDataTest()
        {
            var matfile = new DotMatFile();
            var matrix = new MatlabMatrix(new short[] { 1, -2, 3, -4, 5, -6, 7, -8, 9, -10 });
            matfile["shortData"] = matrix;
            matfile.WriteToDisk("shortData.mat");
        }

        [Test]
        public void UshortDataTest()
        {
            var matfile = new DotMatFile();
            var matrix = new MatlabMatrix(new ushort[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            matfile["ushortData"] = matrix;
            matfile.WriteToDisk("ushortData.mat");
        }

        [Test]
        public void IntDataTest()
        {
            var matfile = new DotMatFile();
            var matrix = new MatlabMatrix(new [] { 1, -2, 3, -4, 5, -6, 7, -8, 9, -10 });
            matfile["intData"] = matrix;
            matfile.WriteToDisk("intData.mat");
        }

        [Test]
        public void UintDataTest()
        {
            var matfile = new DotMatFile();
            var matrix = new MatlabMatrix(new uint[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            matfile["uintData"] = matrix;
            matfile.WriteToDisk("uintData.mat");
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void NullBufferTest()
        {
            var matfile = new DotMatFile();
            int[] buffer = null;
            var matrix = new MatlabMatrix(buffer);
            matfile["nullData"] = matrix;
            matfile.WriteToDisk("nullData.mat");
        }
    }
}
