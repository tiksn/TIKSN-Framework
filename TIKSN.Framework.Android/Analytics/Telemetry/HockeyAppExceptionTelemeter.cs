using System;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
    public class HockeyAppExceptionTelemeter : HockeyAppTelemeterBase, IExceptionTelemeter
    {
        public Task TrackException(Exception exception)
        {
            throw new NotImplementedException();
        }

        public Task TrackException(Exception exception, TelemetrySeverityLevel severityLevel)
        {
            throw new NotImplementedException();
        }
    }
}