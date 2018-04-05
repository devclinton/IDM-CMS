/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System.Collections.Generic;

namespace matfilelib
{
    class FloatBuffer : DataBuffer<float>
    {
        public FloatBuffer(IEnumerable<float> data) : base(data, MatlabType.MiSingle, sizeof(float))
        {
        }

        protected override uint WriteDataElements(System.IO.BinaryWriter binaryWriter)
        {
            uint bytesWritten = 0;
            foreach (float entry in Data)
            {
                binaryWriter.Write(entry);
                bytesWritten += sizeof (float);
            }

            return bytesWritten;
        }
    }
}
