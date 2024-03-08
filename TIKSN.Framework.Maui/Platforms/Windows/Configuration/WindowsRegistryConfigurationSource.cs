using Microsoft.Extensions.Configuration;
using Microsoft.Win32;

namespace TIKSN.Platforms.Windows.Configuration;

public class WindowsRegistryConfigurationSource : IConfigurationSource
{
    public RegistryView RegistryView { get; set; }
    public string RootKey { get; set; }

    public IConfigurationProvider Build(IConfigurationBuilder builder) =>
        new WindowsRegistryConfigurationProvider(this.RootKey, this.RegistryView);
}
