using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TIKSN.DependencyInjection;
using TIKSN.FileSystem;
using TIKSN.Settings;
using Xunit;

namespace TIKSN.Tests.Settings;

public class SettingsServiceTests
{
    private readonly ServiceCollection services;
    private readonly ISettingsService settingsService;
    private static readonly string[] expectedLocal = ["LocalInteger", "LocalString", "LocalGuid"];
    private static readonly string[] expectedRoaming = ["RoamingInteger", "RoamingString", "RoamingGuid"];

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

        actualValue.ShouldBe(expectedValue);

        var actualValueOption = this.settingsService.GetLocalSetting<Guid>("LocalGuid");

        actualValueOption.IsSome.ShouldBeTrue();
        actualValueOption.IfNone(Guid.Empty).ShouldBe(expectedValue);

        this.settingsService.RemoveLocalSetting("LocalGuid");

        var newValue = Guid.NewGuid();
        actualValue = this.settingsService.GetLocalSetting("LocalGuid", newValue);

        actualValue.ShouldBe(newValue);

        actualValueOption = this.settingsService.GetLocalSetting<Guid>("LocalGuid");

        actualValueOption.IsNone.ShouldBeTrue();
    }

    [Fact]
    public void LocalSettingsIntegerTest()
    {
        var rng = new Random();
        var expectedValue = rng.Next();

        this.settingsService.SetLocalSetting("LocalInteger", expectedValue);

        var actualValue = this.settingsService.GetLocalSetting("LocalInteger", expectedValue + 10);

        actualValue.ShouldBe(expectedValue);

        var actualValueOption = this.settingsService.GetLocalSetting<int>("LocalInteger");

        actualValueOption.IsSome.ShouldBeTrue();
        actualValueOption.IfNone(-1).ShouldBe(expectedValue);

        this.settingsService.RemoveLocalSetting("LocalInteger");

        actualValue = this.settingsService.GetLocalSetting("LocalInteger", expectedValue + 120);

        actualValue.ShouldBe(expectedValue + 120);

        actualValueOption = this.settingsService.GetLocalSetting<int>("LocalInteger");

        actualValueOption.IsNone.ShouldBeTrue();
    }

    [Fact]
    public void LocalSettingsListingTest()
    {
        var rng = new Random();

        this.settingsService.SetLocalSetting("LocalInteger", rng.Next());
        this.settingsService.SetLocalSetting("LocalString", $"{Guid.NewGuid()}---{rng.Next()}");
        this.settingsService.SetLocalSetting("LocalGuid", Guid.NewGuid());

        var names = this.settingsService.ListLocalSetting();

        names.ShouldBeEquivalentTo(expectedLocal);
    }

    [Fact]
    public void LocalSettingsStringTest()
    {
        var expectedValue = Guid.NewGuid().ToString();

        this.settingsService.SetLocalSetting("LocalString", expectedValue);

        var actualValue = this.settingsService.GetLocalSetting("LocalString", expectedValue + 10);

        actualValue.ShouldBe(expectedValue);

        var actualValueOption = this.settingsService.GetLocalSetting<string>("LocalString");

        actualValueOption.IsSome.ShouldBeTrue();
        actualValueOption.IfNone(string.Empty).ShouldBe(expectedValue);

        this.settingsService.RemoveLocalSetting("LocalString");

        actualValue = this.settingsService.GetLocalSetting("LocalString", expectedValue + 120);

        actualValue.ShouldBe(expectedValue + 120);

        actualValueOption = this.settingsService.GetLocalSetting<string>("LocalString");

        actualValueOption.IsNone.ShouldBeTrue();
    }

    [Fact]
    public void RoamingSettingsGuidTest()
    {
        var expectedValue = Guid.Parse("368c0eb7-0891-4498-ab6b-2bc10a5c9152");

        this.settingsService.SetRoamingSetting("RoamingGuid", expectedValue);

        var actualValue = this.settingsService.GetRoamingSetting("RoamingGuid", Guid.Parse("853dda84-4b61-4c11-9d79-ea3c4c9d8de1"));

        actualValue.ShouldBe(expectedValue);

        var actualValueOption = this.settingsService.GetRoamingSetting<Guid>("RoamingGuid");

        actualValueOption.IsSome.ShouldBeTrue();
        actualValueOption.IfNone(Guid.Empty).ShouldBe(expectedValue);

        this.settingsService.RemoveRoamingSetting("RoamingGuid");

        var newValue = Guid.NewGuid();
        actualValue = this.settingsService.GetRoamingSetting("RoamingGuid", newValue);

        actualValue.ShouldBe(newValue);

        actualValueOption = this.settingsService.GetRoamingSetting<Guid>("RoamingGuid");

        actualValueOption.IsNone.ShouldBeTrue();
    }

    [Fact]
    public void RoamingSettingsIntegerTest()
    {
        var rng = new Random();
        var expectedValue = rng.Next();

        this.settingsService.SetRoamingSetting("RoamingInteger", expectedValue);

        var actualValue = this.settingsService.GetRoamingSetting("RoamingInteger", expectedValue + 10);

        actualValue.ShouldBe(expectedValue);

        var actualValueOption = this.settingsService.GetRoamingSetting<int>("RoamingInteger");

        actualValueOption.IsSome.ShouldBeTrue();
        actualValueOption.IfNone(-1).ShouldBe(expectedValue);

        this.settingsService.RemoveRoamingSetting("RoamingInteger");

        actualValue = this.settingsService.GetRoamingSetting("RoamingInteger", expectedValue + 120);

        actualValue.ShouldBe(expectedValue + 120);

        actualValueOption = this.settingsService.GetRoamingSetting<int>("RoamingInteger");

        actualValueOption.IsNone.ShouldBeTrue();
    }

    [Fact]
    public void RoamingSettingsListingTest()
    {
        var rng = new Random();

        this.settingsService.SetRoamingSetting("RoamingInteger", rng.Next());
        this.settingsService.SetRoamingSetting("RoamingString", $"{Guid.NewGuid()}---{rng.Next()}");
        this.settingsService.SetRoamingSetting("RoamingGuid", Guid.NewGuid());

        var names = this.settingsService.ListRoamingSetting();

        names.ShouldBeEquivalentTo(expectedRoaming);
    }

    [Fact]
    public void RoamingSettingsStringTest()
    {
        var expectedValue = Guid.NewGuid().ToString();

        this.settingsService.SetRoamingSetting("RoamingString", expectedValue);

        var actualValue = this.settingsService.GetRoamingSetting("RoamingString", expectedValue + 10);

        actualValue.ShouldBe(expectedValue);

        var actualValueOption = this.settingsService.GetRoamingSetting<string>("RoamingString");

        actualValueOption.IsSome.ShouldBeTrue();
        actualValueOption.IfNone(string.Empty).ShouldBe(expectedValue);

        this.settingsService.RemoveRoamingSetting("RoamingString");

        actualValue = this.settingsService.GetRoamingSetting("RoamingString", expectedValue + 120);

        actualValue.ShouldBe(expectedValue + 120);

        actualValueOption = this.settingsService.GetRoamingSetting<string>("RoamingString");

        actualValueOption.IsNone.ShouldBeTrue();
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
