/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System.Collections.Generic;
using System.IO;

namespace matfilelib
{
    class UshortBuffer : DataBuffer<ushort>
    {
        public UshortBuffer(IEnumerable<ushort> data)
            : base(data, MatlabType.MiUint16, sizeof(ushort))
        {
        }

        protected override uint WriteDataElements(BinaryWriter binaryWriter)
        {
            uint bytesWritten = 0;
            foreach (var entry in Data)
            {
                binaryWriter.Write(entry);
                bytesWritten += sizeof(ushort);
            }

            return bytesWritten;
        }
    }
}