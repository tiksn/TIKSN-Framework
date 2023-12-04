namespace TIKSN.Advertising;

public class AdUnit
{
    public AdUnit(string provider, string applicationId, string adUnitId, bool isTest = false)
    {
        this.Provider = provider;
        this.ApplicationId = applicationId;
        this.AdUnitId = adUnitId;
        this.IsTest = isTest;
    }

    public string AdUnitId { get; }

    public string ApplicationId { get; }

    public bool IsTest { get; }

    public string Provider { get; }
}
