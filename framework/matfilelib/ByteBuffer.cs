using System.Collections.Generic;
using System.IO;

namespace matfilelib
{
    class ByteBuffer : DataBuffer<byte>
    {
        public ByteBuffer(IEnumerable<byte> data)
            : base(data, MatlabType.MiUint8, sizeof(byte))
        {
        }

        protected override uint WriteDataElements(BinaryWriter binaryWriter)
        {
            uint bytesWritten = 0;
            foreach (var entry in Data)
            {
                binaryWriter.Write(entry);
                bytesWritten += sizeof(byte);
            }

            return bytesWritten;
        }
    }
}