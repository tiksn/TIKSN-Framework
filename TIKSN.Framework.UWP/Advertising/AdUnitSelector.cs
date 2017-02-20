using System.Diagnostics;
using Template10.Utils;

namespace TIKSN.Advertising
{
	public class AdUnitSelector : IAdUnitSelector
	{
		public AdUnit Select(AdUnitBundle adUnitBundle)
		{
			var deviceDispositions = DeviceUtils.Current().DeviceDisposition();

			if (Debugger.IsAttached)
				return adUnitBundle.DesignTime;

			switch (deviceDispositions)
			{
				case DeviceUtils.DeviceDispositions.Virtual:
				case DeviceUtils.DeviceDispositions.IoT:
					return adUnitBundle.DesignTime;

				case DeviceUtils.DeviceDispositions.Phone:
				case DeviceUtils.DeviceDispositions.Mobile:
					return adUnitBundle.Mobile;

				default:
					return adUnitBundle.Tablet;
			}
		}
	}
}