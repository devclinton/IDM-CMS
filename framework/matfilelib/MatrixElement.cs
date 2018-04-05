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
    public class MatrixElement : IElement
    {
        private readonly MatrixTag _matrixTag;
        private readonly ClassTag _classTag;
        private readonly DataBuffer<int> _dimensions;
        private readonly ArrayName _arrayName;
        protected readonly List<IElement> HeaderElements;
        private readonly List<IElement> _contents;

        protected MatrixElement(MatlabClass matlabClass, IEnumerable<int> dimensions)
        {
            _matrixTag = new MatrixTag(this);
            _classTag = new ClassTag(matlabClass);
            _dimensions = new ArrayDimensions(dimensions);
            _arrayName = new ArrayName(string.Empty);
            HeaderElements = new List<IElement>(new IElement[] { _matrixTag, _classTag, _dimensions, _arrayName });
            _contents = new List<IElement>();
        }

        public string Name
        {
            get { return _arrayName.Name; }
            set { _arrayName.Name = value ?? string.Empty;}
        }

        public List<IElement> Contents
        {
            get { return _contents; }
        }

        public uint Size
        {
            get
            {
                uint size = HeaderElements.Aggregate<IElement, uint>(0, (current, element) => current + element.Size);

                return _contents.Aggregate(size, (current, entry) => current + entry.Size);
            }
        }

        public void WriteToStream(BinaryWriter binaryWriter)
        {
            foreach (var element in HeaderElements)
            {
                element.WriteToStream(binaryWriter);
            }

            foreach (var entry in _contents)
            {
                entry.WriteToStream(binaryWriter);
            }
        }
    }
}
