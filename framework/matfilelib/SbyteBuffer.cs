using System;
using System.Collections.Generic;
using System.IO;

namespace matfilelib
{
    class SbyteBuffer : DataBuffer<sbyte>
    {
        public SbyteBuffer(IEnumerable<sbyte> data)
            : base(data, MatlabType.MiInt8, sizeof(SByte))
        {
        }

        protected override uint WriteDataElements(BinaryWriter binaryWriter)
        {
            uint bytesWritten = 0;
            foreach (var entry in Data)
            {
                binaryWriter.Write(entry);
                bytesWritten += sizeof(SByte);
            }

            return bytesWritten;
        }
    }
}