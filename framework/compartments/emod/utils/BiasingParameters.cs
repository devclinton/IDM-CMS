/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using System.Collections.Generic;

namespace compartments.emod.utils
{
    public class BiasingParameters
    {
        public class RareEventSpec
        {
            public string ExpressionLocale;
            public string ExpressionName;
            public int IntermediateRareEventCount { get { return Thresholds.Count; } }
            public List<double> Thresholds { get; protected set; }

            public RareEventSpec()
            {
                ExpressionLocale = string.Empty;
                ExpressionName   = string.Empty;
                Thresholds       = new List<double>();
            }
        }

        public class RareEventInfo
        {
            private int _binCount;
            public int BinCount
            {
                get { return _binCount; }
                set
                {
                    if (value < 1)
                        throw new ArgumentException("BinCount must be >= 1.");

                    if (value != _binCount)
                    {
                        if (value > 1)
                        {
                            _binCount  = value;
                            Gammas     = new double[_binCount];
                            Thresholds = new double[_binCount - 1];
                        }
                        else
                        {
                            _binCount  = value;
                            Gammas     = new double[_binCount];
                            Thresholds = new double[_binCount];
                        }
                    }
                }
            }

            public double[] Gammas { get; protected set; }
            public double[] Thresholds { get; protected set; }

            public RareEventInfo()
            {
                Gammas     = new double[0];
                Thresholds = new double[0];
            }
        }

        public class ReactionInfo
        {
            public string Name;
            public List<RareEventInfo> RareEvents { get; protected set; }

            public ReactionInfo()
            {
                Name       = string.Empty;
                RareEvents = new List<RareEventInfo>();
            }
        }

        public class LocaleInfo
        {
            public string Name;
            public int ReactionCount { get { return Reactions.Count; } }
            public List<ReactionInfo> Reactions { get; protected set; }

            public LocaleInfo()
            {
                Name      = string.Empty;
                Reactions = new List<ReactionInfo>();
            }
        }

        public RareEventSpec RareEvent { get; protected set; }
        public List<LocaleInfo> Locales { get; protected set; }

        public BiasingParameters()
        {
            RareEvent = new RareEventSpec();
            Locales = new List<LocaleInfo>();
        }

        public static BiasingParameters LoadParametersFromJson(string jsonFilename)
        {
            return BiasingParametersDeserializer.ReadParametersFromJsonFile(jsonFilename);
        }

        public void WriteParametersToJsonFile(string jsonFilename)
        {
            BiasingParametersSerializer.WriteParametersToJsonFile(this, jsonFilename);
        }
    }
}
