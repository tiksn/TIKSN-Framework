using System;
using TIKSN.PowerShell;

namespace PowerShell_Module.Commands
{
	public abstract class Command : CommandBase
	{
		protected override IServiceProvider CreateServiceProvider()
		{
			var compositionRootSetup = new CompositionRootSetup(this);

			return compositionRootSetup.CreateServiceProvider();
		}
	}
}
