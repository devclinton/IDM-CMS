/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

// ReSharper disable InconsistentNaming
namespace distlib.randomvariates
{
    public interface RandomVariateGenerator
    {
        double GenerateUniformOO();
        double GenerateUniformOC();
        double GenerateUniformCO();
        double GenerateUniformCC();
    }
}
// ReSharper restore InconsistentNaming
