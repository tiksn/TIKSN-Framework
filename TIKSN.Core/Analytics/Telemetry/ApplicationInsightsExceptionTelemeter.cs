using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
    public class ApplicationInsightsExceptionTelemeter : IExceptionTelemeter
    {
        public async Task TrackException(Exception exception)
        {
            await TrackExceptionInternal(exception, null);
        }

        public Task TrackException(Exception exception, TelemetrySeverityLevel severityLevel)
        {
            return TrackExceptionInternal(exception, severityLevel);
        }

        private Task TrackExceptionInternal(Exception exception, TelemetrySeverityLevel? severityLevel)
        {
            try
            {
                var telemetry = new ExceptionTelemetry(exception);
                telemetry.SeverityLevel = ApplicationInsightsHelper.ConvertSeverityLevel(severityLevel);
                ApplicationInsightsHelper.TrackException(telemetry);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return Task.FromResult<object>(null);
        }
    }
}