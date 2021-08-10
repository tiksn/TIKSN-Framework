namespace TIKSN.Advertising
{
    public class AdDuplexAdUnitBundle : AdUnitBundle
    {
        public AdDuplexAdUnitBundle(string applicationId, string adUnitId)
            : base(
                new AdUnit(AdProviders.AdDuplex, applicationId, adUnitId, true),
                new AdUnit(AdProviders.AdDuplex, applicationId, adUnitId))
        {
        }
    }
}
