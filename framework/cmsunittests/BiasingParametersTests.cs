/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using System.IO;
using compartments.emod.utils;
using NUnit.Framework;

namespace cmsunittests
{
    [TestFixture]
    class BiasingParametersTests : AssertionHelper
    {
        [Test]
        public void BasingParametersRareEventSpecConstructor()
        {
            var rareEventSpec = new BiasingParameters.RareEventSpec();

            Assert.AreEqual(string.Empty, rareEventSpec.ExpressionLocale);
            Assert.AreEqual(string.Empty, rareEventSpec.ExpressionName);
            Assert.AreEqual(0, rareEventSpec.IntermediateRareEventCount);
            Assert.IsEmpty(rareEventSpec.Thresholds);
        }

        [Test]
        public void BiasingParametersRareEventSpecExpressionLocale()
        {
            const string locale = "test";
            var rareEventSpec = new BiasingParameters.RareEventSpec {ExpressionLocale = locale};
            Assert.AreEqual(locale, rareEventSpec.ExpressionLocale);
        }

        [Test]
        public void BiasingParametersRareEventSpecExpressionName()
        {
            const string name = "test";
            var rareEventSpec = new BiasingParameters.RareEventSpec { ExpressionName = name };
            Assert.AreEqual(name, rareEventSpec.ExpressionName);
        }

        [Test]
        public void BiasingParametersRareEventSpecThresholds()
        {
            var rareEventSpec = new BiasingParameters.RareEventSpec();
            rareEventSpec.Thresholds.Add(42.0);
            Assert.AreEqual(1, rareEventSpec.IntermediateRareEventCount);
            Assert.AreEqual(42.0, rareEventSpec.Thresholds[0]);
        }

        [Test]
        public void BiasingParametersRareEventInfoConstructor()
        {
            var rareEventInfo = new BiasingParameters.RareEventInfo();
            Assert.AreEqual(0, rareEventInfo.BinCount);
            Assert.IsEmpty(rareEventInfo.Gammas);
            Assert.IsEmpty(rareEventInfo.Thresholds);
        }

        [Test]
        public void BiasingParametersRareEventInfoBinCount()
        {
            const int binCount = 42;
            var rareEventInfo = new BiasingParameters.RareEventInfo {BinCount = binCount};
            Assert.AreEqual(binCount, rareEventInfo.BinCount);
            Assert.AreEqual(binCount, rareEventInfo.Gammas.Length);
            Assert.AreEqual(binCount - 1, rareEventInfo.Thresholds.Length);
        }

        [Test]
        public void BiasingParametersRareEventInfoBinCountOne()
        {
            const int binCount = 1;
            var rareEventInfo = new BiasingParameters.RareEventInfo { BinCount = binCount };
            Assert.AreEqual(binCount, rareEventInfo.BinCount);
            Assert.AreEqual(binCount, rareEventInfo.Gammas.Length);
            Assert.AreEqual(binCount, rareEventInfo.Thresholds.Length);
        }

        [Test]
        [ExpectedException("System.ArgumentException", ExpectedMessage = "BinCount must be >= 1.")]
        public void BiasingParametersRareEventInfoBinCountZero()
        {
            const int binCount = 0;
            var rareEventInfo = new BiasingParameters.RareEventInfo { BinCount = binCount };
            Assert.AreEqual(binCount, rareEventInfo.BinCount);
            Assert.AreEqual(binCount, rareEventInfo.Gammas.Length);
            Assert.AreEqual(binCount - 1, rareEventInfo.Thresholds.Length);
        }

        [Test]
        [ExpectedException("System.ArgumentException", ExpectedMessage = "BinCount must be >= 1.")]
        public void BiasingParametersRareEventInfoBinCountMinusTwo()
        {
            const int binCount = -2;
            var rareEventInfo = new BiasingParameters.RareEventInfo { BinCount = binCount };
            Assert.AreEqual(binCount, rareEventInfo.BinCount);
            Assert.AreEqual(binCount, rareEventInfo.Gammas.Length);
            Assert.AreEqual(binCount - 1, rareEventInfo.Thresholds.Length);
        }

