using System.Collections.Generic;
using System.IO;

namespace matfilelib
{
    public class DotMatFile
    {
        private readonly FileHeader _fileHeader;
        private readonly Dictionary<string, MatrixElement> _elements;

        public DotMatFile()
        {
            _fileHeader = new FileHeader();
            _elements = new Dictionary<string, MatrixElement>();
        }

        public void WriteToDisk(string filename, bool compressed = false)
        {
            using (var fileStream = new FileStream(filename, FileMode.Create))
            {
                using (var binaryWriter = new BinaryWriter(fileStream))
                {
                    _fileHeader.WriteToStream(binaryWriter);
                    if (!compressed)
                    {
                        foreach (var element in _elements.Values)
                        {
                            element.WriteToStream(binaryWriter);
                        }
                    }
                    else
                    {
                        var compressedElement = new CompressedElement(_elements.Values);
                        compressedElement.WriteToStream(binaryWriter);
                    }
                }
            }
        }

        public MatrixElement this[string name]
        {
            get { return _elements[name]; }
            set
            {
                value.Name = name;
                if (!_elements.ContainsKey(name))
                {
                    _elements.Add(name, value);
                }
                else
                {
                    _elements[name] = value;
                }
            }
        }
    }
}