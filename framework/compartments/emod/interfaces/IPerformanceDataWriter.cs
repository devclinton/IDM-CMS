/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using compartments.emod.perf;

namespace compartments.emod.interfaces
{
    public interface IPerformanceDataWriter
    {
        void WritePerformanceMeasurements(PerformanceMeasurements measurements,
                                          PerformanceMeasurementConfigurationParameters perfConfig);
    }
}
