namespace TIKSN.Advertising
{
    public class AdUnit
    {
        public AdUnit(string provider, string applicationId, string adUnitId, bool isTest = false)
        {
            Provider = provider;
            ApplicationId = applicationId;
            AdUnitId = adUnitId;
            IsTest = isTest;
        }

        public string AdUnitId { get; }

        public string ApplicationId { get; }

        public bool IsTest { get; }

        public string Provider { get; }
    }
}