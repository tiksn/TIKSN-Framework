using System.Collections.Generic;
using Autofac.Core;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace TIKSN.DependencyInjection
{
	public abstract class AutofacPlatformCompositionRootSetupBase : AutofacCompositionRootSetupBase
	{
		protected AutofacPlatformCompositionRootSetupBase(IConfigurationRoot configurationRoot) : base(configurationRoot)
		{
		}

		protected override IEnumerable<IModule> GetAutofacModules()
		{
			var modules =  base.GetAutofacModules().ToList();

			modules.Add(new PlatformModule());

			return modules;
		}
	}
}
