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