using System;
using System.Collections.Generic;
using NUnit.Framework;
using matfilelib;

namespace cmsunittests.matfile
{
    [TestFixture]
    class MatlabStructureTests
    {
        [Test, ExpectedException(typeof(NullReferenceException))]
        public void NullDictionaryTest()
        {
            var matfile = new DotMatFile();
            var structure = new MatlabStructure(null);
            matfile["structNull"] = structure;
            matfile.WriteToDisk("structNull.mat");
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void EmptyDictionaryTest()
        {
            var matfile = new DotMatFile();
            var empty = new Dictionary<string, MatrixElement>();
            var structure = new MatlabStructure(empty);
            matfile["structEmpty"] = structure;
            matfile.WriteToDisk("structEmpty.mat");
        }

        [Test]
        public void EmptyKeyDictionaryTest()
        {
            var matfile = new DotMatFile();
            var emptyKey = new Dictionary<string, MatrixElement> { { string.Empty, new MatlabString("empty key") } };
            var structure = new MatlabStructure(emptyKey);
            matfile["structEmptyKey"] = structure;
            matfile.WriteToDisk("structEmptyKey.mat");
        }

        [Test]
        public void MatlabStructureTest()
        {
            var matfile = new DotMatFile();
            var mapping = new Dictionary<string, MatrixElement>();
            var cell = new MatlabCell();
            cell.Contents.Add(new MatlabString("cell contents"));
            mapping.Add("cell", cell);
            mapping.Add("matrix", new MatlabMatrix(new [] { 1.1, 2.1, 1.2, 2.2 }, new [] { 2, 2 }));
            mapping.Add("string", new MatlabString("11 dimensional"));
            var innerMapping = new Dictionary<string, MatrixElement>();
            innerMapping.Add("bits", new MatlabMatrix(new [] { 1, 2, 4, 8, 16, 32, 64, 128 }));
            var names = new MatlabCell(new [] { 8, 1 });
            names.Contents.Add(new MatlabString("zero"));
            names.Contents.Add(new MatlabString("one"));
            names.Contents.Add(new MatlabString("two"));
            names.Contents.Add(new MatlabString("three"));
            names.Contents.Add(new MatlabString("four"));
            names.Contents.Add(new MatlabString("five"));
            names.Contents.Add(new MatlabString("size"));
            names.Contents.Add(new MatlabString("seven"));
            innerMapping.Add("names", names);
            var innerStruct = new MatlabStructure(innerMapping);
            mapping.Add("structure", innerStruct);
            var structure = new MatlabStructure(mapping);
            matfile["structure"] = structure;
            matfile.WriteToDisk("structure.mat");
        }
    }
}
