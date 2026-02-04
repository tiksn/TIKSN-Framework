using MediatR;

namespace TIKSN.Integration.Messages.Queries;

public interface IQuery<out TResult> : IRequest<TResult>;
