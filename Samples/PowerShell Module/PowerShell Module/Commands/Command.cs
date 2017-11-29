using System;
using TIKSN.PowerShell;

namespace PowerShell_Module.Commands
{
    public abstract class Command : CommandBase
    {
        protected override IServiceProvider CreateServiceProvider()
        {
            var configurationRootSetup = new ConfigurationRootSetup();
            var compositionRootSetup = new CompositionRootSetup(this, configurationRootSetup.GetConfigurationRoot());

            return compositionRootSetup.CreateServiceProvider();
        }
    }
}
