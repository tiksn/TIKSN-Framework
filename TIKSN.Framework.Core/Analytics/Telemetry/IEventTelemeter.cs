namespace TIKSN.Analytics.Telemetry;

public interface IEventTelemeter
{
    Task TrackEventAsync(string name);

    Task TrackEventAsync(string name, IReadOnlyDictionary<string, string> properties);
}
