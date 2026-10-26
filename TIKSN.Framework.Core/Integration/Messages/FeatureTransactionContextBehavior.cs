using MediatR;
using OpenFeature.Model;
using TIKSN.FeatureManagement;
using TIKSN.Integration.Messages.Commands;
using TIKSN.Integration.Messages.Queries;

namespace TIKSN.Integration.Messages;

public class FeatureTransactionContextBehavior<TTenantIdentity, TUserIdentity, TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TTenantIdentity : IEquatable<TTenantIdentity>
    where TUserIdentity : IEquatable<TUserIdentity>
    where TRequest : notnull
{
    private readonly ITransactionContextPropagator transactionContextPropagator;

    public FeatureTransactionContextBehavior(ITransactionContextPropagator transactionContextPropagator) =>
        this.transactionContextPropagator = transactionContextPropagator ??
            throw new ArgumentNullException(nameof(transactionContextPropagator));

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(next);

        var previousContext = this.transactionContextPropagator.GetTransactionContext();
        var tenantIdString = TryExtractTenantId(request);
        var userIdString = TryExtractUserId(request);

        if (tenantIdString is null && userIdString is null)
        {
            return await next(cancellationToken).ConfigureAwait(false);
        }

        var builder = EvaluationContext.Builder()
            .Merge(previousContext);

        if (!string.IsNullOrEmpty(tenantIdString))
        {
            _ = builder.Set(FeatureEvaluationContextKeys.TenantId, tenantIdString);
        }

        if (!string.IsNullOrEmpty(userIdString))
        {
            _ = builder.Set(FeatureEvaluationContextKeys.UserId, userIdString);
        }

        if (string.IsNullOrEmpty(previousContext.TargetingKey))
        {
            var targetingKey = !string.IsNullOrEmpty(userIdString) ? userIdString : tenantIdString;
            if (!string.IsNullOrEmpty(targetingKey))
            {
                _ = builder.SetTargetingKey(targetingKey);
            }
        }

        this.transactionContextPropagator.SetTransactionContext(builder.Build());
        try
        {
            return await next(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            this.transactionContextPropagator.SetTransactionContext(previousContext);
        }
    }

    private static string? TryExtractTenantId(TRequest request) =>
        request switch
        {
            ITenantCommand<TTenantIdentity> tenantCommand => tenantCommand.TenantID?.ToString(),
            ITenantQuery<TTenantIdentity, TResponse> tenantQuery => tenantQuery.TenantID?.ToString(),
            _ => null,
        };

    private static string? TryExtractUserId(TRequest request) =>
        request switch
        {
            IUserCommand<TUserIdentity> userCommand => userCommand.UserID?.ToString(),
            IUserQuery<TUserIdentity, TResponse> userQuery => userQuery.UserID?.ToString(),
            _ => null,
        };
}

public class TenantFeatureTransactionContextBehavior<TTenantIdentity, TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TTenantIdentity : IEquatable<TTenantIdentity>
    where TRequest : notnull
{
    private readonly ITransactionContextPropagator transactionContextPropagator;

    public TenantFeatureTransactionContextBehavior(ITransactionContextPropagator transactionContextPropagator) =>
        this.transactionContextPropagator = transactionContextPropagator ??
            throw new ArgumentNullException(nameof(transactionContextPropagator));

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(next);

        var tenantIdString = request switch
        {
            ITenantCommand<TTenantIdentity> tenantCommand => tenantCommand.TenantID?.ToString(),
            ITenantQuery<TTenantIdentity, TResponse> tenantQuery => tenantQuery.TenantID?.ToString(),
            _ => null,
        };

        if (string.IsNullOrEmpty(tenantIdString))
        {
            return await next(cancellationToken).ConfigureAwait(false);
        }

        var previousContext = this.transactionContextPropagator.GetTransactionContext();
        var builder = EvaluationContext.Builder()
            .Merge(previousContext)
            .Set(FeatureEvaluationContextKeys.TenantId, tenantIdString);

        if (string.IsNullOrEmpty(previousContext.TargetingKey))
        {
            _ = builder.SetTargetingKey(tenantIdString);
        }

        this.transactionContextPropagator.SetTransactionContext(builder.Build());
        try
        {
            return await next(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            this.transactionContextPropagator.SetTransactionContext(previousContext);
        }
    }
}

public class UserFeatureTransactionContextBehavior<TUserIdentity, TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TUserIdentity : IEquatable<TUserIdentity>
    where TRequest : notnull
{
    private readonly ITransactionContextPropagator transactionContextPropagator;

    public UserFeatureTransactionContextBehavior(ITransactionContextPropagator transactionContextPropagator) =>
        this.transactionContextPropagator = transactionContextPropagator ??
            throw new ArgumentNullException(nameof(transactionContextPropagator));

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(next);

        var userIdString = request switch
        {
            IUserCommand<TUserIdentity> userCommand => userCommand.UserID?.ToString(),
            IUserQuery<TUserIdentity, TResponse> userQuery => userQuery.UserID?.ToString(),
            _ => null,
        };

        if (string.IsNullOrEmpty(userIdString))
        {
            return await next(cancellationToken).ConfigureAwait(false);
        }

        var previousContext = this.transactionContextPropagator.GetTransactionContext();
        var builder = EvaluationContext.Builder()
            .Merge(previousContext)
            .Set(FeatureEvaluationContextKeys.UserId, userIdString);

        if (string.IsNullOrEmpty(previousContext.TargetingKey))
        {
            _ = builder.SetTargetingKey(userIdString);
        }

        this.transactionContextPropagator.SetTransactionContext(builder.Build());
        try
        {
            return await next(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            this.transactionContextPropagator.SetTransactionContext(previousContext);
        }
    }
}
