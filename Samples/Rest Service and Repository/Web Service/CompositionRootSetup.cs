using Autofac;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Web_Service.Data.Entities;
using LiteGuard;

namespace Web_Service
{
	public class CompositionRootSetup : AutofacCompositionRootSetupBase
	{
		private readonly IHostingEnvironment _env;

		public CompositionRootSetup(IHostingEnvironment env)
		{
			Guard.AgainstNullArgument(nameof(env), env);
			_env = env;
		}

		protected override void ConfigureContainerBuilder(ContainerBuilder builder)
		{
		}

		protected override void ConfigureOptions(IServiceCollection services, IConfigurationRoot configuration)
		{
		}

		protected override void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<InternationalizationContext>(options =>
				options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Internationalization;Trusted_Connection=True;"));
		}

		protected override void SetupConfiguration(IConfigurationBuilder builder)
		{
			builder
				.SetBasePath(_env.ContentRootPath)
				.AddYamlFile("appsettings.yaml")
				.AddYamlFile($"appsettings.{_env.EnvironmentName}.yaml", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables();
			base.SetupConfiguration(builder);
		}
	}
}
