using System.Collections.Generic;
using System.IO;

namespace matfilelib
{
    class DoubleBuffer : DataBuffer<double>
    {
        public DoubleBuffer(IEnumerable<double> data) : base(data, MatlabType.MiDouble, sizeof(double))
        {
        }

        protected override uint WriteDataElements(BinaryWriter binaryWriter)
        {
            uint bytesWritten = 0;
            foreach (double entry in Data)
            {
                binaryWriter.Write(entry);
                bytesWritten += sizeof (double);
            }

            return bytesWritten;
        }
    }
}