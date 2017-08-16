using System;
using TIKSN.PowerShell;

namespace PowerShellCoreModule.Commands
{
	public abstract class Command : CommandBase
	{
		protected override IServiceProvider CreateServiceProvider()
		{
			var compositionRootSetup = new CompositionRootSetup(this, ConfigurationRootSetup.ConfigurationRoot);

			return compositionRootSetup.CreateServiceProvider();
		}
	}
}
