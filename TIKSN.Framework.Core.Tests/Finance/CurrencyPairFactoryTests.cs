using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TIKSN.DependencyInjection;
using TIKSN.Finance;
using TIKSN.Globalization;
using Xunit;

namespace TIKSN.Tests.Finance;

public class CurrencyPairFactoryTests
{
    [Fact]
    public void GivenCountries_WhenCurrencyPairCreated_ThenPrincipalRegionCurrenciesShouldBeUsed()
    {
        var serviceProvider = CreateServiceProvider();
        var countryFactory = serviceProvider.GetRequiredService<ICountryFactory>();
        var currencyPairFactory = serviceProvider.GetRequiredService<ICurrencyPairFactory>();

        var pair = currencyPairFactory.Create(countryFactory.Create("US"), countryFactory.Create("GB"));

        pair.BaseCurrency.ISOCurrencySymbol.ShouldBe("USD");
        pair.CounterCurrency.ISOCurrencySymbol.ShouldBe("GBP");
    }

    [Fact]
    public void GivenCurrencyInfos_WhenCurrencyPairCreated_ThenPairShouldMatch()
    {
        var currencyPairFactory = CreateCurrencyPairFactory();
        var usd = new CurrencyInfo("USD");
        var eur = new CurrencyInfo("EUR");

        var pair = currencyPairFactory.Create(usd, eur);

        pair.BaseCurrency.ShouldBe(usd);
        pair.CounterCurrency.ShouldBe(eur);
    }

    [Fact]
    public void GivenCurrencyPair_WhenReversed_ThenPairShouldBeSwapped()
    {
        var currencyPairFactory = CreateCurrencyPairFactory();
        var pair = currencyPairFactory.Create("USD", "EUR");

        var reversed = currencyPairFactory.Reverse(pair);

        reversed.BaseCurrency.ShouldBe(pair.CounterCurrency);
        reversed.CounterCurrency.ShouldBe(pair.BaseCurrency);
    }

    [Fact]
    public void GivenInvalidIsoCurrencySymbol_WhenTryCreateCalled_ThenItShouldReturnFalse()
    {
        var currencyPairFactory = CreateCurrencyPairFactory();

        var created = currencyPairFactory.TryCreate("ZZZ", "EUR", out var pair);

        created.ShouldBeFalse();
        pair.ShouldBeNull();
    }

    [Fact]
    public void GivenInvalidIsoCurrencySymbol_WhenTryReverseCalled_ThenItShouldReturnFalse()
    {
        var currencyPairFactory = CreateCurrencyPairFactory();

        var reversed = currencyPairFactory.TryReverse("USD", "ZZZ", out var pair);

        reversed.ShouldBeFalse();
        pair.ShouldBeNull();
    }

    [Fact]
    public void GivenIsoCurrencySymbols_WhenCurrencyPairCreated_ThenPairShouldMatch()
    {
        var currencyPairFactory = CreateCurrencyPairFactory();

        var pair = currencyPairFactory.Create("USD", "EUR");

        pair.BaseCurrency.ISOCurrencySymbol.ShouldBe("USD");
        pair.CounterCurrency.ISOCurrencySymbol.ShouldBe("EUR");
    }

    [Fact]
    public void GivenIsoCurrencySymbols_WhenReversed_ThenPairShouldBeSwapped()
    {
        var currencyPairFactory = CreateCurrencyPairFactory();

        var reversed = currencyPairFactory.Reverse("USD", "EUR");

        reversed.BaseCurrency.ISOCurrencySymbol.ShouldBe("EUR");
        reversed.CounterCurrency.ISOCurrencySymbol.ShouldBe("USD");
    }

    [Fact]
    public void GivenNullCurrency_WhenTryCreateCalled_ThenItShouldReturnFalse()
    {
        var currencyPairFactory = CreateCurrencyPairFactory();
        CurrencyInfo currency = null;

        var created = currencyPairFactory.TryCreate(currency, new CurrencyInfo("EUR"), out var pair);

        created.ShouldBeFalse();
        pair.ShouldBeNull();
    }

    [Fact]
    public void GivenRegions_WhenCurrencyPairCreated_ThenPairShouldMatch()
    {
        var currencyPairFactory = CreateCurrencyPairFactory();

        var pair = currencyPairFactory.Create(new RegionInfo("US"), new RegionInfo("GB"));

        pair.BaseCurrency.ISOCurrencySymbol.ShouldBe("USD");
        pair.CounterCurrency.ISOCurrencySymbol.ShouldBe("GBP");
    }

    [Fact]
    public void GivenSameCurrencyPair_WhenCreatedTwice_ThenCachedPairShouldBeReturned()
    {
        var currencyPairFactory = CreateCurrencyPairFactory();

        var pair1 = currencyPairFactory.Create("USD", "EUR");
        var pair2 = currencyPairFactory.Create("USD", "EUR");

        pair2.ShouldBeSameAs(pair1);
    }

    [Fact]
    public void GivenSameCurrency_WhenTryCreateCalled_ThenItShouldReturnFalse()
    {
        var currencyPairFactory = CreateCurrencyPairFactory();

        var created = currencyPairFactory.TryCreate("USD", "USD", out var pair);

        created.ShouldBeFalse();
        pair.ShouldBeNull();
    }

    [Fact]
    public void GivenServiceProvider_WhenCurrencyPairFactoryResolved_ThenItShouldNotBeNull()
    {
        var currencyPairFactory = CreateServiceProvider().GetRequiredService<ICurrencyPairFactory>();

        _ = currencyPairFactory.ShouldNotBeNull();
    }

    [Fact]
    public void GivenValidCountries_WhenTryReverseCalled_ThenItShouldReturnReversedPair()
    {
        var serviceProvider = CreateServiceProvider();
        var countryFactory = serviceProvider.GetRequiredService<ICountryFactory>();
        var currencyPairFactory = serviceProvider.GetRequiredService<ICurrencyPairFactory>();

        var reversed = currencyPairFactory.TryReverse(
            countryFactory.Create("US"),
            countryFactory.Create("GB"),
            out var pair);

        reversed.ShouldBeTrue();
        _ = pair.ShouldNotBeNull();
        pair.BaseCurrency.ISOCurrencySymbol.ShouldBe("GBP");
        pair.CounterCurrency.ISOCurrencySymbol.ShouldBe("USD");
    }

    private static ICurrencyPairFactory CreateCurrencyPairFactory()
        => CreateServiceProvider().GetRequiredService<ICurrencyPairFactory>();

    private static ServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        return services.BuildServiceProvider();
    }
}
