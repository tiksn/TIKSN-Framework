using Template10.Utils;

namespace TIKSN.Advertising
{
	public class AdUnitSelector : IAdUnitSelector
	{
		public AdUnit Select(AdUnitBundle adUnitBundle)
		{
			var deviceDispositions = DeviceUtils.Current().DeviceDisposition();

			return null;
		}
	}
}