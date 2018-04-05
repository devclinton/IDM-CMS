/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System.Collections.Generic;

namespace matfilelib
{
    public class MatlabStructure : StructMatrix
    {
        public MatlabStructure(Dictionary<string, MatrixElement> structure)
            : base(new[] { 1, 1 }, structure.Keys)
        {
            foreach (var key in structure.Keys)
            {
                Contents.Add(structure[key]);
            }
        }
    }
}
