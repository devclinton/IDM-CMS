using compartments.demographics.elements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DemographicsTests
{
    /// <summary>
    ///This is a test class for JsonArrayTest and is intended
    ///to contain all JsonArrayTest Unit Tests
    ///</summary>
    [TestClass]
    public class JsonArrayTest
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
        ///A test for JsonArray Constructor
        ///</summary>
        [TestMethod]
        public void JsonArrayConstructorTest()
        {
            var target = new JsonArray();
            Assert.AreEqual(0, target.Items.Count);
        }

        /// <summary>
        ///A test for Add
        ///</summary>
        [TestMethod]
        public void AddTest()
        {
            var target = new JsonArray();
            JsonElement element = new JsonNumber {Value = 42};
            target.Add(element);
            Assert.IsNotNull(target.Items[0]);
            Assert.IsNotNull(target.Items[0] as JsonNumber);
            Assert.AreEqual(42, ((JsonNumber)target.Items[0]).Value);
        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod]
        public void ToStringTest()
        {
            var target = new JsonArray();
            target.Add(new JsonArray());
            const bool truth = true;
            target.Add(new JsonBool { Value = truth });
            target.Add(new JsonNull());
            const int date = 1968;
            target.Add(new JsonNumber { Value = date });
            var obj = new JsonObject();
            const int answer = 42;
            obj.Add("key", new JsonNumber { Value = answer });
            target.Add(obj);
            const string greeting = "Hello, World!";
            target.Add(new JsonString { Value = greeting });
            const string expected = "[[],true,null,1968,{\"key\":42},\"Hello,World!\"]";
            var actual = string.Join(null, target.ToString().Split(new[] {' ', '\t', '\n', '\r'}));
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Items
        ///</summary>
        [TestMethod]
        public void ItemsTest()
        {
            var target = new JsonArray();
            target.Add(new JsonArray());
            const bool truth = true;
            target.Add(new JsonBool { Value = truth });
            target.Add(new JsonNull());
            const int date = 1968;
            target.Add(new JsonNumber { Value = date });
            var obj = new JsonObject();
            const int answer = 42;
            obj.Add("key", new JsonNumber { Value = answer });
            target.Add(obj);
            const string greeting = "Hello, World!";
            target.Add(new JsonString { Value = greeting });
            IList<JsonElement> actual = target.Items;
            Assert.IsNotNull(actual[0] as JsonArray);
            Assert.IsTrue(((JsonBool) actual[1]).Value);
            Assert.IsNotNull(actual[2] as JsonNull);
            Assert.AreEqual(date, ((JsonNumber) actual[3]).Value);
            Assert.IsNotNull(actual[4] as JsonObject);
            Assert.AreEqual(greeting, ((JsonString) actual[5]).Value);
        }
    }
}
