/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace VersionTool
{
    class Program
    {
        static int Main(string[] args)
        {
            int retVal = 0;

            try
            {
                string templateFilename;
                string outputFilename;
                string tfsUri;
                ProcessCommandlineArguments(args, out templateFilename, out outputFilename, out tfsUri);

                int build;
                GetCurrentBuildNumber("buildcount.txt", out build);

                build++;
                WriteCurrentBuildNumber("buildcount.txt", build);

                string username;
                GetEnvironmentVariable("USERNAME", out username);

                string branch;
                int revision;
                //GetSubversionBranchAndRevisionNumber(out branch, out revision);
                GetTfsBranchAndRevisionNumber(tfsUri, out branch, out revision);

                string templateText;
                GetTemplateText(templateFilename, out templateText);

                // Substitute the current user, Subversion branch and revision, and the build count
                string userText = Regex.Replace(templateText, "%USER%", username);
                string branchText = Regex.Replace(userText, "%BRANCH%", branch);
                string revisionText = Regex.Replace(branchText, "%REV%", revision.ToString());
                string buildText = Regex.Replace(revisionText, "%BUILD%", build.ToString());

                WriteProcessedText(outputFilename, buildText);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Error.WriteLine(ex.Message);
                retVal = -1;
            }

            return retVal;
        }

        static void ProcessCommandlineArguments(string[] args, out string templateFilename, out string outputFilename, out string tfsUri)
        {
            int iArg = 0;

            if (args.Length < 2)
                throw new ArgumentException("Must specify template filename and output filename.");

            tfsUri = "http://tfs12:8080/tfs/IVLabs_CMS";

            while ((args[iArg][0] == '-') || (args[iArg][0] == '/'))
            {
                switch (args[iArg].Substring(1).ToUpper())
                {
                    case "D":
                        // Look for "buildcount.txt" in the given directory
                        Console.WriteLine("Getting latest changeset number from '{0}'.", args[iArg + 1]);
                        Directory.SetCurrentDirectory(args[iArg + 1]);
                        break;

                    case "TFSURI":
                        Console.WriteLine("Using '{0}' for TFS server/collection URI.", args[iArg + 1]);
                        tfsUri = args[iArg + 1];
                        break;

                    default:
                        throw new ArgumentException($"Unknown command line switch: '{args[iArg]}'");
                }

                iArg += 2;
            }

            if ((args.Length - iArg) < 2)
                throw new ArgumentException("Must specify template filename and output filename.");

            templateFilename = args[iArg++];
            outputFilename = args[iArg];

            Console.WriteLine("Template file name: '{0}'", templateFilename);
            Console.WriteLine("Output file name:   '{0}'", outputFilename);
        }

        static void GetCurrentBuildNumber(string filename, out int value)
        {
            value = 0;

            // If it exists, open it, parse the current count, increment the count
            if (File.Exists(filename))
            {
                Console.WriteLine("Getting last build number from '{0}'.", filename);
                StreamReader reader = File.OpenText(filename);
                value = Int32.Parse(reader.ReadLine());
                reader.Close();
            }
            else
            {
                Console.WriteLine("'{0}' not found.", filename);
            }

            Console.WriteLine("Last build number = {0}", value);
        }

        static void WriteCurrentBuildNumber(string filename, int build)
        {
            // Update the current count
            using (StreamWriter writer = File.CreateText(filename))
            {
                writer.Write(build.ToString());
                writer.Close();
                Console.WriteLine("Wrote {0} to '{1}'.", build, filename);
            }
        }

        static void GetEnvironmentVariable(string variable, out string value)
        {
            value = Environment.GetEnvironmentVariable(variable);
            Console.WriteLine("{0} = {1}", variable, value);
        }

        static void GetSubversionBranchAndRevisionNumber(out string branch, out int revision)
        {
            var subversion = new Process
                                 {
                                     StartInfo =
                                         {
                                             Arguments = "info",
                                             FileName = @"..\ApacheSVN\svn.exe",
                                             RedirectStandardOutput = true,
                                             UseShellExecute = false,
                                             WorkingDirectory = Directory.GetCurrentDirectory()
                                         }
                                 };
            subversion.Start();
            string output = subversion.StandardOutput.ReadToEnd();
            subversion.WaitForExit();

            branch = "branch not found";
            if (Regex.IsMatch(output, "Repository Root: ([^\r\n]+)"))
            {
                string repositoryPath = Regex.Match(output, "Repository Root: ([^\r\n]+)").Groups[1].Captures[0].Value;
                string[] splits = repositoryPath.Split('/');
                branch = splits[splits.Length - 1];
            }

            string revisionString = "no revision found";
            if (Regex.IsMatch(output, "Revision: ([0-9]+)"))
                revisionString = Regex.Match(output, "Revision: ([0-9]+)").Groups[1].Captures[0].Value;

            revision = Int32.Parse(revisionString);
        }

        static void GetTfsBranchAndRevisionNumber(string tfsUri, out string branch, out int revision)
        {
            revision = -1;

            var tpc = new TfsTeamProjectCollection(new Uri(tfsUri));
            var vcs = tpc.GetService<VersionControlServer>();

            var currentDirectory = Directory.GetCurrentDirectory();
            var workspace = vcs.GetWorkspace(currentDirectory);
            Item directory = vcs.GetItem(currentDirectory);
            // Exclude the leading "$/" from the branch path
            branch = directory.ServerItem.Substring(2);

            var changesetList = vcs.QueryHistory(directory.ServerItem, VersionSpec.Latest, 0, RecursionType.Full, null,
                                                 new ChangesetVersionSpec(1), VersionSpec.Latest, 1, true, false);
            foreach (Changeset changeset in changesetList)
            {
                revision = Math.Max(revision, changeset.ChangesetId);
            }

            Console.WriteLine("Latest changeset for {0} = {1}", directory.ServerItem, revision);
        }

        static void GetTemplateText(string filename, out string text)
        {
            using (StreamReader reader = File.OpenText(filename))
            {
                text = reader.ReadToEnd();
                Console.WriteLine("Loaded template text from '{0}'.", filename);
            }
        }

        static void WriteProcessedText(string filename, string text)
        {
            using (StreamWriter writer = File.CreateText(filename))
            {
                writer.Write(text);
                Console.WriteLine("Wrote output text to '{0}'.", filename);
            }
        }
    }
}
