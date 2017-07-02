using System;

namespace PowerShell_Module.Commands
{
	public abstract class CommandBase : TIKSN.PowerShell.CommandBase
	{
		protected override IServiceProvider CreateServiceProvider()
		{
			var compositionRootSetup = new CompositionRootSetup(this);

			return compositionRootSetup.CreateServiceProvider();
		}
	}
}
