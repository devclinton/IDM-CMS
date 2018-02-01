using System;
using System.Collections.Generic;
using compartments.CommandLine;
using NUnit.Framework;
// ReSharper disable UnusedVariable

namespace cmsunittests
{
    [TestFixture]
    class CommandLineTest : AssertionHelper
    {
        readonly string[] _typeStrings;
        readonly List<CommandLineParameters.OptionInfo> _typeOptions;

        public CommandLineTest()
        {
            _typeStrings = new[] { "--boolean", "--string", "astring", "--numeric", "1984" };
            _typeOptions = new List<CommandLineParameters.OptionInfo>(new[] {
                new CommandLineParameters.OptionInfo("boolean|b", "Boolean switch.", typeof(bool)),
                new CommandLineParameters.OptionInfo("string|s", "String parameter", typeof(string)),
                new CommandLineParameters.OptionInfo("numeric|n", "Numeric parameter", typeof(int))
            });
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "isn't of type", MatchType=MessageMatch.Contains)]
        public void BoolAsString()
        {
            var parameters = new CommandLineParameters(_typeStrings, _typeOptions);
            Expect(parameters.IsValid);
#pragma warning disable 168
            string test = parameters["boolean"];
#pragma warning restore 168
            Expect(false, "FAILED type test: boolean as string didn't throw exception.");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "isn't of type", MatchType = MessageMatch.Contains)]
        public void BoolAsInteger()
        {
            var parameters = new CommandLineParameters(_typeStrings, _typeOptions);
            Expect(parameters.IsValid);
#pragma warning disable 168
            int test = parameters["boolean"];
#pragma warning restore 168
            Expect(false, "FAILED type test: boolean as integer didn't throw exception.");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "isn't of type", MatchType = MessageMatch.Contains)]
        public void StringAsBool()
        {
            var parameters = new CommandLineParameters(_typeStrings, _typeOptions);
            Expect(parameters.IsValid);
#pragma warning disable 168
            bool test = parameters["string"];
#pragma warning restore 168
            Expect(false, "FAILED type test: string as boolean didn't throw exception.");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "isn't of type", MatchType = MessageMatch.Contains)]
        public void StringAsInteger()
        {
            var parameters = new CommandLineParameters(_typeStrings, _typeOptions);
            Expect(parameters.IsValid);
#pragma warning disable 168
            int test = parameters["string"];
#pragma warning restore 168
            Expect(false, "FAILED type test: string as integer didn't throw exception.");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "isn't of type", MatchType = MessageMatch.Contains)]
        public void IntegerAsBool()
        {
            var parameters = new CommandLineParameters(_typeStrings, _typeOptions);
            Expect(parameters.IsValid);
#pragma warning disable 168
            bool test = parameters["numeric"];
#pragma warning restore 168
            Expect(false, "FAILED type test: integer as boolean didn't throw exception.");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "isn't of type", MatchType = MessageMatch.Contains)]
        public void IntegerAsString()
        {
            var parameters = new CommandLineParameters(_typeStrings, _typeOptions);
            Expect(parameters.IsValid);
#pragma warning disable 168
            string test = parameters["numeric"];
#pragma warning restore 168
            Expect(false, "FAILED type test: integer as string didn't throw exception.");
        }

        [Test]
        public void MissingIntegerArgumentTest()
        {
            var options = new List<CommandLineParameters.OptionInfo>(new[] {
                new CommandLineParameters.OptionInfo("count", "Numeric count parameter", typeof(int))
            });
            ReportResult(options, new CommandLineParameters(new[] { "-count" }, options), false, "missing integer argument test.", false);
        }

        [Test]
        public void MissingStringArgumentTest()
        {
            var options = new List<CommandLineParameters.OptionInfo>(new[] {
                new CommandLineParameters.OptionInfo("filename", "String parameter", typeof(string))
            });
            ReportResult(options, new CommandLineParameters(new[] { "-filename" }, options), false, "missing string argument test.", false);
        }

        public void BadlyFormedIntegerTest()
        {
            var options = new List<CommandLineParameters.OptionInfo>(new[] {
                new CommandLineParameters.OptionInfo("samples", "Numeric parameter", typeof(int))
            });
            ReportResult(options, new CommandLineParameters(new[] { "-samples", "ABC" }, options), false, "badly formed numeric argument test.", false);
        }

        [Test]
        public void MissingRequiredParameterTest()
        {
            var options = new List<CommandLineParameters.OptionInfo>(new[] {
                new CommandLineParameters.OptionInfo("required", "First required parameter", typeof(string)),
                new CommandLineParameters.OptionInfo("missing", "Second required parameter", typeof(string))
            });
            ReportResult(options, new CommandLineParameters(new[] { "-required", "value" }, options), false, "missing required parameter test.", false);
        }

        [Test]
        public void BooleanValueTest()
        {
            var options = new List<CommandLineParameters.OptionInfo>(new[] { new CommandLineParameters.OptionInfo("turniton", "Binary switch", false) });
            ReportResult(options, new CommandLineParameters(new[] { "-turniton" }, options), true, "boolean test.", true);
        }

        [Test]
        public void DefaultParameterTest()
        {
            var strings = new[] { "--model", "simplesir.cmdl" };
            var options = new List<CommandLineParameters.OptionInfo>(new[] {
                new CommandLineParameters.OptionInfo("model|m", "Model description file (CMDL/EMODL/SBML)", typeof(string)),
                new CommandLineParameters.OptionInfo("solver", "Solver algorithm", "SSA"),
                new CommandLineParameters.OptionInfo("directory", "Working directory", "."),
                new CommandLineParameters.OptionInfo("duration", "Simulation duration", 100),
                new CommandLineParameters.OptionInfo("runs", "Number of simulation runs", 1),
                new CommandLineParameters.OptionInfo("samples", "Number of samples per run", 100)
            });
            ReportResult(options, new CommandLineParameters(strings, options), true, "default parameter list test.", true);
        }

        [Test]
        public void GoodParameterTest()
        {
            var strings = new[] { "--model", "simplesir.cmdl", "-solver", "ssa", "-directory", "c:\\temp", "-duration", "314", "-runs", "159", "-samples", "1024" };
            var options = new List<CommandLineParameters.OptionInfo>(new[] {
                new CommandLineParameters.OptionInfo("model|m", "Model description file (CMDL/EMODL/SBML)", typeof(string)),
                new CommandLineParameters.OptionInfo("solver", "Solver algorithm", "SSA"),
                new CommandLineParameters.OptionInfo("directory", "Working directory", "."),
                new CommandLineParameters.OptionInfo("duration", "Simulation duration", 100),
                new CommandLineParameters.OptionInfo("runs", "Number of simulation runs", 1),
                new CommandLineParameters.OptionInfo("samples", "Number of samples per run", 100)
            });
            ReportResult(options, new CommandLineParameters(strings, options), true, "good parameter list test.", true);
        }

        [Test]
        public void HelpTest()
        {
            var options = new List<CommandLineParameters.OptionInfo>(new[] {
                new CommandLineParameters.OptionInfo("model", "Model description file (CMDL/EMODL/SBML)", typeof(string)),
                new CommandLineParameters.OptionInfo("solver", "Solver algorithm", "SSA"),
                new CommandLineParameters.OptionInfo("directory", "Working directory", "."),
                new CommandLineParameters.OptionInfo("duration", "Simulation duration", 100),
                new CommandLineParameters.OptionInfo("runs", "Number of simulation runs", 1),
                new CommandLineParameters.OptionInfo("samples", "Number of samples per run", 100)
            });
            ReportResult(options, new CommandLineParameters(new[] { "?" }, options), false, "'?' help test", false);
            ReportResult(options, new CommandLineParameters(new[] { "-?" }, options), false, "'?' help test", false);
            ReportResult(options, new CommandLineParameters(new[] { "-help" }, options), false, "\"-help\" help test", false);
            ReportResult(options, new CommandLineParameters(new[] { "/HELP" }, options), false, "\"/HELP\" help test", false);
        }

        [Test]
        public void TestConfigSetting()
        {
            var options = new List<CommandLineParameters.OptionInfo>(new[] {
                new CommandLineParameters.OptionInfo("config|cfg|c", "config filename", typeof(string))
            });
            ReportResult(options, new CommandLineParameters(new[] { "--config", "config.json" }, options), true, "\"-config\" test", false);
            ReportResult(options, new CommandLineParameters(new[] { "--cfg", "config.json" },    options), true, "\"-cfg\" test",    false);
            ReportResult(options, new CommandLineParameters(new[] { "-c", "config.json" },       options), true, "\"-c\" test",      false);
        }

        public void ReportResult(List<CommandLineParameters.OptionInfo> options, CommandLineParameters parameters, bool expectedValidity, string message, bool showParameters)
        {
            Expect(parameters.IsValid == expectedValidity, "FAILED: " + message);

            if (showParameters)
            {
                foreach (CommandLineParameters.OptionInfo option in options)
                {
                    if (option.Type == typeof(bool)) Console.WriteLine(option.Name + ": {0}", (bool)parameters[option.Name]);
                    if (option.Type == typeof(string)) Console.WriteLine(option.Name + ": {0}",  (string)parameters[option.Name]);
                    if (option.Type == typeof(int)) Console.WriteLine(option.Name + ": {0}", (int)parameters[option.Name]);
                }
            }
            Console.Write("PASSED ");
            Console.WriteLine(message);
            Console.WriteLine();
        }

        [Test]
        public void TestCommandlineParametersArguments()
        {
            var argv = new[] {"--boolean", "--string", "astring", "--numeric", "1984", "42", "foo"};
            var parameters = new CommandLineParameters(argv, _typeOptions);
            Assert.AreEqual(2, parameters.Arguments.Count);
            Assert.Contains("42", parameters.Arguments);
            Assert.Contains("foo", parameters.Arguments);
        }

        [Test]
        public void TestBadNumericArgument()
        {
            var argv = new[] { "--boolean", "--string", "astring", "--numeric", "nineteen eighty-four"};
            var parameters = new CommandLineParameters(argv, _typeOptions);
            Assert.IsFalse(parameters.IsValid);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "must have a default", MatchType = MessageMatch.Contains)]
        public void NullDefaultInOptionInfo()
        {
#pragma warning disable 168
            var option = new CommandLineParameters.OptionInfo("foo", "dummy", (object)null);
#pragma warning restore 168
            Expect(false, "OptionInfo ctor should throw exception on null default.");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "must have a type", MatchType = MessageMatch.Contains)]
        public void NullTypeInOptionInfo()
        {
#pragma warning disable 168
            var option = new CommandLineParameters.OptionInfo("foo", "dummy", null);
#pragma warning restore 168
            Expect(false, "OptionInfo ctor should throw exception on null type.");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "isn't of type bool", MatchType = MessageMatch.Contains)]
        public void NonBooleanOptionAsBoolean()
        {
            var option = new CommandLineParameters.OptionInfo("foo", "a number", typeof (int)) {BooleanValue = true};
            Expect(false, "Setting an integer option to a boolean value should throw an exception.");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "isn't of type string", MatchType = MessageMatch.Contains)]
        public void NonStringOptionAsString()
        {
            var option = new CommandLineParameters.OptionInfo("foo", "a number", typeof (int)) {StringValue = "bar"};
            Expect(false, "Setting an integer option to a boolean value should throw an exception.");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "isn't of type int", MatchType = MessageMatch.Contains)]
        public void NonIntegerOptionAsInteger()
        {
            var option = new CommandLineParameters.OptionInfo("foo", "a string", typeof (string)) {IntegerValue = 42};
            Expect(false, "Setting a string option to an integer value should throw an exception.");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage = "must have a valid name", MatchType = MessageMatch.Contains)]
        public void RequiredOptionWithNullName()
        {
            var option = new CommandLineParameters.OptionInfo(null, "description", typeof(string));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "must have a valid name", MatchType = MessageMatch.Contains)]
        public void RequiredOptionWithEmptyName()
        {
            var option = new CommandLineParameters.OptionInfo("", "description", typeof(string));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage = "must have a valid name", MatchType = MessageMatch.Contains)]
        public void OptionalOptionWithNullName()
        {
            var option = new CommandLineParameters.OptionInfo(null, "description", "foo");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "must have a valid name", MatchType = MessageMatch.Contains)]
        public void OptionalOptionWithEmptyName()
        {
            var option = new CommandLineParameters.OptionInfo("", "description", "foo");
        }

        [Test]
        public void OptionWithNullDescription()
        {
            var option = new CommandLineParameters.OptionInfo("filename|f", null, "output.txt");
            // ReSharper disable once StringCompareIsCultureSpecific.1
            Assert.IsTrue(string.Compare(String.Empty, option.Description) == 0);
        }
    }
}
