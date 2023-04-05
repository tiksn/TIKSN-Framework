namespace TIKSN.Analytics.Telemetry
{
    public interface IExceptionTelemeter
    {
        Task TrackExceptionAsync(Exception exception);

        Task TrackExceptionAsync(Exception exception, TelemetrySeverityLevel severityLevel);
    }
}
