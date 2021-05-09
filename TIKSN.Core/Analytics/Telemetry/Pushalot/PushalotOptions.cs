using System.Collections.Generic;

namespace TIKSN.Analytics.Telemetry.Pushalot
{
    public class PushalotOptions
    {
        public PushalotOptions()
        {
            this.EventAuthorizationTokens = new List<string>();
            this.ExceptionAuthorizationTokens = new List<string>();
            this.MetricAuthorizationTokens = new List<string>();
            this.TraceAuthorizationTokens = new List<string>();
            this.SeverityLevelExceptionAuthorizationTokens = new Dictionary<TelemetrySeverityLevel, List<string>>();
            this.SeverityLevelTraceAuthorizationTokens = new Dictionary<TelemetrySeverityLevel, List<string>>();
        }

        public List<string> EventAuthorizationTokens { get; }

        public List<string> ExceptionAuthorizationTokens { get; }

        public List<string> MetricAuthorizationTokens { get; }

        public Dictionary<TelemetrySeverityLevel, List<string>> SeverityLevelExceptionAuthorizationTokens { get; }

        public Dictionary<TelemetrySeverityLevel, List<string>> SeverityLevelTraceAuthorizationTokens { get; }

        public List<string> TraceAuthorizationTokens { get; }
    }
}
