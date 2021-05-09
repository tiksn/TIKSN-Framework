using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
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
            var container = this.CreateContainerInternal();
            var serviceProvider = new AutofacServiceProvider(container);

            this.ValidateOptions(this._services.Value, serviceProvider);

            return container;
        }

        protected abstract void ConfigureContainerBuilder(ContainerBuilder builder);

        protected IContainer CreateContainerInternal()
        {
            var builder = new ContainerBuilder();
            builder.Populate(this._services.Value);

            foreach (var module in this.GetAutofacModules())
            {
                builder.RegisterModule(module);
            }

            this.ConfigureContainerBuilder(builder);

            return builder.Build();
        }

        protected override IServiceProvider CreateServiceProviderInternal() =>
            new AutofacServiceProvider(this.CreateContainerInternal());

        protected virtual IEnumerable<IModule> GetAutofacModules()
        {
            yield return new CoreModule();
            yield return new PowerShellModule();
        }
    }
}
