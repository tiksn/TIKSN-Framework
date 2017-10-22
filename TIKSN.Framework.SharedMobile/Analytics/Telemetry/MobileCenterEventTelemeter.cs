using System.Collections.Generic;
using System.Threading.Tasks;
using MobileCenterAnalytics = Microsoft.Azure.Mobile.Analytics.Analytics;

namespace TIKSN.Analytics.Telemetry
{
    public class MobileCenterEventTelemeter : IEventTelemeter
    {
        public Task TrackEvent(string name)
        {
            return TrackEvent(name, null);
        }

        public Task TrackEvent(string name, IDictionary<string, string> properties)
        {
            MobileCenterAnalytics.TrackEvent(name, properties);

            return Task.FromResult<object>(null);
        }
    }
}