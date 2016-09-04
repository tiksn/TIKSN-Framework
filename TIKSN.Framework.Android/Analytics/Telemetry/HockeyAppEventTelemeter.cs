using HockeyApp.Android.Metrics;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
    public class HockeyAppEventTelemeter : HockeyAppTelemeterBase, IEventTelemeter
    {
        public Task TrackEvent(string name)
        {
            MetricsManager.TrackEvent(name);

            return Task.FromResult<object>(null);
        }
    }
}