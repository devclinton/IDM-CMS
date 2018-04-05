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
    public class MatlabMatrix : MatrixElement
    {
        public MatlabMatrix(IEnumerable<double> data, MatlabClass matlabClass = MatlabClass.MxDouble)
            : this(data, new[] { 1, data.Count() }, matlabClass)
        {
        }

        public MatlabMatrix(IEnumerable<double> data, int[] dimensions, MatlabClass matlabClass = MatlabClass.MxDouble)
            : base(matlabClass, dimensions)
        {
            int product = dimensions.Aggregate(1, (current, dimension) => current*dimension);

            if (data.Count() != product)
            {
                throw new ArgumentException("Data must contain enough entries for all dimensions.");
            }

            Contents.Add(new DoubleBuffer(data));
        }

        public MatlabMatrix(IEnumerable<float> data, MatlabClass matlabClass = MatlabClass.MxSingle)
            : this(data, new[] { 1, data.Count() }, matlabClass)
        {
        }

        public MatlabMatrix(IEnumerable<float> data, int[] dimensions, MatlabClass matlabClass = MatlabClass.MxSingle)
            : base(matlabClass, dimensions)
        {
            int product = dimensions.Aggregate(1, (current, dimension) => current * dimension);

            if (data.Count() != product)
            {
                throw new ArgumentException("Data must contain enough entries for all dimensions.");
            }

            Contents.Add(new FloatBuffer(data));
        }

        public MatlabMatrix(IEnumerable<byte> data, MatlabClass matlabClass = MatlabClass.MxDouble)
            : this(data, new[] { 1, data.Count() }, matlabClass)
        {
        }

        public MatlabMatrix(IEnumerable<byte> data, int[] dimensions, MatlabClass matlabClass = MatlabClass.MxDouble)
            : base(matlabClass, dimensions)
        {
            int product = dimensions.Aggregate(1, (current, dimension) => current * dimension);

            if (data.Count() != product)
            {
                throw new ArgumentException("Data must contain enough entries for all dimensions.");
            }

            Contents.Add(new ByteBuffer(data));
        }

        public MatlabMatrix(IEnumerable<SByte> data, MatlabClass matlabClass = MatlabClass.MxInt8)
            : this(data, new[] { 1, data.Count() }, matlabClass)
        {
        }

        public MatlabMatrix(IEnumerable<SByte> data, int[] dimensions, MatlabClass matlabClass = MatlabClass.MxInt8)
            : base(matlabClass, dimensions)
        {
            int product = dimensions.Aggregate(1, (current, dimension) => current * dimension);

            if (data.Count() != product)
            {
                throw new ArgumentException("Data must contain enough entries for all dimensions.");
            }

            Contents.Add(new SbyteBuffer(data));
        }

        public MatlabMatrix(IEnumerable<short> data, MatlabClass matlabClass = MatlabClass.MxDouble)
            : this(data, new[] {1, data.Count()}, matlabClass)
        {
        }

        public MatlabMatrix(IEnumerable<short> data, int[] dimensions, MatlabClass matlabClass = MatlabClass.MxDouble)
            : base(matlabClass, dimensions)
        {
            int product = dimensions.Aggregate(1, (current, dimension) => current*dimension);

            if (data.Count() != product)
            {
                throw new ArgumentException("Data must contain enough entries for all dimensions.");
            }

            Contents.Add(new ShortBuffer(data));
        }

        public MatlabMatrix(IEnumerable<ushort> data, MatlabClass matlabClass = MatlabClass.MxDouble)
            : this(data, new[] { 1, data.Count() }, matlabClass)
        {
        }

        public MatlabMatrix(IEnumerable<ushort> data, int[] dimensions, MatlabClass matlabClass = MatlabClass.MxDouble)
            : base(matlabClass, dimensions)
        {
            int product = dimensions.Aggregate(1, (current, dimension) => current * dimension);

            if (data.Count() != product)
            {
                throw new ArgumentException("Data must contain enough entries for all dimensions.");
            }

            Contents.Add(new UshortBuffer(data));
        }

        public MatlabMatrix(IEnumerable<int> data, MatlabClass matlabClass = MatlabClass.MxDouble)
            : this(data, new[] { 1, data.Count() }, matlabClass)
        {
        }

        public MatlabMatrix(IEnumerable<int> data, int[] dimensions, MatlabClass matlabClass = MatlabClass.MxDouble)
            : base(matlabClass, dimensions)
        {
            int product = dimensions.Aggregate(1, (current, dimension) => current * dimension);

            if (data.Count() != product)
            {
                throw new ArgumentException("Data must contain enough entries for all dimensions.");
            }

            Contents.Add(new IntegerBuffer(data));
        }

        public MatlabMatrix(IEnumerable<uint> data, MatlabClass matlabClass = MatlabClass.MxUint32)
            : this(data, new[] { 1, data.Count() }, matlabClass)
        {
        }

        public MatlabMatrix(IEnumerable<uint> data, int[] dimensions, MatlabClass matlabClass = MatlabClass.MxUint32)
            : base(matlabClass, dimensions)
        {
            int product = dimensions.Aggregate(1, (current, dimension) => current * dimension);

            if (data.Count() != product)
            {
                throw new ArgumentException("Data must contain enough entries for all dimensions.");
            }

            Contents.Add(new UintBuffer(data));
        }
    }
}
