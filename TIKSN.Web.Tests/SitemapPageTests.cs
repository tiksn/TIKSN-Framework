namespace TIKSN.Web.Tests
{
	[Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
	public class SitemapPageTests
	{
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Equality001()
		{
			Sitemap.Page p1 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now, Sitemap.Page.Frequency.Always, 0.5m);
			Sitemap.Page p2 = null;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(null == p1);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(null == p2);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Equals001()
		{
			Sitemap.Page p1 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now, Sitemap.Page.Frequency.Always, 0.5m);
			Sitemap.Page p2 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now, Sitemap.Page.Frequency.Always, 0.5m);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(p1.Equals(p1));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(p1.Equals(p2));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(p1 == p1);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(p1 == p2);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(p1 != p1);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(p1 != p2);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(p1.GetHashCode() == p2.GetHashCode());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Equals002()
		{
			Sitemap.Page p1 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now, Sitemap.Page.Frequency.Always, 0.5m);
			Sitemap.Page p2 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now.AddDays(10d), Sitemap.Page.Frequency.Monthly, 0.2m);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(p1.Equals(p1));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(p1.Equals(p2));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(p1 == p1);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(p1 == p2);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(p1 != p1);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(p1 != p2);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(p1.GetHashCode() == p2.GetHashCode());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Equals003()
		{
			Sitemap.Page p1 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now, Sitemap.Page.Frequency.Always, 0.5m);
			Sitemap.Page p2 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/sitemap.aspx"), System.DateTime.Now, Sitemap.Page.Frequency.Always, 0.5m);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(p1.Equals(p1));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(p1.Equals(p2));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(p1 == p1);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(p1 == p2);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(p1 != p1);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(p1 != p2);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(p1.GetHashCode() != p2.GetHashCode());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Equals004()
		{
			Sitemap.Page p1 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now, Sitemap.Page.Frequency.Always, 0.5m);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(p1.Equals(null));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(p1 == null);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(p1 != null);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Equals005()
		{
			Sitemap.Page p1 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now, Sitemap.Page.Frequency.Always, 0.5m);

			System.Collections.Generic.HashSet<Sitemap.Page> pages = new System.Collections.Generic.HashSet<Sitemap.Page>();

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pages.Add(p1));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(pages.Add(p1));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Equals007()
		{
			Sitemap.Page p1 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now, Sitemap.Page.Frequency.Always, 0.5m);
			Sitemap.Page p2 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now.AddDays(10d), Sitemap.Page.Frequency.Monthly, 0.2m);

			System.Collections.Generic.HashSet<Sitemap.Page> pages = new System.Collections.Generic.HashSet<Sitemap.Page>();

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pages.Add(p1));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(pages.Add(p2));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Equals008()
		{
			Sitemap.Page p1 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now, Sitemap.Page.Frequency.Always, 0.5m);

			System.Collections.Generic.List<Sitemap.Page> pages = new System.Collections.Generic.List<Sitemap.Page>();

			pages.Add(p1);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pages.Contains(p1));

			pages.Add(p1);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pages.Contains(p1));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Equals009()
		{
			Sitemap.Page p1 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now, Sitemap.Page.Frequency.Always, 0.5m);
			Sitemap.Page p2 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now.AddDays(10d), Sitemap.Page.Frequency.Monthly, 0.2m);

			System.Collections.Generic.List<Sitemap.Page> pages = new System.Collections.Generic.List<Sitemap.Page>();

			pages.Add(p1);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pages.Contains(p1));

			pages.Add(p2);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pages.Contains(p2));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Equals010()
		{
			Sitemap.Page p1 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now, Sitemap.Page.Frequency.Always, 0.5m);

			System.Collections.Generic.Dictionary<Sitemap.Page, int> pages = new System.Collections.Generic.Dictionary<Sitemap.Page, int>();

			pages.Add(p1, 1);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pages.ContainsKey(p1));

			try
			{
				pages.Add(p1, 2);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.ArgumentException)
			{
			}
			catch
			{
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pages.ContainsKey(p1));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Equals011()
		{
			Sitemap.Page p1 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now, Sitemap.Page.Frequency.Always, 0.5m);
			Sitemap.Page p2 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now.AddDays(10d), Sitemap.Page.Frequency.Monthly, 0.2m);

			System.Collections.Generic.Dictionary<Sitemap.Page, int> pages = new System.Collections.Generic.Dictionary<Sitemap.Page, int>();

			pages.Add(p1, 1);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pages.ContainsKey(p1));

			try
			{
				pages.Add(p2, 2);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.ArgumentException)
			{
			}
			catch
			{
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pages.ContainsKey(p2));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Equals012()
		{
			Sitemap.Page p1 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now, Sitemap.Page.Frequency.Always, 0.5m);
			object p2 = null;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(p1.Equals(p2));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Equals013()
		{
			Sitemap.Page p1 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now, Sitemap.Page.Frequency.Always, 0.5m);
			Sitemap.Page p2 = p1;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(p1.Equals(p2));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Equals014()
		{
			Sitemap.Page p1 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now, Sitemap.Page.Frequency.Always, 0.5m);
			Sitemap s1 = new Sitemap();

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(p1.Equals(s1));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Equals015()
		{
			Sitemap.Page p1 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now, Sitemap.Page.Frequency.Always, 0.5m);
			object p2 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now.AddDays(10d), Sitemap.Page.Frequency.Monthly, 0.2m);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(p1.Equals(p2));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Equals016()
		{
			Sitemap.Page p1 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now, Sitemap.Page.Frequency.Always, 0.5m);
			object p2 = p1;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(p1.Equals(p2));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Inequality001()
		{
			Sitemap.Page p1 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now, Sitemap.Page.Frequency.Always, 0.5m);
			Sitemap.Page p2 = null;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(null != p1);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(null != p2);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Page001()
		{
			try
			{
				Sitemap.Page p1 = new Sitemap.Page(null, null, null, null);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.ArgumentNullException)
			{
			}
			catch
			{
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Page002()
		{
			try
			{
				Sitemap.Page p1 = new Sitemap.Page(null, System.DateTime.Now, Sitemap.Page.Frequency.Always, 0.5m);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.ArgumentNullException)
			{
			}
			catch
			{
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Page003()
		{
			Sitemap.Page p1 = new Sitemap.Page(new System.Uri("http://microsoft.com/"), null, null, null);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("http://microsoft.com/", p1.Address.AbsoluteUri);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(p1.ChangeFrequency.HasValue);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(p1.LastModified.HasValue);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(p1.Priority.HasValue);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Page004()
		{
			try
			{
				Sitemap.Page p1 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now, Sitemap.Page.Frequency.Always, 1.5m);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.ArgumentOutOfRangeException)
			{
			}
			catch
			{
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Page005()
		{
			try
			{
				Sitemap.Page p1 = new Sitemap.Page(new System.Uri("http://www.microsoft.com/"), System.DateTime.Now, Sitemap.Page.Frequency.Always, -0.5m);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.ArgumentOutOfRangeException)
			{
			}
			catch
			{
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
		}
	}
}