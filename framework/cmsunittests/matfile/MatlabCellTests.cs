using System.Collections.Generic;
using NUnit.Framework;
using matfilelib;

namespace cmsunittests.matfile
{
    [TestFixture]
    class MatlabCellTests
    {
        [Test]
        public void CellWithMatrixTest()
        {
            var matfile = new DotMatFile();
            var cell = new MatlabCell();
            cell.Contents.Add(new MatlabMatrix(new [] { 2.8284271247461900976, 3.14159265 }));
            matfile["cellMatrix"] = cell;
            matfile.WriteToDisk("cellMatrix.mat");
        }

        [Test]
        public void CellWithCellTest()
        {
            var matfile = new DotMatFile();
            var outerCell = new MatlabCell();
            var innerCell = new MatlabCell();
            innerCell.Contents.Add(new MatlabString("Hello, World!"));
            outerCell.Contents.Add(innerCell);
            matfile["cellCell"] = outerCell;
            matfile.WriteToDisk("cellCell.mat");
        }

        [Test]
        public void CellWithString()
        {
            var matfile = new DotMatFile();
            var cell = new MatlabCell();
            cell.Contents.Add(new MatlabString("She sells seashells by the seashore."));
            matfile["cellString"] = cell;
            matfile.WriteToDisk("cellString.mat");
        }

        [Test]
        public void CellWithStructure()
        {
            var matfile = new DotMatFile();
            var cell = new MatlabCell();
            var mapping = new Dictionary<string, MatrixElement>();
            mapping.Add("bits", new MatlabMatrix(new byte[] { 1, 2, 4, 8, 16, 32, 64, 128 }));
            var names = new MatlabCell(new[] { 8, 1 });
            names.Contents.Add(new MatlabString("one"));
            names.Contents.Add(new MatlabString("two"));
            names.Contents.Add(new MatlabString("four"));
            names.Contents.Add(new MatlabString("eight"));
            names.Contents.Add(new MatlabString("sixteen"));
            names.Contents.Add(new MatlabString("thirty-two"));
            names.Contents.Add(new MatlabString("sixty-four"));
            names.Contents.Add(new MatlabString("one twenty-eight"));
            mapping.Add("names", names);
            var structure = new MatlabStructure(mapping);
            cell.Contents.Add(structure);
            matfile["cellStructure"] = cell;
            matfile.WriteToDisk("cellStructure.mat");
        }

        [Test]
        public void CellWithMixedContents()
        {
            var matfile = new DotMatFile();
            var cell = new MatlabCell(new[] { 4, 1 });
            var innerCell = new MatlabCell();
            innerCell.Contents.Add(new MatlabString("output files are up-to-date"));
            cell.Contents.Add(innerCell);
            cell.Contents.Add(new MatlabMatrix(new[] { 2.8284271247461900976, 3.14159265 }));
            cell.Contents.Add(new MatlabString("5 succeeded, 0 failed, 1 up-to-date, 0 skipped"));
            var mapping = new Dictionary<string, MatrixElement>();
            mapping.Add("bits", new MatlabMatrix(new byte[] { 1, 2, 4, 8, 16, 32, 64, 128 }));
            var names = new MatlabCell(new[] { 8, 1 });
            names.Contents.Add(new MatlabString("one"));
            names.Contents.Add(new MatlabString("two"));
            names.Contents.Add(new MatlabString("four"));
            names.Contents.Add(new MatlabString("eight"));
            names.Contents.Add(new MatlabString("sixteen"));
            names.Contents.Add(new MatlabString("thirty-two"));
            names.Contents.Add(new MatlabString("sixty-four"));
            names.Contents.Add(new MatlabString("one twenty-eight"));
            mapping.Add("names", names);
            var structure = new MatlabStructure(mapping);
            cell.Contents.Add(structure);
            matfile["cellMixed"] = cell;
            matfile.WriteToDisk("cellMixed.mat");
        }
    }
}
