using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace TIKSN.DependencyInjection
{
	public abstract class AutofacCompositionRootSetupBase : CompositionRootSetupBase
	{
		public IContainer CreateContainer()
		{
			var container = CreateContainerInternal();
			var serviceProvider = new AutofacServiceProvider(container);

			ConfigureLoggingInternal(serviceProvider);

			ValidateOptions(services, serviceProvider);

			return container;
		}

		protected abstract void ConfigureContainerBuilder(ContainerBuilder builder);

		protected IContainer CreateContainerInternal()
		{
			var builder = new ContainerBuilder();
			builder.Populate(services);

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
		}
	}
}