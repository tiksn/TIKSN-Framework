namespace TIKSN.Analytics.Telemetry
{
	public class PushalotMessageBuilder
	{
		public PushalotMessageBuilder()
		{
		}

		public string MessageBody { get; set; }
		public string MessageImage { get; set; }
		public bool MessageIsImportant { get; set; }
		public bool MessageIsSilent { get; set; }
		public string MessageLink { get; set; }
		public string MessageLinkTitle { get; set; }
		public string MessageSource { get; set; }
		public int? MessageTimeToLive { get; set; }
		public string MessageTitle { get; set; }

		public PushalotMessage Build()
		{
			PushalotMessageLink link = null;

			if (!string.IsNullOrEmpty(this.MessageLink))
			{
				link = new PushalotMessageLink(this.MessageLinkTitle, new System.Uri(this.MessageLink));
			}

			PushalotMessageImage image = null;

			if (!string.IsNullOrEmpty(this.MessageImage))
			{
				image = new PushalotMessageImage(new System.Uri(this.MessageImage));
			}

			var message = new PushalotMessage(this.MessageTitle, this.MessageBody, link, this.MessageIsImportant, this.MessageIsSilent, image, this.MessageSource, this.MessageTimeToLive);

			return message;
		}
	}
}