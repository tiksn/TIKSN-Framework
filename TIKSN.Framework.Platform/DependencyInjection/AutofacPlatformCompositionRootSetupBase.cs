using System.Collections.Generic;
using System.Linq;
using Autofac.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TIKSN.DependencyInjection
{
    public abstract class AutofacPlatformCompositionRootSetupBase : AutofacCompositionRootSetupBase
    {
        protected AutofacPlatformCompositionRootSetupBase(IConfigurationRoot configurationRoot) : base(configurationRoot)
        {
        }

        protected override IEnumerable<IModule> GetAutofacModules()
        {
            var modules = base.GetAutofacModules().ToList();

            modules.Add(new PlatformModule());

            return modules;
        }

        protected override void ConfigureServices(IServiceCollection services) => services.AddFrameworkPlatform();
    }
}
