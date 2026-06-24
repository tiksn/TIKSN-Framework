using System.Diagnostics;
using Microsoft.Extensions.Options;

namespace TIKSN.Analytics.Telemetry;

public class CompositeEventTelemeter : IEventTelemeter
{
    private readonly IOptions<CommonTelemetryOptions> commonOptions;
    private readonly IEnumerable<IEventTelemeter> eventTelemeters;

    public CompositeEventTelemeter(
        IOptions<CommonTelemetryOptions> commonOptions,
        IEnumerable<IEventTelemeter> eventTelemeters)
    {
        this.commonOptions = commonOptions ?? throw new ArgumentNullException(nameof(commonOptions));
        this.eventTelemeters = eventTelemeters ?? throw new ArgumentNullException(nameof(eventTelemeters));
    }

    public async Task TrackEventAsync(string name)
    {
        if (this.commonOptions.Value.IsEventTrackingEnabled)
        {
            foreach (var eventTelemeter in this.eventTelemeters)
            {
#pragma warning disable CA1031 // Do not catch general exception types
                try
                {
                    await eventTelemeter.TrackEventAsync(name).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }
    }

    public async Task TrackEventAsync(string name, IReadOnlyDictionary<string, string> properties)
    {
        if (this.commonOptions.Value.IsEventTrackingEnabled)
        {
            foreach (var eventTelemeter in this.eventTelemeters)
            {
#pragma warning disable CA1031 // Do not catch general exception types
                try
                {
                    await eventTelemeter.TrackEventAsync(name, properties).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }
    }
}
