using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Template10.Common;
using Template10.Utils;

namespace TIKSN.Advertising
{
	public class AdUnitSelector : IAdUnitSelector
	{
		private readonly ILogger<AdUnitSelector> _logger;
		private readonly IOptions<AdUnitSelectorOptions> _options;

		public AdUnitSelector(IOptions<AdUnitSelectorOptions> options, ILogger<AdUnitSelector> logger)
		{
			_options = options;
			_logger = logger;
		}

		public AdUnit Select(AdUnitBundle adUnitBundle)
		{
			if (_options.Value.IsDebuggerSensitive && Debugger.IsAttached)
				return adUnitBundle.DesignTime;

			var windowWrapper = WindowWrapper.Current();
			if (windowWrapper == null)
			{
				_logger.LogTrace("Window wrapper is null. CurrentDeviceFamily is used for ad unit selection.");

				switch (DeviceUtils.CurrentDeviceFamily)
				{
					case DeviceUtils.DeviceFamilies.IoT:
						return adUnitBundle.DesignTime;

					case DeviceUtils.DeviceFamilies.Mobile:
						return adUnitBundle.Mobile;

					default:
						return adUnitBundle.Tablet;
				}
			}

			_logger.LogTrace("Window wrapper is not null. DeviceDisposition is used for ad unit selection.");

			var deviceDispositions = DeviceUtils.Current(windowWrapper).DeviceDisposition();

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