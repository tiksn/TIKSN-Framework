using System;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using OpenFeature;
using OpenFeature.Constant;
using OpenFeature.Model;
using Shouldly;
using TIKSN.DependencyInjection;
using TIKSN.FeatureManagement;
using TIKSN.Settings;
using Xunit;
using static LanguageExt.Prelude;

namespace TIKSN.Tests.FeatureManagement;

public class SettingsFeatureProviderTests
{
    [Fact]
    public async Task GivenCustomKeyPrefixAndAllValueTypes_WhenResolved_ThenReturnsConfiguredValues()
    {
        var settingsService = Substitute.For<ISettingsService>();
        _ = settingsService.GetRoamingSetting<double>("Flags.Threshold").Returns(Some(0.75));
        _ = settingsService.GetRoamingSetting<string>("Flags.Banner").Returns(Some("Welcome"));
        _ = settingsService.GetRoamingSetting<string>("Flags.ConfigJson")
            .Returns(Some( /*lang=json,strict*/ "{\"mode\":\"fast\"}"));

        var provider = new SettingsFeatureProvider(
            settingsService,
            Options.Create(new SettingsFeatureProviderOptions
            {
                KeyPrefix = "Flags.",
                LookupScope = SettingsLookupScope.RoamingOnly,
            }));

        provider.GetMetadata().Name.ShouldBe("TIKSN.SettingsFeatureProvider");

        var doubleResult = await provider.ResolveDoubleValueAsync(
            "Threshold",
            defaultValue: 0.0,
            cancellationToken: TestContext.Current.CancellationToken);
        var stringResult = await provider.ResolveStringValueAsync(
            "Banner",
            defaultValue: string.Empty,
            cancellationToken: TestContext.Current.CancellationToken);
        var structResult = await provider.ResolveStructureValueAsync(
            "ConfigJson",
            new Value(),
            cancellationToken: TestContext.Current.CancellationToken);

        doubleResult.Value.ShouldBe(0.75);
        doubleResult.ErrorType.ShouldBe(ErrorType.None);

        stringResult.Value.ShouldBe("Welcome");
        stringResult.ErrorType.ShouldBe(ErrorType.None);

        structResult.Value.AsString.ShouldBe( /*lang=json,strict*/ "{\"mode\":\"fast\"}");
        structResult.ErrorType.ShouldBe(ErrorType.None);
    }

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
        _ = settingsService.GetLocalSetting<int>("Missing").Returns(Option<int>.None);
        _ = settingsService.GetLocalSetting<string>("MissingStruct").Returns(Option<string>.None);

        var provider = new SettingsFeatureProvider(
            settingsService,
            Options.Create(new SettingsFeatureProviderOptions
            {
                KeyPrefix = string.Empty,
                LookupScope = SettingsLookupScope.LocalOnly,
            }));

        var result = await provider.ResolveIntegerValueAsync(
            "Missing",
            defaultValue: 99,
            cancellationToken: TestContext.Current.CancellationToken);
        var structResult = await provider.ResolveStructureValueAsync(
            "MissingStruct",
            new Value("fallback"),
            cancellationToken: TestContext.Current.CancellationToken);

        result.Value.ShouldBe(99);
        result.ErrorType.ShouldBe(ErrorType.FlagNotFound);
        result.Reason.ShouldBe(Reason.Default);

