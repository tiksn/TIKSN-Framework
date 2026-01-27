using LanguageExt;

namespace TIKSN.Data;

public interface IPageResult<out T>
{
    public IReadOnlyList<T> Items { get; }
    public Page Page { get; }
    public Option<long> TotalItems { get; }
    public Option<long> TotalPages { get; }
}
