using System;
using System.Threading.Tasks;
using Shouldly;
using TIKSN.Finance;
using Xunit;

namespace TIKSN.Tests.Finance;

public class CompositeCrossCurrencyConverterTests
{
    [Fact]
    public async Task GetCurrencyPairsAsync001()
    {
        var converter = new CompositeCrossCurrencyConverter(new AverageCurrencyConversionCompositionStrategy());

        converter.Add(CreateFixedRateCurrencyConverter("USD", "EUR", rate: 1.12m));
        converter.Add(CreateFixedRateCurrencyConverter("USD", "GBP", rate: 1.13m));
        converter.Add(CreateFixedRateCurrencyConverter("USD", "CHF", rate: 1.14m));

        var pairs = await converter.GetCurrencyPairsAsync(DateTimeOffset.Now,
            cancellationToken: TestContext.Current.CancellationToken);

        pairs.Count.ShouldBe(3);
    }

    [Fact]
    public async Task GetExchangeRateAsync001()
    {
        var converter = new CompositeCrossCurrencyConverter(new AverageCurrencyConversionCompositionStrategy());

        converter.Add(CreateFixedRateCurrencyConverter("USD", "EUR", rate: 1.12m));
        converter.Add(CreateFixedRateCurrencyConverter("USD", "GBP", rate: 1.13m));
        converter.Add(CreateFixedRateCurrencyConverter("USD", "CHF", rate: 1.14m));

        var rate = await converter.GetExchangeRateAsync(
            Helper.CurrencyPairFactory.Create("USD", "EUR"), DateTimeOffset.Now,
            cancellationToken: TestContext.Current.CancellationToken);

        rate.ShouldBe(1.12m);
    }

    private static FixedRateCurrencyConverter CreateFixedRateCurrencyConverter(
        string baseIsoCurrencySymbol,
        string counterIsoCurrencySymbol,
        decimal rate)
        => new(
            Helper.CurrencyPairFactory.Create(baseIsoCurrencySymbol, counterIsoCurrencySymbol),
            rate,
            Helper.CurrencyPairFactory);
}
