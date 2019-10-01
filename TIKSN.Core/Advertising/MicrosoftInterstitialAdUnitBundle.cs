namespace TIKSN.Advertising
{
    public class MicrosoftInterstitialAdUnitBundle : MicrosoftAdUnitBundle
    {
        public static readonly AdUnit MicrosoftTestModeInterstitialAdUnit = new AdUnit(AdProviders.Microsoft,
            "d25517cb-12d4-4699-8bdc-52040c712cab", "11389925", true);

        public MicrosoftInterstitialAdUnitBundle(string applicationId, string adUnitId)
            : base(MicrosoftTestModeInterstitialAdUnit, applicationId, adUnitId)
        {
        }
    }
}