        [Test]
        public void BiasingParametersReactionInfoConstructor()
        {
            var reactionInfo = new BiasingParameters.ReactionInfo();
            Assert.AreEqual(string.Empty, reactionInfo.Name);
            Assert.IsEmpty(reactionInfo.RareEvents);
        }

        [Test]
        public void BiasingParametersReactionInfoName()
        {
            const string name = "test";
            var reactionInfo = new BiasingParameters.ReactionInfo {Name = name};
            Assert.AreEqual(name, reactionInfo.Name);
        }

        [Test]
        public void BiasingParametersReactionInfoRareEventInfo()
        {
            var reactionInfo = new BiasingParameters.ReactionInfo();
            const int binCount = 42;
            const double e = Math.E;
            const double pi = Math.PI;
            var rareEventInfo = new BiasingParameters.RareEventInfo {BinCount = binCount};
            rareEventInfo.Gammas[0] = e;
            rareEventInfo.Thresholds[1] = pi;
            reactionInfo.RareEvents.Add(rareEventInfo);
            Assert.IsNotEmpty(reactionInfo.RareEvents);
            Assert.AreEqual(rareEventInfo, reactionInfo.RareEvents[0]);
        }

        [Test]
        public void BiasingParametersLocaleInfoConstructor()
        {
            var localeInfo = new BiasingParameters.LocaleInfo();
            Assert.AreEqual(string.Empty, localeInfo.Name);
            Assert.AreEqual(0, localeInfo.ReactionCount);
            Assert.IsEmpty(localeInfo.Reactions);
        }

        [Test]
        public void BiasingParametersLocaleInfoName()
        {
            const string name = "test";
            var localeInfo = new BiasingParameters.LocaleInfo {Name = name};
            Assert.AreEqual(name, localeInfo.Name);
        }

        [Test]
        public void BiasingParametersLocaleInfoReactions()
        {
            var localeInfo = new BiasingParameters.LocaleInfo {Name = "bar"};
            var rareEventInfo = new BiasingParameters.RareEventInfo { BinCount = 42 };
            rareEventInfo.Gammas[0] = Math.E;
            rareEventInfo.Thresholds[1] = Math.PI;
            var reactionInfo = new BiasingParameters.ReactionInfo { Name = "foo" };
            reactionInfo.RareEvents.Add(rareEventInfo);
            localeInfo.Reactions.Add(reactionInfo);
            Assert.AreEqual(1, localeInfo.ReactionCount);
            Assert.IsNotEmpty(localeInfo.Reactions);
            Assert.AreEqual(reactionInfo, localeInfo.Reactions[0]);
        }
        [Test]
        public void BiasingParametersConstructor()
        {
            var parameters = new BiasingParameters();
            Assert.AreEqual(string.Empty, parameters.RareEvent.ExpressionLocale);
            Assert.AreEqual(string.Empty, parameters.RareEvent.ExpressionName);
            Assert.AreEqual(0, parameters.RareEvent.IntermediateRareEventCount);
            Assert.IsEmpty(parameters.RareEvent.Thresholds);
            Assert.IsEmpty(parameters.Locales);
        }

