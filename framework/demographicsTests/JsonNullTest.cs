using Microsoft.VisualStudio.TestTools.UnitTesting;
using compartments.demographics.elements;

namespace DemographicsTests
{
    /// <summary>
    ///This is a test class for JsonNullTest and is intended
    ///to contain all JsonNullTest Unit Tests
    ///</summary>
    [TestClass]
    public class JsonNullTest
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
        ///A test for JsonNull Constructor
        ///</summary>
        [TestMethod]
        public void JsonNullConstructorTest()
        {
            var target = new JsonNull();
            Assert.IsNull(target.Value);
            Assert.AreEqual("null", target.ToString());
        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod]
        public void ToStringTest()
        {
            var target = new JsonNull();
            const string expected = "null";
            string actual = target.ToString();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Value
        ///</summary>
        [TestMethod]
        public void ValueTest()
        {
            var target = new JsonNull();
            Assert.IsNull(target.Value);
        }
    }
}
