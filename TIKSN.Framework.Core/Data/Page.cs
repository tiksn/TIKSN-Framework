namespace TIKSN.Data;

public sealed class Page : IEquatable<Page>
{
    public Page(int number, int size)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(number);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);

        this.Number = number;
        this.Size = size;
    }

    public int Index => this.Number - 1;
    public int Number { get; }
    public int Size { get; }

    public static bool operator !=(Page left, Page right) => !Equals(left, right);

    public static bool operator ==(Page left, Page right) => Equals(left, right);

    public bool Equals(Page other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.Number == other.Number && this.Size == other.Size;
    }

    public override bool Equals(object obj)
        => ReferenceEquals(this, obj) || (obj is Page other && this.Equals(other));

    public override int GetHashCode() => HashCode.Combine(this.Number, this.Size);

    public Page NextPage() => new(this.Number + 1, this.Size);
}
