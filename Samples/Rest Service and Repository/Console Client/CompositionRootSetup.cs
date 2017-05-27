using Autofac;
using Common_Models;
using Console_Client.Rest;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TIKSN.Analytics.Telemetry;
using TIKSN.DependencyInjection;
using TIKSN.Web;
using TIKSN.Web.Rest;

namespace Console_Client
{
	public class CompositionRootSetup : AutofacPlatformCompositionRootSetupBase
	{
		protected override void ConfigureContainerBuilder(ContainerBuilder builder)
		{
			builder.RegisterType<Program>().AsSelf().SingleInstance();
			builder.RegisterType<RestRepository<CultureModel, int>>().As<IRestRepository<CultureModel, int>>();
			builder.RegisterType<CulturesRestRepository>().As<ICulturesRestRepository>();
			builder.RegisterType<HttpClientFactory>().As<IHttpClientFactory>();
			builder.RegisterType<RestAuthenticationTokenProvider>().As<IRestAuthenticationTokenProvider>();
			builder.RegisterType<TelemetryLogger>().As<IExceptionTelemeter>().SingleInstance();
		}

		protected override void ConfigureLogging(ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole();
		}

		protected override void ConfigureOptions(IServiceCollection services, IConfigurationRoot configuration)
		{
			services.Configure<RestRepositoryOptions<CultureModel>>(configuration.GetSection("RestRepository").GetSection("Culture"));
			services.Configure<WebApiOptions<CultureModel>>(configuration.GetSection("RestRepository").GetSection("Culture"));
		}

		protected override void ConfigureServices(IServiceCollection services)
		{
		}

		protected override void SetupConfiguration(IConfigurationBuilder builder)
		{
			builder
				.AddYamlFile("appsettings.yaml");
			base.SetupConfiguration(builder);
		}
	}
}
