/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using compartments.CommandLine;
using compartments.emod;
using compartments.emod.utils;
using compartments.emod.interfaces;
using compartments.emodl;

namespace compartments
{
    public static class Program
    {
        internal static void Main(string[] args)
        {
            var version = Environment.Version;
            Console.WriteLine("CMS Framework version: {0} {1}", VersionInfo.Version, VersionInfo.Description);
            Console.WriteLine("CLR Runtime Version: {0} ({1}-bit)", version, (IntPtr.Size * 8));

            try
            {
                ExecutionParameters executionParameters;

                if (ProcessArguments(args, out executionParameters))
                {
                    ModelInfo model = LoadModel(executionParameters.ModelFileName);

                    if (model != null)
                    {
                        if (executionParameters.WriteModelFile)
                        {
                            using (var writer = new StreamWriter("model.emodl"))
                            {
                                writer.Write(model.ToString());
                            }
                        }

                        RunModel(model, executionParameters.SolverName, executionParameters.Duration, executionParameters.Repeats, executionParameters.Samples);
                    }
                }
            }
            catch (Exception e)
            {
                var currentException = e;
                while (currentException != null)
                {
                    Console.WriteLine("Caught exception during execution: {0}", e.Message);
                    Console.WriteLine("Stack trace: {0}", e.StackTrace);
                    Console.WriteLine();

                    Console.Error.WriteLine("Caught exception during execution: {0}", e.Message);
                    Console.Error.WriteLine("Stack trace: {0}", e.StackTrace);
                    Console.Error.WriteLine();

                    currentException = currentException.InnerException;
                }
            }
        }

        public static bool ProcessArguments(string[] args, out ExecutionParameters executionParameters)
        {
            var options = new List<CommandLineParameters.OptionInfo>(new[] {
                new CommandLineParameters.OptionInfo("model|m", "Model filename", typeof(string)),
                new CommandLineParameters.OptionInfo("config|cfg|c", "Configuration file", string.Empty),
                new CommandLineParameters.OptionInfo("directory|dir|d", "Working directory", ".")
            });
            var parameters = new CommandLineParameters(args, options);

            if (parameters.IsValid)
            {
                executionParameters = new ExecutionParameters { WorkingDirectory = parameters["directory"] };

                Directory.SetCurrentDirectory(executionParameters.WorkingDirectory);

                executionParameters.ModelFileName    = parameters["model"];
                executionParameters.ConfigFileName   = parameters["config"];

                if (executionParameters.ConfigFileName != string.Empty)
                {
                    Configuration.CurrentConfiguration = new Configuration(executionParameters.ConfigFileName);
                    Configuration config = Configuration.CurrentConfiguration;
                    if (config.HasParameter("solver"))   executionParameters.SolverName = config["solver"];
                    if (config.HasParameter("runs"))     executionParameters.Repeats    = config["runs"];
                    if (config.HasParameter("duration")) executionParameters.Duration   = config["duration"];
                    if (config.HasParameter("samples"))  executionParameters.Samples    = config["samples"];
                }

                Console.WriteLine(executionParameters.ToString());
                Console.WriteLine("Current directory: {0}", Directory.GetCurrentDirectory());
            }
            else
            {
                executionParameters = null;
            }

            return parameters.IsValid;
        }

        public static ModelInfo LoadModel(String modelFileName)
        {
            ModelInfo model;

            var modelFileInfo = new FileInfo(modelFileName);
            switch (modelFileInfo.Extension.ToUpper().Substring(1))
            {
                case "CMDL":
                    throw new NotSupportedException("CMDL support not present.");

                case "EMODL":
                    model = EmodlLoader.LoadEMODLFile(modelFileName);
                    break;

                case "SBML":
                case "XML":
                    throw new NotSupportedException("SBML [XML] support not yet present.");

                case "BNGL":
                    throw new NotSupportedException("BNGL support not yet present.");

                default:
                    Console.Error.WriteLine("Unknown model description language for '{0}'", modelFileName);
                    throw new ArgumentException($"Unknown model description language for '{modelFileName}'", nameof(modelFileName));
            }

            return model;
        }

        // ReSharper disable MemberCanBePrivate.Global
        public static void RunModel(ModelInfo model, String solverName, double duration, int repeats, int samples)
        // ReSharper restore MemberCanBePrivate.Global
        {
            Console.WriteLine("Starting simulation...");

            ISolver solver = SolverFactory.CreateSolver(solverName, model, repeats, duration, samples);
            Console.WriteLine("Using solver {0}", solver);
            solver.Solve();
            string outputPrefix = Configuration.CurrentConfiguration.GetParameterWithDefault("output.prefix", "trajectories");
            solver.OutputData(outputPrefix);

            Console.WriteLine("...finished simulation.");
        }

        // ReSharper disable UnusedMember.Global
        public static SimulationResults ExecuteModel(ModelInfo model, String solverName, double duration, int repeats, int samples)
        // ReSharper restore UnusedMember.Global
        {
            Console.WriteLine("Starting simulation...");

            ISolver solver = SolverFactory.CreateSolver(solverName, model, repeats, duration, samples);
            solver.Solve();
            var results = new SimulationResults(solver.GetTrajectoryLabels(), solver.GetTrajectoryData());

            Console.WriteLine("...finished simulation.");

            return results;
        }
    }

    public class ExecutionParameters
    {
        public ExecutionParameters()
        {
            WorkingDirectory = ".";
            SolverName       = "SSA";
            Samples          = 100;
            Repeats          = 1;
            ModelFileName    = String.Empty;
            Duration         = 100.0;
            ConfigFileName   = String.Empty;
            WriteModelFile   = false;
        }

        public string ConfigFileName { get; set; }
        public double Duration { get; set; }
        public string ModelFileName { get; set; }
        public int Repeats { get; set; }
        public int Samples { get; set; }
        public string SolverName { get; set; }
        public string WorkingDirectory { get; set; }
        public bool WriteModelFile { get; set; }

        public override String ToString()
        {
            var builder = new StringBuilder();

            builder.Append("Model:    "); builder.AppendLine(ModelFileName);
            builder.Append("Solver:   "); builder.AppendLine(SolverName);
            builder.Append("Config:   "); builder.AppendLine(ConfigFileName);
            // ReSharper disable SpecifyACultureInStringConversionExplicitly
            builder.Append("Duration: "); builder.AppendLine(Duration.ToString());
            builder.Append("Samples:  "); builder.AppendLine(Samples.ToString());
            builder.Append("Runs:     "); builder.AppendLine(Repeats.ToString());
            // ReSharper restore SpecifyACultureInStringConversionExplicitly
            builder.Append("Working Directory: "); builder.AppendLine(WorkingDirectory);

            return builder.ToString();
        }
    }
}
