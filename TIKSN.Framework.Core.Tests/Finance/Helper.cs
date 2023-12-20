using System.Collections.Generic;
using System.Globalization;
using TIKSN.Finance;

namespace TIKSN.Tests.Finance;

public static class Helper
{
    public static CurrencyInfo SampleCurrency1 => new(new RegionInfo("US"));

    public static CurrencyInfo SampleCurrency2 => new(new RegionInfo("DE"));

    public static CurrencyInfo SampleCurrency3 => new(new RegionInfo("GB"));

    public static CurrencyPair SampleCurrencyPair1 => new(SampleCurrency1, SampleCurrency2);

    public static CurrencyPair SampleCurrencyPair2 => new(SampleCurrency1, SampleCurrency3);

    public static CurrencyPair SampleCurrencyPair3 => new(SampleCurrency2, SampleCurrency3);

    public static IEnumerable<CurrencyPair> SampleCurrencyPairs1 => new List<CurrencyPair>
            {
                SampleCurrencyPair1,
                SampleCurrencyPair2,
                SampleCurrencyPair3
            };

    public static IEnumerable<CurrencyPair> SampleCurrencyPairs2 => new List<CurrencyPair>
            {
                SampleCurrencyPair1,
                SampleCurrencyPair2
            };

    public static IEnumerable<CurrencyPair> SampleCurrencyPairs3 => new List<CurrencyPair>
            {
                SampleCurrencyPair1
            };

    public static Money SampleMoney1 => new(SampleCurrency1, 100m);

    public static decimal GetRandomForeignExchangeRate()
    {
        var rng = new System.Random();

        return (decimal)rng.NextDouble();
    }
}
