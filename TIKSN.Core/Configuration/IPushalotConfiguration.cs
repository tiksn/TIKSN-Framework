using System.Collections.Generic;

namespace TIKSN.Configuration
{
	public interface IPushalotConfiguration
	{
		IEnumerable<string> EventAuthorizationTokens { get; }
		IEnumerable<string> ExceptionAuthorizationTokens { get; }
		IEnumerable<string> MetricAuthorizationTokens { get; }
		IEnumerable<string> TraceAuthorizationTokens { get; }
	}
}
