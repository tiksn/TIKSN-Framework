using Microsoft.Extensions.Localization;
using TIKSN.Localization;

namespace SelectingLocalization
{
	public class TextLocalizer : CompositeAssemblyStringLocalizer
	{
		public TextLocalizer(IResourceNamesCache resourceNamesCache) : base(resourceNamesCache)
		{
		}
	}
}
