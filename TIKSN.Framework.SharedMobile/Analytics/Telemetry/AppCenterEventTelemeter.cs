using System.Collections.Generic;
using System.Threading.Tasks;
using AppCenterAnalytics = Microsoft.AppCenter.Analytics.Analytics;

namespace TIKSN.Analytics.Telemetry
{
    public class AppCenterEventTelemeter : IEventTelemeter
    {
        public Task TrackEventAsync(string name)
        {
            return TrackEventAsync(name, null);
        }

        public Task TrackEventAsync(string name, IDictionary<string, string> properties)
        {
            AppCenterAnalytics.TrackEvent(name, properties);

            return Task.FromResult<object>(null);
        }
    }
}
