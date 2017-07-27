using FluentAssertions;
using Microsoft.Extensions.Localization;
using System.Linq;
using Xunit;

namespace TIKSN.Localization.Tests
{
	public class CompositeAssemblyStringLocalizerTests
	{
		[Fact]
		public void KeyUniqueness()
		{
			var resourceNamesCache = new ResourceNamesCache();
			var testStringLocalizer = new TestStringLocalizer(resourceNamesCache);
			var duplicatesCount = testStringLocalizer.GetAllStrings().GroupBy(item => item.Name.ToLowerInvariant()).Count(item => item.Count() > 1);

			duplicatesCount.Should().Be(0);
		}
	}
}
