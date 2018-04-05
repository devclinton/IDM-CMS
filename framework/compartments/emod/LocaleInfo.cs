/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;

namespace compartments.emod
{
    public class LocaleInfo
    {
        private readonly String _name;

        public LocaleInfo(String name)
        {
            _name = name;
        }

        public String Name
        {
            get { return _name; }
        }

        public static implicit operator string(LocaleInfo l) { return l.Name; }

        public override String ToString() { return string.Format("(locale {0})", Name); }
    }
}
