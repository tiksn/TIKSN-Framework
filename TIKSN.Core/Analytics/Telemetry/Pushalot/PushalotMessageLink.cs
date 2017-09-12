namespace TIKSN.Analytics.Telemetry.Pushalot
{
	public class PushalotMessageLink
	{
		private const int LINK_MAXIMUM_LENGTH = 1000;
		private const int TITLE_MAXIMUM_LENGTH = 100;

		public PushalotMessageLink(string title, System.Uri link)
		{
			if (!string.IsNullOrEmpty(title))
			{
				if (title.Length > TITLE_MAXIMUM_LENGTH)
				{
					throw new System.ArgumentException(string.Format("Message link title must be up to {0} characters long.", TITLE_MAXIMUM_LENGTH), "title");
				}
			}

			if (link == null)
			{
				throw new System.ArgumentNullException("link");
			}

			if (link.AbsoluteUri.Length > LINK_MAXIMUM_LENGTH)
			{
				throw new System.ArgumentException(string.Format("Message link  must be up to {0} characters long.", LINK_MAXIMUM_LENGTH), "link");
			}

			this.Title = title;
			this.Link = link;
		}

		public System.Uri Link { get; private set; }
		public string Title { get; private set; }
	}
}