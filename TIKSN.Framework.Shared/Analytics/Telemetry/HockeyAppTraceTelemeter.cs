using Microsoft.HockeyApp;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
    public class HockeyAppTraceTelemeter : HockeyAppTelemeterBase, ITraceTelemeter
    {
        public HockeyAppTraceTelemeter(IHockeyClient hockeyClient) : base(hockeyClient)
        {
        }

        public Task TrackTrace(string message)
        {
            hockeyClient.TrackTrace(message);

            return Task.FromResult<object>(null);
        }

        public Task TrackTrace(string message, TelemetrySeverityLevel severityLevel)
        {
            hockeyClient.TrackTrace(message, ConvertToSeverityLevel(severityLevel));

            return Task.FromResult<object>(null);
        }
    }
}