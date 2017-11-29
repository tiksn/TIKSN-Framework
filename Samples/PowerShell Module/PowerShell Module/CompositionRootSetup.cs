using System.Management.Automation;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;
using TIKSN.Analytics.Logging;

namespace PowerShell_Module
{
    public class CompositionRootSetup : AutofacCompositionRootSetupBase
    {
        private readonly Cmdlet _cmdlet;

        public CompositionRootSetup(Cmdlet cmdlet, IConfigurationRoot configuration) : base(configuration)
        {
            _cmdlet = cmdlet;
        }

        protected override void ConfigureContainerBuilder(ContainerBuilder builder)
        {
            builder.RegisterType<LoggingSetup>().As<ILoggingSetup>().SingleInstance();
        }

        protected override void ConfigureOptions(IServiceCollection services, IConfigurationRoot configuration)
        {
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_cmdlet);
        }
    }
}
