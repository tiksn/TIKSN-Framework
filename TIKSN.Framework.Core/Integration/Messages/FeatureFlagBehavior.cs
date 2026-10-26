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

        var features = request.Features;
        if (features.IsEmpty)
        {
            return await next(cancellationToken).ConfigureAwait(false);
        }

        var disabledFeatures = new List<string>();
        var anyEnabled = false;

        foreach (var featureKey in features)
        {
            var isEnabled = await this.featureClient
                .GetBooleanValueAsync(featureKey, defaultValue: false, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (isEnabled)
            {
                anyEnabled = true;
            }
            else
            {
                disabledFeatures.Add(featureKey);
            }
        }

        if (request.RequireAllFeatures && disabledFeatures.Count > 0)
        {
            var disabledSeq = disabledFeatures.ToSeq();
            throw new FeatureDisabledException(
                $"Required feature flags are disabled: {string.Join(", ", disabledSeq)}.",
                disabledSeq);
        }

        if (!request.RequireAllFeatures && !anyEnabled)
        {
            var disabledSeq = disabledFeatures.ToSeq();
            throw new FeatureDisabledException(
                $"None of the required feature flags are enabled: {string.Join(", ", disabledSeq)}.",
                disabledSeq);
        }

        return await next(cancellationToken).ConfigureAwait(false);
    }
}
