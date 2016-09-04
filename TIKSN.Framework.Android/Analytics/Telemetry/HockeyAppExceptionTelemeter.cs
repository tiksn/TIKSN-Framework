using System;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
    public class HockeyAppExceptionTelemeter : HockeyAppTelemeterBase, IExceptionTelemeter
    {
        public Task TrackException(Exception exception)
        {
            //TODO: throw new NotImplementedException();

            return Task.FromResult<object>(null);
        }

        public Task TrackException(Exception exception, TelemetrySeverityLevel severityLevel)
        {
            //TODO: throw new NotImplementedException();

            return Task.FromResult<object>(null);
        }
    }
}