/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using System.Threading;
using IronScheme;
using IronScheme.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

using compartments.emod;

namespace compartments.emodl
{
    public class EmodlLoader
    {
        public static ModelInfo modelInfo;
        private static Exception _exception = null;

        public static ModelInfo LoadEMODLFile(string modelFileName)
        {
            modelInfo = null;

            var evt = new AutoResetEvent(false);
            _exception = null;
            var thread = new Thread(delegate()
                {
                    try
                    {
                        Console.WriteLine("Loading EMODL file '{0}'.", modelFileName);
                        "(load {0})".Eval(modelFileName);

                        Console.WriteLine("Finished loading file.");
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine("Caught exception loading '{0}': ", modelFileName);
                        Console.Error.WriteLine(ex.Message);
                        _exception = ex;
                    }
                    finally
                    {
                        evt.Set();
                    }
                }, 20*1024*1024);
            // Background threads don't prevent the process from exiting.
            thread.IsBackground = true;
            thread.Start();
            evt.WaitOne();

            if (_exception != null)
                throw _exception;

            return modelInfo;
        }

        public static ModelInfo LoadEMODLModel(string modelDescription)
        {
            modelInfo = null;

            Console.WriteLine("Loading model description (EMODL)...");
            try
            {
                modelDescription.Eval();

                Console.WriteLine("Finished loading model description (EMODL).");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Caught exception loading model description (EMODL): ");
                Console.Error.WriteLine(ex.Message);
                throw;
            }

            return modelInfo;
        }
    }
}
