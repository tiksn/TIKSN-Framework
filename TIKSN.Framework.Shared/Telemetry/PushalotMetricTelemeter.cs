using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.Analytics.Telemetry;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry
{
	public class PushalotMetricTelemeter : PushalotTelemeterBase, IMetricTelemeter
	{
		public PushalotMetricTelemeter(IConfiguration<PushalotConfiguration> pushalotConfiguration)
			: base(pushalotConfiguration)
		{
		}

		public async Task TrackMetric(string metricName, decimal metricValue)
		{
			await SendMessage("Metric", string.Format("{0}: {1}", metricName, metricValue));
		}

		protected override IEnumerable<string> GetAuthorizationTokens(PushalotConfiguration pushalotConfiguration)
		{
			return pushalotConfiguration.MetricAuthorizationTokens;
		}

		protected override IEnumerable<string> GetAuthorizationTokens(PushalotConfiguration pushalotConfiguration, TelemetrySeverityLevel severityLevel)
		{
			return Enumerable.Empty<string>();
		}
	}
}