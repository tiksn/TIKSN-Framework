using System.Globalization;
using System.Xml;

namespace TIKSN.Web;

public class Sitemap
{
    public ISet<SitemapPage> Pages { get; } = new HashSet<SitemapPage>();

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
                xWriter.WriteValue(p.LastModified.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                xWriter.WriteEndElement();
            }

            if (p.ChangeFrequency.HasValue)
            {
                xWriter.WriteStartElement("changefreq");
                xWriter.WriteValue(p.ChangeFrequency.Value.ToString().ToLower(CultureInfo.InvariantCulture));
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
}
