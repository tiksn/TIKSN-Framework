using System.Diagnostics;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry;

public class CompositeEventTelemeter : IEventTelemeter
{
    private readonly IPartialConfiguration<CommonTelemetryOptions> commonConfiguration;
    private readonly IEnumerable<IEventTelemeter> eventTelemeters;

    public CompositeEventTelemeter(IPartialConfiguration<CommonTelemetryOptions> commonConfiguration,
        IEnumerable<IEventTelemeter> eventTelemeters)
    {
        this.commonConfiguration = commonConfiguration;
        this.eventTelemeters = eventTelemeters;
    }

    public async Task TrackEventAsync(string name)
    {
        if (this.commonConfiguration.GetConfiguration().IsEventTrackingEnabled)
        {
            foreach (var eventTelemeter in this.eventTelemeters)
            {
                try
                {
                    await eventTelemeter.TrackEventAsync(name).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }
    }

    public async Task TrackEventAsync(string name, IReadOnlyDictionary<string, string> properties)
    {
        if (this.commonConfiguration.GetConfiguration().IsEventTrackingEnabled)
        {
            foreach (var eventTelemeter in this.eventTelemeters)
            {
                try
                {
                    await eventTelemeter.TrackEventAsync(name, properties).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }
    }
}
