using System.Threading.Tasks;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry
{
	public class CompositeMetricTelemeter : IMetricTelemeter
	{
		private IConfiguration<CommonConfiguration> commonConfiguration;
		private IMetricTelemeter[] metricTelemeters;

		public CompositeMetricTelemeter(IConfiguration<CommonConfiguration> commonConfiguration, IMetricTelemeter[] metricTelemeters)
		{
			this.commonConfiguration = commonConfiguration;
			this.metricTelemeters = metricTelemeters;
		}

		public async Task TrackMetric(string metricName, decimal metricValue)
		{
			if (commonConfiguration.GetConfiguration().IsMetricTrackingEnabled)
			{
				foreach (var metricTelemeter in metricTelemeters)
				{
					await metricTelemeter.TrackMetric(metricName, metricValue);
				}
			}
		}
	}
}