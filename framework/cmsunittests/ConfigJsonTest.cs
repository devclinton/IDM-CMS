using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using compartments;
using NUnit.Framework;

namespace cmsunittests
{
    [TestFixture, Description("JSON format configuration file tests")]
    class ConfigJsonTest : AssertionHelper
    {
        protected Configuration config;
        protected Configuration root;

        public ConfigJsonTest()
        {
            Configuration.CurrentConfiguration = new Configuration("resources\\sample.json");
            config = Configuration.CurrentConfiguration;
            root   = config.Root;
        }

        [Test, Description("Configuration.HasParameter returns true correctly.")]
        public void TestPresenceTrue()
        {
            Expect(config.HasParameter("randomparam") == true);
            Console.WriteLine("PASSED parameter presence test.");
        }

        [Test, Description("Configuration.HasParameter returns false correctly.")]
        public void TestPresenceFalse()
        {
            Expect(config.HasParameter("missingparam") == false);
            Console.WriteLine("PASSED parameter absence test.");
        }

        [Test]
        public void TestString()
        {
            string test = config["stringparam"];
            Expect(test == "toplevelstring");
            Console.WriteLine("PASSED top level string parameter test.");
        }

        [Test]
        public void TestInteger()
        {
            int test = config["integer"];
            Expect(test == 1968);
            Console.WriteLine("PASSED top level integer parameter test.");
        }

        [Test]
        public void TestFloat()
        {
            float test = config["floatingpoint"];
            Expect(test == 3.14159265f);
            Console.WriteLine("PASSED top level floating point parameter test.");
        }

        [Test]
        public void TestScientific()
        {
            float test = config["scientific"];
            Expect(test == 6.02214179e23f);
            Console.WriteLine("PASSED top level scientific notation floating point parameter test.");
        }

        [Test]
        public void TestIntegerArray()
        {
            bool isGood = true;
            int[] expected = new int[] { 0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55 };
            List<Configuration.Parameter> array = config["array"];
            for (int i = 0; i < expected.Length; i++)
            {
                int test = array[i];
                isGood &= (test == expected[i]);
            }
            Expect(isGood);
            Console.WriteLine("PASSED top level integer array test.");
        }

        [Test]
        public void TestBooleanTrue()
        {
            bool test = config["booleant"];
            Expect(test == true);
            Console.WriteLine("PASSED top level 'true' boolean parameter test.");
        }

        [Test]
        public void TestBooleanFalse()
        {
            bool test = config["booleanf"];
            Expect(test == false);
            Console.WriteLine("PASSED top level 'false' boolean parameter test.");
        }

        [Test]
        public void TestNodeString()
        {
            Configuration cfg = config["object"];
            string test = cfg["objstring"];
            Expect(test == "secondlevel");
            Console.WriteLine("PASSED node string parameter test.");
        }

        [Test]
        public void TestNodeInteger()
        {
            Configuration cfg = config["object"];
            int test = cfg["objinteger"];
            Expect(test == 2011);
            Console.WriteLine("PASSED node integer parameter test.");
        }

        [Test]
        public void TestNodeFloat()
        {
            Configuration cfg = config["object"];
            float test = cfg["objfloat"];
            Expect(test == 6.28318531f);
            Console.WriteLine("PASSED node floating point parameter test.");
        }

        [Test]
        public void TestNodeScientific()
        {
            Configuration cfg = config["object"];
            float test = cfg["objscientific"];
            Expect(test == 2.997924580e8f);
            Console.WriteLine("PASSED node scientific notation floating point parameter test.");
        }

        [Test]
        public void TestNodeObjectArray()
        {
            Configuration cfg = config["object"];
            List<Configuration.Parameter> array = cfg["objarray"];
            Expect((string)array[0] == "arraystring");
            Expect((int)array[1] == 2525);
            Expect((float)array[2] == 2.7182818284590452353602874713527f);
            Expect((bool)array[3] == true);
            Console.WriteLine("PASSED node object array test.");
        }

        [Test]
        public void TestDottedParameter()
        {
            bool parameter = config["child.grandchild.parameter"];
            Expect(parameter == true);
            Console.WriteLine("PASSED config[child.grandchild.parameter] = " + parameter.ToString());
        }

        [Test]
        public void TestDottedArrayElement()
        {
            int eleven = config["child.grandchild.array"][4];
            Expect(eleven == 11);
            Console.WriteLine("PASSED config[child.grandchild.array][4] = " + eleven.ToString());
        }

