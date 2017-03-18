using System;

namespace TIKSN.Advertising
{
	public abstract class MicrosoftAdUnitBundle : AdUnitBundle
	{
		public MicrosoftAdUnitBundle(AdUnit designTime, string tabletApplicationId, string tabletAdUnitId, string mobileApplicationId, string mobileAdUnitId)
			: base(designTime, CreateAdUnit(tabletApplicationId, tabletAdUnitId), CreateAdUnit(mobileApplicationId, mobileAdUnitId))
		{
		}

		private static AdUnit CreateAdUnit(string applicationId, string adUnitId)
		{
			if (applicationId == null && adUnitId == null)
				return null;

			if (applicationId != null && adUnitId != null)
				return new AdUnit(AdProviders.Microsoft, applicationId, adUnitId);

			throw new ArgumentException($"Values of {nameof(applicationId)} and {nameof(adUnitId)} must either be null or have value simultaneously.");
		}
	}
}