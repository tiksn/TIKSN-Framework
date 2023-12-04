namespace TIKSN.Analytics.Telemetry;

public interface ITraceTelemeter
{
    Task TrackTraceAsync(string message);

    Task TrackTraceAsync(string message, TelemetrySeverityLevel severityLevel);
}
