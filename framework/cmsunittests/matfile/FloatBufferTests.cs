using System;
using NUnit.Framework;
using matfilelib;

namespace cmsunittests.matfile
{
    [TestFixture]
    class FloatBufferTests
    {
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void NullBufferTest()
        {
            float[] data = null;
            var matrix = new MatlabMatrix(data);
        }

        [Test]
        public void EmptyBufferTest()
        {
            var matfile = new DotMatFile();
            var data = new float[0];
            var matrix = new MatlabMatrix(data);
            matfile["emptyFloats"] = matrix;
            matfile.WriteToDisk("emptyFloats.mat");
        }

        [Test]
        public void SimpleBufferTest()
        {
            var matfile = new DotMatFile();
            var data = new[] { 0.1f, 1.2f, 2.3f, 4.5f, 8.9f, 16.17f, 32.33f, 64.65f, 128.129f, 255.256f };
            var matrix = new MatlabMatrix(data);
            matfile["simpleFloats"] = matrix;
            matfile.WriteToDisk("simpleFloats.mat");
        }

        [Test]
        public void TwoDeeBufferTest()
        {
            var matfile = new DotMatFile();
            var data = new[] { 1.1f, 2.2f, 3.3f, 4.4f, 5.5f, 6.6f, 7.7f, 8.8f };
            var matrix = new MatlabMatrix(data, new[] { 2, 4 });
            matfile["twodFloats"] = matrix;
            matfile.WriteToDisk("twodFloats.mat");
        }

        [Test]
        public void ThreeDeeBufferTest()
        {
            var matfile = new DotMatFile();
            var data = new[] { 0.1f, 1.2f, 2.3f, 4.5f, 6.7f, 5.6f, 3.4f, 7.8f };
            var matrix = new MatlabMatrix(data, new[] { 2, 2, 2 });
            matfile["threedFloats"] = matrix;
            matfile.WriteToDisk("threedFloats.mat");
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ShortBufferTest()
        {
            var data = new[] { 1.2f, 2.3f, 3.4f, 4.5f, 5.6f, 6.7f, 7.8f, 8.9f };
            var matrix = new MatlabMatrix(data, new[] { 3, 3 });
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void LongBufferTest()
        {
            var data = new[] { 1.2f, 2.3f, 3.4f, 4.5f, 5.6f, 6.7f, 7.8f, 8.9f, 9.10f, 10.11f, 11.12f, 12.13f, 13.14f, 14.15f, 15.16f };
            var matrix = new MatlabMatrix(data, new[] { 3, 3 });
        }
    }
}