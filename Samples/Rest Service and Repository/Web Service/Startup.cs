using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Web_Service
{
	public class Startup
	{
		private IHostingEnvironment _env;

		public Startup(IHostingEnvironment env)
		{
			_env = env;
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			app.UseMvc();
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			var compositionRootSetup = new CompositionRootSetup(_env, services);
			var serviceProvider = compositionRootSetup.CreateServiceProvider();
			return serviceProvider;
		}
	}
}