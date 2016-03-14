using System;
using compartments.demographics;
using compartments.demographics.elements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DemographicsTests
{
    /// <summary>
    ///This is a test class for DemographicsLayerTest and is intended
    ///to contain all DemographicsLayerTest Unit Tests
    ///</summary>
    [TestClass]
    public class DemographicsLayerTest
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
        ///A test for DemographicsLayer Constructor
        ///</summary>
        [TestMethod]
        [DeploymentItem("compartments.exe")]
        public void DemographicsLayerConstructorTest()
        {
            const string fileName = "layer1.compiled.json";
            var target = new DemographicsLayer_Accessor(fileName);
            Console.WriteLine("Testing FileName property => {0}...", fileName);
            Assert.AreEqual(fileName, target.FileName);
            const int nodeId = 1968;
            Console.WriteLine("Testing ::ContainsNode({0}) => true...", nodeId);
            Assert.IsTrue(target.ContainsNode(nodeId));
            const int censusDate = 2000;
            Console.WriteLine("Testing Defaults[NodeAttributes:Census Date] => {0}...", censusDate);
            var defaultNodeAttributes = (JsonObject)target.Defaults["ab"];
            Assert.AreEqual(censusDate, ((JsonNumber)defaultNodeAttributes["ai"]).Value);
            const string initialPopulationString = "ag";
            Console.WriteLine("Testing StringTable['InitialPopulation'] => '{0}'...", initialPopulationString);
            Assert.AreEqual(initialPopulationString, target.StringTable["InitialPopulation"]);
        }

        /// <summary>
        ///A test for ContainsNode
        ///</summary>
        [TestMethod]
        public void ContainsNodeTest()
        {
            var param0 = new PrivateObject(DemographicsLayer.CreateDemographicsLayer("layer1.compiled.json"));
            var target = new DemographicsLayer_Accessor(param0);
            const int nodeId = 1970;
            Console.WriteLine("Testing DemographicsLayer.ContainsNode(1970) => true...");
            bool actual = target.ContainsNode(nodeId);
            Assert.AreEqual(true, actual);
            Console.WriteLine("Testing DemographicsLayer.ContainsNode(1904) => false...");
            actual = target.ContainsNode(1904);
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        ///A test for CreateDemographicsLayer
        ///</summary>
        [TestMethod]
        public void CreateDemographicsLayerTest()
        {
            const string fileName = "layer2.compiled.json";
            DemographicsLayer layer = DemographicsLayer.CreateDemographicsLayer(fileName);
            Console.WriteLine("Testing FileName property => {0}...", fileName);
            Assert.AreEqual(fileName, layer.FileName);
            const int nodeId = 1995;
            Console.WriteLine("Testing ::ContainsNode({0}) => true...", nodeId);
            Assert.IsTrue(layer.ContainsNode(nodeId));
            const int censusDate = 2010;
            Console.WriteLine("Testing Defaults[NodeAttributes:Census Date] => {0}...", censusDate);
            var defaultNodeAttributes = (JsonObject)layer.Defaults["ab"];
            Assert.AreEqual(censusDate, ((JsonNumber)defaultNodeAttributes["ad"]).Value);
            const string initialPopulationString = "ac";
            Console.WriteLine("Testing StringTable['InitialPopulation'] => '{0}'...", initialPopulationString);
            Assert.AreEqual(initialPopulationString, layer.StringTable["InitialPopulation"]);
        }

        /// <summary>
        ///A test for GetNode
        ///</summary>
        [TestMethod]
        public void GetNodeTest()
        {
            var param0 = new PrivateObject(DemographicsLayer.CreateDemographicsLayer("layer3.compiled.json"));
            var target = new DemographicsLayer_Accessor(param0);
            const int nodeId = 1995;
            Console.WriteLine("Testing Layer.GetNode({0})...", nodeId);
            JsonObject actual = target.GetNode(nodeId);
            var nodeAttributes = (JsonObject)actual["ab"];
            Assert.AreEqual(35.991274, ((JsonNumber)nodeAttributes["ac"]).Value);
            Assert.AreEqual(-83.92671, ((JsonNumber)nodeAttributes["ad"]).Value);
        }

        /// <summary>
        ///A test for OnElementParsed
        ///</summary>
        [TestMethod]
        [DeploymentItem("compartments.exe")]
        public void OnElementParsedTest()
        {
            var param0 = new PrivateObject(DemographicsLayer.CreateDemographicsLayer("layer3.compiled.json"));
            var target = new DemographicsLayer_Accessor(param0) { _nodeCount = -1 };

            Console.WriteLine("Testing OnElementParsed('Metadata')...");
            var metadata = new JsonObject();
            metadata.Add("NodeCount", new JsonNumber { Value = 2 });
            target.OnElementParsed(null, "Metadata", metadata);
            Assert.AreEqual(2, target._nodeCount);

            Console.WriteLine("Testing OnElementParsed('StringTable')...");
            var stringTable = new JsonObject();
            stringTable.Add("test", new JsonString { Value = "foo" });
            target.OnElementParsed(null, "StringTable", stringTable);
            Assert.AreEqual("foo", target.StringTable["test"]);

            Console.WriteLine("Testing OnElementParsed('NodeOffsets')...");
            var nodeOffsets = new JsonString {Value = "00000001000001000000000200000200"};
            target.OnElementParsed(null, "NodeOffsets", nodeOffsets);
            Assert.IsTrue(target.ContainsNode(2));
        }

        /// <summary>
        ///A test for ProcessNodeOffsets
        ///</summary>
        [TestMethod]
        [DeploymentItem("compartments.exe")]
        public void ProcessNodeOffsetsTest()
        {
            var param0 = new PrivateObject(DemographicsLayer.CreateDemographicsLayer("layer3.compiled.json"));
            var target = new DemographicsLayer_Accessor(param0);
            const string offsetData = "0000000D000001000000006400000200";
            target.ProcessNodeOffsets(offsetData);
            int nodeId = 13;
            Console.WriteLine("Checking NodeContains({0}) => true...", nodeId);
            Assert.IsTrue(target.ContainsNode(nodeId));
            nodeId = 100;
            Console.WriteLine("Checking _nodeOffset[{0}] => 512 (0x200)...", nodeId);
            Assert.AreEqual((uint)512, target._nodeOffsets[nodeId]);
        }

        /// <summary>
        ///A test for ProcessStringTable
        ///</summary>
        [TestMethod]
        [DeploymentItem("compartments.exe")]
        public void ProcessStringTableTest()
        {
            var param0 = new PrivateObject(DemographicsLayer.CreateDemographicsLayer("layer3.compiled.json"));
            var target = new DemographicsLayer_Accessor(param0);
            var stringTable = new JsonObject();
            stringTable.Add("Christopher", new JsonString { Value = "Lorton" });
            stringTable.Add("Juli Anna", new JsonString { Value = "Swinnerton" });
            target.ProcessStringTable(stringTable);
            Assert.AreEqual("Lorton", target.StringTable["Christopher"]);
            Assert.AreEqual("Swinnerton", target.StringTable["Juli Anna"]);
        }

        /// <summary>
        ///A test for Defaults
        ///</summary>
        [TestMethod]
        [DeploymentItem("compartments.exe")]
        public void DefaultsTest()
        {
            var param0 = new PrivateObject(DemographicsLayer.CreateDemographicsLayer("layer3.compiled.json"));
            var target = new DemographicsLayer_Accessor(param0);
            var expected = new JsonObject();
            target.Defaults = expected;
            JsonObject actual = target.Defaults;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for FileName
        ///</summary>
        [TestMethod]
        [DeploymentItem("compartments.exe")]
        public void FileNameTest()
        {
            const string expected = "layer3.compiled.json";
            var param0 = new PrivateObject(DemographicsLayer.CreateDemographicsLayer(expected));
            var target = new DemographicsLayer_Accessor(param0) { FileName = expected };
            string actual = target.FileName;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for StringTable
        ///</summary>
        [TestMethod]
        [DeploymentItem("compartments.exe")]
        public void StringTableTest()
        {
            var param0 = new PrivateObject(DemographicsLayer.CreateDemographicsLayer("layer3.compiled.json"));
            var target = new DemographicsLayer_Accessor(param0);
            IDictionary<string, string> expected = new Dictionary<string, string>(4);
            expected.Add("NodeID", "aa");
            expected.Add("NodeAttributes", "ab");
            expected.Add("Latitude", "ac");
            expected.Add("Longitude", "ad");
            target.StringTable = expected;
            IDictionary<string, string> actual = target.StringTable;
            Assert.AreEqual(expected.Keys.Count, actual.Keys.Count);
            foreach (string key in expected.Keys)
            {
                Assert.AreEqual(expected[key], actual[key]);
            }
        }

        /// <summary>
        ///A test for NodeIDs
        ///</summary>
        [TestMethod]
        public void NodeIDsTest()
        {
            var param0 = new PrivateObject(DemographicsLayer.CreateDemographicsLayer("layer1.compiled.json"));
            var target = new DemographicsLayer_Accessor(param0);
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
