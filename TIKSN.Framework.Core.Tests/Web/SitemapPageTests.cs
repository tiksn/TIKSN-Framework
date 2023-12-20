using System;
using FluentAssertions;
using TIKSN.Web;
using Xunit;

namespace TIKSN.Tests.Web;

public class SitemapPageTests
{
    [Fact]
    public void Equality001()
    {
        var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);
        Sitemap.Page p2 = null;

        _ = (null == p1).Should().BeFalse();
        _ = (null == p2).Should().BeTrue();
    }

    [Fact]
    public void Equals001()
    {
        var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);
        var p2 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);

        _ = p1.Equals(p1).Should().BeTrue();
        _ = p1.Equals(p2).Should().BeTrue();
        _ = (p1 == p1).Should().BeTrue();
        _ = (p1 == p2).Should().BeTrue();
        _ = (p1 != p1).Should().BeFalse();
        _ = (p1 != p2).Should().BeFalse();
        _ = (p1.GetHashCode() == p2.GetHashCode()).Should().BeTrue();
    }

    [Fact]
    public void Equals002()
    {
        var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);
        var p2 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now.AddDays(10d), Sitemap.Page.Frequency.Monthly, 0.2);

        _ = p1.Equals(p1).Should().BeTrue();
        _ = p1.Equals(p2).Should().BeTrue();
        _ = (p1 == p1).Should().BeTrue();
        _ = (p1 == p2).Should().BeTrue();
        _ = (p1 != p1).Should().BeFalse();
        _ = (p1 != p2).Should().BeFalse();
        _ = (p1.GetHashCode() == p2.GetHashCode()).Should().BeTrue();
    }

    [Fact]
    public void Equals003()
    {
        var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);
        var p2 = new Sitemap.Page(new Uri("http://www.microsoft.com/sitemap.aspx"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);

        _ = p1.Equals(p1).Should().BeTrue();
        _ = p1.Equals(p2).Should().BeFalse();
        _ = (p1 == p1).Should().BeTrue();
        _ = (p1 == p2).Should().BeFalse();
        _ = (p1 != p1).Should().BeFalse();
        _ = (p1 != p2).Should().BeTrue();
        _ = (p1.GetHashCode() != p2.GetHashCode()).Should().BeTrue();
    }

    [Fact]
    public void Equals004()
    {
        var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);

        _ = p1.Equals(null).Should().BeFalse();
        _ = (p1 == null).Should().BeFalse();
        _ = (p1 != null).Should().BeTrue();
    }

    [Fact]
    public void Equals005()
    {
        var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);

        var pages = new System.Collections.Generic.HashSet<Sitemap.Page>();

        _ = pages.Add(p1).Should().BeTrue();
        _ = pages.Add(p1).Should().BeFalse();
    }

    [Fact]
    public void Equals007()
    {
        var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);
        var p2 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now.AddDays(10d), Sitemap.Page.Frequency.Monthly, 0.2);

        var pages = new System.Collections.Generic.HashSet<Sitemap.Page>();

        _ = pages.Add(p1).Should().BeTrue();
        _ = pages.Add(p2).Should().BeFalse();
    }

    [Fact]
    public void Equals008()
    {
        var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);

        var pages = new System.Collections.Generic.List<Sitemap.Page>
        {
            p1
        };

        _ = pages.Should().Contain(p1);

        pages.Add(p1);

        _ = pages.Should().Contain(p1);
    }

    [Fact]
    public void Equals009()
    {
        var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);
        var p2 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now.AddDays(10d), Sitemap.Page.Frequency.Monthly, 0.2);

        var pages = new System.Collections.Generic.List<Sitemap.Page>
        {
            p1
        };

        _ = pages.Should().Contain(p1);

        pages.Add(p2);

        _ = pages.Should().Contain(p2);
    }

    [Fact]
    public void Equals010()
    {
        var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);

        var pages = new System.Collections.Generic.Dictionary<Sitemap.Page, int>
        {
            { p1, 1 }
        };

        _ = pages.ContainsKey(p1).Should().BeTrue();

        _ = new Action(() => pages.Add(p1, 2)).Should().ThrowExactly<ArgumentException>();

        _ = pages.ContainsKey(p1).Should().BeTrue();
    }

    [Fact]
    public void Equals011()
    {
        var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);
        var p2 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now.AddDays(10d), Sitemap.Page.Frequency.Monthly, 0.2);

        var pages = new System.Collections.Generic.Dictionary<Sitemap.Page, int>
        {
            { p1, 1 }
        };

        _ = pages.ContainsKey(p1).Should().BeTrue();

        _ = new Action(() => pages.Add(p2, 2)).Should().ThrowExactly<ArgumentException>();

        _ = pages.ContainsKey(p2).Should().BeTrue();
    }

    [Fact]
    public void Equals012()
    {
        var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);
        object p2 = null;

        _ = p1.Equals(p2).Should().BeFalse();
    }

    [Fact]
    public void Equals013()
    {
        var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);
        var p2 = p1;

        _ = p1.Equals(p2).Should().BeTrue();
    }

    [Fact]
    public void Equals014()
    {
        var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);
        var s1 = new Sitemap();

        _ = p1.Equals(s1).Should().BeFalse();
    }

    [Fact]
    public void Equals015()
    {
        var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);
        object p2 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now.AddDays(10d), Sitemap.Page.Frequency.Monthly, 0.2);

        _ = p1.Equals(p2).Should().BeTrue();
    }

    [Fact]
    public void Equals016()
    {
        var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);
        object p2 = p1;

        _ = p1.Equals(p2).Should().BeTrue();
    }

    [Fact]
    public void Inequality001()
    {
        var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);
        Sitemap.Page p2 = null;

        _ = (null != p1).Should().BeTrue();
        _ = (null != p2).Should().BeFalse();
    }

    [Fact]
    public void Page001() => new Func<object>(() => new Sitemap.Page(null, null, null, null)).Should().ThrowExactly<ArgumentNullException>();

    [Fact]
    public void Page002() => new Func<object>(() => new Sitemap.Page(null, DateTime.Now, Sitemap.Page.Frequency.Always, 0.5)).Should().ThrowExactly<ArgumentNullException>();

    [Fact]
    public void Page003()
    {
        var p1 = new Sitemap.Page(new Uri("http://microsoft.com/"), null, null, null);

        _ = p1.Address.AbsoluteUri.Should().Be("http://microsoft.com/");
        _ = p1.ChangeFrequency.HasValue.Should().BeFalse();
        _ = p1.LastModified.HasValue.Should().BeFalse();
        _ = p1.Priority.HasValue.Should().BeFalse();
    }

    [Fact]
    public void Page004() => new Func<object>(() =>
                new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now,
                    Sitemap.Page.Frequency.Always, 1.5)).Should().ThrowExactly<ArgumentOutOfRangeException>();

    [Fact]
    public void Page005() => new Func<object>(() =>
                new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, -0.5)).Should().ThrowExactly<ArgumentOutOfRangeException>();
}
