using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry.Pushalot
{
	public class PushalotMetricTelemeter : PushalotTelemeterBase, IMetricTelemeter
	{
		public PushalotMetricTelemeter(IPartialConfiguration<PushalotOptions> pushalotConfiguration)
			: base(pushalotConfiguration)
		{
		}

		public async Task TrackMetric(string metricName, decimal metricValue)
		{
			await SendMessage("Metric", string.Format("{0}: {1}", metricName, metricValue));
		}

		protected override IEnumerable<string> GetAuthorizationTokens(PushalotOptions pushalotConfiguration)
		{
			return pushalotConfiguration.MetricAuthorizationTokens;
		}

		protected override IEnumerable<string> GetAuthorizationTokens(PushalotOptions pushalotConfiguration, TelemetrySeverityLevel severityLevel)
		{
			return Enumerable.Empty<string>();
		}
	}
}