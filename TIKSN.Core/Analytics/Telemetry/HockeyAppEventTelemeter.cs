﻿using System;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
    public class HockeyAppEventTelemeter : HockeyAppTelemeterBase, IEventTelemeter
    {
        public Task TrackEvent(string name)
        {
            throw new NotImplementedException();
        }
    }
}