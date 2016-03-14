using System.IO;

namespace matfilelib
{
    class FieldNameLength : IElement
    {
        private readonly uint _fieldNameLength;

        public FieldNameLength(uint fieldNameLength)
        {
            _fieldNameLength = fieldNameLength;
        }

        public uint Size { get { return 8; } }

        public void WriteToStream(BinaryWriter binaryWriter)
        {
            const uint bytesAndType = (uint) ((4 << 16) + MatlabType.MiInt32);
            binaryWriter.Write(bytesAndType);
            binaryWriter.Write(_fieldNameLength);
        }
    }
}