using System.Globalization;
using TIKSN.Shell;

namespace ShellCommands;

[ShellCommand("5692228")]
public class ListRegionalCulturesShellCommand : ShellCommandBase
{
    public ListRegionalCulturesShellCommand(
        IConsoleService consoleService) : base(consoleService)
    {
    }

    [ShellCommandParameter("5692491", Mandatory = true)]
    public string? RegionCode { get; set; }

    public override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var regionalCultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .Select(x => (culture: x, region: new RegionInfo(x.Name)))
            .Where(x => x.region.Name.Equals(this.RegionCode, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.culture)
            .Select(x => new CultureRecord(x.Name, x.EnglishName, x.NativeName))
            .ToArray();

        this.WriteObjects(regionalCultures);

        return Task.CompletedTask;
    }
}
