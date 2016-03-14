using System;
using NUnit.Framework;
using matfilelib;

namespace cmsunittests.matfile
{
    [TestFixture]
    class DoubleBufferTests
    {
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void NullBufferTest()
        {
            double[] data = null;
            var matrix = new MatlabMatrix(data);
        }

        [Test]
        public void EmptyBufferTest()
        {
            var matfile = new DotMatFile();
            var data = new double[0];
            var matrix = new MatlabMatrix(data);
            matfile["emptyDoubles"] = matrix;
            matfile.WriteToDisk("emptyDoubles.mat");
        }

        [Test]
        public void SimpleBufferTest()
        {
            var matfile = new DotMatFile();
            var data = new [] { 0.1, 1.2, 2.3, 4.5, 8.9, 16.17, 32.33, 64.65, 128.129, 255.256 };
            var matrix = new MatlabMatrix(data);
            matfile["simpleDoubles"] = matrix;
            matfile.WriteToDisk("simpleDoubles.mat");
        }

        [Test]
        public void TwoDeeBufferTest()
        {
            var matfile = new DotMatFile();
            var data = new [] { 1.1, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7, 8.8 };
            var matrix = new MatlabMatrix(data, new [] { 2, 4 });
            matfile["twodDoubles"] = matrix;
            matfile.WriteToDisk("twodDoubles.mat");
        }

        [Test]
        public void ThreeDeeBufferTest()
        {
            var matfile = new DotMatFile();
            var data = new [] { 0.1, 1.2, 2.3, 4.5, 6.7, 5.6, 3.4, 7.8 };
            var matrix = new MatlabMatrix(data, new [] { 2, 2, 2 });
            matfile["threedDoubles"] = matrix;
            matfile.WriteToDisk("threedDoubles.mat");
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ShortBufferTest()
        {
            var data = new [] { 1.2, 2.3, 3.4, 4.5, 5.6, 6.7, 7.8, 8.9 };
            var matrix = new MatlabMatrix(data, new [] { 3, 3 });
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void LongBufferTest()
        {
            var data = new[] { 1.2, 2.3, 3.4, 4.5, 5.6, 6.7, 7.8, 8.9, 9.10, 10.11, 11.12, 12.13, 13.14, 14.15, 15.16 };
            var matrix = new MatlabMatrix(data, new [] { 3, 3 });
        }
    }
}