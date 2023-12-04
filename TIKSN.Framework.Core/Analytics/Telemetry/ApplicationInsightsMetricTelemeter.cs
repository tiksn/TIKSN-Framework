using System.Diagnostics;
using Microsoft.ApplicationInsights.DataContracts;

namespace TIKSN.Analytics.Telemetry;

public class ApplicationInsightsMetricTelemeter : IMetricTelemeter
{
    [Obsolete]
    public Task TrackMetricAsync(string metricName, decimal metricValue)
    {
        try
        {
            var telemetry = new MetricTelemetry(metricName, (double)metricValue);
            ApplicationInsightsHelper.TrackMetric(telemetry);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }

        return Task.FromResult<object>(null);
    }
}
