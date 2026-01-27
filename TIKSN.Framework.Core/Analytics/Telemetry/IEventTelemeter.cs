namespace TIKSN.Analytics.Telemetry;

public interface IEventTelemeter
{
    public Task TrackEventAsync(string name);

    public Task TrackEventAsync(string name, IReadOnlyDictionary<string, string> properties);
}
