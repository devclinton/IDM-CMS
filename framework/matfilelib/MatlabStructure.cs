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
