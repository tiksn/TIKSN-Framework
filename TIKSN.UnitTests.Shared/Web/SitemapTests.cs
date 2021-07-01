using Xunit;

namespace TIKSN.Web.Tests
{
    public class SitemapTests
    {
        [Fact]
        public void Pages001()
        {
            Sitemap.Page p1 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);
            Sitemap.Page p2 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);

            Sitemap map = new Sitemap();

            Assert.True(map.Pages.Add(p1));
            Assert.False(map.Pages.Add(p2));
        }

        [Fact]
        public void Pages002()
        {
            Sitemap.Page p1 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);

            Sitemap map = new Sitemap();

            Assert.True(map.Pages.Add(p1));
            Assert.False(map.Pages.Add(p1));
        }

        [Fact]
        public void Pages003()
        {
            Sitemap.Page p1 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);
            Sitemap.Page p2 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now.AddDays(10d), Sitemap.Page.Frequency.Monthly, 0.2);

            Sitemap map = new Sitemap();

            Assert.True(map.Pages.Add(p1));
            Assert.False(map.Pages.Add(p2));
        }

        [Fact]
        public void Write001()
        {
            System.Text.StringBuilder sbuilder = new System.Text.StringBuilder();
            System.Xml.XmlWriter xwriter = System.Xml.XmlWriter.Create(sbuilder);

            Sitemap map = new Sitemap();

            map.Write(xwriter);

            string XmlOutput = sbuilder.ToString();

            Assert.Equal(
                "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                "<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\" />",
                XmlOutput);
        }

        [Fact]
        public void Write002()
        {
            System.Text.StringBuilder sbuilder = new System.Text.StringBuilder();
            System.Xml.XmlWriter xwriter = System.Xml.XmlWriter.Create(sbuilder);

            Sitemap map = new Sitemap();

            map.Pages.Add(new Sitemap.Page(new System.Uri("http://microsoft.com/"), new System.DateTime(2012, 8, 3), Sitemap.Page.Frequency.Daily, 0.2));
            map.Pages.Add(new Sitemap.Page(new System.Uri("http://microsoft.com/sitemap.aspx"), new System.DateTime(2012, 8, 3), Sitemap.Page.Frequency.Daily, 0.2));
            map.Pages.Add(new Sitemap.Page(new System.Uri("http://microsoft.com/default.aspx"), new System.DateTime(2012, 4, 5), Sitemap.Page.Frequency.Monthly, 0.8));

            map.Write(xwriter);

            string XmlOutput = sbuilder.ToString();

            Assert.Equal(
                "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                "<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">" +

                "<url>" +
                "<loc>http://microsoft.com/</loc>" +
                "<lastmod>2012-08-03</lastmod>" +
                "<changefreq>daily</changefreq>" +
                "<priority>0.2</priority>" +
                "</url>" +

                "<url>" +
                "<loc>http://microsoft.com/sitemap.aspx</loc>" +
                "<lastmod>2012-08-03</lastmod>" +
                "<changefreq>daily</changefreq>" +
                "<priority>0.2</priority>" +
                "</url>" +

                "<url>" +
                "<loc>http://microsoft.com/default.aspx</loc>" +
                "<lastmod>2012-04-05</lastmod>" +
                "<changefreq>monthly</changefreq>" +
                "<priority>0.8</priority>" +
                "</url>" +

                "</urlset>",
                XmlOutput);
        }
    }
}
