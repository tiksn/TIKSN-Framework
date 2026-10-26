using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OpenFeature;
using OpenFeature.Model;
using Shouldly;
using TIKSN.DependencyInjection;
using TIKSN.FeatureManagement;
using TIKSN.Integration.Messages;
using TIKSN.Integration.Messages.Commands;
using Xunit;
using static LanguageExt.Prelude;
using Unit = MediatR.Unit;

namespace TIKSN.Tests.Integration.Messages;

public class FeatureFlagAndContextBehaviorTests
{
    [Fact]
    public async Task GivenAllDisabledFeatures_WhenRequireAllFeaturesIsFalse_ThenThrowsFeatureDisabledException()
    {
        var featureClient = Substitute.For<IFeatureClient>();
        _ = featureClient.GetBooleanValueAsync("feature-a", false, null, null, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));
        _ = featureClient.GetBooleanValueAsync("feature-b", false, null, null, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        var behavior = new FeatureFlagBehavior<AnyGatedCommand, Unit>(featureClient);
        var request = new AnyGatedCommand(Seq("feature-a", "feature-b"));

        var exception = await Should.ThrowAsync<FeatureDisabledException>(() =>
            behavior.Handle(request, _ => Task.FromResult(Unit.Value), TestContext.Current.CancellationToken));

        exception.DisabledFeatures.Equals(Seq("feature-a", "feature-b")).ShouldBeTrue();
    }

