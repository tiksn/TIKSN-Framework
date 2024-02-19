using System.Diagnostics;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace TIKSN.Analytics.Telemetry;

public class ApplicationInsightsExceptionTelemeter : IExceptionTelemeter
{
    private readonly TelemetryClient client;

    public ApplicationInsightsExceptionTelemeter(TelemetryClient client)
        => this.client = client ?? throw new ArgumentNullException(nameof(client));

    public Task TrackExceptionAsync(Exception exception)
        => this.TrackExceptionInternalAsync(exception, severityLevel: null);

    public Task TrackExceptionAsync(Exception exception, TelemetrySeverityLevel severityLevel)
        => this.TrackExceptionInternalAsync(exception, severityLevel);

    private Task TrackExceptionInternalAsync(Exception exception, TelemetrySeverityLevel? severityLevel)
    {
#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            var telemetry = new ExceptionTelemetry(exception)
            {
                SeverityLevel = ApplicationInsightsHelper.ConvertSeverityLevel(severityLevel),
            };
            ApplicationInsightsHelper.TrackException(this.client, telemetry);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
#pragma warning restore CA1031 // Do not catch general exception types

        return Task.CompletedTask;
    }
}
