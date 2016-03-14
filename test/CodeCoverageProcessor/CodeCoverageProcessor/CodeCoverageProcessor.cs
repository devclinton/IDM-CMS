using System;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Data.SqlClient;
using System.Reflection;
using Microsoft.VisualStudio.Coverage.Analysis;

namespace CodeCoverageProcessor
{
    class CodeCoverageProcessor
    {
        static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("Usage: {0} filename.coverage [outputfile path]", Path.GetFileName(Assembly.GetExecutingAssembly().CodeBase));
                return -1;
            }

            const String outputFileName = "coverage.xml";
            String outputFilePath = args.Length > 1 ? args[1] : "";
            String inputFileName = args[0];
            
            try
            {
                Int64 testRunId;
                DateTime testRunTime;

                TransformCoverageFileToXml(inputFileName, outputFileName);
                String xmlCoverageFileContents = ReadXmlContentsToString(outputFileName);
                WriteCodeCoverageXmlToSqlDatabase(xmlCoverageFileContents, out testRunId, out testRunTime);
                String dateTimeString = ParseSQLDateTimeToString(testRunTime);
                String xmlCoverageFinalFileName = XmlCoverageFinalFileName(testRunId, dateTimeString, outputFilePath);
                CreateFormattedCoverageXml(xmlCoverageFileContents, xmlCoverageFinalFileName);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return -1;
            }

