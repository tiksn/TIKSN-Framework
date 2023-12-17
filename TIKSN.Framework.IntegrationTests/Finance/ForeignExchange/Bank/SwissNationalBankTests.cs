using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
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
    public async Task Calculation001Async()
    {
        var atTheMoment = this.timeProvider.GetUtcNow();

        foreach (var pair in await this.bank.GetCurrencyPairsAsync(atTheMoment, default).ConfigureAwait(true))
        {
            var before = new Money(pair.BaseCurrency, 100m);

            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, atTheMoment, default).ConfigureAwait(true);

            var rate = await this.bank.GetExchangeRateAsync(pair, atTheMoment, default).ConfigureAwait(true);

            _ = (after.Amount == before.Amount * rate).Should().BeTrue();
            _ = after.Currency.Should().Be(pair.CounterCurrency);
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

            _ = (after.Amount > decimal.Zero).Should().BeTrue();
            _ = after.Currency.Should().Be(pair.CounterCurrency);
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
                new Func<Task>(async () =>
                        await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, moment, default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);
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
            new Func<Task>(async () =>
                    await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, moment, default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);
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

        _ = await new Func<Task>(async () =>
                    await this.bank.ConvertCurrencyAsync(before, bwp, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);
    }

    [Fact]
    public async Task GetCurrencyPairs001Async()
    {
        var moment = this.timeProvider.GetUtcNow().AddMinutes(10d);

        _ = await new Func<Task>(async () => await this.bank.GetCurrencyPairsAsync(moment, default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);
    }

    [Fact]
    public async Task GetCurrencyPairs002Async()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        var distinctPairs = pairs.Distinct();

        _ = (pairs.Count() == distinctPairs.Count()).Should().BeTrue();
    }

    [Fact]
    public async Task GetCurrencyPairs003Async()
    {
        var moment = this.timeProvider.GetUtcNow().AddDays(-10d);

        _ = await new Func<Task>(async () => await this.bank.GetCurrencyPairsAsync(moment, default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);
    }

    [Fact]
    public async Task GetCurrencyPairs004Async()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        foreach (var pair in pairs)
        {
            var reversed = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

            _ = pairs.Should().Contain(p => p == reversed);
        }
    }

    [Fact]
    public async Task GetCurrencyPairs005Async()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        _ = pairs.Should().Contain(p => p.ToString() == "EUR/CHF");
        _ = pairs.Should().Contain(p => p.ToString() == "USD/CHF");
        _ = pairs.Should().Contain(p => p.ToString() == "JPY/CHF");
        _ = pairs.Should().Contain(p => p.ToString() == "GBP/CHF");

        _ = pairs.Should().Contain(p => p.ToString() == "CHF/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "CHF/USD");
        _ = pairs.Should().Contain(p => p.ToString() == "CHF/JPY");
        _ = pairs.Should().Contain(p => p.ToString() == "CHF/GBP");

        _ = pairs.Count().Should().Be(8);
    }

    [Fact]
    public async Task GetExchangeRate001Async()
    {
        var atTheMoment = this.timeProvider.GetUtcNow();

        foreach (var pair in await this.bank.GetCurrencyPairsAsync(atTheMoment, default).ConfigureAwait(true))
        {
            var rate = await this.bank.GetExchangeRateAsync(pair, atTheMoment, default).ConfigureAwait(true);

            _ = (rate > decimal.Zero).Should().BeTrue();
        }
    }

    [Fact]
    public async Task GetExchangeRate002Async()
    {
        var moment = this.timeProvider.GetUtcNow().AddMinutes(10d);

        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true))
        {
            _ = await new Func<Task>(async () => await this.bank.GetExchangeRateAsync(pair, moment, default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);
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

        _ = await new Func<Task>(async () => await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);
    }

    [Fact]
    public async Task GetExchangeRate004Async()
    {
        var moment = this.timeProvider.GetUtcNow().AddDays(-10d);

        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true))
        {
            _ = await new Func<Task>(async () => await this.bank.GetExchangeRateAsync(pair, moment, default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);
        }
    }
}
