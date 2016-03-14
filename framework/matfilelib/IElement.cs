using System.IO;

namespace matfilelib
{
    public interface IElement
    {
        uint Size { get; }
        void WriteToStream(BinaryWriter binaryWriter);
    }
}