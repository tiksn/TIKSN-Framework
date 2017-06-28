using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Localization;
using TIKSN.Localization;
using System.Linq;

namespace SelectingLocalization
{
	public class TextLocalizer : CompositeAssemblyStringLocalizer
	{
		public TextLocalizer(IResourceNamesCache resourceNamesCache) : base(resourceNamesCache)
		{
		}

		protected override IEnumerable<Assembly> GetAssemblies()
		{
			var result = base.GetAssemblies().ToList();

			result.Add(typeof(TextLocalizer).Assembly);

			return result;
		}
	}
}
