/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

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