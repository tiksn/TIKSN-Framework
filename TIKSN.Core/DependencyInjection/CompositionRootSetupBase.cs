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
		protected readonly IConfigurationRoot _configurationRoot;
		protected readonly Lazy<IServiceCollection> _services;

		protected CompositionRootSetupBase(IConfigurationRoot configurationRoot)
		{
			_configurationRoot = configurationRoot;
			_services = new Lazy<IServiceCollection>(CreateServiceCollection, false);
		}

		public IServiceProvider CreateServiceProvider()
		{
			var serviceProvider = CreateServiceProviderInternal();

			var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

			ConfigureLoggingInternal(serviceProvider);

			ValidateOptions(_services.Value, serviceProvider);

			return serviceProvider;
		}

		protected virtual void ConfigureLogging(ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
		{
			loggerFactory.AddDebug(LogLevel.Trace);
		}

		protected void ConfigureLoggingInternal(IServiceProvider serviceProvider)
		{
			var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

			ConfigureLogging(loggerFactory, serviceProvider);

			var serilogLoggerConfiguration = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.Enrich.FromLogContext();

			ConfigureSerilog(serilogLoggerConfiguration, serviceProvider);

			loggerFactory.AddSerilog(serilogLoggerConfiguration.CreateLogger());
		}

		protected abstract void ConfigureOptions(IServiceCollection services, IConfigurationRoot configuration);

		protected virtual void ConfigureSerilog(LoggerConfiguration serilogLoggerConfiguration, IServiceProvider serviceProvider)
		{
		}

		protected abstract void ConfigureServices(IServiceCollection services);

		protected IServiceCollection CreateServiceCollection()
		{
			var services = GetInitialServiceCollection();
			DependencyRegistration.Register(services);

			ConfigureServices(services);

			ConfigureOptions(services, _configurationRoot);

			services.AddSingleton(_configurationRoot);

			return services;
		}

		protected virtual IServiceProvider CreateServiceProviderInternal()
		{
			return _services.Value.BuildServiceProvider();
		}

		protected virtual IServiceCollection GetInitialServiceCollection()
		{
			return new ServiceCollection();
		}

		protected void ValidateOptions(IServiceCollection services, IServiceProvider serviceProvider)
		{
			var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

			foreach (var service in services)
			{
				if (service.ServiceType.GetTypeInfo().IsGenericType &&
					service.ServiceType.GetGenericTypeDefinition() == typeof(IConfigureOptions<>))
				{
					var optionType = service.ServiceType.GenericTypeArguments[0];
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

					foreach (var pInfo in optionType.GetRuntimeProperties())
					{
						var isNotSpecified = false;
						var logger = loggerFactory.CreateLogger(optionsType);

						logger.LogDebug(1959347740, $"{pInfo.Name} property of {optionType.FullName} object is type of {pInfo.PropertyType}.");

						if (pInfo.PropertyType == typeof(string))
							isNotSpecified = string.IsNullOrEmpty(pInfo.GetValue(options.Value)?.ToString());
						else if (pInfo.PropertyType.GetTypeInfo().IsAbstract)
							logger.LogDebug(1714558096, $"{pInfo.Name} property of {optionType.FullName} object is abstract type of {pInfo.PropertyType}. Abstract option types cannot be validated.");
						else if (pInfo.PropertyType.GetConstructor(Type.EmptyTypes) != null && pInfo.GetValue(options.Value) == Activator.CreateInstance(pInfo.PropertyType))
							isNotSpecified = true;

						if (isNotSpecified)
							logger.LogWarning(1881146767, $"{pInfo.Name} property of {optionType.FullName} most likely is not specified in configuration files.");
					}
				}
			}
		}
	}
}