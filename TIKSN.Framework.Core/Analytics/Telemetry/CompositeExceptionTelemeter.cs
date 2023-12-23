using System.Diagnostics;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry;

public class CompositeExceptionTelemeter : IExceptionTelemeter
{
    private readonly IPartialConfiguration<CommonTelemetryOptions> commonConfiguration;
    private readonly IEnumerable<IExceptionTelemeter> exceptionTelemeters;

    public CompositeExceptionTelemeter(
        IPartialConfiguration<CommonTelemetryOptions> commonConfiguration,
        IEnumerable<IExceptionTelemeter> exceptionTelemeters)
    {
        this.commonConfiguration = commonConfiguration ?? throw new ArgumentNullException(nameof(commonConfiguration));
        this.exceptionTelemeters = exceptionTelemeters ?? throw new ArgumentNullException(nameof(exceptionTelemeters));
    }

    public async Task TrackExceptionAsync(Exception exception)
    {
        if (this.commonConfiguration.GetConfiguration().IsExceptionTrackingEnabled)
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
        if (this.commonConfiguration.GetConfiguration().IsExceptionTrackingEnabled)
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