        [Test]
        public void TestSlashedParameter()
        {
            bool parameter = config["child/grandchild/parameter"];
            Expect(parameter == true);
            Console.WriteLine("PASSED config[child/grandchild/parameter] = " + parameter.ToString());
        }

        [Test]
        public void TestSlashedArrayElement()
        {
            int eleven = config["child/grandchild/array"][4];
            Expect(eleven == 11);
            Console.WriteLine("PASSED config[child/grandchild/array][4] = " + eleven.ToString());
        }

        [Test]
        public void TestBackSlashedParameter()
        {
            bool parameter = config[@"child\grandchild\parameter"];
            Expect(parameter == true);
            Console.WriteLine(@"PASSED config[child\grandchild\parameter] = " + parameter.ToString());
        }

        [Test]
        public void TestBackSlashedArrayElement()
        {
            int eleven = config[@"child\grandchild\array"][4];
            Expect(eleven == 11);
            Console.WriteLine(@"PASSED config[child\grandchild\array][4] = " + eleven.ToString());
        }

        [Test]
        public void TestMixedSeparatorParameter()
        {
            bool parameter = config["child.grandchild/parameter"];
            Expect(parameter == true);
            Console.WriteLine("PASSED config[child.grandchild/parameter] = " + parameter.ToString());
        }

        [Test]
        public void TestMixedSeparatorArrayElement()
        {
            int eleven = config[@"child\grandchild.array"][4];
            Expect(eleven == 11);
            Console.WriteLine(@"PASSED config[child\grandchild.array][4] = " + eleven.ToString());
        }

        [Test]
        public void TestRootedParameter()
        {
            bool parameter = config[@"\child\grandchild\parameter"];
            Expect(parameter == true);
            Console.WriteLine(@"PASSED config[\child\grandchild\parameter] = " + parameter.ToString());
        }

        [Test]
        public void TestEmptyConfig()
        {
            Configuration save = Configuration.CurrentConfiguration;
            Configuration.CurrentConfiguration = null;

            Expect(Configuration.CurrentConfiguration.HasParameter("foo") == false);
            Console.WriteLine("PASSED empty configuration HasParameter() call");

            Configuration.CurrentConfiguration = save;
        }

        [Test, Description("Configuration.GetParameterWithDefault(present, bool)")]
        public void TestExistingBooleanParameter()
        {
            Expect(Configuration.CurrentConfiguration.GetParameterWithDefault("booleant", false) == true);
            Console.WriteLine("PASSED GetParameterWithDefault(present, false)");
        }

        [Test, Description("Configuration.GetParameterWithDefault(missing, bool)")]
        public void TestMissingBooleanParameter()
        {
            Expect(Configuration.CurrentConfiguration.GetParameterWithDefault("missing", true) == true);
            Console.WriteLine("PASSED GetParameterWithDefault(missing, true)");
        }

        [Test, Description("Configuration.GetParameterWithDefault(present, integer)")]
        public void TestExistingIntegerParameter()
        {
            Expect(Configuration.CurrentConfiguration.GetParameterWithDefault("integer", 2011) == 1968);
            Console.WriteLine("PASSED GetParameterWithDefault(present, 2011)");
        }

        [Test, Description("Configuration.GetParameterWithDefault(missing, integer)")]
        public void TestMissingIntegerParameter()
        {
            Expect(Configuration.CurrentConfiguration.GetParameterWithDefault("missing", 2011) == 2011);
            Console.WriteLine("PASSED GetParameterWithDefault(missing, 2011)");
        }

        [Test, Description("Configuration.GetParameterWithDefault(present, float)")]
        public void TestExistingFloatParameter()
        {
            Expect(Configuration.CurrentConfiguration.GetParameterWithDefault("floatingpoint", 2.7182817f) == 3.14159265f);
            Console.WriteLine("PASSED GetParameterWithDefault(present, 2.7182817)");
        }

        [Test, Description("Configuration.GetParameterWithDefault(missing, float)")]
        public void TestMissingFloatParameter()
        {
            Expect(Configuration.CurrentConfiguration.GetParameterWithDefault("missing", 2.7182817f) == 2.7182817f);
            Console.WriteLine("PASSED GetParameterWithDefault(missing, 2.7182817)");
        }

        [Test, Description("Configuration.GetParameterWithDefault(present, string)")]
        public void TestExistingStringParameter()
        {
            Expect(Configuration.CurrentConfiguration.GetParameterWithDefault("object.objstring", "Hello, world!") == "secondlevel");
            Console.WriteLine("PASSED GetParameterWithDefault(present, 'Hello, world!')");
        }

