using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;
using TIKSN.FileSystem;
using TIKSN.Settings;
using Xunit;

namespace TIKSN.Tests.Settings;

public class SettingsServiceTests
{
    private readonly ServiceCollection services;
    private readonly ISettingsService settingsService;

    public SettingsServiceTests()
    {
        this.services = new ServiceCollection();
        this.SetupDenepdencies();
        var serviceProvider = this.services.BuildServiceProvider();
        this.settingsService = serviceProvider.GetRequiredService<ISettingsService>();
    }

    [Fact]
    public void LocalSettingsGuidTest()
    {
        var expectedValue = Guid.Parse("368c0eb7-0891-4498-ab6b-2bc10a5c9152");

        this.settingsService.SetLocalSetting("LocalGuid", expectedValue);

        var actualValue = this.settingsService.GetLocalSetting("LocalGuid", Guid.Parse("853dda84-4b61-4c11-9d79-ea3c4c9d8de1"));

        _ = actualValue.Should().Be(expectedValue);

        var actualValueOption = this.settingsService.GetLocalSetting<Guid>("LocalGuid");

        _ = actualValueOption.IsSome.Should().BeTrue();
        _ = actualValueOption.IfNone(Guid.Empty).Should().Be(expectedValue);

        this.settingsService.RemoveLocalSetting("LocalGuid");

        var newValue = Guid.NewGuid();
        actualValue = this.settingsService.GetLocalSetting("LocalGuid", newValue);

        _ = actualValue.Should().Be(newValue);

        actualValueOption = this.settingsService.GetLocalSetting<Guid>("LocalGuid");

        _ = actualValueOption.IsNone.Should().BeTrue();
    }

    [Fact]
    public void LocalSettingsIntegerTest()
    {
        var rng = new Random();
        var expectedValue = rng.Next();

        this.settingsService.SetLocalSetting("LocalInteger", expectedValue);

        var actualValue = this.settingsService.GetLocalSetting("LocalInteger", expectedValue + 10);

        _ = actualValue.Should().Be(expectedValue);

        var actualValueOption = this.settingsService.GetLocalSetting<int>("LocalInteger");

        _ = actualValueOption.IsSome.Should().BeTrue();
        _ = actualValueOption.IfNone(-1).Should().Be(expectedValue);

        this.settingsService.RemoveLocalSetting("LocalInteger");

        actualValue = this.settingsService.GetLocalSetting("LocalInteger", expectedValue + 120);

        _ = actualValue.Should().Be(expectedValue + 120);

        actualValueOption = this.settingsService.GetLocalSetting<int>("LocalInteger");

        _ = actualValueOption.IsNone.Should().BeTrue();
    }

    [Fact]
    public void LocalSettingsListingTest()
    {
        var rng = new Random();

        this.settingsService.SetLocalSetting("LocalInteger", rng.Next());
        this.settingsService.SetLocalSetting("LocalString", $"{Guid.NewGuid()}---{rng.Next()}");
        this.settingsService.SetLocalSetting("LocalGuid", Guid.NewGuid());

        var names = this.settingsService.ListLocalSetting();

        _ = names.Should().BeEquivalentTo(["LocalInteger", "LocalString", "LocalGuid"]);
    }

    [Fact]
    public void LocalSettingsStringTest()
    {
        var expectedValue = Guid.NewGuid().ToString();

        this.settingsService.SetLocalSetting("LocalString", expectedValue);

        var actualValue = this.settingsService.GetLocalSetting("LocalString", expectedValue + 10);

        _ = actualValue.Should().Be(expectedValue);

        var actualValueOption = this.settingsService.GetLocalSetting<string>("LocalString");

        _ = actualValueOption.IsSome.Should().BeTrue();
        _ = actualValueOption.IfNone(string.Empty).Should().Be(expectedValue);

        this.settingsService.RemoveLocalSetting("LocalString");

        actualValue = this.settingsService.GetLocalSetting("LocalString", expectedValue + 120);

        _ = actualValue.Should().Be(expectedValue + 120);

        actualValueOption = this.settingsService.GetLocalSetting<string>("LocalString");

        _ = actualValueOption.IsNone.Should().BeTrue();
    }

