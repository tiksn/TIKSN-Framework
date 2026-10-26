using Microsoft.Extensions.Hosting;
using OpenFeature;
using OpenFeature.Model;

namespace TIKSN.FeatureManagement;

public sealed class HostEnvironmentContextHook : Hook
{
    private readonly IHostEnvironment? hostEnvironment;

    public HostEnvironmentContextHook(IHostEnvironment? hostEnvironment = null) =>
        this.hostEnvironment = hostEnvironment;

    public override ValueTask<EvaluationContext> BeforeAsync<T>(
        HookContext<T> context,
        IReadOnlyDictionary<string, object>? hints = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (this.hostEnvironment is null || string.IsNullOrWhiteSpace(this.hostEnvironment.EnvironmentName))
        {
            return new ValueTask<EvaluationContext>(context.EvaluationContext);
        }

        var evaluationContext = EvaluationContext.Builder()
            .Merge(context.EvaluationContext)
            .Set(FeatureEvaluationContextKeys.DeploymentEnvironment, this.hostEnvironment.EnvironmentName)
            .Build();

        return new ValueTask<EvaluationContext>(evaluationContext);
    }
}
