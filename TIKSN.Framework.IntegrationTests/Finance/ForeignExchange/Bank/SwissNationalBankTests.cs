using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;
using Xunit;

namespace TIKSN.Finance.ForeignExchange.Bank.IntegrationTests;

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
    public async Task Calculation001Async()
    {
        var atTheMoment = this.timeProvider.GetUtcNow();

        foreach (var pair in await this.bank.GetCurrencyPairsAsync(atTheMoment, default).ConfigureAwait(true))
        {
            var before = new Money(pair.BaseCurrency, 100m);

            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, atTheMoment, default).ConfigureAwait(true);

            var rate = await this.bank.GetExchangeRateAsync(pair, atTheMoment, default).ConfigureAwait(true);

            Assert.True(after.Amount == before.Amount * rate);
            Assert.Equal(pair.CounterCurrency, after.Currency);
        }
    }

    [Fact]
    public async Task ConvertCurrency001Async()
    {
        var atTheMoment = this.timeProvider.GetUtcNow();

        foreach (var pair in await this.bank.GetCurrencyPairsAsync(atTheMoment, default).ConfigureAwait(true))
        {
            var before = new Money(pair.BaseCurrency, 100m);
            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, atTheMoment, default).ConfigureAwait(true);

            Assert.True(after.Amount > decimal.Zero);
            Assert.Equal(pair.CounterCurrency, after.Currency);
        }
    }

    [Fact]
    public async Task ConvertCurrency002Async()
    {
        var moment = this.timeProvider.GetUtcNow().AddMinutes(10d);

        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true))
        {
            var before = new Money(pair.BaseCurrency, 100m);

            _ = await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, moment, default).ConfigureAwait(true)).ConfigureAwait(true);
        }
    }

    [Fact]
    public async Task ConvertCurrency004Async()
    {
        var moment = this.timeProvider.GetUtcNow().AddDays(-10d);

        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true))
        {
            var before = new Money(pair.BaseCurrency, 100m);

            _ = await
            Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, moment, default).ConfigureAwait(true)).ConfigureAwait(true);
        }
    }

    [Fact]
    public async Task CounterCurrency003Async()
    {
        var ao = new RegionInfo("AO");
        var bw = new RegionInfo("BW");

        var aoa = new CurrencyInfo(ao);
        var bwp = new CurrencyInfo(bw);

        var before = new Money(aoa, 100m);

        _ = await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await this.bank.ConvertCurrencyAsync(before, bwp, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true)).ConfigureAwait(true);
    }

    [Fact]
    public async Task GetCurrencyPairs001Async()
    {
        var moment = this.timeProvider.GetUtcNow().AddMinutes(10d);

        _ = await Assert.ThrowsAsync<ArgumentException>(async () => await this.bank.GetCurrencyPairsAsync(moment, default).ConfigureAwait(true)).ConfigureAwait(true);
    }

    [Fact]
    public async Task GetCurrencyPairs002Async()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        var distinctPairs = pairs.Distinct();

        Assert.True(pairs.Count() == distinctPairs.Count());
    }

    [Fact]
    public async Task GetCurrencyPairs003Async()
    {
        var moment = this.timeProvider.GetUtcNow().AddDays(-10d);

        _ = await Assert.ThrowsAsync<ArgumentException>(async () => await this.bank.GetCurrencyPairsAsync(moment, default).ConfigureAwait(true)).ConfigureAwait(true);
    }

    [Fact]
    public async Task GetCurrencyPairs004Async()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        foreach (var pair in pairs)
        {
            var reversed = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

            Assert.Contains(pairs, p => p == reversed);
        }
    }

    [Fact]
    public async Task GetCurrencyPairs005Async()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        Assert.Contains(pairs, p => p.ToString() == "EUR/CHF");
        Assert.Contains(pairs, p => p.ToString() == "USD/CHF");
        Assert.Contains(pairs, p => p.ToString() == "JPY/CHF");
        Assert.Contains(pairs, p => p.ToString() == "GBP/CHF");

        Assert.Contains(pairs, p => p.ToString() == "CHF/EUR");
        Assert.Contains(pairs, p => p.ToString() == "CHF/USD");
        Assert.Contains(pairs, p => p.ToString() == "CHF/JPY");
        Assert.Contains(pairs, p => p.ToString() == "CHF/GBP");

        Assert.Equal(8, pairs.Count());
    }

    [Fact]
    public async Task GetExchangeRate001Async()
    {
        var atTheMoment = this.timeProvider.GetUtcNow();

        foreach (var pair in await this.bank.GetCurrencyPairsAsync(atTheMoment, default).ConfigureAwait(true))
        {
            var rate = await this.bank.GetExchangeRateAsync(pair, atTheMoment, default).ConfigureAwait(true);

            Assert.True(rate > decimal.Zero);
        }
    }

    [Fact]
    public async Task GetExchangeRate002Async()
    {
        var moment = this.timeProvider.GetUtcNow().AddMinutes(10d);

        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true))
        {
            _ = await Assert.ThrowsAsync<ArgumentException>(async () => await this.bank.GetExchangeRateAsync(pair, moment, default).ConfigureAwait(true)).ConfigureAwait(true);
        }
    }

    [Fact]
    public async Task GetExchangeRate003Async()
    {
        var ao = new RegionInfo("AO");
        var bw = new RegionInfo("BW");

        var aoa = new CurrencyInfo(ao);
        var bwp = new CurrencyInfo(bw);

        var pair = new CurrencyPair(aoa, bwp);

        _ = await Assert.ThrowsAsync<ArgumentException>(async () => await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true)).ConfigureAwait(true);
    }

    [Fact]
    public async Task GetExchangeRate004Async()
    {
        var moment = this.timeProvider.GetUtcNow().AddDays(-10d);

        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true))
        {
            _ = await Assert.ThrowsAsync<ArgumentException>(async () => await this.bank.GetExchangeRateAsync(pair, moment, default).ConfigureAwait(true)).ConfigureAwait(true);
        }
    }
}
