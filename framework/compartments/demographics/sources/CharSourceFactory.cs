/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System.IO;
using compartments.demographics.interfaces;

namespace compartments.demographics.sources
{
    class CharSourceFactory
    {
        public static ICharSource CharSourceFromFile(string fileName)
        {
            var reader = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            var iSource = new FileCharSource(reader);

            return iSource;
        }

        public static ICharSource CharSourceFromString(string source)
        {
            var iSource = new StringCharSource(source);

            return iSource;
        }
    }
}
