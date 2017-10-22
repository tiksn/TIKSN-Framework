using Microsoft.Azure.Mobile;
using System;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
    public class MobileCenterTraceTelemeter : ITraceTelemeter
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
                    MobileCenterLog.Verbose(MobileCenterLog.LogTag, message);
                    break;

                case TelemetrySeverityLevel.Information:
                    MobileCenterLog.Info(MobileCenterLog.LogTag, message);
                    break;

                case TelemetrySeverityLevel.Warning:
                    MobileCenterLog.Warn(MobileCenterLog.LogTag, message);
                    break;

                case TelemetrySeverityLevel.Error:
                    MobileCenterLog.Error(MobileCenterLog.LogTag, message);
                    break;

                case TelemetrySeverityLevel.Critical:
                    MobileCenterLog.Error(MobileCenterLog.LogTag, message);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(severityLevel));
            }

            return Task.CompletedTask;
        }
    }
}