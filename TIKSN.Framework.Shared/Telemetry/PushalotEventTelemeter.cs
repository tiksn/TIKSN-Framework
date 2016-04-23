using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry
{
	public class PushalotEventTelemeter : PushalotTelemeterBase, IEventTelemeter
	{
		public PushalotEventTelemeter(IConfiguration<PushalotConfiguration> pushalotConfiguration)
			: base(pushalotConfiguration)
		{
		}

		public async Task TrackEvent(string name)
		{
			await SendMessage("Event", name);
		}

		protected override IEnumerable<string> GetAuthorizationTokens(PushalotConfiguration pushalotConfiguration)
		{
			return pushalotConfiguration.EventAuthorizationTokens;
		}

		protected override IEnumerable<string> GetAuthorizationTokens(PushalotConfiguration pushalotConfiguration, TelemetrySeverityLevel severityLevel)
		{
			return Enumerable.Empty<string>();
		}
	}
}