using Microsoft.HockeyApp;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
    public class HockeyAppEventTelemeter : HockeyAppTelemeterBase, IEventTelemeter
    {
        public HockeyAppEventTelemeter(IHockeyClient hockeyClient) : base(hockeyClient)
        {
        }

        public Task TrackEvent(string name)
        {
            hockeyClient.TrackEvent(name);

            return Task.FromResult<object>(null);
        }
    }
}