using System.Diagnostics;
using Microsoft.ApplicationInsights.DataContracts;

namespace TIKSN.Analytics.Telemetry;

public class ApplicationInsightsExceptionTelemeter : IExceptionTelemeter
{
    [Obsolete]
    public async Task TrackExceptionAsync(Exception exception) => await TrackExceptionInternalAsync(exception, severityLevel: null).ConfigureAwait(false);

    [Obsolete]
    public Task TrackExceptionAsync(Exception exception, TelemetrySeverityLevel severityLevel) =>
        TrackExceptionInternalAsync(exception, severityLevel);

    [Obsolete]
    private static Task TrackExceptionInternalAsync(Exception exception, TelemetrySeverityLevel? severityLevel)
    {
        try
        {
            var telemetry = new ExceptionTelemetry(exception)
            {
                SeverityLevel = ApplicationInsightsHelper.ConvertSeverityLevel(severityLevel),
            };
            ApplicationInsightsHelper.TrackException(telemetry);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }

        return Task.CompletedTask;
    }
}
