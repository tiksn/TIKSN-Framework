using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OpenFeature.Constant;
using OpenFeature.Model;
using Shouldly;
using TIKSN.FeatureManagement;
using Xunit;

namespace TIKSN.Tests.FeatureManagement;

public class ConfigurationFeatureProviderTests
{
    [Fact]
    public async Task GivenBooleanFlag_WhenResolved_ThenReturnsConfiguredValue()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureFlags:NewCheckout"] = "true",
            })
            .Build();

        var provider = new ConfigurationFeatureProvider(
            config,
            Options.Create(new ConfigurationFeatureProviderOptions()));

        var result = await provider.ResolveBooleanValueAsync(
            "NewCheckout",
            defaultValue: false,
            cancellationToken: TestContext.Current.CancellationToken);

        result.Value.ShouldBeTrue();
        result.ErrorType.ShouldBe(ErrorType.None);
        result.Reason.ShouldBe(Reason.Static);
    }

    [Fact]
    public async Task GivenInvalidBooleanFlag_WhenResolved_ThenReturnsTypeMismatchError()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureFlags:NewCheckout"] = "not-a-bool",
            })
            .Build();

        var provider = new ConfigurationFeatureProvider(
            config,
            Options.Create(new ConfigurationFeatureProviderOptions()));

        var result = await provider.ResolveBooleanValueAsync(
            "NewCheckout",
            defaultValue: true,
            cancellationToken: TestContext.Current.CancellationToken);

        result.Value.ShouldBeTrue();
        result.ErrorType.ShouldBe(ErrorType.TypeMismatch);
        result.Reason.ShouldBe(Reason.Error);
    }

    [Fact]
    public async Task GivenMissingFlag_WhenResolved_ThenReturnsDefaultAndFlagNotFound()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection([])
            .Build();

        var provider = new ConfigurationFeatureProvider(
            config,
            Options.Create(new ConfigurationFeatureProviderOptions()));

        var result = await provider.ResolveBooleanValueAsync(
            "MissingFlag",
            defaultValue: true,
            cancellationToken: TestContext.Current.CancellationToken);

        result.Value.ShouldBeTrue();
        result.ErrorType.ShouldBe(ErrorType.FlagNotFound);
        result.Reason.ShouldBe(Reason.Default);
    }

    [Fact]
    public async Task GivenNumericAndStringFlags_WhenResolved_ThenReturnsConfiguredValues()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["CustomFlags:MaxItems"] = "42",
                ["CustomFlags:Ratio"] = "3.14",
                ["CustomFlags:WelcomeText"] = "Hello OpenFeature",
            })
            .Build();

        var provider = new ConfigurationFeatureProvider(
            config,
            Options.Create(new ConfigurationFeatureProviderOptions { SectionName = "CustomFlags" }));

        var intResult = await provider.ResolveIntegerValueAsync(
            "MaxItems",
            defaultValue: 0,
            cancellationToken: TestContext.Current.CancellationToken);
        var doubleResult = await provider.ResolveDoubleValueAsync(
            "Ratio",
            defaultValue: 0.0,
            cancellationToken: TestContext.Current.CancellationToken);
        var stringResult = await provider.ResolveStringValueAsync(
            "WelcomeText",
            defaultValue: string.Empty,
            cancellationToken: TestContext.Current.CancellationToken);

        intResult.Value.ShouldBe(42);
        doubleResult.Value.ShouldBe(3.14);
        stringResult.Value.ShouldBe("Hello OpenFeature");
    }

    [Fact]
    public async Task GivenStructuredSubsection_WhenResolved_ThenReturnsStructureValue()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureFlags:Theme:PrimaryColor"] = "#FF0000",
                ["FeatureFlags:Theme:FontSize"] = "14",
                ["FeatureFlags:Theme:DarkMode"] = "true",
            })
            .Build();

        var provider = new ConfigurationFeatureProvider(
            config,
            Options.Create(new ConfigurationFeatureProviderOptions()));

        var result = await provider.ResolveStructureValueAsync(
            "Theme",
            new Value(),
            cancellationToken: TestContext.Current.CancellationToken);

        result.ErrorType.ShouldBe(ErrorType.None);
        result.Value.IsStructure.ShouldBeTrue();
        var structure = result.Value.AsStructure!;
        structure.GetValue("PrimaryColor").AsString.ShouldBe("#FF0000");
        structure.GetValue("FontSize").AsInteger.ShouldBe(14);
        structure.GetValue("DarkMode").AsBoolean.ShouldBe(true);
    }
}
