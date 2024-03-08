using System.Reflection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using TIKSN.Localization;

namespace ShellCommands;

public class MainStringLocalizer : CompositeAssemblyStringLocalizer
{
    public MainStringLocalizer(
        IResourceNamesCache resourceNamesCache,
        ILogger<MainStringLocalizer> logger) : base(
            resourceNamesCache,
            logger)
    {
    }

    protected override IEnumerable<Assembly> GetAssemblies()
        => base.GetAssemblies().ToSeq().Add(this.GetType().Assembly);
}
