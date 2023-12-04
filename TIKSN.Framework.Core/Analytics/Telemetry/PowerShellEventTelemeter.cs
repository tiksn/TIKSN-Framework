using System.Management.Automation;

namespace TIKSN.Analytics.Telemetry;

public class PowerShellEventTelemeter : IEventTelemeter
{
    private readonly Cmdlet cmdlet;

    public PowerShellEventTelemeter(Cmdlet cmdlet) => this.cmdlet = cmdlet;

    public Task TrackEventAsync(string name)
    {
        this.cmdlet.WriteVerbose($"EVENT: {name}");

        return Task.FromResult<object>(null);
    }

    public Task TrackEventAsync(string name, IReadOnlyDictionary<string, string> properties)
    {
        this.cmdlet.WriteVerbose(
            $"EVENT: {name} with {string.Join(", ", properties.Select(item => string.Format("{0} is {1}", item.Key, item.Value)))}");

        return Task.FromResult<object>(null);
    }
}
