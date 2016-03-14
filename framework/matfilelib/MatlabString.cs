using System;

namespace matfilelib
{
    public class MatlabString : MatrixElement
    {
        public MatlabString(string contents) : base(MatlabClass.MxCharacter, new [] { 1, (contents != null) ? contents.Length : 1 })
        {
            if (contents == null)
            {
                throw new ArgumentNullException();
            }

            Contents.Add(new CharacterBuffer(contents));
        }
    }
}
