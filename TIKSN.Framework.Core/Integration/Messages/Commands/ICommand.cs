using MediatR;

namespace TIKSN.Integration.Messages.Commands;

#pragma warning disable CA1040 // Marker interface for framework command messages.
public interface ICommand : IRequest<Unit>;
#pragma warning restore CA1040 // Marker interface for framework command messages.
