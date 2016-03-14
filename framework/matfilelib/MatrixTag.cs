using System.IO;

namespace matfilelib
{
    class MatrixTag : IElement
    {
        private readonly MatrixElement _parent;

        public MatrixTag(MatrixElement parent)
        {
            _parent = parent;
        }

        public void WriteToStream(BinaryWriter binaryWriter)
        {
            binaryWriter.Write((uint)MatlabType.MiMatrix);
            binaryWriter.Write(_parent.Size - 8);
        }

        public uint Size { get { return 8; } }
    }
}