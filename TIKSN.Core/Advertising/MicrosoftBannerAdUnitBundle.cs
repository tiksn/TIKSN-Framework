namespace TIKSN.Advertising
{
    public class MicrosoftBannerAdUnitBundle : MicrosoftAdUnitBundle
    {
        public static readonly AdUnit MicrosoftTestModeBannerAdUnit = new(AdProviders.Microsoft,
            "3f83fe91-d6be-434d-a0ae-7351c5a997f1",
            "10865270", true);

        public MicrosoftBannerAdUnitBundle(string applicationId, string adUnitId)
            : base(MicrosoftTestModeBannerAdUnit, applicationId, adUnitId)
        {
        }
    }
}
