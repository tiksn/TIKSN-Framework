using System;
using Shouldly;
using TIKSN.Web;
using Xunit;

namespace TIKSN.Tests.Web;

public class SitemapPageTests
{
    [Fact]
    public void Equality001()
    {
        var p1 = new SitemapPage(new Uri("https://www.microsoft.com/"), DateTime.Now, SitemapPage.Frequency.Always, 0.5);
        SitemapPage p2 = null;

        (null == p1).ShouldBeFalse();
        (null == p2).ShouldBeTrue();
    }

    [Fact]
    public void Equals001()
    {
        var p1 = new SitemapPage(new Uri("https://www.microsoft.com/"), DateTime.Now, SitemapPage.Frequency.Always, 0.5);
        var p2 = new SitemapPage(new Uri("https://www.microsoft.com/"), DateTime.Now, SitemapPage.Frequency.Always, 0.5);

        p1.Equals(p1).ShouldBeTrue();
        p1.Equals(p2).ShouldBeTrue();
#pragma warning disable CS1718 // Comparison made to same variable
        (p1 == p1).ShouldBeTrue();
#pragma warning restore CS1718 // Comparison made to same variable
        (p1 == p2).ShouldBeTrue();
#pragma warning disable CS1718 // Comparison made to same variable
        (p1 != p1).ShouldBeFalse();
#pragma warning restore CS1718 // Comparison made to same variable
        (p1 != p2).ShouldBeFalse();
        (p1.GetHashCode() == p2.GetHashCode()).ShouldBeTrue();
    }

    [Fact]
    public void Equals002()
    {
        var p1 = new SitemapPage(new Uri("https://www.microsoft.com/"), DateTime.Now, SitemapPage.Frequency.Always, 0.5);
        var p2 = new SitemapPage(new Uri("https://www.microsoft.com/"), DateTime.Now.AddDays(10d), SitemapPage.Frequency.Monthly, 0.2);

        p1.Equals(p1).ShouldBeTrue();
        p1.Equals(p2).ShouldBeTrue();
#pragma warning disable CS1718 // Comparison made to same variable
        (p1 == p1).ShouldBeTrue();
#pragma warning restore CS1718 // Comparison made to same variable
        (p1 == p2).ShouldBeTrue();
#pragma warning disable CS1718 // Comparison made to same variable
        (p1 != p1).ShouldBeFalse();
#pragma warning restore CS1718 // Comparison made to same variable
        (p1 != p2).ShouldBeFalse();
        (p1.GetHashCode() == p2.GetHashCode()).ShouldBeTrue();
    }

    [Fact]
    public void Equals003()
    {
        var p1 = new SitemapPage(new Uri("https://www.microsoft.com/"), DateTime.Now, SitemapPage.Frequency.Always, 0.5);
        var p2 = new SitemapPage(new Uri("https://www.microsoft.com/sitemap.aspx"), DateTime.Now, SitemapPage.Frequency.Always, 0.5);

        p1.Equals(p1).ShouldBeTrue();
        p1.Equals(p2).ShouldBeFalse();
#pragma warning disable CS1718 // Comparison made to same variable
        (p1 == p1).ShouldBeTrue();
#pragma warning restore CS1718 // Comparison made to same variable
        (p1 == p2).ShouldBeFalse();
#pragma warning disable CS1718 // Comparison made to same variable
        (p1 != p1).ShouldBeFalse();
#pragma warning restore CS1718 // Comparison made to same variable
        (p1 != p2).ShouldBeTrue();
        (p1.GetHashCode() != p2.GetHashCode()).ShouldBeTrue();
    }

    [Fact]
    public void Equals004()
    {
        var p1 = new SitemapPage(new Uri("https://www.microsoft.com/"), DateTime.Now, SitemapPage.Frequency.Always, 0.5);

        p1.Equals(null).ShouldBeFalse();
        (p1 == null).ShouldBeFalse();
        (p1 != null).ShouldBeTrue();
    }

    [Fact]
    public void Equals005()
    {
        var p1 = new SitemapPage(new Uri("https://www.microsoft.com/"), DateTime.Now, SitemapPage.Frequency.Always, 0.5);

        var pages = new System.Collections.Generic.HashSet<SitemapPage>();

        pages.Add(p1).ShouldBeTrue();
        pages.Add(p1).ShouldBeFalse();
    }

    [Fact]
    public void Equals007()
    {
        var p1 = new SitemapPage(new Uri("https://www.microsoft.com/"), DateTime.Now, SitemapPage.Frequency.Always, 0.5);
        var p2 = new SitemapPage(new Uri("https://www.microsoft.com/"), DateTime.Now.AddDays(10d), SitemapPage.Frequency.Monthly, 0.2);

        var pages = new System.Collections.Generic.HashSet<SitemapPage>();

        pages.Add(p1).ShouldBeTrue();
        pages.Add(p2).ShouldBeFalse();
    }

    [Fact]
    public void Equals008()
    {
        var p1 = new SitemapPage(new Uri("https://www.microsoft.com/"), DateTime.Now, SitemapPage.Frequency.Always, 0.5);

        var pages = new System.Collections.Generic.List<SitemapPage>
        {
            p1
        };

        pages.ShouldContain(p1);

        pages.Add(p1);

        pages.ShouldContain(p1);
    }

