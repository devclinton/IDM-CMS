/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

namespace compartments.demographics.interfaces
{
    public interface ICharSource
    {
        bool EndOfFile { get; }
        char Current { get; }
        char Next();
        void Seek(uint offset);
    }
}
