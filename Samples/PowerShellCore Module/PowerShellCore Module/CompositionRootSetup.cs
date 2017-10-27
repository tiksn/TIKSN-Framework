using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Management.Automation;
using TIKSN.Analytics.Logging;
using TIKSN.DependencyInjection;

namespace PowerShellCoreModule
{
	public class CompositionRootSetup : AutofacCompositionRootSetupBase
	{
		private readonly Cmdlet _cmdlet;

		public CompositionRootSetup(Cmdlet cmdlet, IConfigurationRoot configurationRoot) : base(configurationRoot)
		{
			_cmdlet = cmdlet;
		}

		protected override void ConfigureContainerBuilder(ContainerBuilder builder)
		{
			builder.RegisterType<LoggingSetup>().As<LoggingSetupBase>();
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
