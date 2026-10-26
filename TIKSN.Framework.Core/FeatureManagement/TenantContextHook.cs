using OpenFeature;
using OpenFeature.Model;
using TIKSN.Identity;

namespace TIKSN.FeatureManagement;

public sealed class TenantContextHook<TTenantIdentity> : Hook
{
    private readonly ITenantIdProvider<TTenantIdentity> tenantIdProvider;

    public TenantContextHook(ITenantIdProvider<TTenantIdentity> tenantIdProvider) =>
        this.tenantIdProvider = tenantIdProvider ?? throw new ArgumentNullException(nameof(tenantIdProvider));

    public override ValueTask<EvaluationContext> BeforeAsync<T>(
        HookContext<T> context,
        IReadOnlyDictionary<string, object>? hints = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        var tenantIdOption = this.tenantIdProvider.FindTenantId();
        return tenantIdOption.Match(
            some =>
            {
                var tenantIdString = some?.ToString();
                if (string.IsNullOrEmpty(tenantIdString))
                {
                    return new ValueTask<EvaluationContext>(context.EvaluationContext);
                }

                var builder = EvaluationContext.Builder()
                    .Merge(context.EvaluationContext)
                    .Set(FeatureEvaluationContextKeys.TenantId, tenantIdString);

                if (string.IsNullOrEmpty(context.EvaluationContext.TargetingKey))
                {
                    _ = builder.SetTargetingKey(tenantIdString);
                }

                return new ValueTask<EvaluationContext>(builder.Build());
            },
            () => new ValueTask<EvaluationContext>(context.EvaluationContext));
    }
}
