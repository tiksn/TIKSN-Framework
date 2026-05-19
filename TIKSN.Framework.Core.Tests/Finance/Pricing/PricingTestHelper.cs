using System.Globalization;
using TIKSN.Finance;

namespace TIKSN.Tests.Finance.Pricing;

internal static class PricingTestHelper
{
    public static Money CreateMoney(decimal amount)
        => new(new CurrencyInfo(new RegionInfo("US")), amount);
}
