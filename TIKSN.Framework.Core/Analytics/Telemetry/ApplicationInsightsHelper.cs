using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace TIKSN.Analytics.Telemetry;

internal static class ApplicationInsightsHelper
{
    internal static SeverityLevel? ConvertSeverityLevel(TelemetrySeverityLevel? severityLevel)
    {
        if (severityLevel.HasValue)
        {
            return severityLevel.Value switch
            {
                TelemetrySeverityLevel.Verbose => SeverityLevel.Verbose,
                TelemetrySeverityLevel.Information => SeverityLevel.Information,
                TelemetrySeverityLevel.Warning => SeverityLevel.Warning,
                TelemetrySeverityLevel.Error => SeverityLevel.Error,
                TelemetrySeverityLevel.Critical => SeverityLevel.Critical,
                _ => throw new NotSupportedException(),
            };
        }

        return null;
    }

    [Obsolete]
    internal static void TrackEvent(EventTelemetry telemetry)
    {
        var client = new TelemetryClient();

        client.TrackEvent(telemetry);
    }

    [Obsolete]
    internal static void TrackException(ExceptionTelemetry telemetry)
    {
        var client = new TelemetryClient();

        client.TrackException(telemetry);
    }

    [Obsolete]
    internal static void TrackMetric(MetricTelemetry telemetry)
    {
        var client = new TelemetryClient();

        client.TrackMetric(telemetry);
    }

    [Obsolete]
    internal static void TrackTrace(TraceTelemetry telemetry)
    {
        var client = new TelemetryClient();

        client.TrackTrace(telemetry);
    }
}
