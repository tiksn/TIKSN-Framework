using System;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using OpenFeature;
using OpenFeature.Constant;
using OpenFeature.Model;
using Shouldly;
using TIKSN.DependencyInjection;
using TIKSN.FeatureManagement;
using TIKSN.Identity;
using TIKSN.Licensing;
using Xunit;
using static LanguageExt.Prelude;

namespace TIKSN.Tests.FeatureManagement;

public class LicenseEntitlementsFeatureProviderTests
{
    [Fact]
    public async Task GivenExpiredOrFutureSystemLicense_WhenResolved_ThenReturnsDefaultAndDisabledReason()
    {
        var fakeTime = new FakeTimeProvider(new DateTimeOffset(2026, 6, 1, 0, 0, 0, TimeSpan.Zero));
        var expiredLicense = CreateTestLicense(
            new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2025, 12, 31, 0, 0, 0, TimeSpan.Zero),
            new TestEntitlements(ProMode: true, MaxUsers: 50));

        var licenseProvider = Substitute.For<ISystemLicenseProvider<TestEntitlements>>();
        _ = licenseProvider.GetLicenseAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Some(expiredLicense)));

        var resolver = new TestLicenseFeatureResolver();
        var provider = new SystemLicenseEntitlementsFeatureProvider<TestEntitlements>(
            licenseProvider,
            resolver,
            fakeTime);

        provider.GetMetadata().Name.ShouldBe("TIKSN.SystemLicenseEntitlementsFeatureProvider");

        var expiredResult = await provider.ResolveBooleanValueAsync(
            "pro-mode",
            defaultValue: false,
            cancellationToken: TestContext.Current.CancellationToken);

        expiredResult.Value.ShouldBeFalse();
        expiredResult.ErrorType.ShouldBe(ErrorType.None);
        expiredResult.Reason.ShouldBe(Reason.Disabled);

        var futureLicense = CreateTestLicense(
            new DateTimeOffset(2027, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2028, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new TestEntitlements(ProMode: true, MaxUsers: 50));
        _ = licenseProvider.GetLicenseAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Some(futureLicense)));

        var futureResult = await provider.ResolveBooleanValueAsync(
            "pro-mode",
            defaultValue: false,
            cancellationToken: TestContext.Current.CancellationToken);

        futureResult.Value.ShouldBeFalse();
        futureResult.ErrorType.ShouldBe(ErrorType.None);
        futureResult.Reason.ShouldBe(Reason.Disabled);
    }

    [Fact]
    public async Task GivenMissingSystemLicenseOrUnmappedFlags_WhenResolved_ThenReturnsDefaultAndFlagNotFound()
    {
        var fakeTime = new FakeTimeProvider(new DateTimeOffset(2026, 6, 1, 0, 0, 0, TimeSpan.Zero));
        var licenseProvider = Substitute.For<ISystemLicenseProvider<TestEntitlements>>();
        _ = licenseProvider.GetLicenseAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Option<License<TestEntitlements>>.None));

        var resolver = new TestLicenseFeatureResolver();
        var provider = new SystemLicenseEntitlementsFeatureProvider<TestEntitlements>(
            licenseProvider,
            resolver,
            fakeTime);

        var noLicenseResult = await provider.ResolveBooleanValueAsync(
            "pro-mode",
            defaultValue: false,
            cancellationToken: TestContext.Current.CancellationToken);

        noLicenseResult.Value.ShouldBeFalse();
        noLicenseResult.ErrorType.ShouldBe(ErrorType.FlagNotFound);
        noLicenseResult.Reason.ShouldBe(Reason.Default);

        var validLicense = CreateTestLicense(
            new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2027, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new TestEntitlements(ProMode: true, MaxUsers: 50));
        _ = licenseProvider.GetLicenseAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Some(validLicense)));

        var doubleResult = await provider.ResolveDoubleValueAsync(
            "unmapped-double",
            defaultValue: 1.5,
            cancellationToken: TestContext.Current.CancellationToken);
        var stringResult = await provider.ResolveStringValueAsync(
            "unmapped-string",
            defaultValue: "basic",
            cancellationToken: TestContext.Current.CancellationToken);
        var structResult = await provider.ResolveStructureValueAsync(
            "unmapped-struct",
            new Value("fallback"),
            cancellationToken: TestContext.Current.CancellationToken);

        doubleResult.Value.ShouldBe(1.5);
        doubleResult.ErrorType.ShouldBe(ErrorType.FlagNotFound);

        stringResult.Value.ShouldBe("basic");
        stringResult.ErrorType.ShouldBe(ErrorType.FlagNotFound);

        structResult.Value.AsString.ShouldBe("fallback");
        structResult.ErrorType.ShouldBe(ErrorType.FlagNotFound);
    }

    [Fact]
    public async Task GivenOpenFeatureBuilderAddLicenseProviders_WhenEvaluated_ThenResolvesSystemTenantAndUserFlags()
    {
        var fakeTime = new FakeTimeProvider(new DateTimeOffset(2026, 6, 1, 0, 0, 0, TimeSpan.Zero));
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var systemLicense = CreateTestLicense(
            new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2027, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new TestEntitlements(ProMode: true, MaxUsers: 1000));
        var tenantLicense = CreateTestLicense(
            new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2027, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new TestEntitlements(ProMode: true, MaxUsers: 100));
        var userLicense = CreateTestLicense(
            new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2027, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new TestEntitlements(ProMode: true, MaxUsers: 1));

        var systemLicenseProvider = Substitute.For<ISystemLicenseProvider<TestEntitlements>>();
        _ = systemLicenseProvider.GetLicenseAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Some(systemLicense)));

        var tenantLicenseProvider = Substitute.For<ITenantLicenseProvider<Guid, TestEntitlements>>();
        _ = tenantLicenseProvider.GetLicenseAsync(tenantId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Some(tenantLicense)));

        var userLicenseProvider = Substitute.For<IUserLicenseProvider<Guid, TestEntitlements>>();
        _ = userLicenseProvider.GetLicenseAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Some(userLicense)));

        var tenantIdProvider = Substitute.For<ITenantIdProvider<Guid>>();
        _ = tenantIdProvider.FindTenantId().Returns(Some(tenantId));

        var userIdProvider = Substitute.For<IUserIdProvider<Guid>>();
        _ = userIdProvider.FindUserId().Returns(Some(userId));

        var services = new ServiceCollection();
        _ = services.AddLogging();
        _ = services.AddSingleton<TimeProvider>(fakeTime);
        _ = services.AddSingleton(systemLicenseProvider);
        _ = services.AddSingleton(tenantLicenseProvider);
        _ = services.AddSingleton(userLicenseProvider);
        _ = services.AddSingleton(tenantIdProvider);
        _ = services.AddSingleton(userIdProvider);
        _ = services.AddSingleton<ILicenseFeatureResolver<TestEntitlements>, TestLicenseFeatureResolver>();
        _ = services.AddOpenFeature(builder => builder
            .AddPolicyName(policy => policy.DefaultNameSelector = _ => "system")
            .AddSystemLicenseEntitlementsProvider<TestEntitlements>("system")
            .AddTenantLicenseEntitlementsProvider<Guid, TestEntitlements>("tenant")
            .AddUserLicenseEntitlementsProvider<Guid, TestEntitlements>("user"));

        await using var serviceProvider = services.BuildServiceProvider();
        foreach (var hostedService in serviceProvider.GetServices<IHostedService>())
        {
            await hostedService.StartAsync(TestContext.Current.CancellationToken);
        }

        var systemClient = serviceProvider.GetRequiredService<IFeatureClient>();
        var systemMaxUsers = await systemClient.GetIntegerValueAsync(
            "max-users",
            defaultValue: 0,
            cancellationToken: TestContext.Current.CancellationToken);
        systemMaxUsers.ShouldBe(1000);

        var tenantClient = serviceProvider.GetRequiredKeyedService<IFeatureClient>("tenant");
        var tenantMaxUsers = await tenantClient.GetIntegerValueAsync(
            "max-users",
            defaultValue: 0,
            cancellationToken: TestContext.Current.CancellationToken);
        tenantMaxUsers.ShouldBe(100);

        var userClient = serviceProvider.GetRequiredKeyedService<IFeatureClient>("user");
        var userMaxUsers = await userClient.GetIntegerValueAsync(
            "max-users",
            defaultValue: 0,
            cancellationToken: TestContext.Current.CancellationToken);
        userMaxUsers.ShouldBe(1);
    }

    [Fact]
    public async Task
        GivenTenantLicenseProvider_WhenResolvedViaProviderOrContextFallback_ThenReturnsTenantEntitlements()
    {
        var fakeTime = new FakeTimeProvider(new DateTimeOffset(2026, 6, 1, 0, 0, 0, TimeSpan.Zero));
        var primaryTenantId = Guid.NewGuid();
        var fallbackTenantId = Guid.NewGuid();
        var ulidTenantId = Ulid.NewUlid();

        var primaryLicense = CreateTestLicense(
            new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2027, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new TestEntitlements(ProMode: true, MaxUsers: 25));
        var fallbackLicense = CreateTestLicense(
            new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2027, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new TestEntitlements(ProMode: true, MaxUsers: 75));

        var tenantLicenseProvider = Substitute.For<ITenantLicenseProvider<Guid, TestEntitlements>>();
        _ = tenantLicenseProvider.GetLicenseAsync(primaryTenantId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Some(primaryLicense)));
        _ = tenantLicenseProvider.GetLicenseAsync(fallbackTenantId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Some(fallbackLicense)));

        var tenantIdProvider = Substitute.For<ITenantIdProvider<Guid>>();
        _ = tenantIdProvider.FindTenantId().Returns(Some(primaryTenantId));

        var resolver = new TestLicenseFeatureResolver();
        var provider = new TenantLicenseEntitlementsFeatureProvider<Guid, TestEntitlements>(
            tenantLicenseProvider,
            tenantIdProvider,
            resolver,
            fakeTime);

        provider.GetMetadata().Name.ShouldBe("TIKSN.TenantLicenseEntitlementsFeatureProvider");

        var primaryResult = await provider.ResolveIntegerValueAsync(
            "max-users",
            defaultValue: 0,
            cancellationToken: TestContext.Current.CancellationToken);
        primaryResult.Value.ShouldBe(25);

        _ = tenantIdProvider.FindTenantId().Returns(Option<Guid>.None);

        var contextWithTenant = EvaluationContext.Builder()
            .Set(FeatureEvaluationContextKeys.TenantId, fallbackTenantId.ToString())
            .Build();
        var fallbackResult = await provider.ResolveIntegerValueAsync(
            "max-users",
            defaultValue: 0,
            contextWithTenant,
            TestContext.Current.CancellationToken);
        fallbackResult.Value.ShouldBe(75);

        var invalidContext = EvaluationContext.Builder()
            .Set(FeatureEvaluationContextKeys.TenantId, "not-a-guid")
            .Build();
        var invalidResult = await provider.ResolveIntegerValueAsync(
            "max-users",
            defaultValue: 5,
            invalidContext,
            TestContext.Current.CancellationToken);
        invalidResult.Value.ShouldBe(5);
        invalidResult.ErrorType.ShouldBe(ErrorType.FlagNotFound);

        var ulidLicenseProvider = Substitute.For<ITenantLicenseProvider<Ulid, TestEntitlements>>();
        _ = ulidLicenseProvider.GetLicenseAsync(ulidTenantId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Some(primaryLicense)));
        var ulidIdProvider = Substitute.For<ITenantIdProvider<Ulid>>();
        _ = ulidIdProvider.FindTenantId().Returns(Option<Ulid>.None);
        var ulidProvider = new TenantLicenseEntitlementsFeatureProvider<Ulid, TestEntitlements>(
            ulidLicenseProvider,
            ulidIdProvider,
            resolver,
            fakeTime);
        var ulidContext = EvaluationContext.Builder()
            .Set(FeatureEvaluationContextKeys.TenantId, ulidTenantId.ToString())
            .Build();
        var ulidResult = await ulidProvider.ResolveBooleanValueAsync(
            "pro-mode",
            defaultValue: false,
            ulidContext,
            TestContext.Current.CancellationToken);
        ulidResult.Value.ShouldBeTrue();
    }

    [Fact]
    public async Task GivenUserLicenseProvider_WhenResolvedViaProviderOrContextFallback_ThenReturnsUserEntitlements()
    {
        var fakeTime = new FakeTimeProvider(new DateTimeOffset(2026, 6, 1, 0, 0, 0, TimeSpan.Zero));
        var primaryUserId = "user-primary";
        var fallbackUserId = "user-fallback";

        var primaryLicense = CreateTestLicense(
            new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2027, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new TestEntitlements(ProMode: true, MaxUsers: 1));
        var fallbackLicense = CreateTestLicense(
            new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2027, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new TestEntitlements(ProMode: false, MaxUsers: 3));

        var userLicenseProvider = Substitute.For<IUserLicenseProvider<string, TestEntitlements>>();
        _ = userLicenseProvider.GetLicenseAsync(primaryUserId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Some(primaryLicense)));
        _ = userLicenseProvider.GetLicenseAsync(fallbackUserId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Some(fallbackLicense)));

        var userIdProvider = Substitute.For<IUserIdProvider<string>>();
        _ = userIdProvider.FindUserId().Returns(Some(primaryUserId));

        var resolver = new TestLicenseFeatureResolver();
        var provider = new UserLicenseEntitlementsFeatureProvider<string, TestEntitlements>(
            userLicenseProvider,
            userIdProvider,
            resolver,
            fakeTime);

        provider.GetMetadata().Name.ShouldBe("TIKSN.UserLicenseEntitlementsFeatureProvider");

        var primaryResult = await provider.ResolveBooleanValueAsync(
            "pro-mode",
            defaultValue: false,
            cancellationToken: TestContext.Current.CancellationToken);
        primaryResult.Value.ShouldBeTrue();

        _ = userIdProvider.FindUserId().Returns(Option<string>.None);

        var contextWithUser = EvaluationContext.Builder()
            .Set(FeatureEvaluationContextKeys.UserId, fallbackUserId)
            .Build();
        var fallbackResult = await provider.ResolveIntegerValueAsync(
            "max-users",
            defaultValue: 0,
            contextWithUser,
            TestContext.Current.CancellationToken);
        fallbackResult.Value.ShouldBe(3);

        var emptyResult = await provider.ResolveIntegerValueAsync(
            "max-users",
            defaultValue: 9,
            EvaluationContext.Empty,
            TestContext.Current.CancellationToken);
        emptyResult.Value.ShouldBe(9);
        emptyResult.ErrorType.ShouldBe(ErrorType.FlagNotFound);
    }

    private static License<TestEntitlements> CreateTestLicense(
        DateTimeOffset notBefore,
        DateTimeOffset notAfter,
        TestEntitlements entitlements)
    {
        var party = new IndividualParty(
            "Test",
            "User",
            "Test User",
            new MailAddress("test@example.com"),
            new Uri("https://example.com"));
        var terms = new LicenseTerms(Ulid.NewUlid(), party, party, notBefore, notAfter);
        return new License<TestEntitlements>(terms, entitlements, Seq<byte>());
    }

    public sealed record TestEntitlements(bool ProMode, int MaxUsers);

    private sealed class TestLicenseFeatureResolver : ILicenseFeatureResolver<TestEntitlements>
    {
        public Option<bool> ResolveBoolean(TestEntitlements entitlements, string flagKey) =>
            flagKey == "pro-mode" ? Some(entitlements.ProMode) : None;

        public Option<int> ResolveInteger(TestEntitlements entitlements, string flagKey) =>
            flagKey == "max-users" ? Some(entitlements.MaxUsers) : None;
    }
}
