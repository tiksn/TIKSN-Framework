using Microsoft.ApplicationInsights.DataContracts;

namespace TIKSN.Analytics.Telemetry
{
	public class ApplicationInsightsMetricTelemeter : IMetricTelemeter
	{
		public void TrackMetric(string metricName, double metricValue)
		{
			var telemetry = new MetricTelemetry(metricName, metricValue);

			ApplicationInsightsHelper.TrackMetric(telemetry);
		}
	}
}
