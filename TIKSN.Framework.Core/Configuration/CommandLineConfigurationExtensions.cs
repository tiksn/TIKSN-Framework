using Microsoft.Extensions.Configuration;
using TIKSN.Analytics.Logging.NLog;

namespace TIKSN.Configuration;

public static class CommandLineConfigurationExtensions
{
    public static IConfigurationBuilder AddFrameworkCommandLine(
        this IConfigurationBuilder configurationBuilder,
        string[] args,
        IDictionary<string, string> switchMappings)
    {
        var frameworkSwitchMappings = MergeFrameworkSwitchMappings(switchMappings);
        return configurationBuilder.AddCommandLine(args, frameworkSwitchMappings);
    }

    private static IDictionary<string, string> GetFrameworkSwitchMappings() => new Dictionary<string, string>
(StringComparer.OrdinalIgnoreCase)
    {
            {
                "--nlog-viewer-address",
                ConfigurationPath.Combine(RemoteNLogViewerOptions.RemoteNLogViewerConfigurationSection,
                nameof(RemoteNLogViewerOptions.Address))
            },
            {
                "--nlog-viewer-include-call-site",
                ConfigurationPath.Combine(RemoteNLogViewerOptions.RemoteNLogViewerConfigurationSection,
                nameof(RemoteNLogViewerOptions.IncludeCallSite))
            },
            {
                "--nlog-viewer-include-source-info",
                ConfigurationPath.Combine(RemoteNLogViewerOptions.RemoteNLogViewerConfigurationSection,
                nameof(RemoteNLogViewerOptions.IncludeSourceInfo))
            },
        };

    private static IDictionary<string, string> MergeFrameworkSwitchMappings(IDictionary<string, string> switchMappings)
    {
        var mappings = GetFrameworkSwitchMappings();

        if (switchMappings is not null)
        {
            foreach (var item in switchMappings)
            {
                mappings[item.Key] = item.Value;
            }
        }

        return mappings;
    }

    public static IConfigurationBuilder AddFrameworkCommandLine(
        this IConfigurationBuilder configurationBuilder,
        string[] args) => configurationBuilder.AddFrameworkCommandLine(args, switchMappings: null);
}
