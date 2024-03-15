using Microsoft.Extensions.Configuration;
using Microsoft.Win32;

namespace TIKSN.Configuration;

public static class WindowsRegistryConfigurationExtensions
{
    public static IConfigurationBuilder AddWindowsRegistry(
        this IConfigurationBuilder configurationBuilder,
        string rootKey,
        RegistryView registryView = RegistryView.Default)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        if (string.IsNullOrEmpty(rootKey))
        {
            throw new ArgumentException($"'{nameof(rootKey)}' cannot be null or empty.", nameof(rootKey));
        }

        return configurationBuilder.Add(new WindowsRegistryConfigurationSource
        {
            RootKey = rootKey,
            RegistryView = registryView,
        });
    }
}
