using Microsoft.AppCenter;
using System;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
    public class AppCenterTraceTelemeter : ITraceTelemeter
    {
        public Task TrackTrace(string message)
        {
            return TrackTrace(message, TelemetrySeverityLevel.Information);
        }

        public Task TrackTrace(string message, TelemetrySeverityLevel severityLevel)
        {
            switch (severityLevel)
            {
                case TelemetrySeverityLevel.Verbose:
                    AppCenterLog.Verbose(AppCenterLog.LogTag, message);
                    break;

                case TelemetrySeverityLevel.Information:
                    AppCenterLog.Info(AppCenterLog.LogTag, message);
                    break;

                case TelemetrySeverityLevel.Warning:
                    AppCenterLog.Warn(AppCenterLog.LogTag, message);
                    break;

                case TelemetrySeverityLevel.Error:
                case TelemetrySeverityLevel.Critical:
                    AppCenterLog.Error(AppCenterLog.LogTag, message);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(severityLevel));
            }

            return Task.CompletedTask;
        }
    }
}