        structResult.Value.AsString.ShouldBe("fallback");
        structResult.ErrorType.ShouldBe(ErrorType.FlagNotFound);
        structResult.Reason.ShouldBe(Reason.Default);
    }

    [Fact]
    public async Task GivenOpenFeatureBuilderAddSettingsProvider_WhenEvaluated_ThenResolvesFlags()
    {
        var settingsService = Substitute.For<ISettingsService>();
        _ = settingsService.GetLocalSetting<bool>("CustomPrefix:DarkTheme").Returns(Some(true));

        var services = new ServiceCollection();
        _ = services.AddLogging();
        _ = services.AddSingleton(settingsService);
        _ = services.AddOpenFeature(builder => builder
            .AddSettingsProvider(options => options.KeyPrefix = "CustomPrefix:"));

        await using var serviceProvider = services.BuildServiceProvider();
        foreach (var hostedService in serviceProvider.GetServices<IHostedService>())
        {
            await hostedService.StartAsync(TestContext.Current.CancellationToken);
        }

        var client = serviceProvider.GetRequiredService<IFeatureClient>();
        var enabled = await client.GetBooleanValueAsync(
            "DarkTheme",
            defaultValue: false,
            cancellationToken: TestContext.Current.CancellationToken);

        enabled.ShouldBeTrue();

        var domainServices = new ServiceCollection();
        _ = domainServices.AddLogging();
        _ = domainServices.AddSingleton(settingsService);
        _ = domainServices.AddOpenFeature(builder => builder
            .AddPolicyName(policy => policy.DefaultNameSelector = _ => "domain-b")
            .AddSettingsProvider("domain-a")
            .AddSettingsProvider("domain-b", options => options.KeyPrefix = "CustomPrefix:"));

        await using var domainServiceProvider = domainServices.BuildServiceProvider();
        foreach (var hostedService in domainServiceProvider.GetServices<IHostedService>())
        {
            await hostedService.StartAsync(TestContext.Current.CancellationToken);
        }

        var domainClient = domainServiceProvider.GetRequiredService<IFeatureClient>();
        var domainEnabled = await domainClient.GetBooleanValueAsync(
            "DarkTheme",
            defaultValue: false,
            cancellationToken: TestContext.Current.CancellationToken);

        domainEnabled.ShouldBeTrue();
    }

    [Fact]
    public async Task GivenRoamingThenLocalScope_WhenRoamingMissing_ThenFallsBackToLocal()
    {
        var settingsService = Substitute.For<ISettingsService>();
        _ = settingsService.GetRoamingSetting<string>("FeatureFlags:Greeting").Returns(Some("RoamingHello"));
        _ = settingsService.GetLocalSetting<string>("FeatureFlags:Greeting").Returns(Some("LocalHello"));
        _ = settingsService.GetRoamingSetting<string>("FeatureFlags:LocalFallback").Returns(Option<string>.None);
        _ = settingsService.GetLocalSetting<string>("FeatureFlags:LocalFallback").Returns(Some("FromLocal"));

        var provider = new SettingsFeatureProvider(
            settingsService,
            Options.Create(new SettingsFeatureProviderOptions
            {
                LookupScope = SettingsLookupScope.RoamingThenLocal,
            }));

        var greeting = await provider.ResolveStringValueAsync(
            "Greeting",
            defaultValue: string.Empty,
            cancellationToken: TestContext.Current.CancellationToken);
        var fallback = await provider.ResolveStringValueAsync(
            "LocalFallback",
            defaultValue: string.Empty,
            cancellationToken: TestContext.Current.CancellationToken);

        greeting.Value.ShouldBe("RoamingHello");
        fallback.Value.ShouldBe("FromLocal");
    }

    [Fact]
    public async Task GivenSettingsServiceThrowsConversionException_WhenResolved_ThenReturnsTypeMismatchError()
    {
        var settingsService = Substitute.For<ISettingsService>();
        _ = settingsService.GetLocalSetting<int>("FeatureFlags:BadInt")
            .Throws(new InvalidOperationException("Cannot convert setting"));
        _ = settingsService.GetLocalSetting<bool>("FeatureFlags:UnsupportedBool")
            .Throws(new NotSupportedException("Type not supported"));
        _ = settingsService.GetLocalSetting<string>("FeatureFlags:BadStruct")
            .Throws(new InvalidOperationException("Cannot convert structure"));

        var provider = new SettingsFeatureProvider(
            settingsService,
            Options.Create(new SettingsFeatureProviderOptions
            {
                LookupScope = SettingsLookupScope.LocalOnly,
            }));

        var intResult = await provider.ResolveIntegerValueAsync(
            "BadInt",
            defaultValue: 12,
            cancellationToken: TestContext.Current.CancellationToken);
        var boolResult = await provider.ResolveBooleanValueAsync(
            "UnsupportedBool",
            defaultValue: true,
            cancellationToken: TestContext.Current.CancellationToken);
        var structResult = await provider.ResolveStructureValueAsync(
            "BadStruct",
            new Value("default"),
            cancellationToken: TestContext.Current.CancellationToken);

        intResult.Value.ShouldBe(12);
        intResult.ErrorType.ShouldBe(ErrorType.TypeMismatch);
        intResult.Reason.ShouldBe(Reason.Error);

        boolResult.Value.ShouldBeTrue();
        boolResult.ErrorType.ShouldBe(ErrorType.TypeMismatch);
        boolResult.Reason.ShouldBe(Reason.Error);

        structResult.Value.AsString.ShouldBe("default");
        structResult.ErrorType.ShouldBe(ErrorType.TypeMismatch);
        structResult.Reason.ShouldBe(Reason.Error);
    }
}
