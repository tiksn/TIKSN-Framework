using Microsoft.ApplicationInsights.DataContracts;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
	public class ApplicationInsightsMetricTelemeter : IMetricTelemeter
	{
		public async Task TrackMetric(string metricName, double metricValue)
		{
			var telemetry = new MetricTelemetry(metricName, metricValue);

			ApplicationInsightsHelper.TrackMetric(telemetry);
		}
	}
}