using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry.Pushalot
{
    public class PushalotEventTelemeter : PushalotTelemeterBase, IEventTelemeter
    {
        public PushalotEventTelemeter(IPartialConfiguration<PushalotOptions> pushalotConfiguration)
            : base(pushalotConfiguration)
        {
        }

        public async Task TrackEvent(string name)
        {
            await SendMessage("Event", name);
        }

        public async Task TrackEvent(string name, IDictionary<string, string> properties)
        {
            await SendMessage("Event", $"{name}. {string.Join(" ", properties.Select(item => string.Format("{0} is {1}", item.Key, item.Value)))}");
        }

        protected override IEnumerable<string> GetAuthorizationTokens(PushalotOptions pushalotConfiguration)
        {
            return pushalotConfiguration.EventAuthorizationTokens;
        }

        protected override IEnumerable<string> GetAuthorizationTokens(PushalotOptions pushalotConfiguration, TelemetrySeverityLevel severityLevel)
        {
            return Enumerable.Empty<string>();
        }
    }
}