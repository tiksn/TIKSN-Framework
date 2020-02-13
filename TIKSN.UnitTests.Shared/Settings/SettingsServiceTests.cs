using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace TIKSN.Settings.Tests
{
    public partial class SettingsServiceTests
    {
        private readonly ServiceCollection services;
        private readonly ISettingsService settingsService;

        public SettingsServiceTests()
        {
            services = new ServiceCollection();
            SetupDenepdencies();
            var serviceProvider = services.BuildServiceProvider();
            settingsService = serviceProvider.GetRequiredService<ISettingsService>();
        }

        [Fact]
        public void LocalSettingsIntegerTest()
        {
            var rng = new Random();
            var expectedValue = rng.Next();

            settingsService.SetLocalSetting("LocalInteger", expectedValue);

            var actualValue = settingsService.GetLocalSetting("LocalInteger", expectedValue + 10);

            Assert.Equal(expectedValue, actualValue);

            settingsService.RemoveLocalSetting("LocalInteger");

            actualValue = settingsService.GetLocalSetting("LocalInteger", expectedValue + 120);

            Assert.Equal(expectedValue + 120, actualValue);
        }

        [Fact]
        public void LocalSettingsGuidTest()
        {
            var expectedValue = Guid.NewGuid();

            settingsService.SetLocalSetting("LocalGuid", expectedValue);

            var actualValue = settingsService.GetLocalSetting("LocalGuid", Guid.NewGuid());

            Assert.Equal(expectedValue, actualValue);

            settingsService.RemoveLocalSetting("LocalGuid");

            var newValue = Guid.NewGuid();
            actualValue = settingsService.GetLocalSetting("LocalGuid", newValue);

            Assert.Equal(newValue, actualValue);
        }

        [Fact]
        public void LocalSettingsListingTest()
        {
            var rng = new Random();

            settingsService.SetLocalSetting("LocalInteger", rng.Next());
            settingsService.SetLocalSetting("LocalString", $"{Guid.NewGuid()}---{rng.Next()}");
            settingsService.SetLocalSetting("LocalGuid", Guid.NewGuid());

            var names = settingsService.ListLocalSetting();

            names.Should().BeEquivalentTo(new[] { "LocalInteger", "LocalString", "LocalGuid" });
        }

        [Fact]
        public void LocalSettingsStringTest()
        {
            var expectedValue = Guid.NewGuid().ToString();

            settingsService.SetLocalSetting("LocalString", expectedValue);

            var actualValue = settingsService.GetLocalSetting("LocalString", expectedValue + 10);

            Assert.Equal(expectedValue, actualValue);

            settingsService.RemoveLocalSetting("LocalString");

            actualValue = settingsService.GetLocalSetting("LocalString", expectedValue + 120);

            Assert.Equal(expectedValue + 120, actualValue);
        }

        [Fact]
        public void RoamingSettingsGuidTest()
        {
            var expectedValue = Guid.NewGuid();

            settingsService.SetRoamingSetting("RoamingGuid", expectedValue);

            var actualValue = settingsService.GetRoamingSetting("RoamingGuid", Guid.NewGuid());

            Assert.Equal(expectedValue, actualValue);

            settingsService.RemoveRoamingSetting("RoamingGuid");

            var newValue = Guid.NewGuid();
            actualValue = settingsService.GetRoamingSetting("RoamingGuid", newValue);

            Assert.Equal(newValue, actualValue);
        }

        [Fact]
        public void RoamingSettingsListingTest()
        {
            var rng = new Random();

            settingsService.SetRoamingSetting("RoamingInteger", rng.Next());
            settingsService.SetRoamingSetting("RoamingString", $"{Guid.NewGuid()}---{rng.Next()}");
            settingsService.SetRoamingSetting("RoamingGuid", Guid.NewGuid());

            var names = settingsService.ListRoamingSetting();

            names.Should().BeEquivalentTo(new[] { "RoamingInteger", "RoamingString", "RoamingGuid" });
        }

        [Fact]
        public void RoamingSettingsIntegerTest()
        {
            var rng = new Random();
            var expectedValue = rng.Next();

            settingsService.SetRoamingSetting("RoamingInteger", expectedValue);

            var actualValue = settingsService.GetRoamingSetting("RoamingInteger", expectedValue + 10);

            Assert.Equal(expectedValue, actualValue);

            settingsService.RemoveRoamingSetting("RoamingInteger");

            actualValue = settingsService.GetRoamingSetting("RoamingInteger", expectedValue + 120);

            Assert.Equal(expectedValue + 120, actualValue);
        }

        [Fact]
        public void RoamingSettingsStringTest()
        {
            var expectedValue = Guid.NewGuid().ToString();

            settingsService.SetRoamingSetting("RoamingString", expectedValue);

            var actualValue = settingsService.GetRoamingSetting("RoamingString", expectedValue + 10);

            Assert.Equal(expectedValue, actualValue);

            settingsService.RemoveRoamingSetting("RoamingString");

            actualValue = settingsService.GetRoamingSetting("RoamingString", expectedValue + 120);

            Assert.Equal(expectedValue + 120, actualValue);
        }

        partial void SetupDenepdencies();
    }
}