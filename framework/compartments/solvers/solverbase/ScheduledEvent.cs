/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System.Collections.Generic;
using compartments.emod;
using compartments.emod.interfaces;

namespace compartments.solvers.solverbase
{
    public class ScheduledEvent : EventBase
    {
        public ScheduledEvent(ScheduledEventInfo eventInfo, IDictionary<string, IValue> nmap, IDictionary<string, IUpdateable> umap) :
            base(eventInfo, nmap, umap)
        {
            Time     = eventInfo.Time;
            Interval = eventInfo.Interval;
        }

        public ScheduledEventInfo Info { get { return info as ScheduledEventInfo; } }
        public double Interval { get; private set; }
        public double Time { get; set; }
    }
}
