/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System.IO;

namespace matfilelib
{
    class ClassTag : IElement
    {
        private readonly MatlabClass _matlabClass;

        public ClassTag(MatlabClass matlabClass)
        {
            _matlabClass = matlabClass;
        }

        public void WriteToStream(BinaryWriter binaryWriter)
        {
            binaryWriter.Write((uint) MatlabType.MiUint32);
            binaryWriter.Write((uint) 8);
            binaryWriter.Write((uint) _matlabClass);
            binaryWriter.Write((uint) 0x0BADF00D);
        }

        public uint Size { get { return (4*sizeof (uint)); } }
    }
}