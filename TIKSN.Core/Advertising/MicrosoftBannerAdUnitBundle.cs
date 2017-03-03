﻿namespace TIKSN.Advertising
{
	public class MicrosoftBannerAdUnitBundle : AdUnitBundle
	{
		public static readonly AdUnit MicrosoftTestModeBannerAdUnit = new AdUnit(AdProviders.Microsoft,
			"3f83fe91-d6be-434d-a0ae-7351c5a997f1",
			"10865270", true);

		public MicrosoftBannerAdUnitBundle(AdUnit tablet, AdUnit mobile) : base(MicrosoftTestModeBannerAdUnit, tablet, mobile)
		{
		}
	}
}