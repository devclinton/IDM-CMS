/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System.Linq;
using compartments.demographics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DemographicsTests
{
    /// <summary>
    ///This is a test class for DemographicsSetTest and is intended
    ///to contain all DemographicsSetTest Unit Tests
    ///</summary>
    [TestClass]
    public class DemographicsSetTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///A test for DemographicsSet Constructor
        ///</summary>
        [TestMethod]
        [DeploymentItem("compartments.exe")]
        public void DemographicsSetConstructorTest()
        {
            var target = new DemographicsSet_Accessor();
            Assert.IsNull(target._baseLayer);
            Assert.IsNotNull(target._layers);
            Assert.AreEqual(0, target._layers.Count);
            Assert.IsNotNull(target._cachedNodeData);
            Assert.AreEqual(0, target._cachedNodeData.Count);
        }

        /// <summary>
        ///A test for AddLayer
        ///</summary>
        [TestMethod]
        public void AddLayerTest()
        {
            var target = new DemographicsSet_Accessor();
            const string fileName = "layer1.compiled.json";
            target.AddLayer(fileName);
            Assert.AreEqual(1, target._layers.Count);
            Assert.IsNotNull(target._baseLayer);
            Assert.AreEqual(fileName, target._baseLayer.FileName);
            const string layer2 = "layer2.compiled.json";
            target.AddLayer(layer2);
            Assert.AreEqual(2, target._layers.Count);
            Assert.AreEqual(layer2, target._layers.Peek().FileName);
        }

        /// <summary>
        ///A test for CreateDemographicsSet
        ///</summary>
        [TestMethod]
        public void CreateDemographicsSetTest()
        {
            DemographicsSet actual = DemographicsSet.CreateDemographicsSet();
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for GetNodeData
        ///</summary>
        [TestMethod]
        public void GetNodeDataTest()
        {
            var target = new DemographicsSet_Accessor();
            target.AddLayer("layer1.compiled.json");
            const int nodeId = 1970;
            NodeData actual = target.GetNodeData(nodeId);
            Assert.AreEqual("Milwaukee, WI", actual.GetString("NodeAttributes.Name"));
            Assert.AreEqual(43.038812, actual.GetDouble("NodeAttributes.Latitude"));
            Assert.AreEqual(-87.906824, actual.GetDouble("NodeAttributes.Longitude"));
            Assert.AreEqual(596974, actual.GetInteger("NodeAttributes.InitialPopulation"));
        }

        /// <summary>
        ///A test for Layers
        ///</summary>
        [TestMethod]
        public void LayersTest()
        {
            var target = new DemographicsSet_Accessor();
            Assert.AreEqual(0, target.Layers.Count());
            target.AddLayer("layer1.compiled.json");
            // actual = target.Layers;
            Assert.AreEqual(1, target.Layers.Count());
            DemographicsLayer layer = target.Layers.First();
            Assert.AreEqual("layer1.compiled.json", layer.FileName);
            Assert.IsTrue(layer.ContainsNode(1995));
            IDictionary<string, string> stringTable = layer.StringTable;
            Assert.AreEqual("ah", stringTable["Seaport"]);
            target.AddLayer("layer2.compiled.json");
            Assert.AreEqual(2, target.Layers.Count());
            layer = target.Layers.First();
            Assert.AreEqual("layer2.compiled.json", layer.FileName);
            Assert.IsTrue(layer.ContainsNode(2000));
            stringTable = layer.StringTable;
            Assert.AreEqual("ac", stringTable["InitialPopulation"]);
        }

        /// <summary>
        ///A test for NodeIDs
        ///</summary>
        [TestMethod]
        public void NodeIDsTest()
        {
            var target = new DemographicsSet_Accessor();
            target.AddLayer("layer2.compiled.json");
            IList<int> expected = new List<int>(new[] {1968, 1970, 1995, 2000});
            IList<int> actual = target.NodeIDs;
            Assert.AreEqual(expected.Count, actual.Count);
            foreach (int id in expected)
            {
                Assert.IsTrue(actual.Contains(id));
            }
        }
    }
}
