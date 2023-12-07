using System.Xml;

namespace TIKSN.Web;

public class SitemapIndex
{
    public IDictionary<Uri, DateOnly?> Sitemaps { get; } = new Dictionary<Uri, DateOnly?>();

    public void Write(XmlWriter xWriter)
    {
        ArgumentNullException.ThrowIfNull(xWriter);

        xWriter.WriteStartDocument();

        xWriter.WriteStartElement("sitemapindex", "http://www.sitemaps.org/schemas/sitemap/0.9");

        foreach (var sitemap in this.Sitemaps)
        {
            xWriter.WriteStartElement("sitemap");

            xWriter.WriteStartElement("loc");
            xWriter.WriteValue(sitemap.Key.AbsoluteUri);
            xWriter.WriteEndElement();

            if (sitemap.Value.HasValue)
            {
                xWriter.WriteStartElement("lastmod");
                xWriter.WriteValue(sitemap.Value.Value.ToString("yyyy-MM-dd"));
                xWriter.WriteEndElement();
            }

            xWriter.WriteEndElement();
        }

        xWriter.WriteEndElement();

        xWriter.Flush();
    }
}
