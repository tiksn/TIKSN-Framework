﻿namespace TIKSN.Web.Tests
{
	[Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
	public class SitemapIndexTests
	{
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void SitemapIndex001()
		{
			SitemapIndex SIndex = new SitemapIndex();

			SIndex.Sitemaps.Add(new System.Uri("http://microsoft.com/"), new System.DateTime(2012, 8, 4, 15, 58, 58, System.DateTimeKind.Utc));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1, SIndex.Sitemaps.Count);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Write001()
		{
			SitemapIndex SIndex = new SitemapIndex();

			System.Text.StringBuilder SBuilder = new System.Text.StringBuilder();

			System.Xml.XmlWriter XWriter = System.Xml.XmlWriter.Create(SBuilder);

			SIndex.Write(XWriter);

			string XmlOutput = SBuilder.ToString();

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>(
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
				"<sitemapindex xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\" />",
				XmlOutput);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Write002()
		{
			SitemapIndex SIndex = new SitemapIndex();

			SIndex.Sitemaps.Add(new System.Uri("http://microsoft.com/"), null);
			SIndex.Sitemaps.Add(new System.Uri("http://microsoft.com/siteindex.xml"), new System.DateTime(2012, 10, 25, 16, 45, 36, System.DateTimeKind.Utc));

			System.Text.StringBuilder SBuilder = new System.Text.StringBuilder();

			System.Xml.XmlWriter XWriter = System.Xml.XmlWriter.Create(SBuilder);

			SIndex.Write(XWriter);

			string XmlOutput = SBuilder.ToString();

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>(
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