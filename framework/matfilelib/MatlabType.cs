/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

namespace matfilelib
{
    enum MatlabType : uint
    {
        Unknown      = 0,
        MiInt8       = 1,
        MiUint8      = 2,
        MiInt16      = 3,
        MiUint16     = 4,
        MiInt32      = 5,
        MiUint32     = 6,
        MiSingle     = 7,
        MiDouble     = 9,
        MiInt64      = 12,
        MiUint64     = 13,
        MiMatrix     = 14,
        MiCompressed = 15,
        MiUtf8       = 16,
        MiUtf16      = 17,
        MiUtf32      = 18
    }
}
