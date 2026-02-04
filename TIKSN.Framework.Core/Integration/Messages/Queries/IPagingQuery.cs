using TIKSN.Data;

namespace TIKSN.Integration.Messages.Queries;

public interface IPagingQuery
{
    public Page Page { get; }
}

public interface IPagingQuery<TEntity>
  : IQuery<PageResult<TEntity>>, IPagingQuery;
