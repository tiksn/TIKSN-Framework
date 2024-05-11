namespace TIKSN.Web;

public sealed class SitemapPage : IEquatable<SitemapPage>
{
    private double? priority;

    public SitemapPage(Uri address, DateTime? lastModified, Frequency? changeFrequency, double? priority)
    {
        this.Address = address ?? throw new ArgumentNullException(nameof(address));
        this.LastModified = lastModified;
        this.ChangeFrequency = changeFrequency;
        this.Priority = priority;
    }

    public enum Frequency
    {
        Always = 0,
        Hourly = 1,
        Daily = 2,
        Weekly = 3,
        Monthly = 4,
        Yearly = 5,
        Never = 6,
    }

    public Uri Address { get; }

    public Frequency? ChangeFrequency { get; }

    public DateTime? LastModified { get; }

    public double? Priority
    {
        get => this.priority;
        private set
        {
            if (!value.HasValue || (value.Value >= 0.0d && value.Value <= 1.0d))
            {
                this.priority = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Valid values range from 0.0 to 1.0.");
            }
        }
    }

    public static bool operator !=(SitemapPage? page1, SitemapPage? page2) => !Equals(page1, page2);

    public static bool operator ==(SitemapPage? page1, SitemapPage? page2) => Equals(page1, page2);

    public bool Equals(SitemapPage? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.Address == other.Address;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is not SitemapPage p)
        {
            return false;
        }

        return this.Equals(p);
    }

    public override int GetHashCode() => this.Address.GetHashCode();

    private static bool Equals(SitemapPage? page1, SitemapPage? page2)
    {
        if (page1 is null)
        {
            return page2 is null;
        }

        return page1.Equals(page2);
    }
}