        [Test]
        public void BiasingParametersLoadParametersFromJson()
        {
            const string filename = "resources\\biasing_sample.json";
            var parameters = BiasingParameters.LoadParametersFromJson(filename);

            // Verify rare event spec
            Assert.AreEqual("site_1", parameters.RareEvent.ExpressionLocale);
            Assert.AreEqual("reExpression", parameters.RareEvent.ExpressionName);
            Assert.AreEqual(2, parameters.RareEvent.IntermediateRareEventCount);
            Assert.AreEqual(24, parameters.RareEvent.Thresholds[0]);
            Assert.AreEqual(32, parameters.RareEvent.Thresholds[1]);

            // Verify locale[s]
            Assert.AreEqual(1, parameters.Locales.Count);
            var localeInfo = parameters.Locales[0];
            Assert.AreEqual("site_1", localeInfo.Name);
            Assert.AreEqual(8, localeInfo.ReactionCount);
            Assert.AreEqual(8, localeInfo.Reactions.Count);

            // Verify first reaction
            var reactionInfo = localeInfo.Reactions[0];
            Assert.AreEqual("R1", reactionInfo.Name);
            Assert.AreEqual(2, reactionInfo.RareEvents.Count);
            var rareEventInfo = reactionInfo.RareEvents[0];
            Assert.AreEqual(2, rareEventInfo.BinCount);
            Assert.AreEqual(2, rareEventInfo.Gammas.Length);
            Assert.AreEqual(1, rareEventInfo.Thresholds.Length);
            Assert.AreEqual(1.06863, rareEventInfo.Gammas[0]);
            Assert.AreEqual(1.56882, rareEventInfo.Gammas[1]);
            Assert.AreEqual(0.00031386, rareEventInfo.Thresholds[0]);

            // Verify last reaction
            reactionInfo = localeInfo.Reactions[localeInfo.ReactionCount - 1];
            Assert.AreEqual("R8", reactionInfo.Name);
            Assert.AreEqual(2, reactionInfo.RareEvents.Count);
            rareEventInfo = reactionInfo.RareEvents[1];
            Assert.AreEqual(7, rareEventInfo.BinCount);
            Assert.AreEqual(7, rareEventInfo.Gammas.Length);
            Assert.AreEqual(6, rareEventInfo.Thresholds.Length);
            Assert.AreEqual(2.37130, rareEventInfo.Gammas[0]);
            Assert.AreEqual(2.191004, rareEventInfo.Gammas[6]);
            Assert.AreEqual(0.3935499, rareEventInfo.Thresholds[5]);
        }

