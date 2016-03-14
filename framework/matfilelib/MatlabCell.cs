using System.Collections.Generic;

namespace matfilelib
{
    public class MatlabCell : MatrixElement
    {
        public MatlabCell() : this(new []{1,1}) {}

        public MatlabCell(IEnumerable<int> dimensions) : base(MatlabClass.MxCell, dimensions)
        {
        }
    }
}
