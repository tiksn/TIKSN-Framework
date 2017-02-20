namespace TIKSN.Advertising
{
	public class AdDuplexAdUnitBundle : AdUnitBundle
	{
		public AdDuplexAdUnitBundle(string applicationId, string adUnitId)
			: base(new AdUnit(applicationId, adUnitId, true), new AdUnit(applicationId, adUnitId), null)
		{
		}
	}
}