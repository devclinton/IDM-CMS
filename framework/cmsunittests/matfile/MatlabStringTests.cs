/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using NUnit.Framework;
using matfilelib;

namespace cmsunittests.matfile
{
    [TestFixture]
    class MatlabStringTests
    {
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void NullStringTest()
        {
            var matfile = new DotMatFile();
            var str = new MatlabString(null);
            matfile["stringNull"] = str;
            matfile.WriteToDisk("stringNull.mat");
        }

        [Test]
        public void EmptyStringTest()
        {
            var matfile = new DotMatFile();
            var str = new MatlabString(string.Empty);
            matfile["stringEmpty"] = str;
            matfile.WriteToDisk("stringEmpty.mat");
        }

        [Test]
        public void ShortStringTest()
        {
            var matfile = new DotMatFile();
            var str = new MatlabString("foo");
            matfile["stringFoo"] = str;
            matfile.WriteToDisk("stringFoo.mat");
        }

        [Test]
        public void LongStringTest()
        {
            var matfile = new DotMatFile();
            var str = new MatlabString("Now is the time for all good men to come to the aid of the party.");
            matfile["stringLong"] = str;
            matfile.WriteToDisk("stringLong.mat");
        }
    }
}
