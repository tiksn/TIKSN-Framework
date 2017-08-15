using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using TIKSN.DependencyInjection.Tests;
using Xunit;
using Xunit.Abstractions;

namespace TIKSN.Localization.Tests
{
	public class StringLocalizerSelectorTests
	{
		private readonly IServiceProvider _serviceProvider;

		public StringLocalizerSelectorTests(ITestOutputHelper testOutputHelper)
		{
			var compositionRoot = new TestCompositionRootSetup(testOutputHelper);
			_serviceProvider = compositionRoot.CreateServiceProvider();
		}

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
			var testStringLocalizer = new TestStringLocalizer(resourceNamesCache, _serviceProvider.GetRequiredService<ILogger<TestStringLocalizer>>());
			var stringLocalizerSelector = new StringLocalizerSelector(testStringLocalizer);

			stringLocalizerSelector.Select(new CultureInfo(cultureName));

			var actualLocalization = stringLocalizerSelector.GetRequiredString(resourceKey);

			actualLocalization.Should().Be(expectedLocalization);
		}
	}
}
