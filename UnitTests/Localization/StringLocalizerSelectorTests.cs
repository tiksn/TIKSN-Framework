using FluentAssertions;
using Microsoft.Extensions.Localization;
using System.Globalization;
using Xunit;

namespace TIKSN.Localization.Tests
{
	public class StringLocalizerSelectorTests
	{
		[Theory]
		[InlineData("066c2ac3-cacc-4271-9ed8-8a5cf9fb8369", "ru", "Реклама")]
		[InlineData("066c2ac3-cacc-4271-9ed8-8a5cf9fb8369", "ru-RU", "Реклама")]
		[InlineData("066c2ac3-cacc-4271-9ed8-8a5cf9fb8369", "hy", "Գովազդ")]
		[InlineData("066c2ac3-cacc-4271-9ed8-8a5cf9fb8369", "hy-AM", "Գովազդ")]
		[InlineData("066c2ac3-cacc-4271-9ed8-8a5cf9fb8369", "en-US", "Advertisement")]
		[InlineData("066c2ac3-cacc-4271-9ed8-8a5cf9fb8369", "en-GB", "Advertisement")]
		[InlineData("066c2ac3-cacc-4271-9ed8-8a5cf9fb8369", "en", "Advertisement")]
		[InlineData("066c2ac3-cacc-4271-9ed8-8a5cf9fb8369", "de-CH", "Advertisement")]
		public void SelectLocalization(string resourceKey, string cultureName, string expectedLocalization)
		{
			var resourceNamesCache = new ResourceNamesCache();
			var testStringLocalizer = new TestStringLocalizer(resourceNamesCache);
			var stringLocalizerSelector = new StringLocalizerSelector(testStringLocalizer);

			stringLocalizerSelector.Select(new CultureInfo(cultureName));

			var actualLocalization = stringLocalizerSelector.GetRequiredString(resourceKey);

			actualLocalization.Should().Be(expectedLocalization);
		}
	}
}
