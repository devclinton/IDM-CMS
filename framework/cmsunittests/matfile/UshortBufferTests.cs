using System;
using NUnit.Framework;
using matfilelib;

namespace cmsunittests.matfile
{
    [TestFixture]
    class UshortBufferTests
    {
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void NullBufferTest()
        {
            ushort[] data = null;
            var matrix = new MatlabMatrix(data);
        }

        [Test]
        public void EmptyBufferTest()
        {
            var matfile = new DotMatFile();
            var data = new ushort[0];
            var matrix = new MatlabMatrix(data);
            matfile["emptyUshorts"] = matrix;
            matfile.WriteToDisk("emptyUshorts.mat");
        }

        [Test]
        public void SimpleBufferTest()
        {
            var matfile = new DotMatFile();
            var data = new ushort[] { 0, 1, 2, 4, 8, 16, 32, 64, 127, 128 };
            var matrix = new MatlabMatrix(data);
            matfile["simpleUshorts"] = matrix;
            matfile.WriteToDisk("simpleUshorts.mat");
        }

        [Test]
        public void TwoDeeBufferTest()
        {
            var matfile = new DotMatFile();
            var data = new ushort[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var matrix = new MatlabMatrix(data, new[] { 2, 4 });
            matfile["twodUshorts"] = matrix;
            matfile.WriteToDisk("twodUshorts.mat");
        }

        [Test]
        public void ThreeDeeBufferTest()
        {
            var matfile = new DotMatFile();
            var data = new ushort[] { 0, 1, 2, 4, 6, 5, 3, 7 };
            var matrix = new MatlabMatrix(data, new[] { 2, 2, 2 });
            matfile["threedUshorts"] = matrix;
            matfile.WriteToDisk("threedUshorts.mat");
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ShortBufferTest()
        {
            var data = new ushort[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var matrix = new MatlabMatrix(data, new[] { 3, 3 });
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void LongBufferTest()
        {
            var data = new ushort[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            var matrix = new MatlabMatrix(data, new[] { 3, 3 });
        }
    }
}