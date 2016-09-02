using Microsoft.HockeyApp;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
    public class HockeyAppMetricTelemeter : HockeyAppTelemeterBase, IMetricTelemeter
    {
        public HockeyAppMetricTelemeter(IHockeyClient hockeyClient) : base(hockeyClient)
        {
        }

        public Task TrackMetric(string metricName, decimal metricValue)
        {
            hockeyClient.TrackMetric(metricName, (double)metricValue);

            return Task.FromResult<object>(null);
        }
    }
}