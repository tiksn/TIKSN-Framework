using OpenFeature;
using OpenFeature.Model;
using TIKSN.Identity;

namespace TIKSN.FeatureManagement;

public sealed class UserContextHook<TUserIdentity> : Hook
{
    private readonly IUserIdProvider<TUserIdentity> userIdProvider;

    public UserContextHook(IUserIdProvider<TUserIdentity> userIdProvider) =>
        this.userIdProvider = userIdProvider ?? throw new ArgumentNullException(nameof(userIdProvider));

    public override ValueTask<EvaluationContext> BeforeAsync<T>(
        HookContext<T> context,
        IReadOnlyDictionary<string, object>? hints = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        var userIdOption = this.userIdProvider.FindUserId();
        return userIdOption.Match(
            some =>
            {
                var userIdString = some?.ToString();
                if (string.IsNullOrEmpty(userIdString))
                {
                    return new ValueTask<EvaluationContext>(context.EvaluationContext);
                }

                var builder = EvaluationContext.Builder()
                    .Merge(context.EvaluationContext)
                    .Set(FeatureEvaluationContextKeys.UserId, userIdString);

                if (string.IsNullOrEmpty(context.EvaluationContext.TargetingKey))
                {
                    _ = builder.SetTargetingKey(userIdString);
                }

                return new ValueTask<EvaluationContext>(builder.Build());
            },
            () => new ValueTask<EvaluationContext>(context.EvaluationContext));
    }
}
