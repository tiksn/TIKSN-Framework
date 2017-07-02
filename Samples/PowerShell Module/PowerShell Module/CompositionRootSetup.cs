using System.Management.Automation;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TIKSN.DependencyInjection;
using TIKSN.PowerShell;

namespace PowerShell_Module
{
	public class CompositionRootSetup : AutofacCompositionRootSetupBase
    {
		private readonly Cmdlet _cmdlet;

		public CompositionRootSetup(Cmdlet cmdlet)
		{
			_cmdlet = cmdlet;
		}

		protected override void ConfigureContainerBuilder(ContainerBuilder builder)
		{
		}

		protected override void ConfigureOptions(IServiceCollection services, IConfigurationRoot configuration)
		{
		}

		protected override void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton(_cmdlet);
		}
	}
}
