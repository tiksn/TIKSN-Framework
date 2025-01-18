using Shouldly;
using TIKSN.Web;
using Xunit;

namespace TIKSN.Tests.Web;

public class SitemapTests
{
    [Fact]
    public void Pages001()
    {
        var p1 = new SitemapPage(new System.Uri("https://www.microsoft.com/"), System.DateTime.Now, SitemapPage.Frequency.Always, 0.5);
        var p2 = new SitemapPage(new System.Uri("https://www.microsoft.com/"), System.DateTime.Now, SitemapPage.Frequency.Always, 0.5);

        var map = new Sitemap();

        map.Pages.Add(p1).ShouldBeTrue();
        map.Pages.Add(p2).ShouldBeFalse();
    }

    [Fact]
    public void Pages002()
    {
        var p1 = new SitemapPage(new System.Uri("https://www.microsoft.com/"), System.DateTime.Now, SitemapPage.Frequency.Always, 0.5);

        var map = new Sitemap();

        map.Pages.Add(p1).ShouldBeTrue();
        map.Pages.Add(p1).ShouldBeFalse();
    }

    [Fact]
    public void Pages003()
    {
        var p1 = new SitemapPage(new System.Uri("https://www.microsoft.com/"), System.DateTime.Now, SitemapPage.Frequency.Always, 0.5);
        var p2 = new SitemapPage(new System.Uri("https://www.microsoft.com/"), System.DateTime.Now.AddDays(10d), SitemapPage.Frequency.Monthly, 0.2);

        var map = new Sitemap();

        map.Pages.Add(p1).ShouldBeTrue();
        map.Pages.Add(p2).ShouldBeFalse();
    }

    [Fact]
    public void Write001()
    {
        var sbuilder = new System.Text.StringBuilder();
        var xwriter = System.Xml.XmlWriter.Create(sbuilder);

        var map = new Sitemap();

        map.Write(xwriter);

        var xmlOutput = sbuilder.ToString();

        xmlOutput.ShouldBe("<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\" />");
    }

    [Fact]
    public void Write002()
    {
        var sbuilder = new System.Text.StringBuilder();
        var xwriter = System.Xml.XmlWriter.Create(sbuilder);

        var map = new Sitemap();

        _ = map.Pages.Add(new SitemapPage(new System.Uri("https://microsoft.com/"), new System.DateTime(2012, 8, 3), SitemapPage.Frequency.Daily, 0.2));
        _ = map.Pages.Add(new SitemapPage(new System.Uri("https://microsoft.com/sitemap.aspx"), new System.DateTime(2012, 8, 3), SitemapPage.Frequency.Daily, 0.2));
        _ = map.Pages.Add(new SitemapPage(new System.Uri("https://microsoft.com/default.aspx"), new System.DateTime(2012, 4, 5), SitemapPage.Frequency.Monthly, 0.8));

        map.Write(xwriter);

        var xmlOutput = sbuilder.ToString();

        xmlOutput.ShouldBe("<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">" +

            "<url>" +
            "<loc>https://microsoft.com/</loc>" +
            "<lastmod>2012-08-03</lastmod>" +
            "<changefreq>daily</changefreq>" +
            "<priority>0.2</priority>" +
            "</url>" +

            "<url>" +
            "<loc>https://microsoft.com/sitemap.aspx</loc>" +
            "<lastmod>2012-08-03</lastmod>" +
            "<changefreq>daily</changefreq>" +
            "<priority>0.2</priority>" +
            "</url>" +

            "<url>" +
            "<loc>https://microsoft.com/default.aspx</loc>" +
            "<lastmod>2012-04-05</lastmod>" +
            "<changefreq>monthly</changefreq>" +
            "<priority>0.8</priority>" +
            "</url>" +

            "</urlset>");
    }
}
