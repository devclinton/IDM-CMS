using System.IO;
using compartments.demographics.interfaces;

namespace compartments.demographics.sources
{
    class FileCharSource : ICharSource
    {
        private readonly FileStream _stream;
        private TextReader _reader;
        private readonly char[] _data = new char[4096];
        private int _count;
        private int _index;

        public FileCharSource(FileStream stream)
        {
            _stream = stream;
            _reader = new StreamReader(_stream);
            GetMoreData();
        }

        private void GetMoreData()
        {
            _count = _reader.Read(_data, 0, _data.Length);
            _index = 0;
        }

        public bool EndOfFile { get { return _count == 0; } }

        public char Current
        {
            get
            {
                return _data[_index];
            }
        }

        public char Next()
        {
            char next = _data[_index++];

            if (_index >= _count)
            {
                GetMoreData();
            }

            return next;
        }

        public void Seek(uint offset)
        {
            _stream.Seek(offset, SeekOrigin.Begin);
            _reader = new StreamReader(_stream);
            GetMoreData();
        }
    }
}
