/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System.Collections.Generic;
using System.IO;

namespace matfilelib
{
    class ArrayDimensions : DataBuffer<int>
    {
        public ArrayDimensions(IEnumerable<int> dimensions) :
            base(dimensions, MatlabType.MiInt32, sizeof(int))
        {
        }

        protected override uint WriteDataElements(BinaryWriter binaryWriter)
        {
            uint bytesWritten = 0;
            foreach (int dimension in Data)
            {
                binaryWriter.Write(dimension);
                bytesWritten += sizeof (int);
            }

            return bytesWritten;
        }
    }
}