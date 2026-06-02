using MediatR;

namespace TIKSN.Integration.Messages.Queries;

#pragma warning disable CA1040 // Marker interface for framework query messages.
public interface IQuery<out TResult> : IRequest<TResult>;
#pragma warning restore CA1040 // Marker interface for framework query messages.
