using System.Threading.Tasks;
using LanguageExt;
using Microsoft.Extensions.Options;
using NSubstitute;
using OpenFeature.Constant;
using Shouldly;
using TIKSN.FeatureManagement;
using TIKSN.Settings;
using Xunit;
using static LanguageExt.Prelude;

namespace TIKSN.Tests.FeatureManagement;

public class SettingsFeatureProviderTests
{
    [Fact]
    public async Task GivenLocalAndRoamingSettings_WhenLocalThenRoaming_ThenPrefersLocalAndFallsBackToRoaming()
    {
        var settingsService = Substitute.For<ISettingsService>();
        _ = settingsService.GetLocalSetting<bool>("FeatureFlags:LocalFlag").Returns(Some(true));
        _ = settingsService.GetLocalSetting<bool>("FeatureFlags:RoamingFlag").Returns(Option<bool>.None);
        _ = settingsService.GetRoamingSetting<bool>("FeatureFlags:RoamingFlag").Returns(Some(true));

        var provider = new SettingsFeatureProvider(
            settingsService,
            Options.Create(new SettingsFeatureProviderOptions
            {
                LookupScope = SettingsLookupScope.LocalThenRoaming,
            }));

        var localResult = await provider.ResolveBooleanValueAsync(
            "LocalFlag",
            defaultValue: false,
            cancellationToken: TestContext.Current.CancellationToken);
        var roamingResult = await provider.ResolveBooleanValueAsync(
            "RoamingFlag",
            defaultValue: false,
            cancellationToken: TestContext.Current.CancellationToken);

        localResult.Value.ShouldBeTrue();
        localResult.ErrorType.ShouldBe(ErrorType.None);
        roamingResult.Value.ShouldBeTrue();
        roamingResult.ErrorType.ShouldBe(ErrorType.None);
    }

    [Fact]
    public async Task GivenMissingSetting_WhenResolved_ThenReturnsDefaultAndFlagNotFound()
    {
        var settingsService = Substitute.For<ISettingsService>();
        _ = settingsService.GetLocalSetting<int>("FeatureFlags:Missing").Returns(Option<int>.None);

        var provider = new SettingsFeatureProvider(
            settingsService,
            Options.Create(new SettingsFeatureProviderOptions
            {
                LookupScope = SettingsLookupScope.LocalOnly,
            }));

        var result = await provider.ResolveIntegerValueAsync(
            "Missing",
            defaultValue: 99,
            cancellationToken: TestContext.Current.CancellationToken);

        result.Value.ShouldBe(99);
        result.ErrorType.ShouldBe(ErrorType.FlagNotFound);
        result.Reason.ShouldBe(Reason.Default);
    }
}
