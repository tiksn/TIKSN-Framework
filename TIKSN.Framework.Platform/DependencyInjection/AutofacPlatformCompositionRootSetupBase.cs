﻿using System.Collections.Generic;
using Autofac.Core;
using System.Linq;

namespace TIKSN.DependencyInjection
{
	public abstract class AutofacPlatformCompositionRootSetupBase : AutofacCompositionRootSetupBase
	{
		protected override IEnumerable<IModule> GetAutofacModules()
		{
			var modules =  base.GetAutofacModules().ToList();

			modules.Add(new PlatformModule());

			return modules;
		}
	}
}
