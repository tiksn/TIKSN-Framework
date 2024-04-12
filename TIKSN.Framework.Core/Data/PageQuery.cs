namespace TIKSN.Data;

public class PageQuery : IEquatable<PageQuery>
{
    public PageQuery(Page page, bool estimateTotalItems = false)
    {
        this.Page = page ?? throw new ArgumentNullException(nameof(page));
        this.EstimateTotalItems = estimateTotalItems;
    }

    public bool EstimateTotalItems { get; }

    public Page Page { get; }

    public static bool operator !=(PageQuery left, PageQuery right) => !Equals(left, right);

    public static bool operator ==(PageQuery left, PageQuery right) => Equals(left, right);

    public bool Equals(PageQuery? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.EstimateTotalItems == other.EstimateTotalItems && Equals(this.Page, other.Page);
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || (obj is PageQuery other && this.Equals(other));

    public override int GetHashCode() => HashCode.Combine(this.EstimateTotalItems, this.Page);
}