        [Test, Description("Configuration.GetParameterWithDefault(missing, string)")]
        public void TestMissingStringParameter()
        {
            Expect(Configuration.CurrentConfiguration.GetParameterWithDefault("missing", "Hello, world!") == "Hello, world!");
            Console.WriteLine("PASSED GetParameterWithDefault(missing, 'Hello, world!')");
        }

        [Test, Description("Configuration.HasParameter returns true correctly.")]
        public void RootTestPresenceTrue()
        {
            Expect(root.HasParameter("randomparam") == true);
            Console.WriteLine("PASSED root parameter presence test.");
        }

        [Test, Description("Configuration.HasParameter returns false correctly.")]
        public void RootTestPresenceFalse()
        {
            Expect(root.HasParameter("missingparam") == false);
            Console.WriteLine("PASSED root parameter absence test.");
        }

        [Test]
        public void RootTestString()
        {
            string test = root["stringparam"];
            Expect(test == "toplevelstring");
            Console.WriteLine("PASSED root top level string parameter test.");
        }

        [Test]
        public void RootTestInteger()
        {
            int test = root["integer"];
            Expect(test == 1968);
            Console.WriteLine("PASSED root top level integer parameter test.");
        }

        [Test]
        public void RootTestFloat()
        {
            float test = root["floatingpoint"];
            Expect(test == 3.14159265f);
            Console.WriteLine("PASSED root top level floating point parameter test.");
        }

        [Test]
        public void RootTestScientific()
        {
            float test = root["scientific"];
            Expect(test == 6.02214179e23f);
            Console.WriteLine("PASSED root top level scientific notation floating point parameter test.");
        }

        [Test]
        public void RootTestIntegerArray()
        {
            bool isGood = true;
            int[] expected = new int[] { 0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55 };
            List<Configuration.Parameter> array = root["array"];
            for (int i = 0; i < expected.Length; i++)
            {
                int test = array[i];
                isGood &= (test == expected[i]);
            }
            Expect(isGood);
            Console.WriteLine("PASSED root top level integer array test.");
        }

        [Test]
        public void RootTestBooleanTrue()
        {
            bool test = root["booleant"];
            Expect(test == true);
            Console.WriteLine("PASSED root top level 'true' boolean parameter test.");
        }

        [Test]
        public void RootTestBooleanFalse()
        {
            bool test = root["booleanf"];
            Expect(test == false);
            Console.WriteLine("PASSED root top level 'false' boolean parameter test.");
        }

        [Test]
        public void RootTestNodeString()
        {
            Configuration cfg = root["object"];
            string test = cfg["objstring"];
            Expect(test == "secondlevel");
            Console.WriteLine("PASSED root node string parameter test.");
        }

        [Test]
        public void RootTestNodeInteger()
        {
            Configuration cfg = root["object"];
            int test = cfg["objinteger"];
            Expect(test == 2011);
            Console.WriteLine("PASSED root node integer parameter test.");
        }

        [Test]
        public void RootTestNodeFloat()
        {
            Configuration cfg = root["object"];
            float test = cfg["objfloat"];
            Expect(test == 6.28318531f);
            Console.WriteLine("PASSED root node floating point parameter test.");
        }

        [Test]
        public void RootTestNodeScientific()
        {
            Configuration cfg = root["object"];
            float test = cfg["objscientific"];
            Expect(test == 2.997924580e8f);
            Console.WriteLine("PASSED root node scientific notation floating point parameter test.");
        }

        [Test]
        public void RootTestNodeObjectArray()
        {
            Configuration cfg = root["object"];
            List<Configuration.Parameter> array = cfg["objarray"];
            Expect((string)array[0] == "arraystring");
            Expect((int)array[1] == 2525);
            Expect((float)array[2] == 2.7182818284590452353602874713527f);
            Expect((bool)array[3] == true);
            Console.WriteLine("PASSED root node object array test.");
        }

        [Test]
        public void RootTestDottedParameter()
        {
            bool parameter = root["child.grandchild.parameter"];
            Expect(parameter == true);
            Console.WriteLine("PASSED root[child.grandchild.parameter] = " + parameter.ToString());
        }

        [Test]
        public void RootTestDottedArrayElement()
        {
            int eleven = root["child.grandchild.array"][4];
            Expect(eleven == 11);
            Console.WriteLine("PASSED root[child.grandchild.array][4] = " + eleven.ToString());
        }

