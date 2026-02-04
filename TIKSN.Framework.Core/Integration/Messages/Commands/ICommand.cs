using MediatR;

namespace TIKSN.Integration.Messages.Commands;

public interface ICommand : IRequest<Unit>;
