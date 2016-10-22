using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using System;

namespace TIKSN.Analytics.Telemetry
{
    internal static class ApplicationInsightsHelper
    {
        internal static SeverityLevel? ConvertSeverityLevel(TelemetrySeverityLevel? severityLevel)
        {
            if (severityLevel.HasValue)
            {
                switch (severityLevel.Value)
                {
                    case TelemetrySeverityLevel.Verbose:
                        return SeverityLevel.Verbose;

                    case TelemetrySeverityLevel.Information:
                        return SeverityLevel.Information;

                    case TelemetrySeverityLevel.Warning:
                        return SeverityLevel.Warning;

                    case TelemetrySeverityLevel.Error:
                        return SeverityLevel.Error;

                    case TelemetrySeverityLevel.Critical:
                        return SeverityLevel.Critical;

                    default:
                        throw new NotSupportedException();
                }
            }

            return null;
        }

        internal static void TrackEvent(EventTelemetry telemetry)
        {
            var client = new TelemetryClient();

            client.TrackEvent(telemetry);
        }

        internal static void TrackException(ExceptionTelemetry telemetry)
        {
            var client = new TelemetryClient();

            client.TrackException(telemetry);
        }

        internal static void TrackMetric(MetricTelemetry telemetry)
        {
            var client = new TelemetryClient();

            client.TrackMetric(telemetry);
        }

        internal static void TrackTrace(TraceTelemetry telemetry)
        {
            var client = new TelemetryClient();

            client.TrackTrace(telemetry);
        }
    }
}