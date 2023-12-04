using Microsoft.Extensions.Configuration;
using Microsoft.Win32;

namespace TIKSN.Configuration;

public static class WindowsRegistryConfigurationExtensions
{
    public static IConfigurationBuilder AddWindowsRegistry(this IConfigurationBuilder configurationBuilder,
        string rootKey, RegistryView registryView = RegistryView.Default) =>
        configurationBuilder.Add(new WindowsRegistryConfigurationSource
        {
            RootKey = rootKey,
            RegistryView = registryView,
        });
}
