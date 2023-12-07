using System.Xml;

namespace TIKSN.Web;

public class Sitemap
{
    public ISet<Page> Pages { get; } = new HashSet<Page>();

    public void Write(XmlWriter xWriter)
    {
        ArgumentNullException.ThrowIfNull(xWriter);

        xWriter.WriteStartDocument();

        xWriter.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

        foreach (var p in this.Pages)
        {
            xWriter.WriteStartElement("url");

            xWriter.WriteStartElement("loc");
            xWriter.WriteValue(p.Address.AbsoluteUri);
            xWriter.WriteEndElement();

            if (p.LastModified.HasValue)
            {
                xWriter.WriteStartElement("lastmod");
                xWriter.WriteValue(p.LastModified.Value.ToString("yyyy-MM-dd"));
                xWriter.WriteEndElement();
            }

            if (p.ChangeFrequency.HasValue)
            {
                xWriter.WriteStartElement("changefreq");
                xWriter.WriteValue(p.ChangeFrequency.Value.ToString().ToLower(System.Globalization.CultureInfo.CurrentCulture));
                xWriter.WriteEndElement();
            }

            if (p.Priority.HasValue)
            {
                xWriter.WriteStartElement("priority");
                xWriter.WriteValue(p.Priority.Value);
                xWriter.WriteEndElement();
            }

            xWriter.WriteEndElement();
        }

        xWriter.WriteEndElement();

        xWriter.Flush();
    }

    public sealed class Page : IEquatable<Page>
    {
        private Uri address;

        private double? priority;

        public Page(Uri address, DateTime? lastModified, Frequency? changeFrequency, double? priority)
        {
            this.Address = address;
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

        public Uri Address
        {
            get => this.address;
            private set
            {
                ArgumentNullException.ThrowIfNull(value);

                this.address = value;
            }
        }

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

        public static bool operator !=(Page page1, Page page2) => !Equals(page1, page2);

        public static bool operator ==(Page page1, Page page2) => Equals(page1, page2);

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

            return this.Address == other.Address;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is not Page p)
            {
                return false;
            }

            return this.Equals(p);
        }

        public override int GetHashCode() => this.Address.GetHashCode();

        private static bool Equals(Page page1, Page page2)
        {
            if (page1 is null)
            {
                return page2 is null;
            }

            return page1.Equals(page2);
        }
    }
}
