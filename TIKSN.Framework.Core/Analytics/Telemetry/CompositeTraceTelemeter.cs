using System.Diagnostics;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry;

public class CompositeTraceTelemeter : ITraceTelemeter
{
    private readonly IPartialConfiguration<CommonTelemetryOptions> commonConfiguration;
    private readonly IEnumerable<ITraceTelemeter> traceTelemeters;

    public CompositeTraceTelemeter(
        IPartialConfiguration<CommonTelemetryOptions> commonConfiguration,
        IEnumerable<ITraceTelemeter> traceTelemeters)
    {
        this.commonConfiguration = commonConfiguration ?? throw new ArgumentNullException(nameof(commonConfiguration));
        this.traceTelemeters = traceTelemeters ?? throw new ArgumentNullException(nameof(traceTelemeters));
    }

    public async Task TrackTraceAsync(string message)
    {
        if (this.commonConfiguration.GetConfiguration().IsTraceTrackingEnabled)
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
        if (this.commonConfiguration.GetConfiguration().IsTraceTrackingEnabled)
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
