/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using System.Reflection;

namespace compartments.emod.utils
{
    public class VersionInfo
    {
        public static string Version
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();

                object[] attributes = assembly.GetCustomAttributes(false);
                foreach (object obj in attributes)
                {
                    var fileVersion = obj as AssemblyFileVersionAttribute;
                    if (fileVersion != null)
                    {
                        return fileVersion.Version;
                    }
                }

                throw new ApplicationException("File version not found in assembly.");
            }
        }

        public static string Description
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();

                object[] attributes = assembly.GetCustomAttributes(false);
                foreach (object obj in attributes)
                {
                    var config = obj as AssemblyConfigurationAttribute;
                    if (config != null)
                    {
                        return config.Configuration;
                    }
                }

                throw new ApplicationException("Assembly description not found in assembly.");
            }
        }
    }
}
