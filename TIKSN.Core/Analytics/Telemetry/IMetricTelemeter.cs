using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
	public interface IMetricTelemeter
	{
		Task TrackMetric(string metricName, double metricValue);
	}
}