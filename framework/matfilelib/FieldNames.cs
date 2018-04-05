/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace matfilelib
{
    class FieldNames : IElement
    {
        private readonly IEnumerable<string> _fieldNames;
        private readonly uint _fieldNameLength;

        public FieldNames(IEnumerable<string> fieldNames)
        {
            var enumerable = fieldNames as string[] ?? fieldNames.ToArray();
            _fieldNames = enumerable;
            _fieldNameLength = (uint) (enumerable.Max(fn => fn.Length) + 1);
        }

        public uint Size { get { return (uint) ((8 + (_fieldNames.Count() * _fieldNameLength) + 7) & ~7); } }

        public void WriteToStream(BinaryWriter binaryWriter)
        {
            binaryWriter.Write((uint) MatlabType.MiInt8);
            binaryWriter.Write((uint)(_fieldNames.Count() * _fieldNameLength));
            foreach (var fieldName in _fieldNames)
            {
                binaryWriter.Write(fieldName.ToCharArray());
                for (int written = fieldName.Length; written < _fieldNameLength; written++)
                {
                    binaryWriter.Write('\0');
                }
            }
            for (var written = (uint) (_fieldNames.Count() * _fieldNameLength); (written & 7) != 0; written++)
            {
                binaryWriter.Write('\0');
            }
        }
    }
}