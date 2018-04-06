namespace TIKSN.Advertising
{
    public abstract class MicrosoftAdUnitBundle : AdUnitBundle
    {
        protected MicrosoftAdUnitBundle(AdUnit designTime, string tabletApplicationId, string tabletAdUnitId, string mobileApplicationId, string mobileAdUnitId)
            : base(designTime, new AdUnit(AdProviders.Microsoft, tabletApplicationId, tabletAdUnitId), new AdUnit(AdProviders.Microsoft, mobileApplicationId, mobileAdUnitId))
        {
        }

        protected MicrosoftAdUnitBundle(AdUnit designTime, string applicationId, string adUnitId)
            : base(designTime, new AdUnit(AdProviders.Microsoft, applicationId, adUnitId))
        {
        }
    }
}