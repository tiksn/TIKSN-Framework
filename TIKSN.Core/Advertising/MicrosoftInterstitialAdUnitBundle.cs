namespace TIKSN.Advertising
{
	public class MicrosoftInterstitialAdUnitBundle : AdUnitBundle
	{
		public static readonly AdUnit MicrosoftTestModeInterstitialAdUnit = new AdUnit(
			"d25517cb-12d4-4699-8bdc-52040c712cab", "11389925", true);

		public MicrosoftInterstitialAdUnitBundle(AdUnit tablet, AdUnit mobile)
			: base(MicrosoftTestModeInterstitialAdUnit, tablet, mobile)
		{
		}
	}
}