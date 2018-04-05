/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

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