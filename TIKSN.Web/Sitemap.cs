namespace TIKSN.Web
{
	public class Sitemap
	{
		private System.Collections.Generic.HashSet<Page> pages;

		public Sitemap()
		{
			this.pages = new System.Collections.Generic.HashSet<Page>();
		}

		public System.Collections.Generic.HashSet<Page> Pages
		{
			get
			{
				return this.pages;
			}
		}

		public void Write(System.Xml.XmlWriter XWriter)
		{
			XWriter.WriteStartDocument();

			XWriter.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

			foreach (Page P in this.Pages)
			{
				XWriter.WriteStartElement("url");

				XWriter.WriteStartElement("loc");
				XWriter.WriteValue(P.Address.AbsoluteUri);
				XWriter.WriteEndElement();

				if (P.LastModified.HasValue)
				{
					XWriter.WriteStartElement("lastmod");
					XWriter.WriteValue(P.LastModified.Value.ToString("yyyy-MM-dd"));
					XWriter.WriteEndElement();
				}

				if (P.ChangeFrequency.HasValue)
				{
					XWriter.WriteStartElement("changefreq");
					XWriter.WriteValue(P.ChangeFrequency.Value.ToString().ToLower());
					XWriter.WriteEndElement();
				}

				if (P.Priority.HasValue)
				{
					XWriter.WriteStartElement("priority");
					XWriter.WriteValue(P.Priority.Value);
					XWriter.WriteEndElement();
				}

				XWriter.WriteEndElement();
			}

			XWriter.WriteEndElement();

			XWriter.Flush();
		}

		public class Page : System.IEquatable<Page>
		{
			private System.Uri address;

			private decimal? priority;

			public Page(System.Uri Address, System.DateTime? LastModified, Frequency? ChangeFrequency, decimal? Priority)
			{
				this.Address = Address;
				this.LastModified = LastModified;
				this.ChangeFrequency = ChangeFrequency;
				this.Priority = Priority;
			}

			public enum Frequency { Always, Hourly, Daily, Weekly, Monthly, Yearly, Never }

			public System.Uri Address
			{
				get
				{
					return this.address;
				}
				private set
				{
					if (object.ReferenceEquals(value, null))
					{
						throw new System.ArgumentNullException("Address");
					}

					this.address = value;
				}
			}

			public Frequency? ChangeFrequency { get; private set; }

			public System.DateTime? LastModified { get; private set; }

			public decimal? Priority
			{
				get
				{
					return this.priority;
				}
				private set
				{
					if (!value.HasValue || (value.Value >= decimal.Zero && value.Value <= decimal.One))
					{
						this.priority = value;
					}
					else
					{
						throw new System.ArgumentOutOfRangeException("Valid values range from 0.0 to 1.0.");
					}
				}
			}

			public static bool operator !=(Page page1, Page page2)
			{
				return !Equals(page1, page2);
			}

			public static bool operator ==(Page page1, Page page2)
			{
				return Equals(page1, page2);
			}

			public bool Equals(Page that)
			{
				if (object.ReferenceEquals(that, null))
					return false;

				if (object.ReferenceEquals(this, that))
					return true;

				return this.Address == that.Address;
			}

			public override bool Equals(object that)
			{
				if (object.ReferenceEquals(that, null))
					return false;

				if (object.ReferenceEquals(this, that))
					return true;

				Page p = that as Page;

				if ((System.Object)p == null)
				{
					return false;
				}

				return this.Equals(p);
			}

			public override int GetHashCode()
			{
				return this.Address.GetHashCode();
			}

			private static bool Equals(Page page1, Page page2)
			{
				if (object.ReferenceEquals(page1, null))
				{
					return object.ReferenceEquals(page2, null);
				}

				return page1.Equals(page2);
			}
		}
	}
}