using System;
using System.IO;
using System.Text;

namespace matfilelib
{
    internal class FileHeader : IElement
    {
        private const UInt32 Subsystem = 0;
        private const UInt16 Version = 0x0100;
        private const UInt16 ByteOrder = 0x4D49; // "MI"

        public uint Size { get { throw new NotImplementedException(); } }

        public void WriteToStream(BinaryWriter binaryWriter)
        {
            var sb = new StringBuilder(116, 116);
            sb.Append("MATLAB 5.0 MAT-file , Platform: EMODCMS, Created on ");
            sb.Append(DateTime.Now.ToString("ddd MMM dd HH:mm:ss yyyy"));
            while (sb.Length < 116)
            {
                sb.Append(' ');
            }

            binaryWriter.Write(sb.ToString().ToCharArray());
            binaryWriter.Write(Subsystem);
            binaryWriter.Write(Subsystem);
            binaryWriter.Write(Version);
            binaryWriter.Write(ByteOrder);
        }
    }
}