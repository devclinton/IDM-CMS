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
    public class TriggeredEvent : EventBase, IBoolean
    {
        private readonly IBoolean _predicate;

        public TriggeredEvent(TriggeredEventInfo eventInfo, IDictionary<string, IBoolean> bmap, IDictionary<string, IValue> nmap, IDictionary<string, IUpdateable> umap) :
            base(eventInfo, nmap, umap)
        {
            _predicate = eventInfo.Predicate.ResolveReferences(bmap, nmap);
            Repeats    = eventInfo.Repeats;
        }

        public TriggeredEventInfo Info { get { return info as TriggeredEventInfo; } }
        public bool Repeats { get; private set; }

        public bool Value
        {
            get { return _predicate.Value; }
        }
    }
}
