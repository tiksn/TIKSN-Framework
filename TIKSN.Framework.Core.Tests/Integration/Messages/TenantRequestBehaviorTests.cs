using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NSubstitute;
using Shouldly;
using TIKSN.Data;
using TIKSN.Data.BareEntityResolvers;
using TIKSN.Integration.Messages;
using TIKSN.Integration.Messages.Commands;
using TIKSN.Integration.Messages.Queries;
using Xunit;

namespace TIKSN.Tests.Integration.Messages;

public class TenantRequestBehaviorTests
{
    private readonly Guid _currentTenantId;
    private readonly RequestHandlerDelegate<Unit> _nextUnit;
    private readonly IServiceProvider _serviceProvider;
    private readonly ITenantIdProvider<Guid> _tenantIdProvider;

    public TenantRequestBehaviorTests()
    {
        this._serviceProvider = Substitute.For<IServiceProvider>();
        this._tenantIdProvider = Substitute.For<ITenantIdProvider<Guid>>();

        this._currentTenantId = Guid.NewGuid();
        _ = this._tenantIdProvider.GetTenantId().Returns(this._currentTenantId);

        this._nextUnit = ct => Task.FromResult(Unit.Value);
    }

    [Fact]
    public async Task Handle_NonTenantRequest_ThrowsInvalidOperationException()
    {
        var behavior =
            new TenantRequestBehavior<Guid, Guid, IRequest<Unit>, Unit>(this._tenantIdProvider, this._serviceProvider);
        var request = Substitute.For<IRequest<Unit>>();

        _ = await Should.ThrowAsync<InvalidOperationException>(() =>
            behavior.Handle(request, this._nextUnit, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_TenantCommandWithoutReferences_ThrowsInvalidOperationException()
    {
        var behavior =
            new TenantRequestBehavior<Guid, Guid, SimpleTenantCommand, Unit>(this._tenantIdProvider,
                this._serviceProvider);
        var request = new SimpleTenantCommand(this._currentTenantId);

        _ = await Should.ThrowAsync<InvalidOperationException>(() =>
            behavior.Handle(request, this._nextUnit, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_TenantCommand_EmptyTenantId_ThrowsCrossTenantInboundException()
    {
        var behavior =
            new TenantRequestBehavior<Guid, Guid, RefTenantCommand, Unit>(this._tenantIdProvider,
                this._serviceProvider);
        var request = new RefTenantCommand(Guid.Empty, []);

        _ = await Should.ThrowAsync<CrossTenantInboundUnauthorizedAccessException>(() =>
            behavior.Handle(request, this._nextUnit, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_TenantCommand_MismatchedTenantId_ThrowsCrossTenantInboundException()
    {
        var behavior =
            new TenantRequestBehavior<Guid, Guid, RefTenantCommand, Unit>(this._tenantIdProvider,
                this._serviceProvider);
        var request = new RefTenantCommand(Guid.NewGuid(), []);

        _ = await Should.ThrowAsync<CrossTenantInboundUnauthorizedAccessException>(() =>
            behavior.Handle(request, this._nextUnit, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_TenantCommand_ValidTenantIdAndNoReferences_Proceeds()
    {
        var behavior =
            new TenantRequestBehavior<Guid, Guid, RefTenantCommand, Unit>(this._tenantIdProvider,
                this._serviceProvider);
        var request = new RefTenantCommand(this._currentTenantId, []);

        var result = await behavior.Handle(request, this._nextUnit, CancellationToken.None);

        result.ShouldBe(Unit.Value);
    }

    [Fact]
    public async Task Handle_TenantCommand_WithReferencesMismatchedTenantId_ThrowsCrossTenantInboundException()
    {
        var entityId = Guid.NewGuid();
        var references = new[]
        {
            new EntityReference<Guid>(typeof(TestEntity), entityId)
        };
        var request = new RefTenantCommand(this._currentTenantId, references);

        var resolver = Substitute.For<IBareEntityResolver<TenantEntity<Guid, Guid>, Guid>>();
        _ = resolver.ResolveAsync(entityId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<TenantEntity<Guid, Guid>>(new TestEntity(entityId,
                Guid.NewGuid()))); // Mismatched tenant

        var resolverType =
            typeof(IBareEntityResolver<,,>).MakeGenericType(typeof(TestEntity), typeof(TenantEntity<Guid, Guid>),
                typeof(Guid));
        _ = this._serviceProvider.GetService(resolverType).Returns(resolver);

        var behavior =
            new TenantRequestBehavior<Guid, Guid, RefTenantCommand, Unit>(this._tenantIdProvider,
                this._serviceProvider);

        _ = await Should.ThrowAsync<CrossTenantInboundUnauthorizedAccessException>(() =>
            behavior.Handle(request, this._nextUnit, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_TenantCommand_WithReferencesValidTenantId_Proceeds()
    {
        var entityId = Guid.NewGuid();
        var references = new[]
        {
            new EntityReference<Guid>(typeof(TestEntity), entityId)
        };
        var request = new RefTenantCommand(this._currentTenantId, references);

        var resolver = Substitute.For<IBareEntityResolver<TenantEntity<Guid, Guid>, Guid>>();
        _ = resolver.ResolveAsync(entityId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<TenantEntity<Guid, Guid>>(new TestEntity(entityId,
                this._currentTenantId))); // Matching tenant

        var resolverType =
            typeof(IBareEntityResolver<,,>).MakeGenericType(typeof(TestEntity), typeof(TenantEntity<Guid, Guid>),
                typeof(Guid));
        _ = this._serviceProvider.GetService(resolverType).Returns(resolver);

        var behavior =
            new TenantRequestBehavior<Guid, Guid, RefTenantCommand, Unit>(this._tenantIdProvider,
                this._serviceProvider);

        var result = await behavior.Handle(request, this._nextUnit, CancellationToken.None);

        result.ShouldBe(Unit.Value);
    }

    [Fact]
    public async Task Handle_TenantQuery_OutboundMismatchedTenantId_ThrowsCrossTenantOutboundException()
    {
        var request = new RefTenantQuery(this._currentTenantId, []);
        var mismatchedResponse = new TestEntity(Guid.NewGuid(), Guid.NewGuid());

        Task<TestEntity> nextQuery(CancellationToken ct = default) => Task.FromResult(mismatchedResponse);

        var behavior =
            new TenantRequestBehavior<Guid, Guid, RefTenantQuery, TestEntity>(this._tenantIdProvider,
                this._serviceProvider);

        _ = await Should.ThrowAsync<CrossTenantOutboundUnauthorizedAccessException>(() =>
            behavior.Handle(request, nextQuery, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_TenantQuery_OutboundValidTenantId_Proceeds()
    {
        var request = new RefTenantQuery(this._currentTenantId, []);
        var validResponse = new TestEntity(Guid.NewGuid(), this._currentTenantId);

        Task<TestEntity> nextQuery(CancellationToken ct = default) => Task.FromResult(validResponse);

        var behavior =
            new TenantRequestBehavior<Guid, Guid, RefTenantQuery, TestEntity>(this._tenantIdProvider,
                this._serviceProvider);

        var result = await behavior.Handle(request, nextQuery, CancellationToken.None);

        result.ShouldBe(validResponse);
    }

    public class RefTenantCommand : ITenantEntityCommand<Guid, Guid>, ITenantEntityReferences<Guid>
    {
        public RefTenantCommand(Guid tenantId, IEnumerable<EntityReference<Guid>> tenantEntityReferences)
        {
            this.TenantID = tenantId;
            this.TenantEntityReferences = tenantEntityReferences;
        }

        public IEnumerable<Guid> TenantEntityIdentities => [];
        public IEnumerable<EntityReference<Guid>> TenantEntityReferences { get; }

        public Guid TenantID { get; }
    }

    public class RefTenantQuery : ITenantEntityQuery<Guid, Guid, TestEntity>, ITenantEntityReferences<Guid>
    {
        public RefTenantQuery(Guid tenantId, IEnumerable<EntityReference<Guid>> tenantEntityReferences)
        {
            this.TenantID = tenantId;
            this.TenantEntityReferences = tenantEntityReferences;
        }

        public IEnumerable<Guid> TenantEntityIdentities => [];
        public IEnumerable<EntityReference<Guid>> TenantEntityReferences { get; }

        public Guid TenantID { get; }
    }

    public class SimpleTenantCommand : ITenantEntityCommand<Guid, Guid>
    {
        public SimpleTenantCommand(Guid tenantId) => this.TenantID = tenantId;
        public IEnumerable<Guid> TenantEntityIdentities => [];

        public Guid TenantID { get; }
    }

    public record TestEntity : TenantEntity<Guid, Guid>
    {
        public TestEntity(Guid id, Guid tenantId) : base(id, tenantId)
        {
        }
    }
}
