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
