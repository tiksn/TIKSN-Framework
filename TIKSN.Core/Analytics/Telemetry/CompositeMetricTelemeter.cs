using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry
{
	public class CompositeMetricTelemeter : IMetricTelemeter
	{
		private ICommonConfiguration commonConfiguration;
		private IMetricTelemeter[] metricTelemeters;

		public CompositeMetricTelemeter(ICommonConfiguration commonConfiguration, IMetricTelemeter[] metricTelemeters)
		{
			this.commonConfiguration = commonConfiguration;
			this.metricTelemeters = metricTelemeters;
		}

		public void TrackMetric(string metricName, double metricValue)
		{
			if (commonConfiguration.IsMetricTrackingEnabled)
			{
				foreach (var metricTelemeter in metricTelemeters)
				{
					metricTelemeter.TrackMetric(metricName, metricValue);
				}
			}
		}
	}
}