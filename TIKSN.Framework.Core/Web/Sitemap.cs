using System.Xml;

namespace TIKSN.Web;

public class Sitemap
{
    public Sitemap() => this.Pages = [];

    public HashSet<Page> Pages { get; }

    public void Write(XmlWriter XWriter)
    {
        XWriter.WriteStartDocument();

        XWriter.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

        foreach (var P in this.Pages)
        {
            XWriter.WriteStartElement("url");

            XWriter.WriteStartElement("loc");
            XWriter.WriteValue(P.Address.AbsoluteUri);
            XWriter.WriteEndElement();

            if (P.LastModified.HasValue)
            {
                XWriter.WriteStartElement("lastmod");
                XWriter.WriteValue(P.LastModified.Value.ToString("yyyy-MM-dd"));
                XWriter.WriteEndElement();
            }

            if (P.ChangeFrequency.HasValue)
            {
                XWriter.WriteStartElement("changefreq");
                XWriter.WriteValue(P.ChangeFrequency.Value.ToString().ToLower(System.Globalization.CultureInfo.CurrentCulture));
                XWriter.WriteEndElement();
            }

            if (P.Priority.HasValue)
            {
                XWriter.WriteStartElement("priority");
                XWriter.WriteValue(P.Priority.Value);
                XWriter.WriteEndElement();
            }

            XWriter.WriteEndElement();
        }

        XWriter.WriteEndElement();

        XWriter.Flush();
    }

    public class Page : IEquatable<Page>
    {
        public enum Frequency { Always = 0, Hourly = 1, Daily = 2, Weekly = 3, Monthly = 4, Yearly = 5, Never = 6 }

        private Uri address;

        private double? priority;

        public Page(Uri Address, DateTime? LastModified, Frequency? ChangeFrequency, double? Priority)
        {
            this.Address = Address;
            this.LastModified = LastModified;
            this.ChangeFrequency = ChangeFrequency;
            this.Priority = Priority;
        }

        public Uri Address
        {
            get => this.address;
            private set
            {
                if (value is null)
                {
                    throw new ArgumentNullException("Address");
                }

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
                    throw new ArgumentOutOfRangeException("Valid values range from 0.0 to 1.0.");
                }
            }
        }

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

        public static bool operator !=(Page page1, Page page2) => !Equals(page1, page2);

        public static bool operator ==(Page page1, Page page2) => Equals(page1, page2);

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
