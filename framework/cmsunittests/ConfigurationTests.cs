using System;
using System.Collections.Generic;
using System.IO;
using compartments;
using NUnit.Framework;
using Newtonsoft.Json;

namespace cmsunittests
{
    [TestFixture]
    class ConfigurationTests : AssertionHelper
    {
        [Test, Description("Configuration ctor with invalid JSON")]
        [ExpectedException(typeof(JsonReaderException))]
        public void ConfigurationCtorWithInvalidJson()
        {
            // ReSharper disable UnusedVariable
            var config = new Configuration("resources\\invalid.json");
            // ReSharper restore UnusedVariable
            // Should not reach this line. Previous call is expected to throw an exception.
        }

        [Test, Description("Configuration ctor with missing file")]
        [ExpectedException(typeof(FileNotFoundException))]
        public void ConfigurationCtorMissingFile()
        {
            string tempFileName = Path.GetTempFileName();
            File.Delete(tempFileName);
            // ReSharper disable UnusedVariable
            var config = new Configuration(tempFileName);
            // ReSharper restore UnusedVariable
            // Should not reach this line. Previous call is expected to throw an exception.
        }

        [Test, Description("ConfigurationFromString() with invalid JSON")]
        [ExpectedException(typeof(JsonReaderException))]
        public void ConfigurationFromStringWithInvalidJson()
        {
            // ReSharper disable UnusedVariable
            var config = Configuration.ConfigurationFromString("Move along. Only gobbledy-gook here.");
            // ReSharper restore UnusedVariable
            // Should not reach this line. Previous call is expected to throw an exception.
        }

        [Test, Description("(double []) GetParameterWithDefault()")]
        public void GetDoubleArrayParameterWithDefault()
        {
            var config = Configuration.ConfigurationFromString("{\"array\":[0.0,1.0,2.7182818284590452353602874713527,2.82842712474619,3.14159265]}");
            double[] values = config.GetParameterWithDefault("array", new[] {-1.0, -2.0, -3.0});
            Assert.AreEqual(0.0, values[0]);
            Assert.AreEqual(1.0, values[1]);
            // The current JSON parser reads numeric values as floats.
            Assert.AreEqual((double)2.7182818284590452353602874713527f, values[2]);
            Assert.AreEqual((double)2.82842712474619f, values[3]);
            Assert.AreEqual((double)3.14159265f, values[4]);
        }

        [Test, Description("(double []) GetParameterWithDefault() - use default")]
        public void GetDoubleArrayParameterWithDefaultReturnDefault()
        {
            var config = Configuration.ConfigurationFromString("{\"array\":[0.0,1.0,2.7182818284590452353602874713527,2.82842712474619,3.14159265]}");
            double[] values = config.GetParameterWithDefault("missing", new[] { -1.0, -2.0, -3.0 });
            Assert.AreEqual(-1.0, values[0]);
            Assert.AreEqual(-2.0, values[1]);
            Assert.AreEqual(-3.0, values[2]);
        }

        [Test, Description("(int []) GetParameterWithDefault()")]
        public void GetIntegerArrayParameterWithDefault()
        {
            var config = Configuration.ConfigurationFromString("{\"array\":[0,1,1,2,3,5,8,13,21,34,55]}");
            int[] values = config.GetParameterWithDefault("array", new[] { -1, -2, -3 });
            Assert.AreEqual( 0, values[ 0]);
            Assert.AreEqual( 1, values[ 1]);
            Assert.AreEqual( 1, values[ 2]);
            Assert.AreEqual( 2, values[ 3]);
            Assert.AreEqual( 3, values[ 4]);
            Assert.AreEqual( 5, values[ 5]);
            Assert.AreEqual( 8, values[ 6]);
            Assert.AreEqual(13, values[ 7]);
            Assert.AreEqual(21, values[ 8]);
            Assert.AreEqual(34, values[ 9]);
            Assert.AreEqual(55, values[10]);
        }

        [Test, Description("(int []) GetParameterWithDefault() - use default")]
        public void GetIntegerArrayParameterWithDefaultReturnDefault()
        {
            var config = Configuration.ConfigurationFromString("{\"array\":[0,1,1,2,3,5,8,13,21,34,55]}");
            int[] values = config.GetParameterWithDefault("missing", new[] { -1, -2, -3 });
            Assert.AreEqual(-1, values[0]);
            Assert.AreEqual(-2, values[1]);
            Assert.AreEqual(-3, values[2]);
        }

        [Test, Description("this[string key] method with missing key")]
        [ExpectedException(typeof(ArgumentException))]
        public void TestThisMissingKey()
        {
            var config = Configuration.ConfigurationFromString("{\"string\":\"Hello, World!\",\"number\":42,\"T\":true,\"F\":false,\"empty\":null}");
            int answer = config["number"];
            Assert.AreEqual(42, answer);
            // ReSharper disable UnusedVariable
            double pi = config["pi"];
            // ReSharper restore UnusedVariable
            // Should not reach this line. Previous call is expected to throw an exception.
        }

        [Test, Description("Test Configuration.Parameter.AsConfiguration")]
        public void ConfigurationParameterAsConfiguration()
        {
            var root = Configuration.ConfigurationFromString("{\"object\":{\"greeting\":\"Hello, World!\",\"answer\":42}}");
            Configuration.Parameter parameter = root["object"];
            var config = parameter.AsConfiguration();
            Assert.AreEqual("Hello, World!", (string) (config["greeting"]));
            // ReSharper disable RedundantCast
            Assert.AreEqual(42, (int) (config["answer"]));
            // ReSharper restore RedundantCast
        }

