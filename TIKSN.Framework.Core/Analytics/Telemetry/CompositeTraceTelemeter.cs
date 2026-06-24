using System.Diagnostics;
using Microsoft.Extensions.Options;

namespace TIKSN.Analytics.Telemetry;

public class CompositeTraceTelemeter : ITraceTelemeter
{
    private readonly IOptions<CommonTelemetryOptions> commonOptions;
    private readonly IEnumerable<ITraceTelemeter> traceTelemeters;

    public CompositeTraceTelemeter(
        IOptions<CommonTelemetryOptions> commonOptions,
        IEnumerable<ITraceTelemeter> traceTelemeters)
    {
        this.commonOptions = commonOptions ?? throw new ArgumentNullException(nameof(commonOptions));
        this.traceTelemeters = traceTelemeters ?? throw new ArgumentNullException(nameof(traceTelemeters));
    }

    public async Task TrackTraceAsync(string message)
    {
        if (this.commonOptions.Value.IsTraceTrackingEnabled)
        {
            foreach (var traceTelemeter in this.traceTelemeters)
            {
#pragma warning disable CA1031 // Do not catch general exception types
                try
                {
                    await traceTelemeter.TrackTraceAsync(message).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }
    }

    public async Task TrackTraceAsync(string message, TelemetrySeverityLevel severityLevel)
    {
        if (this.commonOptions.Value.IsTraceTrackingEnabled)
        {
            foreach (var traceTelemeter in this.traceTelemeters)
            {
#pragma warning disable CA1031 // Do not catch general exception types
                try
                {
                    await traceTelemeter.TrackTraceAsync(message, severityLevel).ConfigureAwait(false);
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
