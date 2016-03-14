using System;
using System.IO;
using NUnit.Framework;
using matfilelib;

namespace cmsunittests.matfile
{
    [TestFixture]
    class DotMatFileTests
    {
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void NullFilenameTest()
        {
            var matFile = new DotMatFile();
            matFile.WriteToDisk(null);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void EmptyFilenameTest()
        {
            var matFile = new DotMatFile();
            matFile.WriteToDisk(string.Empty);
        }

        [Test, ExpectedException]
        public void InvalidPathTest()
        {
            var matFile = new DotMatFile();
            matFile.WriteToDisk("q:\\unlikelydirectory\\foo.mat");
        }

        [Test]
        public void UncompressedTest()
        {
            var matFile = new DotMatFile();
            matFile["greeting"] = new MatlabString("Hello, World!");
            var filename = Path.GetRandomFileName() + ".mat";
            matFile.WriteToDisk(filename);
            // File.Delete(filename);
        }

        [Test]
        public void CompressedTest()
        {
            var matFile = new DotMatFile();
            matFile["greeting"] = new MatlabString("Hello, World!");
            var filename = Path.GetRandomFileName() + ".c.mat";
            matFile.WriteToDisk(filename, compressed: true);
            // File.Delete(filename);
        }

        [Test]
        public void EmptyTest()
        {
            var matFile = new DotMatFile();
            var filename = Path.GetRandomFileName() + ".empty.mat";
            matFile.WriteToDisk(filename);
            // File.Delete(filename);
        }
    }
}