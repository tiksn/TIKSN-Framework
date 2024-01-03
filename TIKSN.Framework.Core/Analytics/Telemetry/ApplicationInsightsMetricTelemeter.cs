using System.Diagnostics;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace TIKSN.Analytics.Telemetry;

public class ApplicationInsightsMetricTelemeter : IMetricTelemeter
{
    private readonly TelemetryClient client;

    public ApplicationInsightsMetricTelemeter(TelemetryClient client)
        => this.client = client ?? throw new ArgumentNullException(nameof(client));

    public Task TrackMetricAsync(string metricName, decimal metricValue)
    {
#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            var telemetry = new MetricTelemetry(metricName, (double)metricValue);
            ApplicationInsightsHelper.TrackMetric(this.client, telemetry);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
#pragma warning restore CA1031 // Do not catch general exception types

        return Task.CompletedTask;
    }
}
