using System;
using System.Text;
using System.Xml;
using Shouldly;
using TIKSN.Web;
using Xunit;

namespace TIKSN.Tests.Web;

public class SitemapIndexTests
{
    [Fact]
    public void SitemapIndex001()
    {
        var sIndex = new SitemapIndex();

        sIndex.Sitemaps.Add(new Uri("https://microsoft.com/"), new DateOnly(2012, 8, 4));

        sIndex.Sitemaps.Count.ShouldBe(1);
    }

    [Fact]
    public void Write001()
    {
        var sIndex = new SitemapIndex();

        var sBuilder = new StringBuilder();

        var xWriter = XmlWriter.Create(sBuilder);

        sIndex.Write(xWriter);

        var xmlOutput = sBuilder.ToString();

        xmlOutput.ShouldBe("<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<sitemapindex xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\" />");
    }

    [Fact]
    public void Write002()
    {
        var sIndex = new SitemapIndex();

        sIndex.Sitemaps.Add(new Uri("https://microsoft.com/"), null);
        sIndex.Sitemaps.Add(new Uri("https://microsoft.com/siteindex.xml"), new DateOnly(2012, 10, 25));

        var sBuilder = new StringBuilder();

        var xWriter = XmlWriter.Create(sBuilder);

        sIndex.Write(xWriter);

        var xmlOutput = sBuilder.ToString();

        xmlOutput.ShouldBe("<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<sitemapindex xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">" +

            "<sitemap>" +
            "<loc>https://microsoft.com/</loc>" +
            "</sitemap>" +

            "<sitemap>" +
            "<loc>https://microsoft.com/siteindex.xml</loc>" +
            "<lastmod>2012-10-25</lastmod>" +
            "</sitemap>" +

            "</sitemapindex>");
    }
}
