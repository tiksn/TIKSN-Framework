using System.Collections.Generic;
using TIKSN.Analytics.Telemetry;

namespace TIKSN.Configuration
{
	public class PushalotConfiguration
	{
		public PushalotConfiguration(
			IEnumerable<string> eventAuthorizationTokens,
			IEnumerable<string> exceptionAuthorizationTokens,
			IEnumerable<string> metricAuthorizationTokens,
			IEnumerable<string> traceAuthorizationTokens)
		{
			this.EventAuthorizationTokens = eventAuthorizationTokens;
			this.ExceptionAuthorizationTokens = exceptionAuthorizationTokens;
			this.MetricAuthorizationTokens = metricAuthorizationTokens;
			this.TraceAuthorizationTokens = traceAuthorizationTokens;
		}

		public IEnumerable<string> EventAuthorizationTokens { get; }

		public IEnumerable<string> ExceptionAuthorizationTokens { get; }

		public IEnumerable<string> MetricAuthorizationTokens { get; }

		public IDictionary<TelemetrySeverityLevel, IEnumerable<string>> SeverityLevelExceptionAuthorizationTokens { get; }

		public IDictionary<TelemetrySeverityLevel, IEnumerable<string>> SeverityLevelTraceAuthorizationTokens { get; }

		public IEnumerable<string> TraceAuthorizationTokens { get; }
	}
}