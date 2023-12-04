namespace TIKSN.Analytics.Telemetry;

public interface IMetricTelemeter
{
    Task TrackMetricAsync(string metricName, decimal metricValue);
}
