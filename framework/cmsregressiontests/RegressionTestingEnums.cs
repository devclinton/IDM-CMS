/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;

namespace cmsregressiontests
{
    // This is a test
    class RegressionTestingEnums
    {
        public enum OutputTypes
        {
            Csv,
            Txt,
            None
        };

        public static String OutputExtension(OutputTypes ot)
        {
            String extension = string.Empty;

            switch(ot)
            {
                case OutputTypes.Csv:
                    extension = ".csv";
                    break;
                case OutputTypes.Txt:
                    extension = ".txt";
                    break;
            }

            return extension;
        }

        public enum ValidationTypes
        {
            Validate,
            NoValidation,
            Probabilistic
        };
    }
}
