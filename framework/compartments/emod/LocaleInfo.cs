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
