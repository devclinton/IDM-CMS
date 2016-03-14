using System;
using NUnit.Framework;
using matfilelib;

namespace cmsunittests.matfile
{
    [TestFixture]
    class UintBufferTests
    {
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void NullBufferTest()
        {
            uint[] data = null;
            var matrix = new MatlabMatrix(data);
        }

        [Test]
        public void EmptyBufferTest()
        {
            var matfile = new DotMatFile();
            var data = new uint[0];
            var matrix = new MatlabMatrix(data);
            matfile["emptyUints"] = matrix;
            matfile.WriteToDisk("emptyUints.mat");
        }

        [Test]
        public void SimpleBufferTest()
        {
            var matfile = new DotMatFile();
            var data = new uint[] { 0, 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536, 131072, 262144, 524288, 1048576, 2097152 };
            var matrix = new MatlabMatrix(data);
            matfile["simpleUints"] = matrix;
            matfile.WriteToDisk("simpleUints.mat");
        }

        [Test]
        public void TwoDeeBufferTest()
        {
            var matfile = new DotMatFile();
            var data = new uint[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536, 131072, 262144, 524288, 1048576, 2097152 };
            var matrix = new MatlabMatrix(data, new[] { 2, 11 });
            matfile["twodUints"] = matrix;
            matfile.WriteToDisk("twodUints.mat");
        }

        [Test]
        public void ThreeDeeBufferTest()
        {
            var matfile = new DotMatFile();
            var data = new uint[] { 4000000, 4000001, 4000002, 4000004, 4000006, 4000005, 4000003, 4000007 };
            var matrix = new MatlabMatrix(data, new[] { 2, 2, 2 });
            matfile["threedUints"] = matrix;
            matfile.WriteToDisk("threedUints.mat");
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ShortBufferTest()
        {
            var data = new uint[] { 4000000, 4000001, 4000002, 4000004, 4000006, 4000005, 4000003, 4000007 };
            var matrix = new MatlabMatrix(data, new[] { 3, 3 });
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void LongBufferTest()
        {
            var data = new uint[] { 4000000, 4000001, 4000002, 4000004, 4000006, 4000005, 4000003, 4000007, 4000008, 4000009, 4000010, 4000011, 4000012, 4000013, 4000014, 4000015 };
            var matrix = new MatlabMatrix(data, new[] { 3, 3 });
        }
    }
}