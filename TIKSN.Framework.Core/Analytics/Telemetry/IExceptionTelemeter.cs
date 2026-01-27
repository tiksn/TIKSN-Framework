namespace TIKSN.Analytics.Telemetry;

public interface IExceptionTelemeter
{
    public Task TrackExceptionAsync(Exception exception);

    public Task TrackExceptionAsync(Exception exception, TelemetrySeverityLevel severityLevel);
}
