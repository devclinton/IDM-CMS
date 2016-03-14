using System.Collections.Generic;
using System.IO;

namespace matfilelib
{
    class CharacterBuffer : DataBuffer<char>
    {
        public CharacterBuffer(IEnumerable<char> data) : base(data, MatlabType.MiUint16, 2)
        {
        }

        protected override uint WriteDataElements(BinaryWriter binaryWriter)
        {
            uint bytesWritten = 0;
            foreach (var character in Data)
            {
                binaryWriter.Write((ushort) character);
                bytesWritten += sizeof (ushort);
            }

            return bytesWritten;
        }
    }
}