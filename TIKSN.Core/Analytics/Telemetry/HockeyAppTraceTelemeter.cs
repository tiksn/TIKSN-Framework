using System;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
    public class HockeyAppTraceTelemeter : HockeyAppTelemeterBase, ITraceTelemeter
    {
        public Task TrackTrace(string message)
        {
            throw new NotImplementedException();
        }

        public Task TrackTrace(string message, TelemetrySeverityLevel severityLevel)
        {
            throw new NotImplementedException();
        }
    }
}