using LanguageExt;
using static LanguageExt.Prelude;

namespace TIKSN.Data;

public static class PaginationQueryableHelper
{
    public static async Task<PageResult<TEntity>> PageAsync<TEntity>(
        IQueryable<TEntity> query,
        PageQuery pageQuery,
        Func<IQueryable<TEntity>, CancellationToken, Task<List<TEntity>>> requestListAsync,
        Func<IQueryable<TEntity>, CancellationToken, Task<long>> requestCountAsync,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        ArgumentNullException.ThrowIfNull(pageQuery);

        ArgumentNullException.ThrowIfNull(requestListAsync);

        ArgumentNullException.ThrowIfNull(requestCountAsync);

        var itemsQuery = query
            .Skip(pageQuery.Page.Index * pageQuery.Page.Size)
            .Take(pageQuery.Page.Size);

        var items = await requestListAsync(itemsQuery, cancellationToken).ConfigureAwait(false);

        Option<long> totalItems = pageQuery.EstimateTotalItems
            ? await requestCountAsync(query, cancellationToken).ConfigureAwait(false)
            : None;

        return new PageResult<TEntity>(pageQuery.Page, items, totalItems);
    }
}
