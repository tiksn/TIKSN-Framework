using Xunit;

namespace TIKSN.Web.Tests
{
    public class SitemapIndexTests
    {
        [Fact]
        public void SitemapIndex001()
        {
            var SIndex = new SitemapIndex();

            SIndex.Sitemaps.Add(new System.Uri("http://microsoft.com/"), new System.DateTime(2012, 8, 4, 15, 58, 58, System.DateTimeKind.Utc));

            _ = Assert.Single(SIndex.Sitemaps);
        }

        [Fact]
        public void Write001()
        {
            var SIndex = new SitemapIndex();

            var SBuilder = new System.Text.StringBuilder();

            var XWriter = System.Xml.XmlWriter.Create(SBuilder);

            SIndex.Write(XWriter);

            var XmlOutput = SBuilder.ToString();

            Assert.Equal(
                "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                "<sitemapindex xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\" />",
                XmlOutput);
        }

        [Fact]
        public void Write002()
        {
            var SIndex = new SitemapIndex();

            SIndex.Sitemaps.Add(new System.Uri("http://microsoft.com/"), null);
            SIndex.Sitemaps.Add(new System.Uri("http://microsoft.com/siteindex.xml"), new System.DateTime(2012, 10, 25, 16, 45, 36, System.DateTimeKind.Utc));

            var SBuilder = new System.Text.StringBuilder();

            var XWriter = System.Xml.XmlWriter.Create(SBuilder);

            SIndex.Write(XWriter);

            var XmlOutput = SBuilder.ToString();

            Assert.Equal(
                "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                "<sitemapindex xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">" +

                "<sitemap>" +
                "<loc>http://microsoft.com/</loc>" +
                "</sitemap>" +

                "<sitemap>" +
                "<loc>http://microsoft.com/siteindex.xml</loc>" +
                "<lastmod>2012-10-25</lastmod>" +
                "</sitemap>" +

                "</sitemapindex>",
                XmlOutput);
        }
    }
}
