using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry
{
	public class PushalotTraceTelemeter : PushalotTelemeterBase, ITraceTelemeter
	{
		public PushalotTraceTelemeter(IConfiguration<PushalotConfiguration> pushalotConfiguration)
			: base(pushalotConfiguration)
		{
		}

		public async Task TrackTrace(string message)
		{
			await SendMessage("Trace", message);
		}

		public async Task TrackTrace(string message, TelemetrySeverityLevel severityLevel)
		{
			await SendMessage(string.Format("Trace: {0}", severityLevel), message);
		}

		protected override IEnumerable<string> GetAuthorizationTokens(PushalotConfiguration pushalotConfiguration)
		{
			return pushalotConfiguration.TraceAuthorizationTokens;
		}

		protected override IEnumerable<string> GetAuthorizationTokens(PushalotConfiguration pushalotConfiguration, TelemetrySeverityLevel severityLevel)
		{
			if (pushalotConfiguration.SeverityLevelTraceAuthorizationTokens.ContainsKey(severityLevel))
			{
				return pushalotConfiguration.SeverityLevelTraceAuthorizationTokens[severityLevel];
			}

			return Enumerable.Empty<string>();
		}
	}
}