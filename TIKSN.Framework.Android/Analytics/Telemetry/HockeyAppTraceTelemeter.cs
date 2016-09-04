using HockeyApp.Android.Utils;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
    public class HockeyAppTraceTelemeter : HockeyAppTelemeterBase, ITraceTelemeter
    {
        public Task TrackTrace(string message)
        {
            return TrackTrace(message, TelemetrySeverityLevel.Verbose);
        }

        public Task TrackTrace(string message, TelemetrySeverityLevel severityLevel)
        {
            switch (severityLevel)
            {
                case TelemetrySeverityLevel.Verbose:
                    HockeyLog.Verbose(message);
                    break;

                case TelemetrySeverityLevel.Information:
                    HockeyLog.Info(message);
                    break;

                case TelemetrySeverityLevel.Warning:
                    HockeyLog.Warn(message);
                    break;

                case TelemetrySeverityLevel.Error:
                    HockeyLog.Error(message);
                    break;

                case TelemetrySeverityLevel.Critical:
                    HockeyLog.Error(message);
                    break;

                default:
                    HockeyLog.Verbose(message);
                    break;
            }

            return Task.FromResult<object>(null);
        }
    }
}