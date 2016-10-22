using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
    public class ApplicationInsightsTraceTelemeter : ITraceTelemeter
    {
        public async Task TrackTrace(string message, TelemetrySeverityLevel severityLevel)
        {
            await TrackTraceInternal(message, severityLevel);
        }

        public async Task TrackTrace(string message)
        {
            await TrackTraceInternal(message, null);
        }

        private async Task TrackTraceInternal(string message, TelemetrySeverityLevel? severityLevel)
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
        }
    }
}