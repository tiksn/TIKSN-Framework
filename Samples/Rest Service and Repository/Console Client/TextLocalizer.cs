using Microsoft.Extensions.Localization;
using TIKSN.Localization;

namespace Console_Client
{
	public class TextLocalizer : CompositeAssemblyStringLocalizer
	{
		public TextLocalizer(IResourceNamesCache resourceNamesCache) : base(resourceNamesCache)
		{
		}
	}
}
