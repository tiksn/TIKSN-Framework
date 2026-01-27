namespace TIKSN.Analytics.Telemetry;

public interface IMetricTelemeter
{
    public Task TrackMetricAsync(string metricName, decimal metricValue);
}