        [Test]
        public void RootTestSlashedParameter()
        {
            bool parameter = root["child/grandchild/parameter"];
            Expect(parameter == true);
            Console.WriteLine("PASSED root[child/grandchild/parameter] = " + parameter.ToString());
        }

        [Test]
        public void RootTestSlashedArrayElement()
        {
            int eleven = root["child/grandchild/array"][4];
            Expect(eleven == 11);
            Console.WriteLine("PASSED root[child/grandchild/array][4] = " + eleven.ToString());
        }

        [Test]
        public void RootTestBackSlashedParameter()
        {
            bool parameter = root[@"child\grandchild\parameter"];
            Expect(parameter == true);
            Console.WriteLine(@"PASSED root[child\grandchild\parameter] = " + parameter.ToString());
        }

        [Test]
        public void RootTestBackSlashedArrayElement()
        {
            int eleven = root[@"child\grandchild\array"][4];
            Expect(eleven == 11);
            Console.WriteLine(@"PASSED root[child\grandchild\array][4] = " + eleven.ToString());
        }

        [Test]
        public void RootTestMixedSeparatorParameter()
        {
            bool parameter = root["child.grandchild/parameter"];
            Expect(parameter == true);
            Console.WriteLine("PASSED root[child.grandchild/parameter] = " + parameter.ToString());
        }

        [Test]
        public void RootTestMixedSeparatorArrayElement()
        {
            int eleven = root[@"child\grandchild.array"][4];
            Expect(eleven == 11);
            Console.WriteLine(@"PASSED root[child\grandchild.array][4] = " + eleven.ToString());
        }

        [Test]
        public void RootTestRootedParameter()
        {
            bool parameter = root[@"\child\grandchild\parameter"];
            Expect(parameter == true);
            Console.WriteLine(@"PASSED root[\child\grandchild\parameter] = " + parameter.ToString());
        }

        [Test, Description("Root.GetParameterWithDefault(present, bool)")]
        public void RootTestExistingBooleanParameter()
        {
            Expect(root.GetParameterWithDefault("booleant", false) == true);
            Console.WriteLine("PASSED root GetParameterWithDefault(present, false)");
        }

        [Test, Description("Root.GetParameterWithDefault(missing, bool)")]
        public void RootTestMissingBooleanParameter()
        {
            Expect(root.GetParameterWithDefault("missing", true) == true);
            Console.WriteLine("PASSED root GetParameterWithDefault(missing, true)");
        }

        [Test, Description("Root.GetParameterWithDefault(present, integer)")]
        public void RootTestExistingIntegerParameter()
        {
            Expect(root.GetParameterWithDefault("integer", 2011) == 1968);
            Console.WriteLine("PASSED root GetParameterWithDefault(present, 2011)");
        }

        [Test, Description("Root.GetParameterWithDefault(missing, integer)")]
        public void RootTestMissingIntegerParameter()
        {
            Expect(root.GetParameterWithDefault("missing", 2011) == 2011);
            Console.WriteLine("PASSED root GetParameterWithDefault(missing, 2011)");
        }

        [Test, Description("Root.GetParameterWithDefault(present, float)")]
        public void RootTestExistingFloatParameter()
        {
            Expect(root.GetParameterWithDefault("floatingpoint", 2.7182817f) == 3.14159265f);
            Console.WriteLine("PASSED root GetParameterWithDefault(present, 2.7182817)");
        }

        [Test, Description("Root.GetParameterWithDefault(missing, float)")]
        public void RootTestMissingFloatParameter()
        {
            Expect(root.GetParameterWithDefault("missing", 2.7182817f) == 2.7182817f);
            Console.WriteLine("PASSED root GetParameterWithDefault(missing, 2.7182817)");
        }

        [Test, Description("Root.GetParameterWithDefault(present, string)")]
        public void RootTestExistingStringParameter()
        {
            Expect(root.GetParameterWithDefault("object.objstring", "Hello, world!") == "secondlevel");
            Console.WriteLine("PASSED root GetParameterWithDefault(present, 'Hello, world!')");
        }

        [Test, Description("Root.GetParameterWithDefault(missing, string)")]
        public void RootTestMissingStringParameter()
        {
            Expect(root.GetParameterWithDefault("missing", "Hello, world!") == "Hello, world!");
            Console.WriteLine("PASSED root GetParameterWithDefault(missing, 'Hello, world!')");
        }
    }
}
