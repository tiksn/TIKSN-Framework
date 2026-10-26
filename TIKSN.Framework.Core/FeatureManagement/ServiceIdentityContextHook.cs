using OpenFeature;
using OpenFeature.Model;
using TIKSN.Identity;

namespace TIKSN.FeatureManagement;

public sealed class ServiceIdentityContextHook : Hook
{
    private readonly ServiceIdentity? serviceIdentity;

    public ServiceIdentityContextHook(ServiceIdentity? serviceIdentity = null) =>
        this.serviceIdentity = serviceIdentity;

    public override ValueTask<EvaluationContext> BeforeAsync<T>(
        HookContext<T> context,
        IReadOnlyDictionary<string, object>? hints = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (this.serviceIdentity is null)
        {
            return new ValueTask<EvaluationContext>(context.EvaluationContext);
        }

        var builder = EvaluationContext.Builder()
            .Merge(context.EvaluationContext)
            .Set(FeatureEvaluationContextKeys.ServiceApplicationName, this.serviceIdentity.ApplicationName)
            .Set(FeatureEvaluationContextKeys.ServiceInstanceId, this.serviceIdentity.InstanceId.ToString())
            .Set(FeatureEvaluationContextKeys.ServicePath, this.serviceIdentity.ServicePath);

        _ = this.serviceIdentity.ComponentPath.Match(
            componentPath => builder.Set(FeatureEvaluationContextKeys.ServiceComponentPath, componentPath),
            () => builder);

        return new ValueTask<EvaluationContext>(builder.Build());
    }
}
