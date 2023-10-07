using LanguageExt;

namespace TIKSN.Data;

public interface IPageResult<out T>
{
    IReadOnlyCollection<T> Items { get; }
    Page Page { get; }
    Option<long> TotalItems { get; }
    Option<long> TotalPages { get; }
}
