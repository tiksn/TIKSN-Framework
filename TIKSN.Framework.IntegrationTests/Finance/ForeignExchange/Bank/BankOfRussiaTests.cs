using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;
using TIKSN.Finance;
using TIKSN.Finance.ForeignExchange.Bank;
using Xunit;

namespace TIKSN.IntegrationTests.Finance.ForeignExchange.Bank;

public class BankOfRussiaTests
{
    private readonly IBankOfRussia bank;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly TimeProvider timeProvider;

    public BankOfRussiaTests()
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        var serviceProvider = services.BuildServiceProvider();
        this.timeProvider = serviceProvider.GetRequiredService<TimeProvider>();
        this.bank = serviceProvider.GetRequiredService<IBankOfRussia>();
        this.httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
    }

    [Fact]
    public async Task Calculate001()
    {
        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default))
        {
            var before = new Money(pair.BaseCurrency, 10m);
            var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default);
            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow(), default);

            _ = (after.Amount == rate * before.Amount).Should().BeTrue();
            _ = (after.Currency == pair.CounterCurrency).Should().BeTrue();
        }
    }

    [Fact]
    public async Task ConvertCurrency001()
    {
        var moment = this.timeProvider.GetUtcNow();

        foreach (var pair in await this.bank.GetCurrencyPairsAsync(moment, default))
        {
            var beforeConversion = new Money(pair.BaseCurrency, 100m);

            var afterComversion = await this.bank.ConvertCurrencyAsync(beforeConversion, pair.CounterCurrency, moment, default);

            _ = (afterComversion.Amount > 0m).Should().BeTrue();
            _ = afterComversion.Currency.Should().Be(pair.CounterCurrency);
        }
    }

    [Fact]
    public async Task ConvertCurrency002()
    {
        var us = new RegionInfo("US");
        var ru = new RegionInfo("RU");

        var usd = new CurrencyInfo(us);
        var rub = new CurrencyInfo(ru);

        var before = new Money(usd, 100m);

        _ = await
            new Func<Task>(async () =>
                    await this.bank.ConvertCurrencyAsync(before, rub, DateTimeOffset.Now.AddMinutes(1d), default)).Should().ThrowExactlyAsync<ArgumentException>();
    }

    [Fact]
    public async Task ConvertCurrency003()
    {
        var ao = new RegionInfo("AO");
        var bw = new RegionInfo("BW");

        var aoa = new CurrencyInfo(ao);
        var bwp = new CurrencyInfo(bw);

        var before = new Money(aoa, 100m);

        _ = await
            new Func<Task>(async () =>
                    await this.bank.ConvertCurrencyAsync(before, bwp, this.timeProvider.GetUtcNow(), default)).Should().ThrowExactlyAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task GetCurrencyPairs001()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in currencyPairs)
        {
            var reversePair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

            _ = currencyPairs.Should().Contain(c => c == reversePair);
        }
    }

    [Fact]
    public async Task GetCurrencyPairs002()
    {
        var pairSet = new HashSet<CurrencyPair>();

        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in currencyPairs)
        {
            _ = pairSet.Add(pair);
        }

        _ = (pairSet.Count == currencyPairs.Count).Should().BeTrue();
    }

    [Fact]
    public async Task GetCurrencyPairs003()
        => _ = await
                new Func<Task>(async () =>
                        await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow().AddDays(10), default)).Should().ThrowExactlyAsync<ArgumentException>();

    [Fact]
    public async Task GetCurrencyPairs004()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        _ = pairs.Should().Contain(c => c.ToString() == "AUD/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "AZN/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "AMD/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "BYN/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "BGN/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "BRL/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "HUF/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "KRW/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "DKK/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "USD/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "EUR/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "INR/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "KZT/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "CAD/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "KGS/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "CNY/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "MDL/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "RON/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "TMT/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "NOK/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "PLN/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "SGD/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "TJS/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "TRY/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "UZS/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "UAH/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "GBP/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "CZK/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "SEK/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "CHF/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "ZAR/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "JPY/RUB");

        _ = pairs.Should().Contain(c => c.ToString() == "RUB/AUD");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/AZN");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/AMD");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/BYN");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/BGN");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/BRL");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/HUF");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/KRW");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/DKK");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/USD");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/EUR");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/INR");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/KZT");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/CAD");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/KGS");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/CNY");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/MDL");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/RON");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/TMT");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/NOK");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/PLN");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/SGD");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/TJS");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/TRY");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/UZS");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/UAH");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/GBP");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/CZK");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/SEK");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/CHF");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/ZAR");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/JPY");
    }

    [Fact]
    public async Task GetCurrencyPairs005()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), default);

        _ = pairs.Should().Contain(c => c.ToString() == "AUD/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "BYR/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "DKK/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "USD/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "EUR/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "ISK/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "KZT/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "CAD/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "CNY/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "NOK/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "SGD/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "TRY/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "UAH/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "GBP/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "SEK/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "CHF/RUB");
        _ = pairs.Should().Contain(c => c.ToString() == "JPY/RUB");

        _ = pairs.Should().Contain(c => c.ToString() == "RUB/AUD");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/BYR");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/DKK");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/USD");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/EUR");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/ISK");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/KZT");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/CAD");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/CNY");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/NOK");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/SGD");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/TRY");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/UAH");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/GBP");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/SEK");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/CHF");
        _ = pairs.Should().Contain(c => c.ToString() == "RUB/JPY");
    }

    [Fact]
    public async Task GetCurrencyPairs006()
    {
        var atTheMoment = this.timeProvider.GetUtcNow();

        var pairs = await this.bank.GetCurrencyPairsAsync(atTheMoment, default);

        var webUrl = string.Format(CultureInfo.InvariantCulture, "https://www.cbr.ru/scripts/XML_daily.asp?date_req={2:00}.{1:00}.{0}", atTheMoment.Year, atTheMoment.Month, atTheMoment.Day);

        var httpClient = this.httpClientFactory.CreateClient();
        var responseStream = await httpClient.GetStreamAsync(webUrl);

        var stream​Reader = new Stream​Reader(responseStream, Encoding.UTF7);

        var xdoc = XDocument.Load(stream​Reader);

        var webPairs = new List<string>();

        foreach (var codeElement in xdoc.Element("ValCurs").Elements("Valute"))
        {
            var code = codeElement.Element("CharCode").Value.Trim().ToUpper(CultureInfo.CurrentCulture);

            webPairs.Add($"{code}/RUB");
            webPairs.Add($"RUB/{code}");
        }

        foreach (var pair in pairs)
        {
            _ = webPairs.Should().Contain(wp => wp == pair.ToString());
        }

        foreach (var webPair in webPairs)
        {
            _ = pairs.Should().Contain(p => p.ToString() == webPair);
        }

        _ = (pairs.Count == webPairs.Count).Should().BeTrue();
    }

    [Fact]
    public async Task GetCurrencyPairs007()
    {
        for (var year = 1994; year <= this.timeProvider.GetUtcNow().Year; year++)
        {
            for (var month = 1; (year < this.timeProvider.GetUtcNow().Year && month <= 12) || month <= this.timeProvider.GetUtcNow().Month; month++)
            {
                var date = new DateTime(year, month, 1);

                _ = await this.bank.GetCurrencyPairsAsync(date, default);
            }
        }
    }

    [Fact]
    public async Task GetExchangeRate001()
    {
        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default))
        {
            var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default);

            _ = (rate > decimal.Zero).Should().BeTrue();
        }
    }

    [Fact]
    public async Task GetExchangeRate002()
    {
        var us = new RegionInfo("US");
        var ru = new RegionInfo("RU");

        var usd = new CurrencyInfo(us);
        var rub = new CurrencyInfo(ru);

        var pair = new CurrencyPair(rub, usd);

        _ = await
                new Func<Task>(async () =>
                        await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow().AddMinutes(1d), default)).Should().ThrowExactlyAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetExchangeRate003()
    {
        var ao = new RegionInfo("AO");
        var bw = new RegionInfo("BW");

        var aoa = new CurrencyInfo(ao);
        var bwp = new CurrencyInfo(bw);

        var pair = new CurrencyPair(bwp, aoa);

        _ = await
                new Func<Task>(async () =>
                        await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default)).Should().ThrowExactlyAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task GetExchangeRate004()
    {
        var moment = this.timeProvider.GetUtcNow().AddYears(-1);

        foreach (var pair in await this.bank.GetCurrencyPairsAsync(moment, default))
        {
            var rate = await this.bank.GetExchangeRateAsync(pair, moment, default);

            _ = (rate > decimal.Zero).Should().BeTrue();
        }
    }
}
