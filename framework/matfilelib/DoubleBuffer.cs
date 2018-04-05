/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System.Collections.Generic;
using System.IO;

namespace matfilelib
{
    class DoubleBuffer : DataBuffer<double>
    {
        public DoubleBuffer(IEnumerable<double> data) : base(data, MatlabType.MiDouble, sizeof(double))
        {
        }

        protected override uint WriteDataElements(BinaryWriter binaryWriter)
        {
            uint bytesWritten = 0;
            foreach (double entry in Data)
            {
                binaryWriter.Write(entry);
                bytesWritten += sizeof (double);
            }

            return bytesWritten;
        }
    }
}