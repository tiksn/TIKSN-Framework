using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;
using Xunit;

namespace TIKSN.Finance.ForeignExchange.Bank.IntegrationTests;

public class NationalBankOfUkraineTests
{
    private readonly INationalBankOfUkraine bank;

    public NationalBankOfUkraineTests()
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        var serviceProvider = services.BuildServiceProvider();
        this.bank = serviceProvider.GetRequiredService<INationalBankOfUkraine>();
    }

    [Fact]
    public async Task ConvertCurrencyAsync001Async()
    {
        var date = new DateTimeOffset(2016, 05, 06, 0, 0, 0, TimeSpan.Zero);
        var pairs = await this.bank.GetCurrencyPairsAsync(date, default).ConfigureAwait(true);

        foreach (var pair in pairs)
        {
            var baseMoney = new Money(pair.BaseCurrency, 100);
            var convertedMoney = await this.bank.ConvertCurrencyAsync(baseMoney, pair.CounterCurrency, date, default).ConfigureAwait(true);

            Assert.Equal(pair.CounterCurrency, convertedMoney.Currency);
            Assert.True(convertedMoney.Amount > decimal.Zero);
        }
    }

    [Fact]
    public async Task GetCurrencyPairsAsync001Async()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(new DateTimeOffset(2016, 05, 06, 0, 0, 0, TimeSpan.Zero), default).ConfigureAwait(true);

        Assert.True(pairs.Any());
    }

    [Fact]
    public async Task GetExchangeRateAsync001Async()
    {
        var date = new DateTimeOffset(2016, 05, 06, 0, 0, 0, TimeSpan.Zero);
        var pairs = await this.bank.GetCurrencyPairsAsync(date, default).ConfigureAwait(true);

        foreach (var pair in pairs)
        {
            var rate = await this.bank.GetExchangeRateAsync(pair, date, default).ConfigureAwait(true);

            Assert.True(rate > decimal.Zero);
        }
    }
}
