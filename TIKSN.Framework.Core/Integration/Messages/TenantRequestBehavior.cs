using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.Data;
using TIKSN.Data.BareEntityResolvers;
using TIKSN.Integration.Messages.Commands;
using TIKSN.Integration.Messages.Queries;

namespace TIKSN.Integration.Messages;

public class TenantRequestBehavior<TEntityIdentity, TTenantIdentity, TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TEntityIdentity : IEquatable<TEntityIdentity>
    where TTenantIdentity : IEquatable<TTenantIdentity>
    where TRequest : notnull
{
#pragma warning disable S2743 // Static fields should not be used in generic types
    private static readonly Type _entityAwareBareEntityResolverType = typeof(IBareEntityResolver<,,>);
#pragma warning restore S2743 // Static fields should not be used in generic types

    private readonly IServiceProvider _serviceProvider;
    private readonly ITenantIdProvider<TTenantIdentity> _tenantIdProvider;

    public TenantRequestBehavior(
        ITenantIdProvider<TTenantIdentity> tenantIdProvider,
        IServiceProvider serviceProvider)
    {
        this._tenantIdProvider = tenantIdProvider ?? throw new ArgumentNullException(nameof(tenantIdProvider));
        this._serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        await this.InspectRequestAsync(request, cancellationToken).ConfigureAwait(false);

        var response = await next(cancellationToken).ConfigureAwait(false);

        await this.InspectResponseAsync(response, cancellationToken).ConfigureAwait(false);

        return response;
    }

    private static Task InspectTenantEntityAsync(
        TTenantIdentity tenantId,
        ITenantEntity<TEntityIdentity, TTenantIdentity> tenantEntity,
        CancellationToken cancellationToken)
    {
        if (tenantEntity.TenantID.Equals(default) || !tenantEntity.TenantID.Equals(tenantId))
        {
            throw new CrossTenantOutboundUnauthorizedAccessException();
        }

        cancellationToken.ThrowIfCancellationRequested();

        return Task.CompletedTask;
    }

    private async Task InspectAffectingTenantEntitiesAsync(
        IEnumerable<EntityReference<TEntityIdentity>> affectingEntities,
        TTenantIdentity tenantId,
        CancellationToken cancellationToken)
    {
        var affectingEntitiesList = affectingEntities.ToList();
        var bareEntityResolvers = affectingEntitiesList
            .Select(x => x.EntityType)
            .Distinct()
            .ToDictionary(k => k, v => _entityAwareBareEntityResolverType
                .MakeGenericType(
                    v,
                    typeof(TenantEntity<TEntityIdentity, TTenantIdentity>),
                    typeof(TEntityIdentity)));

        foreach (var affectingTenantEntity in affectingEntitiesList)
        {
            var bareEntityResolver =
                (IBareEntityResolver<TenantEntity<TEntityIdentity, TTenantIdentity>, TEntityIdentity>)
                this._serviceProvider.GetRequiredService(bareEntityResolvers[affectingTenantEntity.EntityType]);

            var bareTenantEntity = await bareEntityResolver
                .ResolveAsync(affectingTenantEntity.EntityID, cancellationToken)
                .ConfigureAwait(false);
            if (bareTenantEntity.TenantID.Equals(default) || !bareTenantEntity.TenantID.Equals(tenantId))
            {
                throw new CrossTenantInboundUnauthorizedAccessException();
            }
        }
    }

    private Task InspectRequestAsync(
        TRequest request,
        CancellationToken cancellationToken)
    {
        if (request is ITenantEntityCommand<TEntityIdentity, TTenantIdentity> tenantCommand)
        {
            return this.InspectTenantCommandAsync(tenantCommand, cancellationToken);
        }

        if (request is ITenantEntityQuery<TEntityIdentity, TTenantIdentity, TResponse> tenantQuery)
        {
            return this.InspectTenantQueryAsync(tenantQuery, cancellationToken);
        }

        throw new InvalidOperationException("Request is not Tenant Command or Tenant Query");
    }

    private async Task InspectResponseAsync(
        TResponse? response,
        CancellationToken cancellationToken)
    {
        if (response is not null and not Unit)
        {
            var tenantId = this._tenantIdProvider.GetTenantId();

            if (response is ITenantEntity<TEntityIdentity, TTenantIdentity> tenantEntity)
            {
                await InspectTenantEntityAsync(tenantId, tenantEntity, cancellationToken).ConfigureAwait(false);
            }
            else if (response is IEnumerable<ITenantEntity<TEntityIdentity, TTenantIdentity>> tenantEntities)
            {
                foreach (var currentTenantEntity in tenantEntities)
                {
                    await InspectTenantEntityAsync(tenantId, currentTenantEntity, cancellationToken)
                        .ConfigureAwait(false);
                }
            }
            else if (response is IPageResult<ITenantEntity<TEntityIdentity, TTenantIdentity>> tenantEntitiesPageResult)
            {
                foreach (var currentTenantEntity in tenantEntitiesPageResult.Items)
                {
                    await InspectTenantEntityAsync(tenantId, currentTenantEntity, cancellationToken)
                        .ConfigureAwait(false);
                }
            }
            else
            {
                throw new InvalidOperationException($"Unknown response type {response.GetType()?.FullName}");
            }
        }
    }

    private async Task InspectTenantCommandAsync(
        ITenantEntityCommand<TEntityIdentity, TTenantIdentity> tenantCommand,
        CancellationToken cancellationToken)
    {
        var tenantId = this._tenantIdProvider.GetTenantId();

        if (tenantCommand.TenantID.Equals(default) || !tenantCommand.TenantID.Equals(tenantId))
        {
            throw new CrossTenantInboundUnauthorizedAccessException();
        }

        if (tenantCommand is ITenantEntityReferences<TEntityIdentity> tenantEntityReferences)
        {
            await this.InspectAffectingTenantEntitiesAsync(
                tenantEntityReferences.TenantEntityReferences,
                tenantId,
                cancellationToken).ConfigureAwait(false);
        }
        else
        {
            throw new InvalidOperationException("Tenant Command does not implement ITenantEntityReferences");
        }
    }

    private async Task InspectTenantQueryAsync(
        ITenantEntityQuery<TEntityIdentity, TTenantIdentity, TResponse> tenantQuery,
        CancellationToken cancellationToken)
    {
        var tenantId = this._tenantIdProvider.GetTenantId();

        if (tenantQuery.TenantID.Equals(default) || !tenantQuery.TenantID.Equals(tenantId))
        {
            throw new CrossTenantInboundUnauthorizedAccessException();
        }

        if (tenantQuery is ITenantEntityReferences<TEntityIdentity> tenantEntityReferences)
        {
            await this.InspectAffectingTenantEntitiesAsync(
                tenantEntityReferences.TenantEntityReferences,
                tenantId,
                cancellationToken).ConfigureAwait(false);
        }
        else
        {
            throw new InvalidOperationException("Tenant Query does not implement ITenantEntityReferences");
        }
    }
}