        [Test]
        [ExpectedException("System.ArgumentNullException")]
        public void BiasingParametersLoadParametersFromNull()
        {
            // ReSharper disable UnusedVariable
            var parameters = BiasingParameters.LoadParametersFromJson(null);
            // ReSharper restore UnusedVariable
            Assert.Fail("LoadParametersFromJson(null) should throw an ArgumentNullException.");
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void BiasingParametersLoadParametersFromInvalidJson()
        {
            const string jsonFilename = "resources\\invalid.json";
            // ReSharper disable UnusedVariable
            var parameters = BiasingParameters.LoadParametersFromJson(jsonFilename);
            // ReSharper restore UnusedVariable
            Assert.Fail("LoadParametersFromInvalidJson({0}) should throw an ArgumentException.", jsonFilename);
        }

        [Test]
        [ExpectedException("System.IO.FileNotFoundException")]
        public void BiasingParametersLoadParametersFromMissingFile()
        {
            string tempFileName = Path.GetTempFileName();
            File.Delete(tempFileName);
            // ReSharper disable UnusedVariable
            var parameters = BiasingParameters.LoadParametersFromJson(tempFileName);
            // ReSharper restore UnusedVariable
            Assert.Fail("LoadParametersFromMissingFile(tempFileName) should throw an IO.FileNotFoundException.");
        }

        [Test]
        public void BiasingParametersWriteParametersToJson()
        {
            const string inputFile = "resources\\biasing_sample.json";
            var parameters = BiasingParameters.LoadParametersFromJson(inputFile);
            string outputFile = Path.GetTempFileName();
            bool areSame = false;
            try
            {
                Console.WriteLine("Writing biasing parameters to '{0}'...", outputFile);
                parameters.WriteParametersToJsonFile(outputFile);
                FileAssert.AreEqual(inputFile, outputFile);
                areSame = true;
            }
            finally
            {
                if (areSame)
                {
                    File.Delete(outputFile);
                }
            }
        }

        [Test]
        [ExpectedException("compartments.emod.utils.BiasingParametersDeserializer+MissingRareEventSpecificationException", ExpectedMessage = "JSON data doesn't contain correct 'RARE_EVENT' section.")]
        public void BiasingParametersJsonFileMissingRareEventParameter()
        {
            const string inputFile = "resources\\biasing_sample_RARE_EVENT_missing.json";
            // ReSharper disable UnusedVariable
            var parameters = BiasingParameters.LoadParametersFromJson(inputFile);
            // ReSharper restore UnusedVariable
            Assert.Fail("BiasingParametersJsonFileMissingRareEventParameter() should throw a System.ArgumentException.");
        }

        [Test]
        [ExpectedException("compartments.emod.utils.BiasingParametersDeserializer+ExpressionSpecificationException", ExpectedMessage = "JSON data doesn't contain correct 'EXPRESSION' section.")]
        public void BiasingParametersJsonFileMissingExpressionParameter()
        {
            const string inputFile = "resources\\biasing_sample_EXPRESSION_missing.json";
            // ReSharper disable UnusedVariable
            var parameters = BiasingParameters.LoadParametersFromJson(inputFile);
            // ReSharper restore UnusedVariable
            Assert.Fail("BiasingParametersJsonFileMissingExpressionParameter() should throw a System.ArgumentException.");
        }

        [Test]
        [ExpectedException("System.ArgumentNullException")]
        public void BiasingParametersJsonFileMissingIreCountParameter()
        {
            const string inputFile = "resources\\biasing_sample_IRE_COUNT_missing.json";
            // ReSharper disable UnusedVariable
            var parameters = BiasingParameters.LoadParametersFromJson(inputFile);
            // ReSharper restore UnusedVariable
            Assert.Fail("BiasingParametersJsonFileMissingIreCountParameter() should throw a System.ArgumentException.");
        }

        [Test]
        [ExpectedException("compartments.emod.utils.BiasingParametersDeserializer+ReactionArrayCountException")]
        public void BiasingParametersJsonFileReactionCountLessThanReactionArraySize()
        {
            const string inputFile = "resources\\biasing_sample_REACTION_COUNT_less_than_REACTION_array_size.json";
            // ReSharper disable UnusedVariable
            var parameters = BiasingParameters.LoadParametersFromJson(inputFile);
            // ReSharper restore UnusedVariable
            Assert.Fail("BiasingParametersJsonFileReactionCountLessThanReactionArraySize() should throw a System.ArgumentException.");
        }

        [Test]
        [ExpectedException("compartments.emod.utils.BiasingParametersDeserializer+RareEventInfoArrayLengthException")]
        public void BiasingParametersJsonFileIreCountNotRareEventInfoSize()
        {
            const string inputFile = "resources\\biasing_sample_IRE_COUNT_not_RARE_EVENT_INFO_array_size.json";
            // ReSharper disable UnusedVariable
            var parameters = BiasingParameters.LoadParametersFromJson(inputFile);
            // ReSharper restore UnusedVariable
            Assert.Fail("BiasingParametersJsonFileIreCountNotRareEventInfoSize() should throw a System.ArgumentException.");
        }

        [Test]
        [ExpectedException("compartments.emod.utils.BiasingParametersDeserializer+GammaArrayLengthException")]
        public void BiasingParametersJsonFileBinCountNotGammasSize()
        {
            const string inputFile = "resources\\biasing_sample_BIN_COUNT_not_GAMMAS_array_size.json";
            // ReSharper disable UnusedVariable
            var parameters = BiasingParameters.LoadParametersFromJson(inputFile);
            // ReSharper restore UnusedVariable
            Assert.Fail("BiasingParametersJsonFileBinCountNotGammasSize() should throw a System.ArgumentException.");
        }

        [Test]
        [ExpectedException("compartments.emod.utils.BiasingParametersDeserializer+CutoffArrayLengthException")]
        public void BiasingParametersJsonFileBinCountNotCutoffSize()
        {
            const string inputFile = "resources\\biasing_sample_BIN_COUNT_not_CUTOFF_array_size.json";
            // ReSharper disable UnusedVariable
            var parameters = BiasingParameters.LoadParametersFromJson(inputFile);
            // ReSharper restore UnusedVariable
            Assert.Fail("BiasingParametersJsonFileBinCountNotCutoffSize() should throw a System.ArgumentException.");
        }
    }
}
