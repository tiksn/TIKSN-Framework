using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Localization;
using TIKSN.Localization;
using System.Linq;
using System;
using System.Resources;
using System.Globalization;

namespace SelectingLocalization
{
	public class TextLocalizer : CompositeAssemblyStringLocalizer
	{
		public TextLocalizer(IResourceNamesCache resourceNamesCache) : base(resourceNamesCache)
		{
			var result = new List<IStringLocalizer>();

			var assemblies = GetAssemblies().ToArray();

			Console.WriteLine();
			foreach (var assembly in assemblies)
			{
				foreach (var manifestResourceName in assembly.GetManifestResourceNames())
				{
					Console.WriteLine($"manifestResourceName = {manifestResourceName}");
					if (manifestResourceName.EndsWith(".resources", StringComparison.OrdinalIgnoreCase))
					{
						var resourceName = manifestResourceName.Substring(0, manifestResourceName.Length - 10);
						var resourceManager = new ResourceManager(resourceName, assembly);
						var rmsl = new ResourceManagerStringLocalizer(resourceManager, assembly, resourceName, resourceNamesCache);
					}
				}
			}
		}

		protected override IEnumerable<Assembly> GetAssemblies()
		{
			var result = base.GetAssemblies().ToList();

			result.Add(typeof(TextLocalizer).Assembly);

			return result;
		}
	}
}
