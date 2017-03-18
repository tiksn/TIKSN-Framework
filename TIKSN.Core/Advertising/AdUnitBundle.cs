using System;

namespace TIKSN.Advertising
{
	public class AdUnitBundle
	{
		public AdUnitBundle(AdUnit designTime, AdUnit tablet, AdUnit mobile)
		{
			Tablet = tablet ?? mobile ?? designTime;
			DesignTime = designTime;
			Mobile = mobile ?? tablet ?? designTime;

			if (designTime == null)
				throw new ArgumentNullException(nameof(designTime));

			if (!designTime.IsTest)
				throw new ArgumentException($"Value of {nameof(designTime)}.{nameof(designTime.IsTest)} must be true.", nameof(designTime));

			if (!tablet.IsTest || !mobile.IsTest)
				throw new ArgumentException($"Value of {nameof(tablet)}.{nameof(tablet.IsTest)} and {nameof(mobile)}.{nameof(mobile.IsTest)} must be false.");

			if (tablet == null || mobile == null)
				throw new ArgumentException($"Arguments {nameof(tablet)} and {nameof(mobile)} cannot be null simultaneously.");
		}

		public AdUnit DesignTime { get; }

		public AdUnit Mobile { get; }

		public AdUnit Tablet { get; }
	}
}