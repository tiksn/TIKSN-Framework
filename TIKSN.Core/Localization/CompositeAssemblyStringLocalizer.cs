using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace TIKSN.Localization
{
	public abstract class CompositeAssemblyStringLocalizer : CompositeStringLocalizer
	{
		private readonly IResourceNamesCache resourceNamesCache;
		private Lazy<IEnumerable<IStringLocalizer>> localizers;

		protected CompositeAssemblyStringLocalizer(IResourceNamesCache resourceNamesCache)
		{
			this.resourceNamesCache = resourceNamesCache;
			localizers = new Lazy<IEnumerable<IStringLocalizer>>(CreateLocalizers, false);
		}

		public override IEnumerable<IStringLocalizer> Localizers
		{
			get
			{
				return localizers.Value;
			}
		}

		protected abstract IEnumerable<Assembly> GetAssemblies();

		private IEnumerable<IStringLocalizer> CreateLocalizers()
		{
			var result = new List<IStringLocalizer>();

			var assemblies = GetAssemblies();

			foreach (var assembly in assemblies)
			{
				foreach (var manifestResourceName in assembly.GetManifestResourceNames())
				{
					if (manifestResourceName.EndsWith(".resources", StringComparison.OrdinalIgnoreCase))
					{
						var resourceName = manifestResourceName.Substring(0, manifestResourceName.Length - 10);
						var resourceManager = new ResourceManager(resourceName, assembly);
						result.Add(new ResourceManagerStringLocalizer(resourceManager, assembly, resourceName, resourceNamesCache));
					}
				}
			}

			return result;
		}
	}
}