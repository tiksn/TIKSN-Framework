namespace TIKSN.Advertising
{
    public abstract class MicrosoftAdUnitBundle : AdUnitBundle
    {
        protected MicrosoftAdUnitBundle(AdUnit designTime, string applicationId, string adUnitId)
            : base(designTime, new AdUnit(AdProviders.Microsoft, applicationId, adUnitId))
        {
        }
    }
}
