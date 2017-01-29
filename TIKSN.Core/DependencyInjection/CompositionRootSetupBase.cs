using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace TIKSN.DependencyInjection
{
	public abstract class CompositionRootSetupBase
	{
		protected readonly IServiceCollection services;

		protected CompositionRootSetupBase()
		{
			services = new ServiceCollection();
			DependencyRegistration.Register(services);

			ConfigureServices(services);

			var configuration = GetConfigurationRoot();

			ConfigureOptions(services, configuration);

			//services.AddSingleton(configuration);
			//services.AddSingleton<IConfiguration>(configuration);
		}

		public IServiceProvider CreateServiceProvider()
		{
			var serviceProvider = CreateServiceProviderInternal();

			var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

			ConfigureLogging(loggerFactory);

			var serilogLoggerConfiguration = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.Enrich.FromLogContext();

			ConfigureSerilog(serilogLoggerConfiguration, serviceProvider);

			loggerFactory.AddSerilog(serilogLoggerConfiguration.CreateLogger());

			ValidateOptions(services, serviceProvider, loggerFactory);

			return serviceProvider;
		}

		protected virtual void ConfigureLogging(ILoggerFactory loggerFactory)
		{
			loggerFactory.AddDebug(LogLevel.Trace);
		}

		protected abstract void ConfigureOptions(IServiceCollection services, IConfigurationRoot configuration);

		protected abstract void ConfigureSerilog(LoggerConfiguration serilogLoggerConfiguration, IServiceProvider serviceProvider);

		protected abstract void ConfigureServices(IServiceCollection services);

		protected virtual IServiceProvider CreateServiceProviderInternal()
		{
			return services.BuildServiceProvider();
		}

		protected abstract IConfigurationRoot GetConfigurationRoot();

		private void ValidateOptions(IServiceCollection services, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
		{
			foreach (var service in services)
			{
				if (service.ServiceType.GetTypeInfo().IsGenericType &&
					service.ServiceType.GetGenericTypeDefinition() == typeof(IConfigureOptions<>))
				{
					var optionType = service.ServiceType.GetGenericArguments()[0];
					var optionsType = typeof(IOptions<>).MakeGenericType(optionType);

					var options = (IOptions<object>)serviceProvider.GetRequiredService(optionsType);

					try
					{
						var validationContext = new ValidationContext(options.Value);
						Validator.ValidateObject(options.Value, validationContext);
					}
					catch (Exception ex)
					{
						throw new Exception($"Option model of type {optionType.FullName} is invalid. Please check configuration files.", ex);
					}

					foreach (var pInfo in optionType.GetProperties())
					{
						var isNotSpecified = false;
						var logger = loggerFactory.CreateLogger(optionsType);

						if (pInfo.PropertyType == typeof(string))
							isNotSpecified = string.IsNullOrEmpty(pInfo.GetValue(options.Value)?.ToString());
						else if (pInfo.GetValue(options.Value) == Activator.CreateInstance(pInfo.PropertyType) && pInfo.PropertyType.GetConstructor(Type.EmptyTypes) != null)
							isNotSpecified = true;
						else
							logger.LogDebug(2016759580, $"{pInfo.Name} property of {optionType.FullName} object is type of {pInfo.PropertyType}.");

						if (isNotSpecified)
							logger.LogWarning(1881146767, $"{pInfo.Name} property of {optionType.FullName} most likely is not specified in configuration files.");
					}
				}
			}
		}
	}
}