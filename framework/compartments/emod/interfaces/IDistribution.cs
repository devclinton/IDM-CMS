/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using distlib.randomvariates;

namespace compartments.emod.interfaces
{
    interface IDistribution
    {
        double pdf();
        double pdf(double x);
        double cdf();
        double cdf(double x);
        void SetRNG(RandomVariateGenerator rng);
    }
}
