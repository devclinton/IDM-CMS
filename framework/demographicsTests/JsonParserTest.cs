using compartments.demographics;
using compartments.demographics.elements;
using compartments.demographics.interfaces;
using compartments.demographics.sources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemographicsTests
{
    /// <summary>
    ///This is a test class for JsonParserTest and is intended
    ///to contain all JsonParserTest Unit Tests
    ///</summary>
    [TestClass]
    public class JsonParserTest
    {
        private bool _elementParsed;

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
        ///A test for JsonParser Constructor
        ///</summary>
        [TestMethod]
        [DeploymentItem("compartments.exe")]
        public void JsonParserConstructorTest()
        {
            ICharSource source = new StringCharSource("{\"array\":[[],false,null,2.718281828,{},\"foo\"],\"bool\":true,\"null\":null,\"number\":2.828427125,\"object\":{\"array\":[[],false,null,2.718281828,{},\"foo\"],\"bool\":true,\"null\":null,\"number\":2.828427125,\"object\":{},\"string\":\"Hello, World!\"},\"string\":\"Hello, World!\"}");
            var target = new JsonParser_Accessor(source);
            Assert.AreEqual(source, target.CharSource);
            var root = target.ParseElement() as JsonObject;
            Assert.IsNotNull(root);
            Assert.IsNotNull(root["array"]);
            Assert.IsNotNull(root["string"]);
            Assert.AreEqual("Hello, World!", ((JsonString) root["string"]).Value);
        }

        /// <summary>
        ///A test for CreateJsonParser
        ///</summary>
        [TestMethod]
        public void CreateJsonParserTest()
        {
            ICharSource source = new StringCharSource("{\"array\":[[],false,null,2.718281828,{},\"foo\"],\"bool\":true,\"null\":null,\"number\":2.828427125,\"object\":{\"array\":[[],false,null,2.718281828,{},\"foo\"],\"bool\":true,\"null\":null,\"number\":2.828427125,\"object\":{},\"string\":\"Hello, World!\"},\"string\":\"Hello, World!\"}");
            JsonParser actual = JsonParser.CreateJsonParser(source);
            Assert.AreSame(source, actual.CharSource);
            var root = actual.ParseElement() as JsonObject;
            Assert.IsNotNull(root);
            Assert.IsNotNull(root["bool"]);
            Assert.IsNotNull(root["object"]);
            Assert.AreEqual(2.828427125, ((JsonNumber)root["number"]).Value);
        }

        /// <summary>
        ///A test for OnElementParsed
        ///</summary>
        [TestMethod]
        [DeploymentItem("compartments.exe")]
        public void OnElementParsedTest()
        {
            ICharSource source = new StringCharSource("");
            var param0 = new PrivateObject(JsonParser.CreateJsonParser(source));
            var target = new JsonParser_Accessor(param0);
            const string name = "Metadata";
            JsonElement element = new JsonObject();
            target.add_ElementParsed(HandleElementParsed);
            target.OnElementParsed(name, element);
            Assert.IsTrue(_elementParsed);
        }

        private void HandleElementParsed(object sender, string name, JsonElement jsonElement)
        {
            if (string.Equals(name, "Metadata") && ((jsonElement as JsonObject) != null))
            {
                _elementParsed = true;
            }
        }

        /// <summary>
        ///A test for ParseArray
        ///</summary>
        [TestMethod]
        [DeploymentItem("compartments.exe")]
        public void ParseArrayTest()
        {
            ICharSource source = new StringCharSource("[0,1,2,3,4,5,6,7,8,9]");
            var param0 = new PrivateObject(JsonParser.CreateJsonParser(source));
            var target = new JsonParser_Accessor(param0);
            JsonElement actual = target.ParseArray();
            var array = actual as JsonArray;
            Assert.IsNotNull(array);
            Assert.AreEqual(10, array.Items.Count);
            Assert.AreEqual(1.0, ((JsonNumber)array[1]).Value);
            Assert.AreEqual(9.0, ((JsonNumber)array[9]).Value);
        }

        /// <summary>
        ///A test for ParseElement
        ///</summary>
        [TestMethod]
        public void ParseElementTest()
        {
            ICharSource source = new StringCharSource("{\"array\":[],\"bool\":true,\"null\":null,\"number\":2012,\"object\":{},\"string\":\"Hello, World!\"}");
            var param0 = new PrivateObject(JsonParser.CreateJsonParser(source));
            var target = new JsonParser_Accessor(param0);
            JsonElement actual = target.ParseElement();
            var obj = actual as JsonObject;
            Assert.IsNotNull(obj);
            Assert.AreEqual(6, obj.Items.Count);
            Assert.IsNotNull(obj["bool"]);
            Assert.AreEqual("Hello, World!", ((JsonString) obj["string"]).Value);

            source = new StringCharSource("[[],true,null,2012,{},\"Hello, World!\"]");
            param0 = new PrivateObject(JsonParser.CreateJsonParser(source));
            target = new JsonParser_Accessor(param0);
            actual = target.ParseElement();
            var array = actual as JsonArray;
            Assert.IsNotNull(array);
            Assert.AreEqual(6, array.Items.Count);
            Assert.IsNotNull(array[2]);
            Assert.AreEqual(2012, ((JsonNumber)array[3]).Value);
        }

        /// <summary>
        ///A test for ParseKeyword
        ///</summary>
        [TestMethod]
        [DeploymentItem("compartments.exe")]
        public void ParseKeywordTest()
        {
            ICharSource source = new StringCharSource("true,");
            var param0 = new PrivateObject(JsonParser.CreateJsonParser(source));
            var target = new JsonParser_Accessor(param0);
            JsonElement actual = target.ParseKeyword();
            Assert.IsNotNull(actual as JsonBool);
            Assert.IsTrue(((JsonBool) actual).Value);

            source = new StringCharSource("false}");
            param0 = new PrivateObject(JsonParser.CreateJsonParser(source));
            target = new JsonParser_Accessor(param0);
            actual = target.ParseKeyword();
            Assert.IsNotNull(actual as JsonBool);
            Assert.IsFalse(((JsonBool)actual).Value);

            source = new StringCharSource("null]");
            param0 = new PrivateObject(JsonParser.CreateJsonParser(source));
            target = new JsonParser_Accessor(param0);
            actual = target.ParseKeyword();
            Assert.IsNotNull(actual as JsonNull);
            Assert.IsNull(((JsonNull)actual).Value);
        }

        /// <summary>
        ///A test for ParseNumber
        ///</summary>
        [TestMethod]
        [DeploymentItem("compartments.exe")]
        public void ParseNumberTest()
        {
            ICharSource source = new StringCharSource("6.02214e23}");
            var param0 = new PrivateObject(JsonParser.CreateJsonParser(source));
            var target = new JsonParser_Accessor(param0);
            JsonElement actual = target.ParseNumber();
            Assert.IsNotNull(actual as JsonNumber);
            Assert.AreEqual(6.02214e23, ((JsonNumber) actual).Value);
        }

        /// <summary>
        ///A test for ParseObject
        ///</summary>
        [TestMethod]
        [DeploymentItem("compartments.exe")]
        public void ParseObjectTest()
        {
            ICharSource source = new StringCharSource("{\"array\":[],\"bool\":true,\"null\":null,\"number\":2012,\"object\":{},\"string\":\"Hello, World!\"}");
            var param0 = new PrivateObject(JsonParser.CreateJsonParser(source));
            var target = new JsonParser_Accessor(param0);

            JsonElement actual = target.ParseObject();
            var obj = actual as JsonObject;
            Assert.IsNotNull(obj);
            Assert.AreEqual(6, obj.Items.Count);
            Assert.IsNotNull(obj["number"]);
            Assert.AreEqual(2012, ((JsonNumber) obj["number"]).Value);
            Assert.IsNotNull(obj["object"] as JsonObject);
        }

        /// <summary>
        ///A test for ParseString
        ///</summary>
        [TestMethod]
        [DeploymentItem("compartments.exe")]
        public void ParseStringTest()
        {

            ICharSource source = new StringCharSource(" \n\"Escaped characters: \\\"\\\\\\/\\b\\f\\n\\r\\t\\u03C0\"");
            var param0 = new PrivateObject(JsonParser.CreateJsonParser(source));
            var target = new JsonParser_Accessor(param0);

            JsonString actual = target.ParseString();
            Assert.IsNotNull(actual);
            Assert.AreEqual("Escaped characters: \"\\/\b\f\n\r\t\u03C0", actual.Value);
        }

        /// <summary>
        ///A test for ParseValue
        ///</summary>
        [TestMethod]
        [DeploymentItem("compartments.exe")]
        public void ParseValueTest()
        {
            ICharSource source = new StringCharSource(" \n\"Escaped characters: \\\"\\\\\\/\\b\\f\\n\\r\\t\\u03C0\"");
            var param0 = new PrivateObject(JsonParser.CreateJsonParser(source));
            var target = new JsonParser_Accessor(param0);
            JsonElement actual = target.ParseValue();
            var str = actual as JsonString;
            Assert.IsNotNull(str);
            Assert.AreEqual("Escaped characters: \"\\/\b\f\n\r\t\u03C0", str.Value);

            source = new StringCharSource(" \n{\"key\":\"hole\"}");
            param0 = new PrivateObject(JsonParser.CreateJsonParser(source));
            target = new JsonParser_Accessor(param0);
            actual = target.ParseValue();
            var obj = actual as JsonObject;
            Assert.IsNotNull(obj);
            Assert.AreEqual(1, obj.Items.Count);
            Assert.IsNotNull(obj["key"]);
            Assert.IsNotNull(obj["key"] as JsonString);
            Assert.AreEqual("hole", ((JsonString) obj["key"]).Value);

            source = new StringCharSource(" \t[\"Hello!\",{},null]");
            param0 = new PrivateObject(JsonParser.CreateJsonParser(source));
            target = new JsonParser_Accessor(param0);
            actual = target.ParseValue();
            var array = actual as JsonArray;
            Assert.IsNotNull(array);
            Assert.AreEqual(3, array.Items.Count);
            Assert.IsNotNull(array[1] as JsonObject);
            Assert.AreEqual("Hello!", ((JsonString) array[0]).Value);

            source = new StringCharSource("true]");
            param0 = new PrivateObject(JsonParser.CreateJsonParser(source));
            target = new JsonParser_Accessor(param0);
            actual = target.ParseValue();
            var truth = actual as JsonBool;
            Assert.IsNotNull(truth);
            Assert.IsTrue(truth.Value);

            source = new StringCharSource("\tfalse}");
            param0 = new PrivateObject(JsonParser.CreateJsonParser(source));
            target = new JsonParser_Accessor(param0);
            actual = target.ParseValue();
            var negative = actual as JsonBool;
            Assert.IsNotNull(negative);
            Assert.IsFalse(negative.Value);

            source = new StringCharSource("\nnull,");
            param0 = new PrivateObject(JsonParser.CreateJsonParser(source));
            target = new JsonParser_Accessor(param0);
            actual = target.ParseValue();
            var nil = actual as JsonNull;
            Assert.IsNotNull(nil);

            source = new StringCharSource(" -2}");
            param0 = new PrivateObject(JsonParser.CreateJsonParser(source));
            target = new JsonParser_Accessor(param0);
            actual = target.ParseValue();
            var number = actual as JsonNumber;
            Assert.IsNotNull(number);
            Assert.AreEqual(-2.0, number.Value);
        }

        /// <summary>
        ///A test for SkipWhitespace
        ///</summary>
        [TestMethod]
        [DeploymentItem("compartments.exe")]
        public void SkipWhitespaceTest()
        {
            ICharSource source = new StringCharSource(" \n\"Escaped characters: \\\"\\\\\\/\\b\\f\\n\\r\\t\\u03C0\"");
            var param0 = new PrivateObject(JsonParser.CreateJsonParser(source));
            var target = new JsonParser_Accessor(param0);
            target.SkipWhitespace();
            Assert.AreEqual('\"', target._source.Next());
            Assert.AreEqual('E', target._source.Current);
        }

        /// <summary>
        ///A test for CharSource
        ///</summary>
        [TestMethod]
        public void CharSourceTest()
        {
            ICharSource source = new StringCharSource(" \n\"Escaped characters: \\\"\\\\\\/\\b\\f\\n\\r\\t\\u03C0\"");
            var param0 = new PrivateObject(JsonParser.CreateJsonParser(source));
            var target = new JsonParser_Accessor(param0);

            ICharSource actual = target.CharSource;
            while (!actual.EndOfFile)
            {
                Assert.AreEqual(source.Current, actual.Next());
            }
        }

        /// <summary>
        ///A test for Terminate
        ///</summary>
        [TestMethod]
        public void TerminateTest()
        {
            ICharSource source = new StringCharSource(" \n\"Escaped characters: \\\"\\\\\\/\\b\\f\\n\\r\\t\\u03C0\"");
            var param0 = new PrivateObject(JsonParser.CreateJsonParser(source));
            var target = new JsonParser_Accessor(param0);
            Assert.IsFalse(target.Terminate);
            const bool expected = true;
            target.Terminate = expected;
            bool actual = target.Terminate;
            Assert.AreEqual(expected, actual);
        }
    }
}
