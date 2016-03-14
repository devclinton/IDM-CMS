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
