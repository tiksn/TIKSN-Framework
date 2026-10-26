using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenFeature;
using OpenFeature.Constant;
using OpenFeature.Model;
using Shouldly;
using TIKSN.DependencyInjection;
using TIKSN.FeatureManagement;
using Xunit;

namespace TIKSN.Tests.FeatureManagement;

public class ConfigurationFeatureProviderTests
{
    [Fact]
    public async Task GivenArraySubsection_WhenResolvedAsStructure_ThenReturnsListValue()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureFlags:AllowedRetries:0"] = "1",
                ["FeatureFlags:AllowedRetries:1"] = "2",
                ["FeatureFlags:AllowedRetries:2"] = "5",
            })
            .Build();

        var provider = new ConfigurationFeatureProvider(
            config,
            Options.Create(new ConfigurationFeatureProviderOptions()));

        var result = await provider.ResolveStructureValueAsync(
            "AllowedRetries",
            new Value(),
            cancellationToken: TestContext.Current.CancellationToken);

        result.ErrorType.ShouldBe(ErrorType.None);
        result.Value.IsList.ShouldBeTrue();
        var list = result.Value.AsList!;
        list.Count.ShouldBe(3);
        list[0].AsInteger.ShouldBe(1);
        list[1].AsInteger.ShouldBe(2);
        list[2].AsInteger.ShouldBe(5);
    }

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

        provider.GetMetadata().Name.ShouldBe("TIKSN.ConfigurationFeatureProvider");

        var result = await provider.ResolveBooleanValueAsync(
            "NewCheckout",
            defaultValue: false,
            cancellationToken: TestContext.Current.CancellationToken);

        result.Value.ShouldBeTrue();
        result.ErrorType.ShouldBe(ErrorType.None);
        result.Reason.ShouldBe(Reason.Static);
    }

    [Fact]
    public async Task GivenInvalidFlags_WhenResolved_ThenReturnsTypeMismatchError()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureFlags:BadBool"] = "not-a-bool",
                ["FeatureFlags:BadInt"] = "not-an-int",
                ["FeatureFlags:BadDouble"] = "not-a-double",
            })
            .Build();

        var provider = new ConfigurationFeatureProvider(
            config,
            Options.Create(new ConfigurationFeatureProviderOptions()));

        var boolResult = await provider.ResolveBooleanValueAsync(
            "BadBool",
            defaultValue: true,
            cancellationToken: TestContext.Current.CancellationToken);
        var intResult = await provider.ResolveIntegerValueAsync(
            "BadInt",
            defaultValue: 10,
            cancellationToken: TestContext.Current.CancellationToken);
        var doubleResult = await provider.ResolveDoubleValueAsync(
            "BadDouble",
            defaultValue: 2.5,
            cancellationToken: TestContext.Current.CancellationToken);

        boolResult.Value.ShouldBeTrue();
        boolResult.ErrorType.ShouldBe(ErrorType.TypeMismatch);
        boolResult.Reason.ShouldBe(Reason.Error);

        intResult.Value.ShouldBe(10);
        intResult.ErrorType.ShouldBe(ErrorType.TypeMismatch);
        intResult.Reason.ShouldBe(Reason.Error);

        doubleResult.Value.ShouldBe(2.5);
        doubleResult.ErrorType.ShouldBe(ErrorType.TypeMismatch);
        doubleResult.Reason.ShouldBe(Reason.Error);
    }

    [Fact]
    public async Task GivenMissingFlags_WhenResolved_ThenReturnsDefaultAndFlagNotFound()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection([])
            .Build();

        var provider = new ConfigurationFeatureProvider(
            config,
            Options.Create(new ConfigurationFeatureProviderOptions { SectionName = string.Empty }));

        var boolResult = await provider.ResolveBooleanValueAsync(
            "MissingBool",
            defaultValue: true,
            cancellationToken: TestContext.Current.CancellationToken);
        var intResult = await provider.ResolveIntegerValueAsync(
            "MissingInt",
            defaultValue: 7,
            cancellationToken: TestContext.Current.CancellationToken);
        var doubleResult = await provider.ResolveDoubleValueAsync(
            "MissingDouble",
            defaultValue: 1.5,
            cancellationToken: TestContext.Current.CancellationToken);
        var stringResult = await provider.ResolveStringValueAsync(
            "MissingString",
            defaultValue: "fallback",
            cancellationToken: TestContext.Current.CancellationToken);
        var structResult = await provider.ResolveStructureValueAsync(
            "MissingStructure",
            new Value("default-struct"),
            cancellationToken: TestContext.Current.CancellationToken);

        boolResult.Value.ShouldBeTrue();
        boolResult.ErrorType.ShouldBe(ErrorType.FlagNotFound);
        boolResult.Reason.ShouldBe(Reason.Default);

        intResult.Value.ShouldBe(7);
        intResult.ErrorType.ShouldBe(ErrorType.FlagNotFound);

        doubleResult.Value.ShouldBe(1.5);
        doubleResult.ErrorType.ShouldBe(ErrorType.FlagNotFound);

        stringResult.Value.ShouldBe("fallback");
        stringResult.ErrorType.ShouldBe(ErrorType.FlagNotFound);

        structResult.Value.AsString.ShouldBe("default-struct");
        structResult.ErrorType.ShouldBe(ErrorType.FlagNotFound);
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
    public async Task GivenOpenFeatureBuilderAddConfigurationProvider_WhenEvaluated_ThenResolvesFlags()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["CustomSection:EnabledFlag"] = "true",
            })
            .Build();

        var services = new ServiceCollection();
        _ = services.AddLogging();
        _ = services.AddSingleton<IConfiguration>(config);
        _ = services.AddOpenFeature(builder => builder
            .AddConfigurationProvider(options => options.SectionName = "CustomSection"));

        await using var serviceProvider = services.BuildServiceProvider();
        foreach (var hostedService in serviceProvider.GetServices<IHostedService>())
        {
            await hostedService.StartAsync(TestContext.Current.CancellationToken);
        }

        var client = serviceProvider.GetRequiredService<IFeatureClient>();
        var enabled = await client.GetBooleanValueAsync(
            "EnabledFlag",
            defaultValue: false,
            cancellationToken: TestContext.Current.CancellationToken);

        enabled.ShouldBeTrue();

        var domainServices = new ServiceCollection();
        _ = domainServices.AddLogging();
        _ = domainServices.AddSingleton<IConfiguration>(config);
        _ = domainServices.AddOpenFeature(builder => builder
            .AddPolicyName(policy => policy.DefaultNameSelector = _ => "domain-b")
            .AddConfigurationProvider("domain-a")
            .AddConfigurationProvider("domain-b", options => options.SectionName = "CustomSection"));

        await using var domainServiceProvider = domainServices.BuildServiceProvider();
        foreach (var hostedService in domainServiceProvider.GetServices<IHostedService>())
        {
            await hostedService.StartAsync(TestContext.Current.CancellationToken);
        }

        var domainClient = domainServiceProvider.GetRequiredService<IFeatureClient>();
        var domainEnabled = await domainClient.GetBooleanValueAsync(
            "EnabledFlag",
            defaultValue: false,
            cancellationToken: TestContext.Current.CancellationToken);

        domainEnabled.ShouldBeTrue();
    }

    [Fact]
    public async Task GivenStructuredSubsection_WhenResolved_ThenReturnsStructureValue()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureFlags:Theme:PrimaryColor"] = "#FF0000",
                ["FeatureFlags:Theme:FontSize"] = "14",
                ["FeatureFlags:Theme:Scale"] = "1.25",
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
        structure.GetValue("Scale").AsDouble.ShouldBe(1.25);
        structure.GetValue("DarkMode").AsBoolean.ShouldBe(true);
    }
}