    [Fact]
    public void RoamingSettingsGuidTest()
    {
        var expectedValue = Guid.Parse("368c0eb7-0891-4498-ab6b-2bc10a5c9152");

        this.settingsService.SetRoamingSetting("RoamingGuid", expectedValue);

        var actualValue = this.settingsService.GetRoamingSetting("RoamingGuid", Guid.Parse("853dda84-4b61-4c11-9d79-ea3c4c9d8de1"));

        _ = actualValue.Should().Be(expectedValue);

        var actualValueOption = this.settingsService.GetRoamingSetting<Guid>("RoamingGuid");

        _ = actualValueOption.IsSome.Should().BeTrue();
        _ = actualValueOption.IfNone(Guid.Empty).Should().Be(expectedValue);

        this.settingsService.RemoveRoamingSetting("RoamingGuid");

        var newValue = Guid.NewGuid();
        actualValue = this.settingsService.GetRoamingSetting("RoamingGuid", newValue);

        _ = actualValue.Should().Be(newValue);

        actualValueOption = this.settingsService.GetRoamingSetting<Guid>("RoamingGuid");

        _ = actualValueOption.IsNone.Should().BeTrue();
    }

    [Fact]
    public void RoamingSettingsIntegerTest()
    {
        var rng = new Random();
        var expectedValue = rng.Next();

        this.settingsService.SetRoamingSetting("RoamingInteger", expectedValue);

        var actualValue = this.settingsService.GetRoamingSetting("RoamingInteger", expectedValue + 10);

        _ = actualValue.Should().Be(expectedValue);

        var actualValueOption = this.settingsService.GetRoamingSetting<int>("RoamingInteger");

        _ = actualValueOption.IsSome.Should().BeTrue();
        _ = actualValueOption.IfNone(-1).Should().Be(expectedValue);

        this.settingsService.RemoveRoamingSetting("RoamingInteger");

        actualValue = this.settingsService.GetRoamingSetting("RoamingInteger", expectedValue + 120);

        _ = actualValue.Should().Be(expectedValue + 120);

        actualValueOption = this.settingsService.GetRoamingSetting<int>("RoamingInteger");

        _ = actualValueOption.IsNone.Should().BeTrue();
    }

    [Fact]
    public void RoamingSettingsListingTest()
    {
        var rng = new Random();

        this.settingsService.SetRoamingSetting("RoamingInteger", rng.Next());
        this.settingsService.SetRoamingSetting("RoamingString", $"{Guid.NewGuid()}---{rng.Next()}");
        this.settingsService.SetRoamingSetting("RoamingGuid", Guid.NewGuid());

        var names = this.settingsService.ListRoamingSetting();

        _ = names.Should().BeEquivalentTo(["RoamingInteger", "RoamingString", "RoamingGuid"]);
    }

    [Fact]
    public void RoamingSettingsStringTest()
    {
        var expectedValue = Guid.NewGuid().ToString();

        this.settingsService.SetRoamingSetting("RoamingString", expectedValue);

        var actualValue = this.settingsService.GetRoamingSetting("RoamingString", expectedValue + 10);

        _ = actualValue.Should().Be(expectedValue);

        var actualValueOption = this.settingsService.GetRoamingSetting<string>("RoamingString");

        _ = actualValueOption.IsSome.Should().BeTrue();
        _ = actualValueOption.IfNone(string.Empty).Should().Be(expectedValue);

        this.settingsService.RemoveRoamingSetting("RoamingString");

        actualValue = this.settingsService.GetRoamingSetting("RoamingString", expectedValue + 120);

        _ = actualValue.Should().Be(expectedValue + 120);

        actualValueOption = this.settingsService.GetRoamingSetting<string>("RoamingString");

        _ = actualValueOption.IsNone.Should().BeTrue();
    }

    private void SetupDenepdencies()
    {
        _ = this.services.AddFrameworkCore();
        _ = this.services.AddSingleton<ISettingsService, FileSettingsService>();
        _ = this.services.AddSingleton(new KnownFoldersConfiguration(this.GetType().Assembly,
            KnownFolderVersionConsideration.None));

        var configurationRoot = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();

        configurationRoot["RelativePath"] = "settings.db";

        _ = this.services.ConfigurePartial<FileSettingsServiceOptions, FileSettingsServiceOptionsValidator>(
            configurationRoot);
    }
}
