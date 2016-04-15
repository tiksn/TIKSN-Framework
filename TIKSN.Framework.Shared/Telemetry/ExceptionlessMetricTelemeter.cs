using Exceptionless;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
	public class ExceptionlessMetricTelemeter : ExceptionlessTelemeterBase, IMetricTelemeter
	{
		public async Task TrackMetric(string metricName, decimal metricValue)
		{
			ExceptionlessClient.Default.CreateFeatureUsage(metricName).SetValue(metricValue).Submit();
		}
	}
}
