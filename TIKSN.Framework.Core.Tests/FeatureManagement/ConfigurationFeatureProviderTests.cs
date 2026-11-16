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
                ["CustomSection:CustomFlag"] = "go",
            })
            .Build();

        var services = new ServiceCollection();
        _ = services.AddLogging();
        _ = services.AddSingleton<IConfiguration>(config);
        _ = services.AddOpenFeature(builder => builder
            .AddConfigurationProvider(options =>
            {
                options.SectionName = "CustomSection";
                options.TrueValues.Add("go");
            }));

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
        var customEnabled = await client.GetBooleanValueAsync(
            "CustomFlag",
            defaultValue: false,
            cancellationToken: TestContext.Current.CancellationToken);

        enabled.ShouldBeTrue();
        customEnabled.ShouldBeTrue();

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

    [Fact]
    public async Task GivenBooleanWords_WhenResolved_ThenReturnsConfiguredValuesAndVariants()
    {
        var values = new Dictionary<string, string?>
        {
            ["FeatureFlags:True0"] = "True",
            ["FeatureFlags:True1"] = "ON",
            ["FeatureFlags:True2"] = "Yes",
            ["FeatureFlags:True3"] = "enabled",
            ["FeatureFlags:True4"] = "Active",
            ["FeatureFlags:True5"] = "show",
            ["FeatureFlags:True6"] = "1",
            ["FeatureFlags:False0"] = "false",
            ["FeatureFlags:False1"] = "OFF",
            ["FeatureFlags:False2"] = "no",
            ["FeatureFlags:False3"] = "disabled",
            ["FeatureFlags:False4"] = "Inactive",
            ["FeatureFlags:False5"] = "hide",
            ["FeatureFlags:False6"] = "0",
            ["FeatureFlags:CustomTrue"] = "go",
            ["FeatureFlags:CustomFalse"] = "stop",
            ["FeatureFlags:Unknown"] = "perhaps",
        };
        var config = new ConfigurationBuilder().AddInMemoryCollection(values).Build();
        var options = new ConfigurationFeatureProviderOptions();
        options.TrueValues.Add("go");
        options.FalseValues.Add("stop");
        var provider = new ConfigurationFeatureProvider(
            config,
            Options.Create(options));

        for (var i = 0; i < 7; i++)
        {
            var trueResult = await provider.ResolveBooleanValueAsync(
                $"True{i}", false, cancellationToken: TestContext.Current.CancellationToken);
            trueResult.Value.ShouldBeTrue();
            trueResult.ErrorType.ShouldBe(ErrorType.None);
            trueResult.Reason.ShouldBe(Reason.Static);
            trueResult.Variant.ShouldBe(values[$"FeatureFlags:True{i}"]);

            var falseResult = await provider.ResolveBooleanValueAsync(
                $"False{i}", true, cancellationToken: TestContext.Current.CancellationToken);
            falseResult.Value.ShouldBeFalse();
            falseResult.ErrorType.ShouldBe(ErrorType.None);
            falseResult.Reason.ShouldBe(Reason.Static);
            falseResult.Variant.ShouldBe(values[$"FeatureFlags:False{i}"]);
        }

        var customTrueResult = await provider.ResolveBooleanValueAsync(
            "CustomTrue", false, cancellationToken: TestContext.Current.CancellationToken);
        var customFalseResult = await provider.ResolveBooleanValueAsync(
            "CustomFalse", true, cancellationToken: TestContext.Current.CancellationToken);
        var unknownResult = await provider.ResolveBooleanValueAsync(
            "Unknown", true, cancellationToken: TestContext.Current.CancellationToken);

        customTrueResult.Value.ShouldBeTrue();
        customTrueResult.Variant.ShouldBe("go");
        customFalseResult.Value.ShouldBeFalse();
        customFalseResult.Variant.ShouldBe("stop");
        unknownResult.Value.ShouldBeTrue();
        unknownResult.ErrorType.ShouldBe(ErrorType.TypeMismatch);
        unknownResult.Reason.ShouldBe(Reason.Error);
        unknownResult.ErrorMessage.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GivenVariantSchema_WhenResolvedAsTypedValues_ThenUsesDefaultVariant()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["CustomFlags:Bool:Variants:on"] = "active",
                ["CustomFlags:Bool:Variants:off"] = "inactive",
                ["CustomFlags:Bool:DefaultVariant"] = "on",
                ["CustomFlags:BoolOff:Variants:on"] = "active",
                ["CustomFlags:BoolOff:Variants:off"] = "inactive",
                ["CustomFlags:BoolOff:DefaultVariant"] = "off",
                ["CustomFlags:String:Variants:first"] = "hello",
                ["CustomFlags:String:Variants:second"] = "world",
                ["CustomFlags:String:DefaultVariant"] = "second",
                ["CustomFlags:Int:Variants:small"] = "12",
                ["CustomFlags:Int:Variants:large"] = "42",
                ["CustomFlags:Int:DefaultVariant"] = "large",
                ["CustomFlags:Double:Variants:low"] = "1.25",
                ["CustomFlags:Double:Variants:high"] = "2.5",
                ["CustomFlags:Double:DefaultVariant"] = "high",
                ["CustomFlags:Struct:Variants:compact:Count"] = "2",
                ["CustomFlags:Struct:Variants:compact:Name"] = "compact",
                ["CustomFlags:Struct:DefaultVariant"] = "compact",
            })
            .Build();
        var provider = new ConfigurationFeatureProvider(
            config,
            Options.Create(new ConfigurationFeatureProviderOptions
            {
                SectionName = "CustomFlags",
                EnableVariantSchema = true,
            }));

        var boolResult = await provider.ResolveBooleanValueAsync(
            "Bool", false, cancellationToken: TestContext.Current.CancellationToken);
        var boolOffResult = await provider.ResolveBooleanValueAsync(
            "BoolOff", true, cancellationToken: TestContext.Current.CancellationToken);
        var stringResult = await provider.ResolveStringValueAsync(
            "String", string.Empty, cancellationToken: TestContext.Current.CancellationToken);
        var intResult = await provider.ResolveIntegerValueAsync(
            "Int", 0, cancellationToken: TestContext.Current.CancellationToken);
        var doubleResult = await provider.ResolveDoubleValueAsync(
            "Double", 0, cancellationToken: TestContext.Current.CancellationToken);
        var structureResult = await provider.ResolveStructureValueAsync(
            "Struct", new Value(), cancellationToken: TestContext.Current.CancellationToken);

        boolResult.Value.ShouldBeTrue();
        boolResult.Variant.ShouldBe("on");
        boolOffResult.Value.ShouldBeFalse();
        boolOffResult.Variant.ShouldBe("off");
        stringResult.Value.ShouldBe("world");
        stringResult.Variant.ShouldBe("second");
        intResult.Value.ShouldBe(42);
        intResult.Variant.ShouldBe("large");
        doubleResult.Value.ShouldBe(2.5);
        doubleResult.Variant.ShouldBe("high");
        structureResult.Value.IsStructure.ShouldBeTrue();
        structureResult.Value.AsStructure!.GetValue("Count").AsInteger.ShouldBe(2);
        structureResult.Value.AsStructure.GetValue("Name").AsString.ShouldBe("compact");
        structureResult.Variant.ShouldBe("compact");
    }

    [Fact]
    public async Task GivenDisabledOrInvalidVariantSchema_WhenResolved_ThenReturnsExpectedErrors()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureFlags:Disabled:Variants:on"] = "true",
                ["FeatureFlags:Disabled:DefaultVariant"] = "on",
                ["FeatureFlags:Disabled:Disabled"] = "true",
                ["FeatureFlags:Missing:Variants:on"] = "true",
                ["FeatureFlags:Missing:DefaultVariant"] = "off",
                ["FeatureFlags:Mismatch:Variants:on"] = "not-a-bool",
                ["FeatureFlags:Mismatch:DefaultVariant"] = "on",
            })
            .Build();
        var provider = new ConfigurationFeatureProvider(
            config,
            Options.Create(new ConfigurationFeatureProviderOptions { EnableVariantSchema = true }));

        var disabled = await provider.ResolveBooleanValueAsync(
            "Disabled", false, cancellationToken: TestContext.Current.CancellationToken);
        var missing = await provider.ResolveBooleanValueAsync(
            "Missing", true, cancellationToken: TestContext.Current.CancellationToken);
        var mismatch = await provider.ResolveBooleanValueAsync(
            "Mismatch", false, cancellationToken: TestContext.Current.CancellationToken);

        disabled.Value.ShouldBeFalse();
        disabled.ErrorType.ShouldBe(ErrorType.None);
        disabled.Reason.ShouldBe(Reason.Disabled);
        disabled.Variant.ShouldBeNull();
        missing.ErrorType.ShouldBe(ErrorType.General);
        missing.ErrorMessage.ShouldNotBeNull();
        missing.ErrorMessage!.ShouldContain("Missing");
        missing.ErrorMessage.ShouldContain("off");
        mismatch.Value.ShouldBeFalse();
        mismatch.ErrorType.ShouldBe(ErrorType.TypeMismatch);
        mismatch.Reason.ShouldBe(Reason.Error);
    }

    [Fact]
    public async Task GivenVariantSchemaDisabled_WhenResolvedAsStructure_ThenKeepsTheWholeSection()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureFlags:Theme:Variants:light:Color"] = "white",
                ["FeatureFlags:Theme:DefaultVariant"] = "light",
            })
            .Build();
        var provider = new ConfigurationFeatureProvider(
            config,
            Options.Create(new ConfigurationFeatureProviderOptions()));

        var result = await provider.ResolveStructureValueAsync(
            "Theme", new Value(), cancellationToken: TestContext.Current.CancellationToken);

        result.ErrorType.ShouldBe(ErrorType.None);
        result.Value.AsStructure!.GetValue("Variants").AsStructure!.GetValue("light")
            .AsStructure!.GetValue("Color").AsString.ShouldBe("white");
        result.Value.AsStructure.GetValue("DefaultVariant").AsString.ShouldBe("light");
    }
}
