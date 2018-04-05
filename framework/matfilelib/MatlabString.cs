/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

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
