using System.Xml;

namespace TIKSN.Web;

public class SitemapIndex
{
    public SitemapIndex() => this.Sitemaps = new Dictionary<Uri, DateTime?>();

    public Dictionary<Uri, DateTime?> Sitemaps { get; }

    public void Write(XmlWriter XWriter)
    {
        XWriter.WriteStartDocument();

        XWriter.WriteStartElement("sitemapindex", "http://www.sitemaps.org/schemas/sitemap/0.9");

        foreach (var SM in this.Sitemaps)
        {
            XWriter.WriteStartElement("sitemap");

            XWriter.WriteStartElement("loc");
            XWriter.WriteValue(SM.Key.AbsoluteUri);
            XWriter.WriteEndElement();

            if (SM.Value.HasValue)
            {
                XWriter.WriteStartElement("lastmod");
                XWriter.WriteValue(SM.Value.Value.ToString("yyyy-MM-dd"));
                XWriter.WriteEndElement();
            }

            XWriter.WriteEndElement();
        }

        XWriter.WriteEndElement();

        XWriter.Flush();
    }
}
