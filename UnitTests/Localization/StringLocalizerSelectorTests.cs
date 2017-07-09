using FluentAssertions;
using Microsoft.Extensions.Localization;
using System.Globalization;
using Xunit;

namespace TIKSN.Localization.Tests
{
	public class StringLocalizerSelectorTests
	{
		[Theory]
		[InlineData("931254976", "ru", "Реклама")]
		[InlineData("931254976", "ru-RU", "Реклама")]
		[InlineData("931254976", "hy", "Գովազդ")]
		[InlineData("931254976", "hy-AM", "Գովազդ")]
		[InlineData("931254976", "en-US", "Advertisement")]
		[InlineData("931254976", "en-GB", "Advertisement")]
		[InlineData("931254976", "en", "Advertisement")]
		[InlineData("931254976", "de-CH", "Advertisement")]
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
