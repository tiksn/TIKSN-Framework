using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using TIKSN.PowerShell;

namespace TIKSN.DependencyInjection
{
    public abstract class AutofacCompositionRootSetupBase : CompositionRootSetupBase
    {
        protected AutofacCompositionRootSetupBase(IConfigurationRoot configurationRoot) : base(configurationRoot)
        {
        }

        public IContainer CreateContainer()
        {
            var container = CreateContainerInternal();
            var serviceProvider = new AutofacServiceProvider(container);

            ValidateOptions(_services.Value, serviceProvider);

            return container;
        }

        protected abstract void ConfigureContainerBuilder(ContainerBuilder builder);

        protected IContainer CreateContainerInternal()
        {
            var builder = new ContainerBuilder();
            builder.Populate(_services.Value);

            foreach (var module in GetAutofacModules())
            {
                builder.RegisterModule(module);
            }

            ConfigureContainerBuilder(builder);

            return builder.Build();
        }

        protected override IServiceProvider CreateServiceProviderInternal()
        {
            return new AutofacServiceProvider(CreateContainerInternal());
        }

        protected virtual IEnumerable<IModule> GetAutofacModules()
        {
            yield return new CoreModule();
            yield return new PowerShellModule();
        }
    }
}