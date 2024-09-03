using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;
using TIKSN.Finance;
using TIKSN.Finance.ForeignExchange.Bank;
using Xunit;

namespace TIKSN.IntegrationTests.Finance.ForeignExchange.Bank;

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
    public async Task ConvertCurrencyAsync001()
    {
        var date = new DateTimeOffset(2016, 05, 06, 0, 0, 0, TimeSpan.Zero);
        var pairs = await this.bank.GetCurrencyPairsAsync(date, default);

        foreach (var pair in pairs)
        {
            var baseMoney = new Money(pair.BaseCurrency, 100);
            var convertedMoney = await this.bank.ConvertCurrencyAsync(baseMoney, pair.CounterCurrency, date, default);

            _ = convertedMoney.Currency.Should().Be(pair.CounterCurrency);
            _ = (convertedMoney.Amount > decimal.Zero).Should().BeTrue();
        }
    }

    [Fact]
    public async Task GetCurrencyPairsAsync001()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(new DateTimeOffset(2016, 05, 06, 0, 0, 0, TimeSpan.Zero), default);

        _ = (pairs.Count != 0).Should().BeTrue();
    }

    [Fact]
    public async Task GetExchangeRateAsync001()
    {
        var date = new DateTimeOffset(2016, 05, 06, 0, 0, 0, TimeSpan.Zero);
        var pairs = await this.bank.GetCurrencyPairsAsync(date, default);

        foreach (var pair in pairs)
        {
            var rate = await this.bank.GetExchangeRateAsync(pair, date, default);

            _ = (rate > decimal.Zero).Should().BeTrue();
        }
    }
}
