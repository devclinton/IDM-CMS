/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

namespace compartments.solvers.solverbase
{
    public class JsonOutputOptions
    {
        public string Filename { get; set; }
        public bool ChannelTitles { get; set; }
        public bool CompressOutput { get; set; }
        public bool WriteJsonFile { get; set; }
    }
}