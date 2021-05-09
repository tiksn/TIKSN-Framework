using System;

namespace TIKSN.Analytics.Telemetry.Pushalot
{
    public class PushalotMessageLink
    {
        private const int LINK_MAXIMUM_LENGTH = 1000;
        private const int TITLE_MAXIMUM_LENGTH = 100;

        public PushalotMessageLink(string title, Uri link)
        {
            if (!string.IsNullOrEmpty(title))
            {
                if (title.Length > TITLE_MAXIMUM_LENGTH)
                {
                    throw new ArgumentException(
                        string.Format("Message link title must be up to {0} characters long.", TITLE_MAXIMUM_LENGTH),
                        "title");
                }
            }

            if (link == null)
            {
                throw new ArgumentNullException("link");
            }

            if (link.AbsoluteUri.Length > LINK_MAXIMUM_LENGTH)
            {
                throw new ArgumentException(
                    string.Format("Message link  must be up to {0} characters long.", LINK_MAXIMUM_LENGTH), "link");
            }

            this.Title = title;
            this.Link = link;
        }

        public Uri Link { get; }
        public string Title { get; }
    }
}
