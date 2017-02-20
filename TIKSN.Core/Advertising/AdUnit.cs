namespace TIKSN.Advertising
{
	public class AdUnit
	{
		public AdUnit(string applicationId, string adUnitId, bool isTest = false)
		{
			ApplicationId = applicationId;
			AdUnitId = adUnitId;
			IsTest = isTest;
		}

		public string AdUnitId { get; }
		public string ApplicationId { get; }

		public bool IsTest { get; }
	}
}