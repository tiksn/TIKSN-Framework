using HockeyApp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
    public class HockeyAppMetricTelemeter : HockeyAppTelemeterBase, IMetricTelemeter
    {
        public Task TrackMetric(string metricName, decimal metricValue)
        {
            var measurements = new Dictionary<string, double>();
            measurements.Add(metricName, (double)metricValue);

            MetricsManager.TrackEvent(metricName, null, measurements);

            return Task.FromResult<object>(null);
        }
    }
}