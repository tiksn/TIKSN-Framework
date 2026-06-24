using System.Diagnostics;
using Microsoft.Extensions.Options;

namespace TIKSN.Analytics.Telemetry;

public class CompositeExceptionTelemeter : IExceptionTelemeter
{
    private readonly IOptions<CommonTelemetryOptions> commonOptions;
    private readonly IEnumerable<IExceptionTelemeter> exceptionTelemeters;

    public CompositeExceptionTelemeter(
        IOptions<CommonTelemetryOptions> commonOptions,
        IEnumerable<IExceptionTelemeter> exceptionTelemeters)
    {
        this.commonOptions = commonOptions ?? throw new ArgumentNullException(nameof(commonOptions));
        this.exceptionTelemeters = exceptionTelemeters ?? throw new ArgumentNullException(nameof(exceptionTelemeters));
    }

    public async Task TrackExceptionAsync(Exception exception)
    {
        if (this.commonOptions.Value.IsExceptionTrackingEnabled)
        {
            foreach (var exceptionTelemeter in this.exceptionTelemeters)
            {
#pragma warning disable CA1031 // Do not catch general exception types
                try
                {
                    await exceptionTelemeter.TrackExceptionAsync(exception).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }
    }

    public async Task TrackExceptionAsync(Exception exception, TelemetrySeverityLevel severityLevel)
    {
        if (this.commonOptions.Value.IsExceptionTrackingEnabled)
        {
            foreach (var exceptionTelemeter in this.exceptionTelemeters)
            {
#pragma warning disable CA1031 // Do not catch general exception types
                try
                {
                    await exceptionTelemeter.TrackExceptionAsync(exception, severityLevel).ConfigureAwait(false);
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
