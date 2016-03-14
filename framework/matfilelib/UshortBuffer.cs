using System.Collections.Generic;
using System.IO;

namespace matfilelib
{
    class UshortBuffer : DataBuffer<ushort>
    {
        public UshortBuffer(IEnumerable<ushort> data)
            : base(data, MatlabType.MiUint16, sizeof(ushort))
        {
        }

        protected override uint WriteDataElements(BinaryWriter binaryWriter)
        {
            uint bytesWritten = 0;
            foreach (var entry in Data)
            {
                binaryWriter.Write(entry);
                bytesWritten += sizeof(ushort);
            }

            return bytesWritten;
        }
    }
}