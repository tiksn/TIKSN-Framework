namespace TIKSN.Web
{
	public class SitemapIndex
	{
		private System.Collections.Generic.Dictionary<System.Uri, System.DateTime?> sitemaps;

		public SitemapIndex()
		{
			this.sitemaps = new System.Collections.Generic.Dictionary<System.Uri, System.DateTime?>();
		}

		public System.Collections.Generic.Dictionary<System.Uri, System.DateTime?> Sitemaps
		{
			get
			{
				return this.sitemaps;
			}
		}

		public void Write(System.Xml.XmlWriter XWriter)
		{
			XWriter.WriteStartDocument();

			XWriter.WriteStartElement("sitemapindex", "http://www.sitemaps.org/schemas/sitemap/0.9");

			foreach (var SM in this.Sitemaps)
			{
				XWriter.WriteStartElement("sitemap");

				XWriter.WriteStartElement("loc");
				XWriter.WriteValue(SM.Key.AbsoluteUri);
				XWriter.WriteEndElement();

				if (SM.Value.HasValue)
				{
					XWriter.WriteStartElement("lastmod");
					XWriter.WriteValue(SM.Value.Value.ToString("yyyy-MM-dd"));
					XWriter.WriteEndElement();
				}

				XWriter.WriteEndElement();
			}

			XWriter.WriteEndElement();

			XWriter.Flush();
		}
	}
}