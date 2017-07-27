using Microsoft.Extensions.Localization;

namespace TIKSN.Localization.Tests
{
	public class TestStringLocalizer : CompositeAssemblyStringLocalizer
	{
		public TestStringLocalizer(IResourceNamesCache resourceNamesCache) : base(resourceNamesCache)
		{
		}
	}
}
