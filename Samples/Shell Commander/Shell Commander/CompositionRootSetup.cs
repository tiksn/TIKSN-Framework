using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using TIKSN.Analytics.Logging;
using TIKSN.DependencyInjection;

namespace Shell_Commander
{
    public class CompositionRootSetup : AutofacPlatformCompositionRootSetupBase
    {
        public CompositionRootSetup(IConfigurationRoot configurationRoot) : base(configurationRoot)
        {
        }

        protected override void ConfigureContainerBuilder(ContainerBuilder builder)
        {
            builder.RegisterType<TextsStringLocalizer>().As<IStringLocalizer>().SingleInstance();
        }

        protected override void ConfigureOptions(IServiceCollection services, IConfigurationRoot configuration)
        {
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            PlatformDependencyRegistration.Register(services);
        }

        protected override IEnumerable<ILoggingSetup> GetLoggingSetups()
        {
            yield return new LoggingSetup();
        }
    }
}