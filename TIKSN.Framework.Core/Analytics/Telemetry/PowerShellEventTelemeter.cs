using System.Management.Automation;

namespace TIKSN.Analytics.Telemetry;

public class PowerShellEventTelemeter : IEventTelemeter
{
    private readonly Cmdlet cmdlet;

    public PowerShellEventTelemeter(Cmdlet cmdlet) => this.cmdlet = cmdlet;

    public Task TrackEventAsync(string name)
    {
        this.cmdlet.WriteVerbose($"EVENT: {name}");

        return Task.CompletedTask;
    }

    public Task TrackEventAsync(string name, IReadOnlyDictionary<string, string> properties)
    {
        this.cmdlet.WriteVerbose(
            $"EVENT: {name} with {string.Join(", ", properties.Select(item => $"{item.Key} is {item.Value}"))}");

        return Task.FromResult<object>(null);
    }
}
