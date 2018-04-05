/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

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
