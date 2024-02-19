using System.Diagnostics;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace TIKSN.Analytics.Telemetry;

public class ApplicationInsightsTraceTelemeter : ITraceTelemeter
{
    private readonly TelemetryClient client;

    public ApplicationInsightsTraceTelemeter(TelemetryClient client)
        => this.client = client ?? throw new ArgumentNullException(nameof(client));

    public Task TrackTraceAsync(string message, TelemetrySeverityLevel severityLevel)
        => this.TrackTraceInternalAsync(message, severityLevel);

    public Task TrackTraceAsync(string message)
        => this.TrackTraceInternalAsync(message, severityLevel: null);

    private Task TrackTraceInternalAsync(string message, TelemetrySeverityLevel? severityLevel)
    {
#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            var telemetry = new TraceTelemetry(message)
            {
                SeverityLevel = ApplicationInsightsHelper.ConvertSeverityLevel(severityLevel),
            };
            ApplicationInsightsHelper.TrackTrace(this.client, telemetry);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
#pragma warning restore CA1031 // Do not catch general exception types

        return Task.CompletedTask;
    }
}
