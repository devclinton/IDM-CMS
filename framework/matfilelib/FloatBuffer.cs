using System.Collections.Generic;

namespace matfilelib
{
    class FloatBuffer : DataBuffer<float>
    {
        public FloatBuffer(IEnumerable<float> data) : base(data, MatlabType.MiSingle, sizeof(float))
        {
        }

        protected override uint WriteDataElements(System.IO.BinaryWriter binaryWriter)
        {
            uint bytesWritten = 0;
            foreach (float entry in Data)
            {
                binaryWriter.Write(entry);
                bytesWritten += sizeof (float);
            }

            return bytesWritten;
        }
    }
}
