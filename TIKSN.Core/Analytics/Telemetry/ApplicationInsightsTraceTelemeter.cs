using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.DataContracts;

namespace TIKSN.Analytics.Telemetry
{
    public class ApplicationInsightsTraceTelemeter : ITraceTelemeter
    {
        public Task TrackTrace(string message, TelemetrySeverityLevel severityLevel) =>
            this.TrackTraceInternal(message, severityLevel);

        public Task TrackTrace(string message) => this.TrackTraceInternal(message, null);

        private Task TrackTraceInternal(string message, TelemetrySeverityLevel? severityLevel)
        {
            try
            {
                var telemetry = new TraceTelemetry(message);
                telemetry.SeverityLevel = ApplicationInsightsHelper.ConvertSeverityLevel(severityLevel);
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
