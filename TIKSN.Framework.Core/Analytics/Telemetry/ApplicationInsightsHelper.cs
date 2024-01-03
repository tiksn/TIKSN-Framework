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

    internal static void TrackEvent(TelemetryClient client, EventTelemetry telemetry)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(telemetry);

        client.TrackEvent(telemetry);
    }

    internal static void TrackException(TelemetryClient client, ExceptionTelemetry telemetry)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(telemetry);

        client.TrackException(telemetry);
    }

    internal static void TrackMetric(TelemetryClient client, MetricTelemetry telemetry)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(telemetry);

        client.TrackMetric(telemetry);
    }

    internal static void TrackTrace(TelemetryClient client, TraceTelemetry telemetry)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(telemetry);

        client.TrackTrace(telemetry);
    }
}
