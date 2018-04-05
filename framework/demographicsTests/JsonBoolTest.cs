/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using compartments.demographics.elements;

namespace DemographicsTests
{
    /// <summary>
    ///This is a test class for JsonBoolTest and is intended
    ///to contain all JsonBoolTest Unit Tests
    ///</summary>
    [TestClass]
    public class JsonBoolTest
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
        ///A test for JsonBool Constructor
        ///</summary>
        [TestMethod]
        public void JsonBoolConstructorTest()
        {
            var target = new JsonBool();
            Assert.IsFalse(target.Value);
        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod]
        public void ToStringTest()
        {
            var target = new JsonBool();
            string expected = "false";
            string actual = target.ToString();
            Assert.AreEqual(expected, actual);
            target.Value = true;
            expected = "true";
            actual = target.ToString();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Value
        ///</summary>
        [TestMethod]
        public void ValueTest()
        {
            var target = new JsonBool { Value = false };
            bool actual = target.Value;
            Assert.AreEqual(false, actual);
            target.Value = true;
            actual = target.Value;
            Assert.AreEqual(true, actual);
        }
    }
}
