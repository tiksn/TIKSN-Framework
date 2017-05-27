using Autofac;
using Common_Models;
using Console_Client.Rest;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TIKSN.DependencyInjection;
using TIKSN.Web.Rest;

namespace Console_Client
{
	public class CompositionRootSetup : AutofacCompositionRootSetupBase
	{
		protected override void ConfigureContainerBuilder(ContainerBuilder builder)
		{
			builder.RegisterType<Program>().AsSelf().SingleInstance();
			builder.RegisterType<RestRepository<CultureModel, int>>().As<IRestRepository<CultureModel, int>>();
			builder.RegisterType<CulturesRestRepository>().As<ICulturesRestRepository>();
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
