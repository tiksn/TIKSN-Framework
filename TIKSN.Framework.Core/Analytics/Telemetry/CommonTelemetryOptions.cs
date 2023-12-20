namespace TIKSN.Analytics.Telemetry;

public class CommonTelemetryOptions
{
    public CommonTelemetryOptions()
    {
        this.IsEventTrackingEnabled = true;
        this.IsExceptionTrackingEnabled = true;
        this.IsMetricTrackingEnabled = true;
        this.IsTraceTrackingEnabled = true;
    }

    public bool IsEventTrackingEnabled { get; set; }

    public bool IsExceptionTrackingEnabled { get; set; }

    public bool IsMetricTrackingEnabled { get; set; }

    public bool IsTraceTrackingEnabled { get; set; }
}
