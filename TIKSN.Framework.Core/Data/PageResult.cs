using LanguageExt;

namespace TIKSN.Data;

public sealed class PageResult<T> : IPageResult<T>
{
    public PageResult(Page page, IReadOnlyCollection<T> items, Option<long> totalItems)
    {
        if (totalItems.IsSome && totalItems.Single() < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(totalItems));
        }

        this.Page = page ?? throw new ArgumentNullException(nameof(page));
        this.Items = items ?? throw new ArgumentNullException(nameof(items));
        this.TotalItems = totalItems;
        this.TotalPages = EstimateTotalPages(totalItems, page.Size);
    }

    public IReadOnlyCollection<T> Items { get; }

    public Page Page { get; }

    public Option<long> TotalItems { get; }

    public Option<long> TotalPages { get; }

    private static Option<long> EstimateTotalPages(Option<long> totalItems, int pageSize)
        => totalItems
            .Map(totalItemsValue
                => totalItemsValue % pageSize == 0 ? totalItemsValue / pageSize : (totalItemsValue / pageSize) + 1);
}