    [Fact]
    public async Task GivenAllEnabledFeaturesOrEmpty_WhenRequireAllFeaturesIsTrue_ThenProceeds()
    {
        var featureClient = Substitute.For<IFeatureClient>();
        _ = featureClient.GetBooleanValueAsync("feature-a", false, null, null, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        var behavior = new FeatureFlagBehavior<AllGatedCommand, Unit>(featureClient);

        var enabledResult = await behavior.Handle(
            new AllGatedCommand(Seq1("feature-a")),
            _ => Task.FromResult(Unit.Value),
            TestContext.Current.CancellationToken);
        enabledResult.ShouldBe(Unit.Value);

        var emptyResult = await behavior.Handle(
            new AllGatedCommand(Seq<string>()),
            _ => Task.FromResult(Unit.Value),
            TestContext.Current.CancellationToken);
        emptyResult.ShouldBe(Unit.Value);
    }

    [Fact]
    public async Task GivenDisabledFeature_WhenRequireAllFeaturesIsTrue_ThenThrowsFeatureDisabledException()
    {
        var featureClient = Substitute.For<IFeatureClient>();
        _ = featureClient.GetBooleanValueAsync("feature-a", false, null, null, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));
        _ = featureClient.GetBooleanValueAsync("feature-b", false, null, null, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        var behavior = new FeatureFlagBehavior<AllGatedCommand, Unit>(featureClient);
        var request = new AllGatedCommand(Seq("feature-a", "feature-b"));

        var exception = await Should.ThrowAsync<FeatureDisabledException>(() =>
            behavior.Handle(request, _ => Task.FromResult(Unit.Value), TestContext.Current.CancellationToken));

        exception.DisabledFeatures.Equals(Seq1("feature-b")).ShouldBeTrue();
    }

    [Fact]
    public void GivenFeatureDisabledExceptionConstructors_WhenInstantiated_ThenPreservesProperties()
    {
        var defaultEx = new FeatureDisabledException();
        defaultEx.DisabledFeatures.IsEmpty.ShouldBeTrue();

        var msgEx = new FeatureDisabledException("Custom message");
        msgEx.Message.ShouldBe("Custom message");
        msgEx.DisabledFeatures.IsEmpty.ShouldBeTrue();

        var inner = new InvalidOperationException("Inner");
        var msgInnerEx = new FeatureDisabledException("Custom message", inner);
        msgInnerEx.InnerException.ShouldBe(inner);
        msgInnerEx.DisabledFeatures.IsEmpty.ShouldBeTrue();

        var fullEx = new FeatureDisabledException("Full message", Seq1("flag-1"), inner);
        fullEx.Message.ShouldBe("Full message");
        fullEx.InnerException.ShouldBe(inner);
        fullEx.DisabledFeatures.Equals(Seq1("flag-1")).ShouldBeTrue();
    }

    [Fact]
    public void GivenMediatRServiceConfigurationExtensions_WhenCalled_ThenRegistersBehaviors()
    {
        var config = new MediatRServiceConfiguration();

        _ = config
            .AddFeatureFlagPipelineBehavior()
            .AddFeatureTransactionContextPipelineBehavior()
            .AddTenantFeatureTransactionContextPipelineBehavior()
            .AddUserFeatureTransactionContextPipelineBehavior();

        config.BehaviorsToRegister.Count.ShouldBe(4);
    }

    [Fact]
    public async Task GivenOneEnabledFeature_WhenRequireAllFeaturesIsFalse_ThenProceeds()
    {
        var featureClient = Substitute.For<IFeatureClient>();
        _ = featureClient.GetBooleanValueAsync("feature-a", false, null, null, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));
        _ = featureClient.GetBooleanValueAsync("feature-b", false, null, null, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        var behavior = new FeatureFlagBehavior<AnyGatedCommand, Unit>(featureClient);
        var request = new AnyGatedCommand(Seq("feature-a", "feature-b"));

        var result = await behavior.Handle(request, _ => Task.FromResult(Unit.Value),
            TestContext.Current.CancellationToken);

        result.ShouldBe(Unit.Value);
    }

    [Fact]
    public async Task
        GivenTenantOnlyAndUserOnlyCommands_WhenTransactionContextBehaviorsRun_ThenPropagatesAndRestoresContext()
    {
        var propagator = new AsyncLocalTransactionContextPropagator();
        propagator.SetTransactionContext(EvaluationContext.Empty);

        var tenantBehavior = new TenantFeatureTransactionContextBehavior<Guid, SampleTenantCommand, Unit>(propagator);
        var tenantId = Guid.NewGuid();
        EvaluationContext? capturedTenantContext = null;

        _ = await tenantBehavior.Handle(
            new SampleTenantCommand(tenantId),
            _ =>
            {
                capturedTenantContext = propagator.GetTransactionContext();
                return Task.FromResult(Unit.Value);
            },
            TestContext.Current.CancellationToken);

        _ = capturedTenantContext.ShouldNotBeNull();
        capturedTenantContext.GetValue(FeatureEvaluationContextKeys.TenantId).AsString.ShouldBe(tenantId.ToString());
        capturedTenantContext.TargetingKey.ShouldBe(tenantId.ToString());
        propagator.GetTransactionContext().ContainsKey(FeatureEvaluationContextKeys.TenantId).ShouldBeFalse();

        var userBehavior = new UserFeatureTransactionContextBehavior<Guid, SampleUserCommand, Unit>(propagator);
        var userId = Guid.NewGuid();
        EvaluationContext? capturedUserContext = null;

        _ = await userBehavior.Handle(
            new SampleUserCommand(userId),
            _ =>
            {
                capturedUserContext = propagator.GetTransactionContext();
                return Task.FromResult(Unit.Value);
            },
            TestContext.Current.CancellationToken);

        _ = capturedUserContext.ShouldNotBeNull();
        capturedUserContext.GetValue(FeatureEvaluationContextKeys.UserId).AsString.ShouldBe(userId.ToString());
        capturedUserContext.TargetingKey.ShouldBe(userId.ToString());
        propagator.GetTransactionContext().ContainsKey(FeatureEvaluationContextKeys.UserId).ShouldBeFalse();
    }

    [Fact]
    public async Task GivenTenantUserCommand_WhenTransactionContextBehaviorRuns_ThenPropagatesAndRestoresContext()
    {
        var propagator = new AsyncLocalTransactionContextPropagator();
        var initialContext = EvaluationContext.Builder().Set("initial", "value").Build();
        propagator.SetTransactionContext(initialContext);

        var behavior = new FeatureTransactionContextBehavior<Guid, Guid, SampleTenantUserCommand, Unit>(propagator);
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new SampleTenantUserCommand(tenantId, userId);

        EvaluationContext? capturedContext = null;

        _ = await behavior.Handle(
            command,
            _ =>
            {
                capturedContext = propagator.GetTransactionContext();
                return Task.FromResult(Unit.Value);
            },
            TestContext.Current.CancellationToken);

        _ = capturedContext.ShouldNotBeNull();
        capturedContext.GetValue(FeatureEvaluationContextKeys.TenantId).AsString.ShouldBe(tenantId.ToString());
        capturedContext.GetValue(FeatureEvaluationContextKeys.UserId).AsString.ShouldBe(userId.ToString());
        capturedContext.TargetingKey.ShouldBe(userId.ToString());
        capturedContext.GetValue("initial").AsString.ShouldBe("value");

        var restoredContext = propagator.GetTransactionContext();
        restoredContext.ContainsKey(FeatureEvaluationContextKeys.TenantId).ShouldBeFalse();
        restoredContext.ContainsKey(FeatureEvaluationContextKeys.UserId).ShouldBeFalse();
        restoredContext.GetValue("initial").AsString.ShouldBe("value");
    }

    public sealed record AllGatedCommand(Seq<string> Features) : ICommand, IFeatureGatedRequest;

    public sealed record AnyGatedCommand(Seq<string> Features) : ICommand, IFeatureGatedRequest
    {
        public bool RequireAllFeatures => false;
    }

    public sealed record SampleTenantCommand(Guid TenantID) : ITenantCommand<Guid>;

    public sealed record SampleTenantUserCommand(Guid TenantID, Guid UserID) : ITenantUserCommand<Guid, Guid>;

    public sealed record SampleUserCommand(Guid UserID) : IUserCommand<Guid>;
}
