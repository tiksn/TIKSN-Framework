using FluentAssertions;
using Xunit;

namespace TIKSN.Advertising.Tests
{
    public class MicrosoftBannerAdUnitBundleTests
    {
        [Fact]
        public void CreateMicrosoftBannerAdUnitBundle()
        {
            string applicationId = "b008491d-9b9b-4b78-9d5c-28e08b573268";
            string adUnitId = "d34e3fdd-1840-455d-abab-f8698b5f3318";

            var bundle = new MicrosoftBannerAdUnitBundle(applicationId, adUnitId);

            bundle.Production.ApplicationId.Should().Be(applicationId);
            bundle.Production.AdUnitId.Should().Be(adUnitId);
            bundle.Production.IsTest.Should().BeFalse();
            bundle.DesignTime.IsTest.Should().BeTrue();
        }
    }
}