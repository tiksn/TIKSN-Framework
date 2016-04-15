using Microsoft.ApplicationInsights.DataContracts;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
	public class ApplicationInsightsMetricTelemeter : IMetricTelemeter
	{
		public async Task TrackMetric(string metricName, decimal metricValue)
		{
			var telemetry = new MetricTelemetry(metricName, (double)metricValue);

			ApplicationInsightsHelper.TrackMetric(telemetry);
		}
	}
}