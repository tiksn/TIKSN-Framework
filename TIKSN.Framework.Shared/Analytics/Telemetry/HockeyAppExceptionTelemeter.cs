using Microsoft.HockeyApp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
    public class HockeyAppExceptionTelemeter : HockeyAppTelemeterBase, IExceptionTelemeter
    {
        public HockeyAppExceptionTelemeter(IHockeyClient hockeyClient) : base(hockeyClient)
        {
        }

        public Task TrackException(Exception exception)
        {
            hockeyClient.TrackException(exception);

            return Task.FromResult<object>(null);
        }

        public Task TrackException(Exception exception, TelemetrySeverityLevel severityLevel)
        {
            var properties = new Dictionary<string, string> { { "SeverityLevel", severityLevel.ToString() } };

            hockeyClient.TrackException(exception, properties);

            return Task.FromResult<object>(null);
        }
    }
}