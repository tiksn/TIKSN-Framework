using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using TIKSN.DependencyInjection;

namespace Shell_Commander
{
	public class CompositionRootSetup : AutofacPlatformCompositionRootSetupBase
	{
		protected override void ConfigureContainerBuilder(ContainerBuilder builder)
		{
			builder.RegisterType<TextsStringLocalizer>().As<IStringLocalizer>().SingleInstance();
		}

		protected override void ConfigureLogging(ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole();
		}

		protected override void ConfigureOptions(IServiceCollection services, IConfigurationRoot configuration)
		{
		}

		protected override void ConfigureServices(IServiceCollection services)
		{
			PlatformDependencyRegistration.Register(services);
		}
	}
}