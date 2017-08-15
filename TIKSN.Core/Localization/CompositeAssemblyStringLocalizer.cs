using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace TIKSN.Localization
{
	public abstract class CompositeAssemblyStringLocalizer : CompositeStringLocalizer
	{
		private readonly IResourceNamesCache resourceNamesCache;
		private Lazy<IEnumerable<IStringLocalizer>> localizers;
		private readonly ILogger<CompositeAssemblyStringLocalizer> _logger;

		protected CompositeAssemblyStringLocalizer(IResourceNamesCache resourceNamesCache, ILogger<CompositeAssemblyStringLocalizer> logger)
		{
			this.resourceNamesCache = resourceNamesCache;
			localizers = new Lazy<IEnumerable<IStringLocalizer>>(CreateLocalizers, false);
			_logger = logger;
		}

		public override IEnumerable<IStringLocalizer> Localizers
		{
			get
			{
				return localizers.Value;
			}
		}

		protected virtual IEnumerable<Assembly> GetAssemblies()
		{
			yield return typeof(CompositeAssemblyStringLocalizer).GetTypeInfo().Assembly;

			yield return typeof(LanguageLocalizationParameters).GetTypeInfo().Assembly;

			yield return typeof(RegionLocalizationParameters).GetTypeInfo().Assembly;
		}

		private IEnumerable<IStringLocalizer> CreateLocalizers()
		{
			var result = new List<IStringLocalizer>();

			var assemblies = GetAssemblies().ToArray();

			foreach (var assembly in assemblies)
			{
				foreach (var manifestResourceName in assembly.GetManifestResourceNames())
				{
					if (manifestResourceName.EndsWith(".resources", StringComparison.OrdinalIgnoreCase))
					{
						var resourceName = manifestResourceName.Substring(0, manifestResourceName.Length - 10);
						var resourceManager = new ResourceManager(resourceName, assembly);
						result.Add(new ResourceManagerStringLocalizer(resourceManager, assembly, resourceName, resourceNamesCache, _logger));
					}
				}
			}

			return result;
		}
	}
}