using System.Collections.Generic;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
    public interface IEventTelemeter
    {
        Task TrackEventAsync(string name);

        Task TrackEventAsync(string name, IDictionary<string, string> properties);
    }
}
