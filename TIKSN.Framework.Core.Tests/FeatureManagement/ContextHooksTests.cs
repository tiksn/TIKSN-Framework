using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using OpenFeature.Constant;
using OpenFeature.Model;
using Shouldly;
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
    }

    private static HookContext<bool> CreateHookContext(EvaluationContext? context = null) =>
        new(
            "test-flag",
            defaultValue: false,
            FlagValueType.Boolean,
            new ClientMetadata("test-client", "1.0.0"),
            new Metadata("test-provider"),
            context ?? EvaluationContext.Empty);
}
