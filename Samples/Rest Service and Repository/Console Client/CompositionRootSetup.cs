using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TIKSN.DependencyInjection;

namespace Console_Client
{
	public class CompositionRootSetup : AutofacCompositionRootSetupBase
	{
		protected override void ConfigureContainerBuilder(ContainerBuilder builder)
		{
		}

		protected override void ConfigureOptions(IServiceCollection services, IConfigurationRoot configuration)
		{
		}

		protected override void ConfigureServices(IServiceCollection services)
		{
		}

		protected override void ConfigureLogging(ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole();
		}
	}
}
