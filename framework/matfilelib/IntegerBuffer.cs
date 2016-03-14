using System.Collections.Generic;
using System.IO;

namespace matfilelib
{
    class IntegerBuffer : DataBuffer<int>
    {
        public IntegerBuffer(IEnumerable<int> data) : base(data, MatlabType.MiInt32, sizeof(int))
        {
        }

        protected override uint WriteDataElements(BinaryWriter binaryWriter)
        {
            uint bytesWritten = 0;
            foreach (var entry in Data)
            {
                binaryWriter.Write(entry);
                bytesWritten += sizeof (int);
            }

            return bytesWritten;
        }
    }
}