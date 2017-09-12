namespace TIKSN.Analytics.Telemetry.Pushalot
{
	public class PushalotMessageImage
	{
		private const int IMAGE_MAXIMUM_LENGTH = 250;

		public PushalotMessageImage(System.Uri image)
		{
			if (image == null)
			{
				throw new System.ArgumentNullException("image");
			}

			if (image.AbsoluteUri.Length > IMAGE_MAXIMUM_LENGTH)
			{
				throw new System.ArgumentException(string.Format("Message image URL  must be up to {0} characters long.", IMAGE_MAXIMUM_LENGTH), "image");
			}

			this.Image = image;
		}

		public System.Uri Image { get; private set; }
	}
}