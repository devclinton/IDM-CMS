/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using compartments.demographics.elements;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemographicsTests
{
    /// <summary>
    ///This is a test class for JsonElementTest and is intended
    ///to contain all JsonElementTest Unit Tests
    ///</summary>
    [TestClass]
    public class JsonElementTest
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

        internal virtual JsonElement CreateJsonElement()
        {
            JsonElement target = new JsonBool { Value = true };
            return target;
        }

        /// <summary>
        ///A test for Parent
        ///</summary>
        [TestMethod]
        public void ParentTest()
        {
            JsonElement target = CreateJsonElement();
            Assert.IsNull(target.Parent);
            JsonElement expected = new JsonObject();
            target.Parent = expected;
            JsonElement actual = target.Parent;
            Assert.AreEqual(expected, actual);
        }
    }
}
