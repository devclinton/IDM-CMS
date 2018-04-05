/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using compartments.demographics.interfaces;

namespace compartments.demographics.sources
{
    public class StringCharSource : ICharSource
    {
        private readonly string _source;
        private uint _index;

        public StringCharSource(string source)
        {
            _source = source;
        }

        public bool EndOfFile
        {
            get { return _index >= _source.Length; }
        }

        public char Current
        {
            get { return _source[(int)_index]; }
        }

        public char Next()
        {
            return _source[(int)_index++];
        }

        public void Seek(uint offset)
        {
            if (offset < _source.Length)
            {
                _index = offset;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}
