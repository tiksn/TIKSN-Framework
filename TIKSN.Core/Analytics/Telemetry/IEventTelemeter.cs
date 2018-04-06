using System.Collections.Generic;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
    public interface IEventTelemeter
    {
        Task TrackEvent(string name);

        Task TrackEvent(string name, IDictionary<string, string> properties);
    }
}