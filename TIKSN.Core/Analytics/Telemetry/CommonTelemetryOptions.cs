namespace TIKSN.Analytics.Telemetry
{
    public class CommonTelemetryOptions
    {
        public CommonTelemetryOptions()
        {
            IsEventTrackingEnabled = true;
            IsExceptionTrackingEnabled = true;
            IsMetricTrackingEnabled = true;
            IsTraceTrackingEnabled = true;
        }

        public bool IsEventTrackingEnabled { get; set; }

        public bool IsExceptionTrackingEnabled { get; set; }

        public bool IsMetricTrackingEnabled { get; set; }

        public bool IsTraceTrackingEnabled { get; set; }
    }
}