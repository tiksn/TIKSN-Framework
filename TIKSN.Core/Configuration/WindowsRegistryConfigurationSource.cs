using Microsoft.Extensions.Configuration;
using Microsoft.Win32;

namespace TIKSN.Configuration
{
    public class WindowsRegistryConfigurationSource : IConfigurationSource
    {
        public RegistryView RegistryView { get; set; }
        public string RootKey { get; set; }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new WindowsRegistryConfigurationProvider(RootKey, RegistryView);
        }
    }
}