using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace TIKSN.DependencyInjection
{
	public abstract class AutofacCompositionRootSetupBase : CompositionRootSetupBase
	{
		protected abstract void ConfigureContainerBuilder(ContainerBuilder builder);

		protected override IServiceProvider CreateServiceProviderInternal()
		{
			var builder = new ContainerBuilder();
			builder.Populate(services);

			foreach (var module in GetAutofacModules())
			{
				builder.RegisterModule(module);
			}

			ConfigureContainerBuilder(builder);

			var applicationContainer = builder.Build();

			return new AutofacServiceProvider(applicationContainer);
		}

		protected virtual IEnumerable<IModule> GetAutofacModules()
		{
			yield return new CoreModule();
		}
	}
}