            return 0;
        }

        private static void TransformCoverageFileToXml(String inputCoverageFileName, String outputXmlFileName)
        {
            using (CoverageInfo info = CoverageInfo.CreateFromFile(inputCoverageFileName))
            {
                var coverageDataset = info.BuildDataSet();
                coverageDataset.ExportXml(outputXmlFileName);
            }
        }

        private static String ReadXmlContentsToString(String xmlCoverageFileName)
        {
            string tempXmlFile;

            using (StreamReader sr = new StreamReader(xmlCoverageFileName))
            {
                tempXmlFile = sr.ReadToEnd();
            }

            return tempXmlFile;
        }

        private static void CreateFormattedCoverageXml(String xmlCoverageFileContents, String xmlCoverageFinalFileName)
        {
            using (StreamWriter sw = new StreamWriter(xmlCoverageFinalFileName))
            {
                sw.WriteLine("<?xml version=\"1.0\"?>");
                sw.WriteLine("<?xml-stylesheet type=\"text/xsl\" href=\"coverage.xsl\"?>");
                sw.Write(xmlCoverageFileContents);
            }
        }

        private static String ParseSQLDateTimeToString(DateTime testRunTime)
        {
            String[] formats = testRunTime.GetDateTimeFormats();
            String tempString = formats[93];

            tempString = tempString.Replace(':', '-');
            tempString = tempString.Replace(' ', '-');
            
            return tempString;
        }

        private static String XmlCoverageFinalFileName(Int64 testRunId, String dateTimeString, String outputFilePath)
        {
            return outputFilePath + dateTimeString + "-RunId-" + testRunId.ToString() + ".xml";
        }

        private static void WriteCodeCoverageXmlToSqlDatabase(String xmlContents, out Int64 testRunId, out DateTime testRunTime)
        {
            const String connectionString = "Integrated Security=SSPI;Persist Security Info=False;User ID=emod_job;Initial Catalog=CMS;Data Source=ivlabsdvapp03";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                testRunId = 0;
                testRunTime = new DateTime();

                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = "sp_LoadCoverageFile";
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("coverageXml", SqlDbType.Text);
                    command.Parameters["coverageXml"].Value = xmlContents;

                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows && reader.Read())
                            {
                                testRunId = reader.GetInt64(reader.GetOrdinal("testRunId"));
                                testRunTime = reader.GetDateTime(reader.GetOrdinal("InsertionDate"));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Error occurred: {0}", e.Message);
                        throw;
                    }
                }
            }
        }

        public static void DoAnalysis(String inputFile)
        {
            using (CoverageInfo info = CoverageInfo.CreateFromFile(inputFile))
            {
                List<BlockLineRange> lines = new List<BlockLineRange>();
                System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\CodeCoverage\myTest.txt");
                List<CodeCoverageInfo> ccInfoList = new List<CodeCoverageInfo>();
                uint totalBlocksCovered = 0;
                uint totalBlocksNotCovered = 0;

                foreach (ICoverageModule module in info.Modules)
                {
                    byte[] coverageBuffer = module.GetCoverageBuffer(null);

                    using (ISymbolReader reader = module.Symbols.CreateReader())
                    {
                        uint methodId;
                        string methodName;
                        string undecoratedMethodName;
                        string className;
                        string namespaceName;

                        lines.Clear();
                        while (reader.GetNextMethod(
                            out methodId,
                            out methodName,
                            out undecoratedMethodName,
                            out className,
                            out namespaceName,
                            lines))
                        {
                            CodeCoverageInfo ccInfo = new CodeCoverageInfo();
                            CoverageStatistics stats = CoverageInfo.GetMethodStatistics(coverageBuffer, lines);

                            ccInfo.MethodId = methodId;
                            ccInfo.MethodName = methodName;
                            ccInfo.UndecoratedMethodName = undecoratedMethodName;
                            ccInfo.ClassName = className;
                            ccInfo.NamespaceName = namespaceName;
                            ccInfo.Statistics = stats;

                            totalBlocksCovered += ccInfo.Statistics.BlocksCovered;
                            totalBlocksNotCovered += ccInfo.Statistics.BlocksNotCovered;

                            ccInfoList.Add(ccInfo);
                            lines.Clear();
                        }
                    }
                }

                WriteCodeCoverageTotalCoverage(totalBlocksCovered, totalBlocksNotCovered, file);
                IEnumerable<CodeCoverageInfo> sortedList = ccInfoList.OrderBy(mi => mi.NamespaceName + mi.ClassName + mi.MethodName);
                foreach (var mi in sortedList)
                {
                    WriteCodeCoverageInfo(lines, mi, file);
                }


                file.Close();
            }
        }

        private static void WriteCodeCoverageTotalCoverage(uint totalBlocksCovered, uint totalBlocksNotCovered, StreamWriter file)
        {
            uint totalBlocks = totalBlocksCovered + totalBlocksNotCovered;
            double percentCoverage = (double)(totalBlocksCovered * 100) / totalBlocks;
            file.WriteLine("TOTALS:");
            file.WriteLine("    {0} total blocks", totalBlocks);
            file.WriteLine("    {0} blocks covered", totalBlocksCovered);
            file.WriteLine("    {0} blocks not covered", totalBlocksNotCovered);
            file.WriteLine("    {0}% code coverage", percentCoverage.ToString("F"));
        }

        private static void WriteCodeCoverageInfo(List<BlockLineRange> lines, CodeCoverageInfo mi, StreamWriter file)
        {
            file.WriteLine("Method {0}{1}{2}{3}{4} has:",
                           mi.NamespaceName == null ? "" : mi.NamespaceName,
                           string.IsNullOrEmpty(mi.NamespaceName) ? "" : ".",
                           mi.ClassName == null ? "" : mi.ClassName,
                           string.IsNullOrEmpty(mi.ClassName) ? "" : ".",
                           mi.MethodName
                );
            Console.WriteLine("Method {0}{1}{2}{3}{4} has:",
                              mi.NamespaceName == null ? "" : mi.NamespaceName,
                              string.IsNullOrEmpty(mi.NamespaceName) ? "" : ".",
                              mi.ClassName == null ? "" : mi.ClassName,
                              string.IsNullOrEmpty(mi.ClassName) ? "" : ".",
                              mi.MethodName
                );
            Console.WriteLine("    {0} blocks covered", mi.Statistics.BlocksCovered);
            Console.WriteLine("    {0} blocks not covered", mi.Statistics.BlocksNotCovered);
            Console.WriteLine("    {0} lines covered", mi.Statistics.LinesCovered);
            Console.WriteLine("    {0} lines partially covered", mi.Statistics.LinesPartiallyCovered);
            Console.WriteLine("    {0} lines not covered", mi.Statistics.LinesNotCovered);
            file.WriteLine("    {0} blocks covered", mi.Statistics.BlocksCovered);
            file.WriteLine("    {0} blocks not covered", mi.Statistics.BlocksNotCovered);
            file.WriteLine("    {0} lines covered", mi.Statistics.LinesCovered);
            file.WriteLine("    {0} lines partially covered", mi.Statistics.LinesPartiallyCovered);
            file.WriteLine("    {0} lines not covered", mi.Statistics.LinesNotCovered);
            lines.Clear();
        }
    }
}