        [Test, Description("Test Configuration.Parameter.AsDouble")]
        public void ConfigurationParameterAsDouble()
        {
            var root = Configuration.ConfigurationFromString("{\"object\":{\"greeting\":\"Hello, World!\",\"answer\":42}}");
            Configuration.Parameter parameter = root["object.answer"];
            var answer = parameter.AsDouble();
            Assert.AreEqual(42.0, answer);
        }

        [Test, Description("Test Configuration.Parameter 'this' method when not an array")]
        [ExpectedException(typeof(ArgumentException))]
        public void ConfigurationParameterThisNotArray()
        {
            var root = Configuration.ConfigurationFromString("{\"object\":{\"greeting\":\"Hello, World!\",\"answer\":42}}");
            Configuration.Parameter parameter = root["object.answer"];
            // ReSharper disable UnusedVariable
            Configuration.Parameter p = parameter[2];
            // ReSharper restore UnusedVariable
            // Should not reach this line. Previous call is expected to throw an exception.
        }

        [Test, Description("Test Configuration.Parameter implicit cast to string when not string")]
        [ExpectedException(typeof(ArgumentException))]
        public void ConfigurationParameterNonStringToString()
        {
            var root = Configuration.ConfigurationFromString("{\"object\":{\"greeting\":\"Hello, World!\",\"answer\":42}}");
            Configuration.Parameter parameter = root["object.answer"];
            // ReSharper disable UnusedVariable
            var greeting = (string) parameter;
            // ReSharper restore UnusedVariable
            // Should not reach this line. Previous call is expected to throw an exception.
        }

        [Test, Description("Test Configuration.Parameter implicit cast to bool when not bool")]
        [ExpectedException(typeof(ArgumentException))]
        public void ConfigurationParameterNonBooleanToBoolean()
        {
            var root = Configuration.ConfigurationFromString("{\"object\":{\"greeting\":\"Hello, World!\",\"answer\":42}}");
            Configuration.Parameter parameter = root["object.answer"];
            // ReSharper disable UnusedVariable
            var greeting = (bool)parameter;
            // ReSharper restore UnusedVariable
            // Should not reach this line. Previous call is expected to throw an exception.
        }

        [Test, Description("Test Configuration.Parameter implicit cast to Configuration when not object")]
        [ExpectedException(typeof(ArgumentException))]
        public void ConfigurationParameterNonObjectToConfiguration()
        {
            var root = Configuration.ConfigurationFromString("{\"object\":{\"greeting\":\"Hello, World!\",\"answer\":42}}");
            Configuration.Parameter parameter = root["object.answer"];
            // ReSharper disable UnusedVariable
            var greeting = (Configuration)parameter;
            // ReSharper restore UnusedVariable
            // Should not reach this line. Previous call is expected to throw an exception.
        }

        [Test, Description("Test Configuration.Parameter implicit cast to List<Parameter> when not array")]
        [ExpectedException(typeof(ArgumentException))]
        public void ConfigurationParameterNonArrayToList()
        {
            var root = Configuration.ConfigurationFromString("{\"object\":{\"greeting\":\"Hello, World!\",\"answer\":42}}");
            Configuration.Parameter parameter = root["object"];
            // ReSharper disable UnusedVariable
            var greeting = (List<Configuration.Parameter>)parameter;
            // ReSharper restore UnusedVariable
            // Should not reach this line. Previous call is expected to throw an exception.
        }

        [Test, Description("Test Configuration.Parameter implicit cast to float when not numeric")]
        [ExpectedException(typeof(ArgumentException))]
        public void ConfigurationParameterNonNumericToFloat()
        {
            var root = Configuration.ConfigurationFromString("{\"object\":{\"greeting\":\"Hello, World!\",\"answer\":42}}");
            Configuration.Parameter parameter = root["object.greeting"];
            // ReSharper disable UnusedVariable
            var greeting = (float)parameter;
            // ReSharper restore UnusedVariable
            // Should not reach this line. Previous call is expected to throw an exception.
        }

        [Test, Description("Test Configuration.Parameter implicit cast to integer when not numeric")]
        [ExpectedException(typeof(ArgumentException))]
        public void ConfigurationParameterNonNumericToInteger()
        {
            var root = Configuration.ConfigurationFromString("{\"object\":{\"greeting\":\"Hello, World!\",\"answer\":42}}");
            Configuration.Parameter parameter = root["object.greeting"];
            // ReSharper disable UnusedVariable
            var greeting = (int)parameter;
            // ReSharper restore UnusedVariable
            // Should not reach this line. Previous call is expected to throw an exception.
        }

        [Test, Description("Test Configuration.Parameter implicit cast to integer from floating point")]
        public void ConfigurationParameterFloatingPointToInteger()
        {
            var root = Configuration.ConfigurationFromString("{\"object\":{\"greeting\":\"Hello, World!\",\"e\":2.7182818284590452353602874713527,\"pi\":3.14159265}}");
            Configuration.Parameter pe = root["object.e"];
            var e = (int)pe;
            Assert.AreEqual(2, e);
            Configuration.Parameter ppi = root["object.pi"];
            var pi = (int)ppi;
            Assert.AreEqual(3, pi);
        }
    }
}
