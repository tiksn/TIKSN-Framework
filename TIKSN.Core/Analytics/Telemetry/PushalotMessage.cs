﻿namespace TIKSN.Analytics.Telemetry
{
	public class PushalotMessage
	{
		private const int BODY_MAXIMUM_LENGTH = 32768;
		private const int SOURCE_MAXIMUM_LENGTH = 25;
		private const int TIME_TO_LIVE_MAXIMUM_VALUE = 43200;
		private const int TIME_TO_LIVE_MINIMUM_VALUE = 0;
		private const int TITLE_MAXIMUM_LENGTH = 250;

		public PushalotMessage(string title, string body, PushalotMessageLink link, bool isImportant, bool isSilent, PushalotMessageImage image, string source, int? timeToLive)
		{
			if (!string.IsNullOrEmpty(title))
			{
				if (title.Length > TITLE_MAXIMUM_LENGTH)
				{
					throw new System.ArgumentException(string.Format("Message title length must be up to {0} characters long.", TITLE_MAXIMUM_LENGTH), "title");
				}
			}

			if (string.IsNullOrEmpty(body))
			{
				throw new System.ArgumentException("Message body cannot be null or empty string.", "body");
			}

			if (body.Length > BODY_MAXIMUM_LENGTH)
			{
				throw new System.ArgumentException(string.Format("Message body length must be up to {0} characters long.", BODY_MAXIMUM_LENGTH), "body");
			}

			if (!string.IsNullOrEmpty(source))
			{
				if (source.Length > SOURCE_MAXIMUM_LENGTH)
				{
					throw new System.ArgumentException(string.Format("Message source length must be up to {0} characters long.", SOURCE_MAXIMUM_LENGTH), "source");
				}
			}

			if (timeToLive.HasValue)
			{
				if (timeToLive.Value < TIME_TO_LIVE_MINIMUM_VALUE || timeToLive.Value > TIME_TO_LIVE_MAXIMUM_VALUE)
				{
					throw new System.ArgumentOutOfRangeException("timeToLive", string.Format("Message time-to-live must be between {0} and {1}", TIME_TO_LIVE_MINIMUM_VALUE, TIME_TO_LIVE_MAXIMUM_VALUE));
				}
			}

			this.Title = title;
			this.Body = body;
			this.IsImportant = isImportant;
			this.IsSilent = isSilent;
			this.Image = image;
			this.Source = source;
			this.TimeToLive = timeToLive;
			this.Link = link;
		}

		public string Body { get; private set; }
		public PushalotMessageImage Image { get; private set; }
		public bool IsImportant { get; private set; }
		public bool IsSilent { get; private set; }
		public PushalotMessageLink Link { get; private set; }
		public string Source { get; private set; }
		public int? TimeToLive { get; private set; }
		public string Title { get; private set; }
	}
}