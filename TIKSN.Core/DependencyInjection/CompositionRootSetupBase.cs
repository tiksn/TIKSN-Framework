using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TIKSN.Analytics.Logging;

namespace TIKSN.DependencyInjection
{
    public abstract class CompositionRootSetupBase
    {
        protected readonly IConfigurationRoot _configurationRoot;
        protected readonly Lazy<IServiceCollection> _services;

        protected CompositionRootSetupBase(IConfigurationRoot configurationRoot)
        {
            this._configurationRoot = configurationRoot;
            this._services = new Lazy<IServiceCollection>(this.CreateServiceCollection, false);
        }

        public IServiceProvider CreateServiceProvider()
        {
            var serviceProvider = this.CreateServiceProviderInternal();

            this.ValidateOptions(this._services.Value, serviceProvider);

            return serviceProvider;
        }

        protected virtual void ConfigureLoggingBuilder(ILoggingBuilder loggingBuilder)
        {
            foreach (var loggingSetup in this.GetLoggingSetups())
            {
                loggingSetup.Setup(loggingBuilder);
            }
        }

        protected abstract void ConfigureOptions(IServiceCollection services, IConfigurationRoot configuration);

        protected abstract void ConfigureServices(IServiceCollection services);

        protected IServiceCollection CreateServiceCollection()
        {
            var services = this.GetInitialServiceCollection();
            services.AddFrameworkCore();
            services.AddLogging(this.ConfigureLoggingBuilder);

            this.ConfigureServices(services);

            this.ConfigureOptions(services, this._configurationRoot);

            services.AddSingleton(this._configurationRoot);

            return services;
        }

        protected virtual IServiceProvider CreateServiceProviderInternal() =>
            this._services.Value.BuildServiceProvider();

        protected virtual IServiceCollection GetInitialServiceCollection() => new ServiceCollection();

        protected abstract IEnumerable<ILoggingSetup> GetLoggingSetups();

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
                        throw new Exception(
                            $"Option model of type {optionType.FullName} is invalid. Please check configuration files.",
                            ex);
                    }

                    foreach (var pInfo in optionType.GetRuntimeProperties())
                    {
                        var isNotSpecified = false;
                        var logger = loggerFactory.CreateLogger(optionsType);

                        logger.LogDebug(1959347740,
                            $"{pInfo.Name} property of {optionType.FullName} object is type of {pInfo.PropertyType}.");

                        if (pInfo.PropertyType == typeof(string))
                        {
                            isNotSpecified = string.IsNullOrEmpty(pInfo.GetValue(options.Value)?.ToString());
                        }
                        else if (pInfo.PropertyType.GetTypeInfo().IsAbstract)
                        {
                            logger.LogDebug(1714558096,
                                $"{pInfo.Name} property of {optionType.FullName} object is abstract type of {pInfo.PropertyType}. Abstract option types cannot be validated.");
                        }
                        else if (pInfo.PropertyType.GetConstructor(Type.EmptyTypes) != null &&
                                 pInfo.GetValue(options.Value) == Activator.CreateInstance(pInfo.PropertyType))
                        {
                            isNotSpecified = true;
                        }

                        if (isNotSpecified)
                        {
                            logger.LogWarning(1881146767,
                                $"{pInfo.Name} property of {optionType.FullName} most likely is not specified in configuration files.");
                        }
                    }
                }
            }
        }
    }
}
