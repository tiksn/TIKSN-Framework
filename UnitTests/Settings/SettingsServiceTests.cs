using Microsoft.Extensions.DependencyInjection;
using System;
using TIKSN.Framework.UnitTests.DI;
using TIKSN.Settings;
using Xunit;

namespace TIKSN.Framework.UnitTests.Settings
{
	public class SettingsServiceTests
	{
		private readonly ISettingsService settingsService;

		public SettingsServiceTests()
		{
			settingsService = Dependencies.ServiceProvider.GetRequiredService<ISettingsService>();
		}

		[Fact]
		public void LocalSettingsIntegerTest()
		{
			var rng = new Random();
			var expectedValue = rng.Next();

			settingsService.SetLocalSetting("LocalInteger", expectedValue);

			var actualValue = settingsService.GetLocalSetting("LocalInteger", expectedValue + 10);

			Assert.Equal(expectedValue, actualValue);
		}

		[Fact]
		public void LocalSettingsStringTest()
		{
			var expectedValue = Guid.NewGuid().ToString();

			settingsService.SetLocalSetting("LocalString", expectedValue);

			var actualValue = settingsService.GetLocalSetting("LocalString", expectedValue + 10);

			Assert.Equal(expectedValue, actualValue);
		}

		[Fact]
		public void RoamingSettingsIntegerTest()
		{
			var rng = new Random();
			var expectedValue = rng.Next();

			settingsService.SetRoamingSetting("RoamingInteger", expectedValue);

			var actualValue = settingsService.GetRoamingSetting("RoamingInteger", expectedValue + 10);

			Assert.Equal(expectedValue, actualValue);
		}

		[Fact]
		public void RoamingSettingsStringTest()
		{
			var expectedValue = Guid.NewGuid().ToString();

			settingsService.SetRoamingSetting("RoamingString", expectedValue);

			var actualValue = settingsService.GetRoamingSetting("RoamingString", expectedValue + 10);

			Assert.Equal(expectedValue, actualValue);
		}
	}
}