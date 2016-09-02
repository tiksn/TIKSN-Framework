using System;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
    public class HockeyAppMetricTelemeter : HockeyAppTelemeterBase, IMetricTelemeter
    {
        public Task TrackMetric(string metricName, decimal metricValue)
        {
            throw new NotImplementedException();
        }
    }
}