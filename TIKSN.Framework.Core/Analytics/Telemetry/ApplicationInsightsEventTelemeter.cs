using System.Diagnostics;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace TIKSN.Analytics.Telemetry;

public class ApplicationInsightsEventTelemeter : IEventTelemeter
{
    private readonly TelemetryClient client;

    public ApplicationInsightsEventTelemeter(TelemetryClient client)
        => this.client = client ?? throw new ArgumentNullException(nameof(client));

    public Task TrackEventAsync(string name)
    {
#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            var telemetry = new EventTelemetry(name);
            ApplicationInsightsHelper.TrackEvent(this.client, telemetry);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
#pragma warning restore CA1031 // Do not catch general exception types

        return Task.CompletedTask;
    }

    public Task TrackEventAsync(string name, IReadOnlyDictionary<string, string> properties)
    {
#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            var telemetry = new EventTelemetry(name);
            if (properties is not null)
            {
                foreach (var property in properties)
                {
                    telemetry.Properties.Add(property);
                }
            }

            ApplicationInsightsHelper.TrackEvent(this.client, telemetry);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
#pragma warning restore CA1031 // Do not catch general exception types

        return Task.CompletedTask;
    }
}
