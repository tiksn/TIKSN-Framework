namespace TIKSN.Analytics.Telemetry;

public interface ITraceTelemeter
{
    public Task TrackTraceAsync(string message);

    public Task TrackTraceAsync(string message, TelemetrySeverityLevel severityLevel);
}
