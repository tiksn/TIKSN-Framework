using System.Collections.Generic;

namespace TIKSN.Analytics.Telemetry.Pushalot
{
    public class PushalotOptions
    {
        public PushalotOptions()
        {
            EventAuthorizationTokens = new List<string>();
            ExceptionAuthorizationTokens = new List<string>();
            MetricAuthorizationTokens = new List<string>();
            TraceAuthorizationTokens = new List<string>();
            SeverityLevelExceptionAuthorizationTokens = new Dictionary<TelemetrySeverityLevel, List<string>>();
            SeverityLevelTraceAuthorizationTokens = new Dictionary<TelemetrySeverityLevel, List<string>>();
        }

        public List<string> EventAuthorizationTokens { get; }

        public List<string> ExceptionAuthorizationTokens { get; }

        public List<string> MetricAuthorizationTokens { get; }

        public Dictionary<TelemetrySeverityLevel, List<string>> SeverityLevelExceptionAuthorizationTokens { get; }

        public Dictionary<TelemetrySeverityLevel, List<string>> SeverityLevelTraceAuthorizationTokens { get; }

        public List<string> TraceAuthorizationTokens { get; }
    }
}