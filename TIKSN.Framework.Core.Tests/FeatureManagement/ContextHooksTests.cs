using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using OpenFeature;
using OpenFeature.Constant;
using OpenFeature.Model;
using Shouldly;
using TIKSN.DependencyInjection;
using TIKSN.FeatureManagement;
using TIKSN.Identity;
using Xunit;
using static LanguageExt.Prelude;

namespace TIKSN.Tests.FeatureManagement;

public class ContextHooksTests
{
    [Fact]
    public async Task GivenGlobalizationContextHook_WhenBeforeAsyncCalled_ThenSetsCultureAndRegion()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");

            var hook = new GlobalizationContextHook();
            var result = await hook.BeforeAsync(
                CreateHookContext(),
                cancellationToken: TestContext.Current.CancellationToken);

            result.GetValue(FeatureEvaluationContextKeys.CultureName).AsString.ShouldBe("en-US");
            result.GetValue(FeatureEvaluationContextKeys.UiCultureName).AsString.ShouldBe("fr-FR");
            result.GetValue(FeatureEvaluationContextKeys.RegionTwoLetterIso).AsString.ShouldBe("US");
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }

    [Fact]
    public async Task GivenGlobalizationContextHook_WhenNeutralOrInvariantCulture_ThenOmitsRegionWithoutThrowing()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("en");
            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

            var hook = new GlobalizationContextHook();
            var result = await hook.BeforeAsync(
                CreateHookContext(),
                cancellationToken: TestContext.Current.CancellationToken);

            result.GetValue(FeatureEvaluationContextKeys.CultureName).AsString.ShouldBe("en");
            result.ContainsKey(FeatureEvaluationContextKeys.UiCultureName).ShouldBeFalse();
            result.ContainsKey(FeatureEvaluationContextKeys.RegionTwoLetterIso).ShouldBeFalse();
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }

    [Fact]
    public async Task GivenHostEnvironmentContextHook_WhenBeforeAsyncCalled_ThenSetsDeploymentEnvironment()
    {
        var hostEnvironment = Substitute.For<IHostEnvironment>();
        _ = hostEnvironment.EnvironmentName.Returns("Staging");

        var hook = new HostEnvironmentContextHook(hostEnvironment);
        var result = await hook.BeforeAsync(
            CreateHookContext(),
            cancellationToken: TestContext.Current.CancellationToken);

        result.GetValue(FeatureEvaluationContextKeys.DeploymentEnvironment).AsString.ShouldBe("Staging");
    }

    [Fact]
    public async Task GivenHostEnvironmentContextHook_WhenNullOrWhitespace_ThenDoesNotSetDeploymentEnvironment()
    {
        var nullHook = new HostEnvironmentContextHook();
        var nullResult = await nullHook.BeforeAsync(
            CreateHookContext(),
            cancellationToken: TestContext.Current.CancellationToken);
        nullResult.ContainsKey(FeatureEvaluationContextKeys.DeploymentEnvironment).ShouldBeFalse();

        var emptyEnv = Substitute.For<IHostEnvironment>();
        _ = emptyEnv.EnvironmentName.Returns("   ");
        var emptyHook = new HostEnvironmentContextHook(emptyEnv);
        var emptyResult = await emptyHook.BeforeAsync(
            CreateHookContext(),
            cancellationToken: TestContext.Current.CancellationToken);
        emptyResult.ContainsKey(FeatureEvaluationContextKeys.DeploymentEnvironment).ShouldBeFalse();
    }

    [Fact]
    public async Task GivenOpenFeatureBuilderHookExtensions_WhenEvaluated_ThenEnrichesEvaluationContext()
    {
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var tenantIdProvider = Substitute.For<ITenantIdProvider<Guid>>();
        _ = tenantIdProvider.FindTenantId().Returns(Some(tenantId));

        var userIdProvider = Substitute.For<IUserIdProvider<Guid>>();
        _ = userIdProvider.FindUserId().Returns(Some(userId));

        var hostEnvironment = Substitute.For<IHostEnvironment>();
        _ = hostEnvironment.EnvironmentName.Returns("Production");

        var serviceIdentity = new ServiceIdentity("OrderService", Seq1("Api"), ServiceInstanceId.Create(7L));
        var capturingProvider = new ContextCapturingProvider();

        var services = new ServiceCollection();
        _ = services.AddLogging();
        _ = services.AddSingleton(hostEnvironment);
        _ = services.AddSingleton(serviceIdentity);
        _ = services.AddSingleton(tenantIdProvider);
        _ = services.AddSingleton(userIdProvider);
        _ = services.AddOpenFeature(builder => builder
            .AddFrameworkContextHooks()
            .AddTenantContextHook<Guid>()
            .AddUserContextHook<Guid>()
            .AddProvider(_ => capturingProvider));

        await using var serviceProvider = services.BuildServiceProvider();
        foreach (var hostedService in serviceProvider.GetServices<IHostedService>())
        {
            await hostedService.StartAsync(TestContext.Current.CancellationToken);
        }

        var client = serviceProvider.GetRequiredService<IFeatureClient>();

        var value = await client.GetBooleanValueAsync(
            "check-context",
            defaultValue: false,
            cancellationToken: TestContext.Current.CancellationToken);

        value.ShouldBeTrue();
        _ = capturingProvider.LastContext.ShouldNotBeNull();
        capturingProvider.LastContext.GetValue(FeatureEvaluationContextKeys.DeploymentEnvironment).AsString
            .ShouldBe("Production");
        capturingProvider.LastContext.GetValue(FeatureEvaluationContextKeys.ServiceApplicationName).AsString
            .ShouldBe("OrderService");
        capturingProvider.LastContext.GetValue(FeatureEvaluationContextKeys.TenantId).AsString
            .ShouldBe(tenantId.ToString());
        capturingProvider.LastContext.GetValue(FeatureEvaluationContextKeys.UserId).AsString
            .ShouldBe(userId.ToString());
    }

    [Fact]
    public async Task GivenServiceIdentityContextHook_WhenBeforeAsyncCalled_ThenSetsServiceAttributes()
    {
        var instanceId = ServiceInstanceId.Create(42L);
        var serviceIdentity = new ServiceIdentity("MyApp", Seq("Billing", "Worker"), instanceId);

        var hook = new ServiceIdentityContextHook(serviceIdentity);
        var result = await hook.BeforeAsync(
            CreateHookContext(),
            cancellationToken: TestContext.Current.CancellationToken);

        result.GetValue(FeatureEvaluationContextKeys.ServiceApplicationName).AsString.ShouldBe("MyApp");
        result.GetValue(FeatureEvaluationContextKeys.ServiceComponentPath).AsString.ShouldBe("Billing.Worker");
        result.GetValue(FeatureEvaluationContextKeys.ServiceInstanceId).AsString.ShouldBe("42");
        result.GetValue(FeatureEvaluationContextKeys.ServicePath).AsString.ShouldBe("MyApp.Billing.Worker");
    }

    [Fact]
    public async Task GivenServiceIdentityContextHook_WhenNullOrMinimalIdentity_ThenSetsOnlyAvailableAttributes()
    {
        var nullHook = new ServiceIdentityContextHook();
        var nullResult = await nullHook.BeforeAsync(
            CreateHookContext(),
            cancellationToken: TestContext.Current.CancellationToken);
        nullResult.ContainsKey(FeatureEvaluationContextKeys.ServiceApplicationName).ShouldBeFalse();

        var minimalIdentity = new ServiceIdentity("MinimalApp", Seq<string>(), ServiceInstanceId.Create(1L));
        var minimalHook = new ServiceIdentityContextHook(minimalIdentity);
        var minimalResult = await minimalHook.BeforeAsync(
            CreateHookContext(),
            cancellationToken: TestContext.Current.CancellationToken);
        minimalResult.GetValue(FeatureEvaluationContextKeys.ServiceApplicationName).AsString.ShouldBe("MinimalApp");
        minimalResult.ContainsKey(FeatureEvaluationContextKeys.ServiceComponentPath).ShouldBeFalse();
        minimalResult.GetValue(FeatureEvaluationContextKeys.ServiceInstanceId).AsString.ShouldBe("1");
        minimalResult.GetValue(FeatureEvaluationContextKeys.ServicePath).AsString.ShouldBe("MinimalApp");
    }

    [Fact]
    public async Task GivenTenantAndUserHooks_WhenBeforeAsyncCalled_ThenSetsIdsAndTargetingKey()
    {
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var tenantIdProvider = Substitute.For<ITenantIdProvider<Guid>>();
        _ = tenantIdProvider.FindTenantId().Returns(Some(tenantId));

        var userIdProvider = Substitute.For<IUserIdProvider<Guid>>();
        _ = userIdProvider.FindUserId().Returns(Some(userId));

        var tenantHook = new TenantContextHook<Guid>(tenantIdProvider);
        var userHook = new UserContextHook<Guid>(userIdProvider);

        var afterTenant = await tenantHook.BeforeAsync(
            CreateHookContext(),
            cancellationToken: TestContext.Current.CancellationToken);
        afterTenant.GetValue(FeatureEvaluationContextKeys.TenantId).AsString.ShouldBe(tenantId.ToString());
        afterTenant.TargetingKey.ShouldBe(tenantId.ToString());

        var afterUser = await userHook.BeforeAsync(
            CreateHookContext(afterTenant),
            cancellationToken: TestContext.Current.CancellationToken);
        afterUser.GetValue(FeatureEvaluationContextKeys.UserId).AsString.ShouldBe(userId.ToString());
        afterUser.TargetingKey.ShouldBe(tenantId.ToString());

        var directUser = await userHook.BeforeAsync(
            CreateHookContext(),
            cancellationToken: TestContext.Current.CancellationToken);
        directUser.TargetingKey.ShouldBe(userId.ToString());
    }

    [Fact]
    public async Task GivenTenantAndUserHooks_WhenProvidersReturnNone_ThenLeavesContextUnchanged()
    {
        var tenantIdProvider = Substitute.For<ITenantIdProvider<Guid>>();
        _ = tenantIdProvider.FindTenantId().Returns(Option<Guid>.None);

        var userIdProvider = Substitute.For<IUserIdProvider<Guid>>();
        _ = userIdProvider.FindUserId().Returns(Option<Guid>.None);

        var tenantHook = new TenantContextHook<Guid>(tenantIdProvider);
        var userHook = new UserContextHook<Guid>(userIdProvider);

        var afterTenant = await tenantHook.BeforeAsync(
            CreateHookContext(),
            cancellationToken: TestContext.Current.CancellationToken);
        afterTenant.ContainsKey(FeatureEvaluationContextKeys.TenantId).ShouldBeFalse();

        var afterUser = await userHook.BeforeAsync(
            CreateHookContext(),
            cancellationToken: TestContext.Current.CancellationToken);
        afterUser.ContainsKey(FeatureEvaluationContextKeys.UserId).ShouldBeFalse();
    }

    private static HookContext<bool> CreateHookContext(EvaluationContext? context = null) =>
        new(
            "test-flag",
            defaultValue: false,
            FlagValueType.Boolean,
            new ClientMetadata("test-client", "1.0.0"),
            new Metadata("test-provider"),
            context ?? EvaluationContext.Empty);

    private sealed class ContextCapturingProvider : FeatureProvider
    {
        public EvaluationContext? LastContext { get; private set; }

        public override Metadata GetMetadata() => new("ContextCapturingProvider");

        public override Task<ResolutionDetails<bool>> ResolveBooleanValueAsync(
            string flagKey,
            bool defaultValue,
            EvaluationContext? context = null,
            CancellationToken cancellationToken = default)
        {
            this.LastContext = context;
            return Task.FromResult(new ResolutionDetails<bool>(flagKey, value: true, ErrorType.None, Reason.Static));
        }

        public override Task<ResolutionDetails<double>> ResolveDoubleValueAsync(
            string flagKey,
            double defaultValue,
            EvaluationContext? context = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new ResolutionDetails<double>(flagKey, defaultValue, ErrorType.None, Reason.Default));

        public override Task<ResolutionDetails<int>> ResolveIntegerValueAsync(
            string flagKey,
            int defaultValue,
            EvaluationContext? context = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new ResolutionDetails<int>(flagKey, defaultValue, ErrorType.None, Reason.Default));

        public override Task<ResolutionDetails<string>> ResolveStringValueAsync(
            string flagKey,
            string defaultValue,
            EvaluationContext? context = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new ResolutionDetails<string>(flagKey, defaultValue, ErrorType.None, Reason.Default));

        public override Task<ResolutionDetails<Value>> ResolveStructureValueAsync(
            string flagKey,
            Value defaultValue,
            EvaluationContext? context = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new ResolutionDetails<Value>(flagKey, defaultValue, ErrorType.None, Reason.Default));
    }
}
