/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using NUnit.Framework;
using compartments;
using compartments.solvers.solverbase;

namespace cmsunittests
{
    [TestFixture]
    class OutputOptionsTests : AssertionHelper
    {
        [Test]
        public void CsvOutputOptionsDefaults()
        {
            Configuration.CurrentConfiguration = null;
            var csvOptions = CsvSupport.GetCsvOutputOptions("trajectories");
            Assert.IsNotNull(csvOptions);
            Assert.IsFalse(csvOptions.CompressOutput);
            Assert.AreEqual("trajectories.csv", csvOptions.Filename);
            Assert.IsTrue(csvOptions.WriteCsvFile);
            Assert.IsTrue(csvOptions.WriteRealizationIndex);
        }

        [Test]
        public void CsvOutputOptionsFromConfig()
        {
            const string configText = "{\"output\":{\"writecsv\":false,\"compress\":true,\"writerealizationindex\":false}}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configText);
            var csvOptions = CsvSupport.GetCsvOutputOptions("realizations");
            Assert.IsNotNull(csvOptions);
            Assert.IsTrue(csvOptions.CompressOutput);
            Assert.AreEqual("realizations.csv", csvOptions.Filename);
            Assert.IsFalse(csvOptions.WriteCsvFile);
            Assert.IsFalse(csvOptions.WriteRealizationIndex);
        }

        [Test]
        public void JsonOutputOptionsDefaults()
        {
            Configuration.CurrentConfiguration = null;
            var jsonOptions = JsonSupport.GetJsonOutputOptions("trajectories");
            Assert.IsNotNull(jsonOptions);
            Assert.IsFalse(jsonOptions.CompressOutput);
            Assert.AreEqual("trajectories.json", jsonOptions.Filename);
            Assert.IsFalse(jsonOptions.WriteJsonFile);
        }

        [Test]
        public void JsonOutputOptionsFromConfig()
        {
            const string configText = "{\"output\":{\"writejson\":true,\"compress\":true}}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configText);
            var jsonOptions = JsonSupport.GetJsonOutputOptions("realizations");
            Assert.IsNotNull(jsonOptions);
            Assert.IsTrue(jsonOptions.CompressOutput);
            Assert.AreEqual("realizations.json", jsonOptions.Filename);
            Assert.IsTrue(jsonOptions.WriteJsonFile);
        }

        [Test]
        public void MatlabOutputOptionsDefaults()
        {
            Configuration.CurrentConfiguration = null;
            var matlabOptions = MatlabSupport.GetMatlabOutputOptions("matfile");
            Assert.IsNotNull(matlabOptions);
            Assert.AreEqual("matfile.mat", matlabOptions.Filename);
            Assert.IsTrue(matlabOptions.CompressOutput);
            Assert.IsFalse(matlabOptions.WriteMatFile);
            Assert.IsFalse(matlabOptions.UseNewFormat);
        }

        [Test]
        public void MatlabOutputOptionsFromConfig()
        {
            const string configText = "{\"output\":{\"writematfile\":false,\"compress\":false,\"newmatformat\":true}}";
            Configuration.CurrentConfiguration = Configuration.ConfigurationFromString(configText);
            var matlabOptions = MatlabSupport.GetMatlabOutputOptions("realizations");
            Assert.IsNotNull(matlabOptions);
            Assert.AreEqual("realizations.mat", matlabOptions.Filename);
            Assert.IsFalse(matlabOptions.CompressOutput);
            Assert.IsFalse(matlabOptions.WriteMatFile);
            Assert.IsTrue(matlabOptions.UseNewFormat);
        }
    }
}
