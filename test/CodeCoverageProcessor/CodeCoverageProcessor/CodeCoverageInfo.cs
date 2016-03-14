using Microsoft.VisualStudio.Coverage.Analysis;

namespace CodeCoverageProcessor
{
    class CodeCoverageInfo
    {
        private uint methodId;
        private string methodName;
        private string undecoratedMethodName;
        private string className;
        private string namespaceName;
        private CoverageStatistics statistics;

        public uint MethodId
        {
            get { return methodId; }
            set { methodId = value; }
        }

        public string MethodName
        {
            get { return methodName; }
            set { methodName = value; }
        }

        public string UndecoratedMethodName
        {
            get { return undecoratedMethodName; }
            set { undecoratedMethodName = value; }
        }

        public string ClassName
        {
            get { return className; }
            set { className = value; }
        }

        public string NamespaceName
        {
            get { return namespaceName; }
            set { namespaceName = value; }
        }

        public CoverageStatistics Statistics
        {
            get { return statistics; }
            set { statistics = value; }
        }
    }
}
