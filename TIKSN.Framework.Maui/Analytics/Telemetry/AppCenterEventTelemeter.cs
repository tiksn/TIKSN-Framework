using AppCenterAnalytics = Microsoft.AppCenter.Analytics.Analytics;

namespace TIKSN.Analytics.Telemetry;

public class AppCenterEventTelemeter : IEventTelemeter
{
    public Task TrackEventAsync(string name)
    {
        AppCenterAnalytics.TrackEvent(name);
        return Task.CompletedTask;
    }

    public Task TrackEventAsync(string name, IReadOnlyDictionary<string, string> properties)
    {
        if (properties is null)
        {
            AppCenterAnalytics.TrackEvent(name);
        }
        else if (properties is IDictionary<string, string> dictionary)
        {
            AppCenterAnalytics.TrackEvent(name, dictionary);
        }
        else
        {
            AppCenterAnalytics.TrackEvent(name, properties.ToDictionary(StringComparer.Ordinal));
        }

        return Task.CompletedTask;
    }
}
