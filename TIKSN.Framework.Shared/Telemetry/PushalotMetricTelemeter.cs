using System;
using System.Collections.Generic;
using TIKSN.Analytics.Telemetry;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry
{
	public class PushalotMetricTelemeter : PushalotTelemeterBase, IMetricTelemeter
	{
		public PushalotMetricTelemeter(IPushalotConfiguration pushalotConfiguration)
			:base(pushalotConfiguration)
		{

		}

		public void TrackMetric(string metricName, double metricValue)
		{
			SendMessage("Metric", string.Format("{0}: {1}", metricName, metricValue));
		}

		protected override IEnumerable<string> GetAuthorizationTokens(IPushalotConfiguration pushalotConfiguration)
		{
			return pushalotConfiguration.MetricAuthorizationTokens;
		}
	}
}
