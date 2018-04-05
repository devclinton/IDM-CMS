/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using compartments.demographics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemographicsTests
{
    [TestClass]
    public class SystemTests
    {
        [TestMethod]
        public void OneLayerNodeTest()
        {
            DemographicsSet set = DemographicsSet.CreateDemographicsSet();
            set.AddLayer("layer1.compiled.json");

            NodeData node = set.GetNodeData(1968);
            Assert.AreEqual("Radford, VA", node.GetString("NodeAttributes:Name"));
            Assert.AreEqual(37.131796, node.GetDouble("NodeAttributes:Latitude"));
            Assert.AreEqual(-80.576449, node.GetDouble("NodeAttributes:Longitude"));
            Assert.AreEqual(15859, node.GetInteger("NodeAttributes\\InitialPopulation"));
        }

        [TestMethod]
        public void OneLayerDefaultsTest()
        {
            DemographicsSet set = DemographicsSet.CreateDemographicsSet();
            set.AddLayer("layer1.compiled.json");

            NodeData node = set.GetNodeData(1995);
            Assert.AreEqual(2000, node.GetInteger("NodeAttributes.Census Date"));
        }

        [TestMethod]
        public void TwoLayerNodeTest()
        {
            DemographicsSet set = DemographicsSet.CreateDemographicsSet();
            set.AddLayer("layer1.compiled.json");
            set.AddLayer("layer2.compiled.json");

            NodeData node = set.GetNodeData(1970);
            Assert.AreEqual("Milwaukee, WI", node.GetString("NodeAttributes/Name"));
            Assert.AreEqual(43.038812, node.GetDouble("NodeAttributes/Latitude"));
            Assert.AreEqual(-87.906824, node.GetDouble("NodeAttributes/Longitude"));
            Assert.AreEqual(594833, node.GetInteger("NodeAttributes/InitialPopulation"));
        }

        [TestMethod]
        public void TwoLayerDefaultsTest()
        {
            DemographicsSet set = DemographicsSet.CreateDemographicsSet();
            set.AddLayer("layer1.compiled.json");
            set.AddLayer("layer2.compiled.json");

            NodeData node = set.GetNodeData(1968);
            Assert.AreEqual(2010, node.GetInteger("NodeAttributes.Census Date"));
            Assert.AreEqual(0, node.GetInteger("NodeAttributes.Seaport"));
        }

        [TestMethod]
        public void ThreeLayerNodeTest()
        {
            DemographicsSet set = DemographicsSet.CreateDemographicsSet();
            set.AddLayer("layer1.compiled.json");
            set.AddLayer("layer2.compiled.json");
            set.AddLayer("layer3.compiled.json");

            NodeData node = set.GetNodeData(2000);
            Assert.AreEqual("San Diego, CA", node.GetString("NodeAttributes/Name"));
            Assert.AreEqual(32.884484, node.GetDouble("NodeAttributes/Latitude"));
            Assert.AreEqual(-117.225260, node.GetDouble("NodeAttributes/Longitude"));
            Assert.AreEqual(1307402, node.GetInteger("NodeAttributes/InitialPopulation"));

        }

        [TestMethod]
        public void ThreeLayerDefaultsTest()
        {
            DemographicsSet set = DemographicsSet.CreateDemographicsSet();
            set.AddLayer("layer1.compiled.json");
            set.AddLayer("layer2.compiled.json");
            set.AddLayer("layer3.compiled.json");

            NodeData node = set.GetNodeData(1995);
            Assert.AreEqual(0, node.GetInteger("NodeAttributes.Seaport"));          // layer 1
            Assert.AreEqual(2010, node.GetInteger("NodeAttributes.Census Date"));   // layer 2
            Assert.AreEqual(35.991274, node.GetDouble("NodeAttributes.Latitude"));  // layer 3
        }

        // JSON file missing the "Metadata" object entry (including the crucial "NodeCount")
        [TestMethod]
        [ExpectedException(typeof(System.ApplicationException))]
        public void MissingMetadataTest()
        {
            DemographicsSet set = DemographicsSet.CreateDemographicsSet();
            set.AddLayer("nometadata.json");
        }

        // JSON file missing the "StringTable" object entry (necessary for decoding compressed key names)
        [TestMethod]
        [ExpectedException(typeof(System.ApplicationException))]
        public void MissingStringTableTest()
        {
            DemographicsSet set = DemographicsSet.CreateDemographicsSet();
            set.AddLayer("nostringtable.json");
        }

        // JSON file missing the "NodeOffsets" entry (necessary for seeking to node entries in the file)
        [TestMethod]
        [ExpectedException(typeof(System.ApplicationException))]
        public void MissingNodeOffsetsTest()
        {
            DemographicsSet set = DemographicsSet.CreateDemographicsSet();
            set.AddLayer("nooffsets.json");
        }

        // JSON file with malformed syntax in the "header" (Metadata/StringTable/NodeOffsets)
        [TestMethod]
        [ExpectedException(typeof(System.Exception))]
        public void MalformedHeaderTest()
        {
            DemographicsSet set = DemographicsSet.CreateDemographicsSet();
            set.AddLayer("badheader.json");
        }

        // JSON file with malformed syntax in a node entry
        [TestMethod]
        [ExpectedException(typeof(System.Exception))]
        public void MalformedNodeTest()
        {
            DemographicsSet set = DemographicsSet.CreateDemographicsSet();
            set.AddLayer("badnode.json");
            NodeData node = set.GetNodeData(2000);
            Assert.AreEqual("San Diego, CA", node.GetString("NodeAttributes:Name"));
        }

        // JSON file with too few nodes in the NodeOffset entry
        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void NodeCountNodeOffsetsMismatchTest1()
        {
            DemographicsSet set = DemographicsSet.CreateDemographicsSet();
            set.AddLayer("missingoffset.json");
        }

        // JSON file with too many nodes in the NodeOffset entry
        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void NodeCountNodeOffsetsMismatchTest2()
        {
            DemographicsSet set = DemographicsSet.CreateDemographicsSet();
            set.AddLayer("extraoffset.json");
            NodeData node = set.GetNodeData(1);
            Assert.AreEqual(1, node.GetInteger("NodeID"));
        }

        // JSON file missing the final node entry
        [TestMethod]
        [ExpectedException(typeof(System.Exception))]
        public void MissingNodeDataTest()
        {
            DemographicsSet set = DemographicsSet.CreateDemographicsSet();
            set.AddLayer("missingnode.json");
            NodeData node = set.GetNodeData(2000);
            Assert.AreEqual("San Diego, CA", node.GetString("NodeAttributes:Name"));
        }

        // Correct handling of missing values (key)
        [TestMethod]
        [ExpectedException(typeof(System.NullReferenceException))]
        public void MissingValueTest()
        {
            DemographicsSet set = DemographicsSet.CreateDemographicsSet();
            set.AddLayer("layer1.compiled.json");
            NodeData node = set.GetNodeData(1968);
            int hospitalCount = node.GetInteger("NodeAttributes:HospitalCount");
            Assert.AreEqual(1, hospitalCount);
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidCastException))]
        public void IncorrectDataTypeTest1()
        {
            DemographicsSet set = DemographicsSet.CreateDemographicsSet();
            set.AddLayer("layer1.compiled.json");
            NodeData node = set.GetNodeData(1968);
            string name = node.GetString("NodeAttributes:Latitude");
            Assert.AreEqual("Radford, VA", name);
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidCastException))]
        public void IncorrectDataTypeTest2()
        {
            DemographicsSet set = DemographicsSet.CreateDemographicsSet();
            set.AddLayer("layer1.compiled.json");
            NodeData node = set.GetNodeData(1968);
            int population = node.GetInteger("NodeAttributes:Name");
            Assert.AreEqual(population, 1000000);
        }
    }
}
