using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;

namespace cmsregressiontests
{
    [TestFixture]
    class RegressionTests : AssertionHelper
    {
        //[TestCase("garki-delay", "garki-delay.emodl", "garki-delay.cfg", "SSA", null)]
        //[TestCase("garki-delay", "garki-delay.emodl", "garki-delay.cfg", "FIRST", null)]
        //[TestCase("garki-delay", "garki-delay.emodl", "garki-delay.cfg", "TAU", null)]
        //[TestCase("garki-delay", "garki-delay.emodl", "garki-delay.cfg", "R", null)]
        //[TestCase("garki-delay", "garki-delay.emodl", "garki-delay.cfg", "RFAST", null)]
        //[TestCase("garki-delay", "garki-delay.emodl", "garki-delay.cfg", "TSSA", null)]
        //[TestCase("garki-delay", "garki-delay.emodl", "garki-delay.cfg", "DFSP", null)]
        //[TestCase("garki-delay", "garki-delay.emodl", "garki-delay.cfg", "OTSSA", null)]
        //[TestCase("garki-delay", "garki-delay.emodl", "garki-delay.cfg", "OTSSA", null)]
        //[TestCase("garki-delay", "garki-delay.emodl", "garki-delay.cfg", "NEXT", null)]
        //[TestCase("garki-delay", "garki-delay.emodl", "garki-delay.cfg", "BLEAP", null)]
        //[TestCase("garki-delay", "garki-delay.emodl", "garki-delay.cfg", "HYBRID", null)]
        [TestCase("BLeapTest", "modelfile.emodl", "config.json", null, @"BLeaping\", RegressionTestingEnums.OutputTypes.Csv, RegressionTestingEnums.ValidationTypes.Validate)]
        [TestCase("GibsonBruckTest", "modelfile.emodl", "config.json", null, @"GibsonBruck\", RegressionTestingEnums.OutputTypes.Csv, RegressionTestingEnums.ValidationTypes.NoValidation)]
        [TestCase("SSATest", "modelfile.emodl", "config.json", null, @"Gillespie\", RegressionTestingEnums.OutputTypes.Csv, RegressionTestingEnums.ValidationTypes.NoValidation)]
        [TestCase("FirstReactionTest", "modelfile.emodl", "config.json", null, @"GillespieFirstReaction\", RegressionTestingEnums.OutputTypes.Csv, RegressionTestingEnums.ValidationTypes.NoValidation)]
        [TestCase("MidPointLeapingTest", "modelfile.emodl", "config.json", null, @"MidPointLeaping\", RegressionTestingEnums.OutputTypes.Csv, RegressionTestingEnums.ValidationTypes.NoValidation)]
        [TestCase("TauLeapingTest", "modelfile.emodl", "config.json", null, @"TauLeaping\", RegressionTestingEnums.OutputTypes.Csv, RegressionTestingEnums.ValidationTypes.NoValidation)]
        [TestCase("FractionalDiffusion", "fractional_sir_delta.emodl", "fractional_sir_delta.json", null, @"FractionalDiffusion\", RegressionTestingEnums.OutputTypes.Csv, RegressionTestingEnums.ValidationTypes.NoValidation)]
        [TestCase("DiffusionDFSP", "diffusion_fisher.emodl", "diffusion_fisher.json", null, @"DiffusionDFSP\", RegressionTestingEnums.OutputTypes.Csv, RegressionTestingEnums.ValidationTypes.NoValidation)]
        [TestCase("DiffusionISSA", "diffusion_fisher.emodl", "diffusion_fisher.json", null, @"DiffusionISSA\", RegressionTestingEnums.OutputTypes.Csv, RegressionTestingEnums.ValidationTypes.NoValidation)]
        [TestCase("ExitTimes", "exit_time_sir.emodl", "exit_time_sir.json", null, @"ExitTimes\", RegressionTestingEnums.OutputTypes.Txt, RegressionTestingEnums.ValidationTypes.NoValidation)]
        [TestCase("RLeaping", "rleaping_sir.emodl", "rleaping_sir.json", null, @"RLeaping\", RegressionTestingEnums.OutputTypes.Csv, RegressionTestingEnums.ValidationTypes.NoValidation)]
        [TestCase("RLeapingFast", "rleaping_fast_sir.emodl", "rleaping_fast_sir.json", null, @"RLeapingFast\", RegressionTestingEnums.OutputTypes.Csv, RegressionTestingEnums.ValidationTypes.NoValidation)]
        [TestCase("dwSSATest", "rever_isom.emodl", "rever_isom.cfg", null, @"dwSSA\", RegressionTestingEnums.OutputTypes.Txt, RegressionTestingEnums.ValidationTypes.NoValidation)]
        public void CompartmentsRegressionTests(String testName, String model, String config, String solver, String modelDirectory, RegressionTestingEnums.OutputTypes ot, RegressionTestingEnums.ValidationTypes vt)
        {
            const String outputDirectory = @"output\";
            String outputFileExtension = RegressionTestingEnums.OutputExtension(ot);
            String expectedTrajectoriesFileName = modelDirectory + testName + outputFileExtension;
            String tempTestName = null != solver ? testName + "_" + solver : testName;
            Directory.CreateDirectory(outputDirectory);

            String logFile = outputDirectory + tempTestName + ".log.txt";
            String errFile = outputDirectory + tempTestName + ".err.txt";
            String tempModelFileName = tempTestName + ".emodl";
            CreateTemporaryFile(model, modelDirectory, outputDirectory + tempModelFileName, solver, tempTestName);
            String tempConfigFileName = tempTestName + ".cfg";
            CreateTemporaryFile(config, modelDirectory, outputDirectory + tempConfigFileName, solver, tempTestName);
            String trajectoriesFileName = outputDirectory + tempTestName + outputFileExtension;
            String trajectoriesJson = outputDirectory + tempTestName + ".json";
            String trajectoriesMatlab = outputDirectory + tempTestName + ".mat";

            ExecuteCompartmentsExe(tempModelFileName, tempConfigFileName, outputDirectory, logFile, errFile);

            switch(vt)
            {
                case RegressionTestingEnums.ValidationTypes.Validate:
                    ValidateOutputFiles(expectedTrajectoriesFileName, trajectoriesFileName, trajectoriesJson, trajectoriesMatlab);
                    break;
                case RegressionTestingEnums.ValidationTypes.Probabilistic:
                    ProbabilisticValidation();
                    break;
            }

            var listOfTempFiles = new List<string>
                {
                    logFile,
                    errFile,
                    trajectoriesJson,
                    trajectoriesMatlab,
                    trajectoriesFileName,
                    outputDirectory + tempConfigFileName,
                    outputDirectory + tempModelFileName
                };

            DeleteTemporaryFiles(listOfTempFiles);
        }

        private static void ValidateOutputFiles(String expectedTrajectoriesFileName, String trajectoriesFileName, String trajectoriesJson, String trajectoriesMatlab)
        {
            FileAssert.AreEqual(expectedTrajectoriesFileName, trajectoriesFileName);
            Assert.True(File.Exists(trajectoriesJson));
            Assert.True(File.Exists(trajectoriesMatlab));
        }
        
        private static void ProbabilisticValidation()
        {
            
        }

        private static void ExecuteCompartmentsExe(String model, String config, String outputDirectory, String logFile, String errFile)
        {
            var process = new Process();
            const string processFileName = @"compartments.exe";

            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = processFileName;
            process.StartInfo.Arguments = "-model" + " " + model + " " + "-config" + " " + config + " " + "-directory" + " " + outputDirectory;

            Assert.True(process.Start(), "The Executable {0} failed to start.", processFileName);

            String stdOut = process.StandardOutput.ReadToEnd();
            String stdErr = process.StandardError.ReadToEnd();
            WriteStreamToFile(stdOut, logFile);
            WriteStreamToFile(stdErr, errFile);
        }

        private static void CreateTemporaryFile(String sourceFileName, String sourceDirectory, String tempFileName, String solver, String prefix)
        {
            String src = ReadFileToStringAfterSkippingNLines(sourceDirectory + sourceFileName, 0);

            if (null != solver)
            {
                src = src.Replace(@"$SOLVER$", solver);
            }

            if (null != prefix)
            {
                src = src.Replace(@"$PREFIX$", prefix);
            }

            WriteStreamToFile(src, tempFileName);
        }


        private static void DeleteTemporaryFiles(IEnumerable<string> listOfFiles)
        {
            try
            {
                foreach (String file in listOfFiles)
                {
                    File.Delete(file);
                }
            }
            catch (Exception)
            {
                {}
                throw;
            }
        }

        private static String ReadFileToStringAfterSkippingNLines(String fileName, long linesToSkip)
        {
            String tempString;

            using (TextReader reader = new StreamReader(fileName))
            {
                for (int i = 0; i < linesToSkip; i++)
                {
                    reader.ReadLine();
                }

                tempString = reader.ReadToEnd();
            }

            return tempString;
        }

        private static void WriteStreamToFile(String stream, String fileName)
        {
            using (var writer = new StreamWriter(fileName))
            {
                writer.Write(stream);
            }
        }
    }
}
