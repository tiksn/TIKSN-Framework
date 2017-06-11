using FluentAssertions;
using Xunit;

namespace TIKSN.Advertising.Tests
{
	public class MicrosoftInterstitialAdUnitBundleTests
	{
		[Fact]
		public void CreateMicrosoftInterstitialAdUnitBundle()
		{
			string tabletApplicationId = "b008491d-9b9b-4b78-9d5c-28e08b573268";
			string tabletAdUnitId = "d34e3fdd-1840-455d-abab-f8698b5f3318";
			string mobileApplicationId = "6d0d7c55-0f1a-4208-a7b9-a6943c84488a";
			string mobileAdUnitId = "b7c073c3-5e40-47ae-b02b-eb2fa3f90036";

			var bundle = new MicrosoftInterstitialAdUnitBundle(tabletApplicationId, tabletAdUnitId, mobileApplicationId, mobileAdUnitId);

			bundle.Tablet.ApplicationId.Should().Be(tabletApplicationId);
			bundle.Tablet.AdUnitId.Should().Be(tabletAdUnitId);
			bundle.Tablet.IsTest.Should().BeFalse();
			bundle.Mobile.ApplicationId.Should().Be(mobileApplicationId);
			bundle.Mobile.AdUnitId.Should().Be(mobileAdUnitId);
			bundle.Mobile.IsTest.Should().BeFalse();
			bundle.DesignTime.IsTest.Should().BeTrue();
		}
	}
}
