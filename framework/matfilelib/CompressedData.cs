using System.Collections.Generic;
using System.IO;
using Ionic.Zlib;
using CompressionMode = Ionic.Zlib.CompressionMode;

namespace matfilelib
{
    class CompressedData : IElement
    {
        private readonly IEnumerable<MatrixElement> _elements;
        private readonly byte[] _bytes;

        public CompressedData(IEnumerable<MatrixElement> elements)
        {
            _elements = elements;

            // Get uncompressed data
            byte[] uncompressed;
            using (var memoryStream = new MemoryStream())
            {
                using (var binaryWriter = new BinaryWriter(memoryStream))
                {
                    foreach (var element in _elements)
                    {
                        element.WriteToStream(binaryWriter);
                    }
                    binaryWriter.Flush();
                    memoryStream.Flush();
                    uncompressed = memoryStream.ToArray();
                }
            }

            // Get compressed data
            using (var memoryStream = new MemoryStream())
            {
                using (var compressor = new ZlibStream(memoryStream, CompressionMode.Compress, CompressionLevel.BestCompression))
                {
                    compressor.Write(uncompressed, 0, uncompressed.Length);
                }
                _bytes = memoryStream.ToArray();
            }
        }

        public uint Size
        {
            get
            {
                return (uint)_bytes.Length;
            }
        }

        public void WriteToStream(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(_bytes);
        }
    }
}