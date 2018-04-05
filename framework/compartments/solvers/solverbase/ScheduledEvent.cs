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
