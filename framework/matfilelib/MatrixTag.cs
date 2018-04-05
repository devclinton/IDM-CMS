/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

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