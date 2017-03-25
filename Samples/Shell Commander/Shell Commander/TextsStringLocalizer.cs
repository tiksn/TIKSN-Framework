using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Reflection;
using TIKSN.DependencyInjection;
using TIKSN.Localization;

namespace Shell_Commander
{
	public class TextsStringLocalizer : CompositeAssemblyStringLocalizer
	{
		public TextsStringLocalizer(IResourceNamesCache resourceNamesCache) : base(resourceNamesCache)
		{
		}

		protected override IEnumerable<Assembly> GetAssemblies()
		{
			yield return this.GetType().GetTypeInfo().Assembly;

			yield return typeof(DependencyRegistration).GetTypeInfo().Assembly;
		}
	}
}