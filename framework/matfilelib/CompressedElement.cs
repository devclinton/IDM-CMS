/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System.Collections.Generic;
using System.IO;

namespace matfilelib
{
    class CompressedElement : IElement
    {
        private readonly IEnumerable<MatrixElement> _contents;
        private readonly CompressedData _compressedData;

        public CompressedElement(IEnumerable<MatrixElement> contents)
        {
            _contents = contents;
            _compressedData = new CompressedData(_contents);
        }

        public uint Size
        {
            get
            {
                return 8 + _compressedData.Size;
            }
        }

        public void WriteToStream(BinaryWriter binaryWriter)
        {
            binaryWriter.Write((uint)MatlabType.MiCompressed);
            binaryWriter.Write(_compressedData.Size);
            _compressedData.WriteToStream(binaryWriter);
        }
    }
}