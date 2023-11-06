using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.DataContracts;

namespace TIKSN.Analytics.Telemetry
{
    public class ApplicationInsightsTraceTelemeter : ITraceTelemeter
    {
        [Obsolete]
        public Task TrackTraceAsync(string message, TelemetrySeverityLevel severityLevel) =>
            TrackTraceInternalAsync(message, severityLevel);

        [Obsolete]
        public Task TrackTraceAsync(string message) => TrackTraceInternalAsync(message, severityLevel: null);

        [Obsolete]
        private static Task TrackTraceInternalAsync(string message, TelemetrySeverityLevel? severityLevel)
        {
            try
            {
                var telemetry = new TraceTelemetry(message)
                {
                    SeverityLevel = ApplicationInsightsHelper.ConvertSeverityLevel(severityLevel),
                };
                ApplicationInsightsHelper.TrackTrace(telemetry);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return Task.FromResult<object>(null);
        }
    }
}
