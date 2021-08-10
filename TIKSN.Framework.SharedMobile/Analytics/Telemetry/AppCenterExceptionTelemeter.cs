using System;
using System.Threading.Tasks;
using Microsoft.AppCenter;

namespace TIKSN.Analytics.Telemetry
{
    public class AppCenterExceptionTelemeter : IExceptionTelemeter
    {
        public Task TrackExceptionAsync(Exception exception)
        {
            return TrackExceptionAsync(exception, TelemetrySeverityLevel.Information);
        }

        public Task TrackExceptionAsync(Exception exception, TelemetrySeverityLevel severityLevel)
        {
            switch (severityLevel)
            {
                case TelemetrySeverityLevel.Verbose:
                    AppCenterLog.Verbose(AppCenterLog.LogTag, exception.Message, exception);
                    break;

                case TelemetrySeverityLevel.Information:
                    AppCenterLog.Info(AppCenterLog.LogTag, exception.Message, exception);
                    break;

                case TelemetrySeverityLevel.Warning:
                    AppCenterLog.Warn(AppCenterLog.LogTag, exception.Message, exception);
                    break;

                case TelemetrySeverityLevel.Error:
                case TelemetrySeverityLevel.Critical:
                    AppCenterLog.Error(AppCenterLog.LogTag, exception.Message, exception);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(severityLevel));
            }

            return Task.CompletedTask;
        }
    }
}
