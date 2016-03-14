using System.Collections.Generic;
using System.IO;

namespace matfilelib
{
    class ShortBuffer : DataBuffer<short>
    {
        public ShortBuffer(IEnumerable<short> data)
            : base(data, MatlabType.MiInt16, sizeof (short))
        {
        }

        protected override uint WriteDataElements(BinaryWriter binaryWriter)
        {
            uint bytesWritten = 0;
            foreach (var entry in Data)
            {
                binaryWriter.Write(entry);
                bytesWritten += sizeof (short);
            }

            return bytesWritten;
        }
    }
}