using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
	public class ApplicationInsightsMetricTelemeter : IMetricTelemeter
	{
		public async Task TrackMetric(string metricName, decimal metricValue)
		{
			try
			{
				var telemetry = new MetricTelemetry(metricName, (double)metricValue);
				ApplicationInsightsHelper.TrackMetric(telemetry);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}
	}
}