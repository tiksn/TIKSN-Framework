using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry
{
	public class CompositeMetricTelemeter : IMetricTelemeter
	{
		private readonly IConfiguration<CommonTelemetryOptions> commonConfiguration;
		private readonly IEnumerable<IMetricTelemeter> metricTelemeters;

		public CompositeMetricTelemeter(IConfiguration<CommonTelemetryOptions> commonConfiguration, IEnumerable<IMetricTelemeter> metricTelemeters)
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
					try
					{
						await metricTelemeter.TrackMetric(metricName, metricValue);
					}
					catch (Exception ex)
					{
						Debug.WriteLine(ex);
					}
				}
			}
		}
	}
}