using System;
using Xunit;

namespace TIKSN.Web.Tests
{
    public class SitemapPageTests
    {
        [Fact]
        public void Equality001()
        {
            var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);
            Sitemap.Page p2 = null;

            Assert.False(null == p1);
            Assert.True(null == p2);
        }

        [Fact]
        public void Equals001()
        {
            var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);
            var p2 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);

            Assert.True(p1.Equals(p1));
            Assert.True(p1.Equals(p2));
            Assert.True(p1 == p1);
            Assert.True(p1 == p2);
            Assert.False(p1 != p1);
            Assert.False(p1 != p2);
            Assert.True(p1.GetHashCode() == p2.GetHashCode());
        }

        [Fact]
        public void Equals002()
        {
            var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);
            var p2 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now.AddDays(10d), Sitemap.Page.Frequency.Monthly, 0.2);

            Assert.True(p1.Equals(p1));
            Assert.True(p1.Equals(p2));
            Assert.True(p1 == p1);
            Assert.True(p1 == p2);
            Assert.False(p1 != p1);
            Assert.False(p1 != p2);
            Assert.True(p1.GetHashCode() == p2.GetHashCode());
        }

        [Fact]
        public void Equals003()
        {
            var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);
            var p2 = new Sitemap.Page(new Uri("http://www.microsoft.com/sitemap.aspx"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);

            Assert.True(p1.Equals(p1));
            Assert.False(p1.Equals(p2));
            Assert.True(p1 == p1);
            Assert.False(p1 == p2);
            Assert.False(p1 != p1);
            Assert.True(p1 != p2);
            Assert.True(p1.GetHashCode() != p2.GetHashCode());
        }

        [Fact]
        public void Equals004()
        {
            var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);

            Assert.False(p1.Equals(null));
            Assert.False(p1 == null);
            Assert.True(p1 != null);
        }

        [Fact]
        public void Equals005()
        {
            var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);

            var pages = new System.Collections.Generic.HashSet<Sitemap.Page>();

            Assert.True(pages.Add(p1));
            Assert.False(pages.Add(p1));
        }

        [Fact]
        public void Equals007()
        {
            var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);
            var p2 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now.AddDays(10d), Sitemap.Page.Frequency.Monthly, 0.2);

            var pages = new System.Collections.Generic.HashSet<Sitemap.Page>();

            Assert.True(pages.Add(p1));
            Assert.False(pages.Add(p2));
        }

        [Fact]
        public void Equals008()
        {
            var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);

            var pages = new System.Collections.Generic.List<Sitemap.Page>
            {
                p1
            };

            Assert.Contains(p1, pages);

            pages.Add(p1);

            Assert.Contains(p1, pages);
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

            Assert.Contains(p1, pages);

            pages.Add(p2);

            Assert.Contains(p2, pages);
        }

        [Fact]
        public void Equals010()
        {
            var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);

            var pages = new System.Collections.Generic.Dictionary<Sitemap.Page, int>
            {
                { p1, 1 }
            };

            Assert.True(pages.ContainsKey(p1));

            _ = Assert.Throws<ArgumentException>(() => pages.Add(p1, 2));

            Assert.True(pages.ContainsKey(p1));
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

            Assert.True(pages.ContainsKey(p1));

            _ = Assert.Throws<ArgumentException>(() => pages.Add(p2, 2));

            Assert.True(pages.ContainsKey(p2));
        }

        [Fact]
        public void Equals012()
        {
            var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);
            object p2 = null;

            Assert.False(p1.Equals(p2));
        }

        [Fact]
        public void Equals013()
        {
            var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);
            var p2 = p1;

            Assert.True(p1.Equals(p2));
        }

        [Fact]
        public void Equals014()
        {
            var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);
            var s1 = new Sitemap();

            Assert.False(p1.Equals(s1));
        }

        [Fact]
        public void Equals015()
        {
            var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);
            object p2 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now.AddDays(10d), Sitemap.Page.Frequency.Monthly, 0.2);

            Assert.True(p1.Equals(p2));
        }

        [Fact]
        public void Equals016()
        {
            var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);
            object p2 = p1;

            Assert.True(p1.Equals(p2));
        }

        [Fact]
        public void Inequality001()
        {
            var p1 = new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, 0.5);
            Sitemap.Page p2 = null;

            Assert.True(null != p1);
            Assert.False(null != p2);
        }

        [Fact]
        public void Page001() => Assert.Throws<ArgumentNullException>(() => new Sitemap.Page(null, null, null, null));

        [Fact]
        public void Page002() => Assert.Throws<ArgumentNullException>(
                () => new Sitemap.Page(null, DateTime.Now, Sitemap.Page.Frequency.Always, 0.5));

        [Fact]
        public void Page003()
        {
            var p1 = new Sitemap.Page(new Uri("http://microsoft.com/"), null, null, null);

            Assert.Equal("http://microsoft.com/", p1.Address.AbsoluteUri);
            Assert.False(p1.ChangeFrequency.HasValue);
            Assert.False(p1.LastModified.HasValue);
            Assert.False(p1.Priority.HasValue);
        }

        [Fact]
        public void Page004() => Assert.Throws<ArgumentOutOfRangeException>(
                () =>
                    new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now,
                        Sitemap.Page.Frequency.Always, 1.5));

        [Fact]
        public void Page005() => Assert.Throws<ArgumentOutOfRangeException>(
                () =>
                    new Sitemap.Page(new Uri("http://www.microsoft.com/"), DateTime.Now, Sitemap.Page.Frequency.Always, -0.5));
    }
}
