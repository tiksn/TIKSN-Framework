using MediatR;
using OpenFeature;
using TIKSN.FeatureManagement;

namespace TIKSN.Integration.Messages;

public class FeatureFlagBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IFeatureGatedRequest
    where TResponse : notnull
{
    private readonly IFeatureClient featureClient;

    public FeatureFlagBehavior(IFeatureClient featureClient) =>
        this.featureClient = featureClient ?? throw new ArgumentNullException(nameof(featureClient));

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(next);

        var featureValues = new Dictionary<string, bool>(StringComparer.Ordinal);
        var disabledFeatures = new List<string>();
        var isSatisfied = await this.IsSatisfiedAsync(
                request.FeatureRequirement,
                featureValues,
                disabledFeatures,
                cancellationToken)
            .ConfigureAwait(false);

        if (!isSatisfied)
        {
            var disabledFeatureSequence = disabledFeatures.ToSeq();
            var disabledFeatureNames = string.Join(", ", disabledFeatureSequence);
            throw new FeatureDisabledException(
                $"Feature requirements are not satisfied. Disabled feature flags: {disabledFeatureNames}.",
                disabledFeatureSequence);
        }

        return await next(cancellationToken).ConfigureAwait(false);
    }

    private async Task<bool> AreAllFeaturesEnabledAsync(
        AllFeatureFlagsRequirement allOf,
        Dictionary<string, bool> featureValues,
        List<string> disabledFeatures,
        CancellationToken cancellationToken)
    {
        var allEnabled = !allOf.Requirements.IsEmpty;
        foreach (var child in allOf.Requirements)
        {
            var childEnabled = await this.IsSatisfiedAsync(
                    child,
                    featureValues,
                    disabledFeatures,
                    cancellationToken)
                .ConfigureAwait(false);
            allEnabled &= childEnabled;
        }

        return allEnabled;
    }

    private async Task<bool> IsAnyFeatureEnabledAsync(
        AnyFeatureFlagRequirement anyOf,
        Dictionary<string, bool> featureValues,
        List<string> disabledFeatures,
        CancellationToken cancellationToken)
    {
        var anyEnabled = false;
        foreach (var child in anyOf.Requirements)
        {
            var childEnabled = await this.IsSatisfiedAsync(
                    child,
                    featureValues,
                    disabledFeatures,
                    cancellationToken)
                .ConfigureAwait(false);
            anyEnabled |= childEnabled;
        }

        return anyEnabled;
    }

    private async Task<bool> IsFeatureEnabledAsync(
        FeatureFlagEnabledRequirement featureFlag,
        Dictionary<string, bool> featureValues,
        List<string> disabledFeatures,
        CancellationToken cancellationToken)
    {
        if (featureValues.TryGetValue(featureFlag.FeatureFlagKey, out var cachedValue))
        {
            return cachedValue;
        }

        var isEnabled = await this.featureClient
            .GetBooleanValueAsync(
                featureFlag.FeatureFlagKey,
                defaultValue: false,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        featureValues.Add(featureFlag.FeatureFlagKey, isEnabled);
        if (!isEnabled)
        {
            disabledFeatures.Add(featureFlag.FeatureFlagKey);
        }

        return isEnabled;
    }

    private async Task<bool> IsSatisfiedAsync(
        FeatureFlagRequirement requirement,
        Dictionary<string, bool> featureValues,
        List<string> disabledFeatures,
        CancellationToken cancellationToken)
    {
        if (requirement.Value is null)
        {
            throw new ArgumentException("Feature requirement must contain a case.", nameof(requirement));
        }

        return requirement switch
        {
            FeatureFlagEnabledRequirement featureFlag => await this.IsFeatureEnabledAsync(
                    featureFlag,
                    featureValues,
                    disabledFeatures,
                    cancellationToken)
                .ConfigureAwait(false),
            AnyFeatureFlagRequirement anyOf => await this.IsAnyFeatureEnabledAsync(
                    anyOf,
                    featureValues,
                    disabledFeatures,
                    cancellationToken)
                .ConfigureAwait(false),
            AllFeatureFlagsRequirement allOf => await this.AreAllFeaturesEnabledAsync(
                    allOf,
                    featureValues,
                    disabledFeatures,
                    cancellationToken)
                .ConfigureAwait(false),
            _ => throw new ArgumentOutOfRangeException(nameof(requirement), requirement,
                "Unsupported feature flag requirement."),
        };
    }
}
