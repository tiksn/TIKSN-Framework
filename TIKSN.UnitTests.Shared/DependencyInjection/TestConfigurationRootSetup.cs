using Microsoft.Extensions.Configuration;
using TIKSN.Configuration;

namespace TIKSN.DependencyInjection.Tests
{
    public class TestConfigurationRootSetup : ConfigurationRootSetupBase
    {
        protected override void SetupConfiguration(IConfigurationBuilder builder)
        {
            base.SetupConfiguration(builder);

            builder.AddUserSecrets("TIKSN-Framework-tests-bee60aa7-e44d-4baf-9587-a0a3d5800585");
        }
    }
}