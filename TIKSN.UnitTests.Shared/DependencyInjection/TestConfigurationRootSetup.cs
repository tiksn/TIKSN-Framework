using Microsoft.Extensions.Configuration;
using TIKSN.Configuration;

namespace TIKSN.DependencyInjection.Tests
{
    public class TestConfigurationRootSetup : ConfigurationRootSetupBase
    {
        protected override void SetupConfiguration(IConfigurationBuilder builder)
        {
            base.SetupConfiguration(builder);
        }
    }
}
