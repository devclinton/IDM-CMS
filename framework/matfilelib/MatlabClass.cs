/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

namespace matfilelib
{
    public enum MatlabClass : uint
    {
        MxCell      = 1,
        MxStructure = 2,
        MxObject    = 3,
        MxCharacter = 4,
        MxSparse    = 5,
        MxDouble    = 6,
        MxSingle    = 7,
        MxInt8      = 8,
        MxUint8     = 9,
        MxInt16     = 10,
        MxUint16    = 11,
        MxInt32     = 12,
        MxUint32    = 13,
        MxInt64     = 14,
        MxUint64    = 15
    }
}
