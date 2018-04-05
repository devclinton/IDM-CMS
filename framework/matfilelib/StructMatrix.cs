/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace matfilelib
{
    public class StructMatrix : MatrixElement
    {
        private readonly FieldNameLength _fieldNameLength;
        private readonly FieldNames _fieldNames;

        protected StructMatrix(IEnumerable<int> dimensions, IEnumerable<string> fieldNames)
            : base(MatlabClass.MxStructure, dimensions)
        {
            var enumerable = fieldNames as string[] ?? fieldNames.ToArray();
            if (enumerable.Length == 0)
            {
                throw new ArgumentException();
            }

            _fieldNameLength = new FieldNameLength((uint) (enumerable.Max(fn => fn.Length) + 1));
            _fieldNames = new FieldNames(enumerable);
            HeaderElements.Add(_fieldNameLength);
            HeaderElements.Add(_fieldNames);
        }
    }
}