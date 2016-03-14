using compartments.demographics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using compartments.demographics.elements;

namespace DemographicsTests
{
    /// <summary>
    ///This is a test class for NodeDataTest and is intended
    ///to contain all NodeDataTest Unit Tests
    ///</summary>
    [TestClass]
    public class NodeDataTest
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
        ///A test for NodeData Constructor
        ///</summary>
        [TestMethod]
        [DeploymentItem("compartments.exe")]
        public void NodeDataConstructorTest()
        {
            const int nodeId = 2000;
            DemographicsSet parentSet = DemographicsSet.CreateDemographicsSet();
            parentSet.AddLayer("layer1.compiled.json");
            var target = new NodeData_Accessor(nodeId, parentSet);
            Assert.AreEqual("San Diego, CA", target.GetString("NodeAttributes:Name"));
            Assert.AreEqual(1223400, target.GetInteger("NodeAttributes/InitialPopulation"));
        }

        /// <summary>
        ///A test for CreateNodeData
        ///</summary>
        [TestMethod]
        public void CreateNodeDataTest()
        {
            const int nodeId = 1995;
            DemographicsSet parentSet = DemographicsSet.CreateDemographicsSet();
            parentSet.AddLayer("layer1.compiled.json");
            NodeData actual = NodeData.CreateNodeData(nodeId, parentSet);
            Assert.AreEqual(35.96064, actual.GetDouble("NodeAttributes\\Latitude"));
            Assert.AreEqual(-83.920735, actual.GetDouble("NodeAttributes\\Longitude"));
        }

        /// <summary>
        ///A test for GetDouble
        ///</summary>
        [TestMethod]
        public void GetDoubleTest()
        {
            DemographicsSet set = DemographicsSet.CreateDemographicsSet();
            set.AddLayer("layer1.compiled.json");
            var param0 = new PrivateObject(set.GetNodeData(1970));
            var target = new NodeData_Accessor(param0);
            const string key = "NodeAttributes.Latitude";
            const double expected = 43.038812;
            double actual = target.GetDouble(key);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetFloat
        ///</summary>
        [TestMethod]
        public void GetFloatTest()
        {
            DemographicsSet set = DemographicsSet.CreateDemographicsSet();
            set.AddLayer("layer1.compiled.json");
            var param0 = new PrivateObject(set.GetNodeData(1970));
            var target = new NodeData_Accessor(param0);
            const string key = "NodeAttributes.Longitude";
            const float expected = -87.906824F;
            float actual = target.GetFloat(key);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetInteger
        ///</summary>
        [TestMethod]
        public void GetIntegerTest()
        {
            DemographicsSet set = DemographicsSet.CreateDemographicsSet();
            set.AddLayer("layer1.compiled.json");
            var param0 = new PrivateObject(set.GetNodeData(1970));
            var target = new NodeData_Accessor(param0);
            const string key = "NodeAttributes.InitialPopulation";
            const int expected = 596974;
            int actual = target.GetInteger(key);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetObjectEntry
        ///</summary>
        [TestMethod]
        [DeploymentItem("compartments.exe")]
        public void GetObjectEntryTest()
        {
            IEnumerable<string> keys = new[] { "NodeAttributes", "InitialPopulation" };
            JsonElement obj = new JsonObject();
            ((JsonObject)obj).Add("ab", new JsonObject());
            ((JsonObject)(((JsonObject)obj)["ab"])).Add("ag", new JsonNumber { Value = 596974 });
            IDictionary<string, string> stringTable = new Dictionary<string, string>();
            stringTable.Add("NodeAttributes", "ab");
            stringTable.Add("InitialPopulation", "ag");
            JsonElement actual = NodeData_Accessor.GetObjectEntry(keys, obj, stringTable);
            var population = actual as JsonNumber;
            Assert.IsNotNull(population);
            Assert.AreEqual(596974, population.Value);
        }

        /// <summary>
        ///A test for GetString
        ///</summary>
        [TestMethod]
        public void GetStringTest()
        {
            DemographicsSet set = DemographicsSet.CreateDemographicsSet();
            set.AddLayer("layer1.compiled.json");
            var param0 = new PrivateObject(set.GetNodeData(1970));
            var target = new NodeData_Accessor(param0);
            const string expected = "Milwaukee, WI";
            const string key = "NodeAttributes:Name";
            string actual = target.GetString(key);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetValue
        ///</summary>
        [TestMethod]
        [DeploymentItem("compartments.exe")]
        public void GetValueTest()
        {
            DemographicsSet set = DemographicsSet.CreateDemographicsSet();
            set.AddLayer("layer1.compiled.json");
            set.AddLayer("layer2.compiled.json");
            set.AddLayer("layer3.compiled.json");
            var param0 = new PrivateObject(set.GetNodeData(1970));
            var target = new NodeData_Accessor(param0);
            const string key = "NodeAttributes:Census Date";
            JsonElement actual = target.GetValue(key);
            Assert.IsNotNull(actual);
            Assert.AreEqual(2010, ((JsonNumber) actual).Value);
        }
    }
}
