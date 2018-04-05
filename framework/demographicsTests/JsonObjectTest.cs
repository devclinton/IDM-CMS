/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using compartments.demographics.elements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DemographicsTests
{
    /// <summary>
    ///This is a test class for JsonObjectTest and is intended
    ///to contain all JsonObjectTest Unit Tests
    ///</summary>
    [TestClass]
    public class JsonObjectTest
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
        ///A test for JsonObject Constructor
        ///</summary>
        [TestMethod]
        public void JsonObjectConstructorTest()
        {
            var target = new JsonObject();
            Assert.IsNotNull(target.Items);
            Assert.AreEqual(0, target.Items.Count);
        }

        /// <summary>
        ///A test for Add
        ///</summary>
        [TestMethod]
        public void AddTest()
        {
            var target = new JsonObject();
            const string key = "key";
            JsonElement value = new JsonNumber {Value = 42};
            target.Add(key, value);
            Assert.AreEqual(1, target.Items.Count);
            Assert.IsNotNull(target[key] as JsonNumber);
            Assert.AreEqual(42, ((JsonNumber)target[key]).Value);
        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod]
        public void ToStringTest()
        {
            var target = new JsonObject();
            target.Add("array", new JsonArray());
            target.Add("bool", new JsonBool {Value = true});
            target.Add("null", new JsonNull());
            target.Add("number", new JsonNumber {Value = 42});
            target.Add("object", new JsonObject());
            target.Add("string", new JsonString {Value = "Hello, World!"});
            const string expected = "{\"array\":[],\"bool\":true,\"null\":null,\"number\":42,\"object\":{},\"string\":\"Hello,World!\"}";
            var actual = string.Join(null, target.ToString().Split(new[] { ' ', '\t', '\n', '\r' }));
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Item
        ///</summary>
        [TestMethod]
        public void ItemTest()
        {
            var target = new JsonObject();
            const string key = "myKey";
            target.Add(key, new JsonNull());
            var actual = target[key] as JsonNull;
            if (actual != null)
            {
                Assert.AreEqual(null, actual.Value);
            }
            else
            {
                Assert.Fail();
            }
        }

        /// <summary>
        ///A test for Items
        ///</summary>
        [TestMethod]
        public void ItemsTest()
        {
            var target = new JsonObject();
            target.Add("array", new JsonArray());
            target.Add("bool", new JsonBool { Value = true });
            target.Add("null", new JsonNull());
            target.Add("number", new JsonNumber { Value = 42 });
            target.Add("object", new JsonObject());
            target.Add("string", new JsonString { Value = "Hello, World!" });
            IDictionary<string, JsonElement> actual = target.Items;
            Assert.AreEqual(6, actual.Count);
            Assert.IsNotNull(actual["null"] as JsonNull);
            Assert.AreEqual(42.0, ((JsonNumber)actual["number"]).Value);
        }
    }
}
