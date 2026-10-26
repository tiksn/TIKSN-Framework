using System.Globalization;
using OpenFeature;
using OpenFeature.Model;

namespace TIKSN.FeatureManagement;

public sealed class GlobalizationContextHook : Hook
{
    public override ValueTask<EvaluationContext> BeforeAsync<T>(
        HookContext<T> context,
        IReadOnlyDictionary<string, object>? hints = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        var builder = EvaluationContext.Builder()
            .Merge(context.EvaluationContext);

        var currentCulture = CultureInfo.CurrentCulture;
        var currentUiCulture = CultureInfo.CurrentUICulture;

        if (!string.IsNullOrEmpty(currentCulture.Name))
        {
            _ = builder.Set(FeatureEvaluationContextKeys.CultureName, currentCulture.Name);

            if (!currentCulture.IsNeutralCulture)
            {
                try
                {
                    var regionInfo = new RegionInfo(currentCulture.Name);
                    _ = builder.Set(FeatureEvaluationContextKeys.RegionTwoLetterIso, regionInfo.TwoLetterISORegionName);
                }
                catch (ArgumentException)
                {
                    // Custom or invariant culture without a specific region
                }
            }
        }

        if (!string.IsNullOrEmpty(currentUiCulture.Name))
        {
            _ = builder.Set(FeatureEvaluationContextKeys.UiCultureName, currentUiCulture.Name);
        }

        return new ValueTask<EvaluationContext>(builder.Build());
    }
}