    [Fact]
    public void Equals009()
    {
        var p1 = new SitemapPage(new Uri("https://www.microsoft.com/"), DateTime.Now, SitemapPage.Frequency.Always, 0.5);
        var p2 = new SitemapPage(new Uri("https://www.microsoft.com/"), DateTime.Now.AddDays(10d), SitemapPage.Frequency.Monthly, 0.2);

        var pages = new System.Collections.Generic.List<SitemapPage>
        {
            p1
        };

        pages.ShouldContain(p1);

        pages.Add(p2);

        pages.ShouldContain(p2);
    }

    [Fact]
    public void Equals010()
    {
        var p1 = new SitemapPage(new Uri("https://www.microsoft.com/"), DateTime.Now, SitemapPage.Frequency.Always, 0.5);

        var pages = new System.Collections.Generic.Dictionary<SitemapPage, int>
        {
            { p1, 1 }
        };

        pages.ContainsKey(p1).ShouldBeTrue();

        _ = new Action(() => pages.Add(p1, 2)).ShouldThrow<ArgumentException>();

        pages.ContainsKey(p1).ShouldBeTrue();
    }

    [Fact]
    public void Equals011()
    {
        var p1 = new SitemapPage(new Uri("https://www.microsoft.com/"), DateTime.Now, SitemapPage.Frequency.Always, 0.5);
        var p2 = new SitemapPage(new Uri("https://www.microsoft.com/"), DateTime.Now.AddDays(10d), SitemapPage.Frequency.Monthly, 0.2);

        var pages = new System.Collections.Generic.Dictionary<SitemapPage, int>
        {
            { p1, 1 }
        };

        pages.ContainsKey(p1).ShouldBeTrue();

        _ = new Action(() => pages.Add(p2, 2)).ShouldThrow<ArgumentException>();

        pages.ContainsKey(p2).ShouldBeTrue();
    }

    [Fact]
    public void Equals012()
    {
        var p1 = new SitemapPage(new Uri("https://www.microsoft.com/"), DateTime.Now, SitemapPage.Frequency.Always, 0.5);
        object p2 = null;

        p1.Equals(p2).ShouldBeFalse();
    }

    [Fact]
    public void Equals013()
    {
        var p1 = new SitemapPage(new Uri("https://www.microsoft.com/"), DateTime.Now, SitemapPage.Frequency.Always, 0.5);
        var p2 = p1;

        p1.Equals(p2).ShouldBeTrue();
    }

    [Fact]
    public void Equals014()
    {
        var p1 = new SitemapPage(new Uri("https://www.microsoft.com/"), DateTime.Now, SitemapPage.Frequency.Always, 0.5);
        var s1 = new Sitemap();

        p1.Equals(s1).ShouldBeFalse();
    }

    [Fact]
    public void Equals015()
    {
        var p1 = new SitemapPage(new Uri("https://www.microsoft.com/"), DateTime.Now, SitemapPage.Frequency.Always, 0.5);
        object p2 = new SitemapPage(new Uri("https://www.microsoft.com/"), DateTime.Now.AddDays(10d), SitemapPage.Frequency.Monthly, 0.2);

        p1.Equals(p2).ShouldBeTrue();
    }

    [Fact]
    public void Equals016()
    {
        var p1 = new SitemapPage(new Uri("https://www.microsoft.com/"), DateTime.Now, SitemapPage.Frequency.Always, 0.5);
        object p2 = p1;

        p1.Equals(p2).ShouldBeTrue();
    }

    [Fact]
    public void Inequality001()
    {
        var p1 = new SitemapPage(new Uri("https://www.microsoft.com/"), DateTime.Now, SitemapPage.Frequency.Always, 0.5);
        SitemapPage p2 = null;

        (null != p1).ShouldBeTrue();
        (null != p2).ShouldBeFalse();
    }

    [Fact]
    public void Page001() => new Func<object>(() => new SitemapPage(null, null, null, null)).ShouldThrow<ArgumentNullException>();

    [Fact]
    public void Page002() => new Func<object>(() => new SitemapPage(null, DateTime.Now, SitemapPage.Frequency.Always, 0.5)).ShouldThrow<ArgumentNullException>();

    [Fact]
    public void Page003()
    {
        var p1 = new SitemapPage(new Uri("https://microsoft.com/"), null, null, null);

        p1.Address.AbsoluteUri.ShouldBe("https://microsoft.com/");
        p1.ChangeFrequency.HasValue.ShouldBeFalse();
        p1.LastModified.HasValue.ShouldBeFalse();
        p1.Priority.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void Page004() => new Func<object>(() =>
                new SitemapPage(new Uri("https://www.microsoft.com/"), DateTime.Now,
                    SitemapPage.Frequency.Always, 1.5)).ShouldThrow<ArgumentOutOfRangeException>();

    [Fact]
    public void Page005() => new Func<object>(() =>
                new SitemapPage(new Uri("https://www.microsoft.com/"), DateTime.Now, SitemapPage.Frequency.Always, -0.5)).ShouldThrow<ArgumentOutOfRangeException>();
}
