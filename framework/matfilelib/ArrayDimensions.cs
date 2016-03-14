using System.Collections.Generic;
using System.IO;

namespace matfilelib
{
    class ArrayDimensions : DataBuffer<int>
    {
        public ArrayDimensions(IEnumerable<int> dimensions) :
            base(dimensions, MatlabType.MiInt32, sizeof(int))
        {
        }

        protected override uint WriteDataElements(BinaryWriter binaryWriter)
        {
            uint bytesWritten = 0;
            foreach (int dimension in Data)
            {
                binaryWriter.Write(dimension);
                bytesWritten += sizeof (int);
            }

            return bytesWritten;
        }
    }
}