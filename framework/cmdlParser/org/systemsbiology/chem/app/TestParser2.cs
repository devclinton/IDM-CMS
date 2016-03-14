using System;

using org.systemsbiology.chem;
using org.systemsbiology.util;

namespace org.systemsbiology.chem.app
{
    public class TestParser2
    {
        public TestParser2(System.String cmdlFile, System.String outputParserResults, System.String outputFullParser)
        {
            DoOut dO = null;
            System.IO.Stream pInStream = null;
            IModelBuilder modBldr = null;
            Model mod = null;
            IncludeHandler pIncHndlr = null;
            System.IO.FileInfo pCmdlFile = null;
            System.IO.FileInfo fileDir = null;
            bool outParse = false;
            bool outFull = false;

            dO = new DoOut();
            dO.dOut("Create class registry", true);

            if (outputParserResults.Equals("true"))
                outParse = true;

            if (outputFullParser.Equals("true"))
                outFull = true;

/*J
            ClassRegistry modBldReg = new ClassRegistry(typeof(org.systemsbiology.chem.IModelBuilder));

            try
            {
                dO.dOut("Build registry", true);
                modBldReg.buildRegistry();
            }
            catch (System.Exception e)
            {
                //UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Throwable.toString' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                dO.dOut("Error... " + e.ToString(), true);
            }

            // now we need the parser alias....
            try
            {
                modBldr = (IModelBuilder) modBldReg.getInstance("command-language");
            }
            catch (System.Exception e)
            {
                //UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Throwable.toString' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                dO.dOut("Get Instance error... " + e.ToString(), true);
            }
*/
            modBldr = new ModelBuilderCommandLanguage();

            // dah - get an input stream for the cmdl file
            pCmdlFile = new System.IO.FileInfo(cmdlFile);
            try
            {
                dO.dOut("Create file input stream for " + cmdlFile, true);
                //UPGRADE_TODO: Constructor 'java.io.FileInputStream.FileInputStream' was converted to 'System.IO.FileStream.FileStream' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioFileInputStreamFileInputStream_javaioFile'"
                pInStream = new System.IO.FileStream(pCmdlFile.FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            }
            catch (System.IO.FileNotFoundException)
            {
                dO.dOut("FILE NOT FOUND " + cmdlFile, true);
                return;
            }

            // need include handler
            pIncHndlr = new IncludeHandler();
            fileDir = new System.IO.FileInfo(pCmdlFile.DirectoryName);
            pIncHndlr.Directory = fileDir;

            // this step must be done
            try
            {
                dO.dOut("Try to build the model which parses it....", true);
                mod = modBldr.buildModel(pInStream, pIncHndlr);
            }
            catch (System.Exception e)
            {
                //UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Throwable.toString' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                dO.dOut("issue build model " + e.ToString(), true);
            }

            dO.dOut("Done Parsing............", true);

            if (outParse)
            {
                dO.dOut("Model built for model " + mod.ToString(), true);
            }

            if (outFull)
            {
                ParserInter pi = new ParserInter(mod, dO);
                try
                {
                    pi.doStuff();
                }
                catch (System.IO.IOException e)
                {
                    //UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Throwable.toString' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                    dO.dOut("IOException thrown " + e.ToString(), true);
                }
            }
        }

        [STAThread]
        public static void  Main(System.String[] args)
        {
            if (args.Length < 3)
            {
                System.Console.Out.WriteLine("\n\ntestParser2 <cmdl file> <true - output Parser results> <true - full parser experience>\n\n");
                return ;
            }
            TestParser2 tp = new TestParser2(args [0], args [1], args [2]);
        }
    }
}