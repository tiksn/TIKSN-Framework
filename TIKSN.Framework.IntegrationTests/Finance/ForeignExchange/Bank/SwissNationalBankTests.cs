using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TIKSN.DependencyInjection;
using TIKSN.Finance;
using TIKSN.Finance.ForeignExchange.Bank;
using Xunit;

namespace TIKSN.IntegrationTests.Finance.ForeignExchange.Bank;

public class SwissNationalBankTests
{
    private readonly ISwissNationalBank bank;
    private readonly TimeProvider timeProvider;

    public SwissNationalBankTests()
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        var serviceProvider = services.BuildServiceProvider();
        this.bank = serviceProvider.GetRequiredService<ISwissNationalBank>();
        this.timeProvider = serviceProvider.GetRequiredService<TimeProvider>();
    }

    [Fact]
    public async Task Calculation001()
    {
        var atTheMoment = this.timeProvider.GetUtcNow();

        foreach (var pair in await this.bank.GetCurrencyPairsAsync(atTheMoment, default))
        {
            var before = new Money(pair.BaseCurrency, 100m);

            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, atTheMoment, default);

            var rate = await this.bank.GetExchangeRateAsync(pair, atTheMoment, default);

            (after.Amount == before.Amount * rate).ShouldBeTrue();
            after.Currency.ShouldBe(pair.CounterCurrency);
        }
    }

    [Fact]
    public async Task ConvertCurrency001()
    {
        var atTheMoment = this.timeProvider.GetUtcNow();

        foreach (var pair in await this.bank.GetCurrencyPairsAsync(atTheMoment, default))
        {
            var before = new Money(pair.BaseCurrency, 100m);
            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, atTheMoment, default);

            (after.Amount > decimal.Zero).ShouldBeTrue();
            after.Currency.ShouldBe(pair.CounterCurrency);
        }
    }

    [Fact]
    public async Task ConvertCurrency002()
    {
        var moment = this.timeProvider.GetUtcNow().AddMinutes(10d);

        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default))
        {
            var before = new Money(pair.BaseCurrency, 100m);

            _ = await
                new Func<Task>(async () =>
                        await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, moment, default)).ShouldThrowAsync<ArgumentException>();
        }
    }

    [Fact]
    public async Task ConvertCurrency004()
    {
        var moment = this.timeProvider.GetUtcNow().AddDays(-10d);

        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default))
        {
            var before = new Money(pair.BaseCurrency, 100m);

            _ = await
            new Func<Task>(async () =>
                    await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, moment, default)).ShouldThrowAsync<ArgumentException>();
        }
    }

    [Fact]
    public async Task CounterCurrency003()
    {
        var ao = new RegionInfo("AO");
        var bw = new RegionInfo("BW");

        var aoa = new CurrencyInfo(ao);
        var bwp = new CurrencyInfo(bw);

        var before = new Money(aoa, 100m);

        _ = await new Func<Task>(async () =>
                    await this.bank.ConvertCurrencyAsync(before, bwp, this.timeProvider.GetUtcNow(), default)).ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetCurrencyPairs001()
    {
        var moment = this.timeProvider.GetUtcNow().AddMinutes(10d);

        _ = await new Func<Task>(async () => await this.bank.GetCurrencyPairsAsync(moment, default)).ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetCurrencyPairs002()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        var distinctPairs = pairs.Distinct();

        (pairs.Count == distinctPairs.Count()).ShouldBeTrue();
    }

    [Fact]
    public async Task GetCurrencyPairs003()
    {
        var moment = this.timeProvider.GetUtcNow().AddDays(-10d);

        _ = await new Func<Task>(async () => await this.bank.GetCurrencyPairsAsync(moment, default)).ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetCurrencyPairs004()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in pairs)
        {
            var reversed = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

            pairs.ShouldContain(p => p == reversed);
        }
    }

    [Fact]
    public async Task GetCurrencyPairs005()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        pairs.ShouldContain(p => p.ToString() == "EUR/CHF");
        pairs.ShouldContain(p => p.ToString() == "USD/CHF");
        pairs.ShouldContain(p => p.ToString() == "JPY/CHF");
        pairs.ShouldContain(p => p.ToString() == "GBP/CHF");

        pairs.ShouldContain(p => p.ToString() == "CHF/EUR");
        pairs.ShouldContain(p => p.ToString() == "CHF/USD");
        pairs.ShouldContain(p => p.ToString() == "CHF/JPY");
        pairs.ShouldContain(p => p.ToString() == "CHF/GBP");

        pairs.Count.ShouldBe(8);
    }

    [Fact]
    public async Task GetExchangeRate001()
    {
        var atTheMoment = this.timeProvider.GetUtcNow();

        foreach (var pair in await this.bank.GetCurrencyPairsAsync(atTheMoment, default))
        {
            var rate = await this.bank.GetExchangeRateAsync(pair, atTheMoment, default);

            (rate > decimal.Zero).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task GetExchangeRate002()
    {
        var moment = this.timeProvider.GetUtcNow().AddMinutes(10d);

        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default))
        {
            _ = await new Func<Task>(async () => await this.bank.GetExchangeRateAsync(pair, moment, default)).ShouldThrowAsync<ArgumentException>();
        }
    }

    [Fact]
    public async Task GetExchangeRate003()
    {
        var ao = new RegionInfo("AO");
        var bw = new RegionInfo("BW");

        var aoa = new CurrencyInfo(ao);
        var bwp = new CurrencyInfo(bw);

        var pair = new CurrencyPair(aoa, bwp);

        _ = await new Func<Task>(async () => await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default)).ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetExchangeRate004()
    {
        var moment = this.timeProvider.GetUtcNow().AddDays(-10d);

        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default))
        {
            _ = await new Func<Task>(async () => await this.bank.GetExchangeRateAsync(pair, moment, default)).ShouldThrowAsync<ArgumentException>();
        }
    }
}
