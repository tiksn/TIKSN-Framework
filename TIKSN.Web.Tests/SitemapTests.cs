namespace TIKSN.Web.Tests
{
	[Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
	public class SitemapTests
	{
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Pages001()
		{
			Sitemap.Page p1 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now, Sitemap.Page.Frequency.Always, 0.5m);
			Sitemap.Page p2 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now, Sitemap.Page.Frequency.Always, 0.5m);

			Sitemap map = new Sitemap();

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(map.Pages.Add(p1));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(map.Pages.Add(p2));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Pages002()
		{
			Sitemap.Page p1 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now, Sitemap.Page.Frequency.Always, 0.5m);

			Sitemap map = new Sitemap();

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(map.Pages.Add(p1));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(map.Pages.Add(p1));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Pages003()
		{
			Sitemap.Page p1 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now, Sitemap.Page.Frequency.Always, 0.5m);
			Sitemap.Page p2 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now.AddDays(10d), Sitemap.Page.Frequency.Monthly, 0.2m);

			Sitemap map = new Sitemap();

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(map.Pages.Add(p1));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(map.Pages.Add(p2));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Write001()
		{
			System.Text.StringBuilder sbuilder = new System.Text.StringBuilder();
			System.Xml.XmlWriter xwriter = System.Xml.XmlWriter.Create(sbuilder);

			Sitemap map = new Sitemap();

			map.Write(xwriter);

			string XmlOutput = sbuilder.ToString();

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>(
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
				"<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\" />",
				XmlOutput);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Write002()
		{
			System.Text.StringBuilder sbuilder = new System.Text.StringBuilder();
			System.Xml.XmlWriter xwriter = System.Xml.XmlWriter.Create(sbuilder);

			Sitemap map = new Sitemap();

			map.Pages.Add(new Sitemap.Page(new System.Uri("http://microsoft.com/"), new System.DateTime(2012, 8, 3), Sitemap.Page.Frequency.Daily, 0.2m));
			map.Pages.Add(new Sitemap.Page(new System.Uri("http://microsoft.com/sitemap.aspx"), new System.DateTime(2012, 8, 3), Sitemap.Page.Frequency.Daily, 0.2m));
			map.Pages.Add(new Sitemap.Page(new System.Uri("http://microsoft.com/default.aspx"), new System.DateTime(2012, 4, 5), Sitemap.Page.Frequency.Monthly, 0.8m));

			map.Write(xwriter);

			string XmlOutput = sbuilder.ToString();

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>(
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