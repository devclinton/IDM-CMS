using System.Collections.Generic;
using System.IO;

namespace matfilelib
{
    class UintBuffer : DataBuffer<uint>
    {
        public UintBuffer(IEnumerable<uint> data)
            : base(data, MatlabType.MiUint32, sizeof(uint))
        {
        }

        protected override uint WriteDataElements(BinaryWriter binaryWriter)
        {
            uint bytesWritten = 0;
            foreach (var entry in Data)
            {
                binaryWriter.Write(entry);
                bytesWritten += sizeof(uint);
            }

            return bytesWritten;
        }
    }
}