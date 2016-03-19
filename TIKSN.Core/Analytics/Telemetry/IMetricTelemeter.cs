namespace TIKSN.Analytics.Telemetry
{
	public interface IMetricTelemeter
	{
		void TrackMetric(string metricName, double metricValue);
	}
}