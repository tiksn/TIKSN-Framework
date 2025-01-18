using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
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

            (after.Amount == rate * before.Amount).ShouldBeTrue();
            (after.Currency == pair.CounterCurrency).ShouldBeTrue();
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

            (afterComversion.Amount > 0m).ShouldBeTrue();
            afterComversion.Currency.ShouldBe(pair.CounterCurrency);
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
                    await this.bank.ConvertCurrencyAsync(before, rub, DateTimeOffset.Now.AddMinutes(1d), default)).ShouldThrowAsync<ArgumentException>();
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
                    await this.bank.ConvertCurrencyAsync(before, bwp, this.timeProvider.GetUtcNow(), default)).ShouldThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task GetCurrencyPairs001()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in currencyPairs)
        {
            var reversePair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

            currencyPairs.ShouldContain(c => c == reversePair);
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

        (pairSet.Count == currencyPairs.Count).ShouldBeTrue();
    }

    [Fact]
    public async Task GetCurrencyPairs003()
        => await
                new Func<Task>(async () =>
                        await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow().AddDays(10), default)).ShouldThrowAsync<ArgumentException>();

    [Fact]
    public async Task GetCurrencyPairs004()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        pairs.ShouldContain(c => c.ToString() == "AUD/RUB");
        pairs.ShouldContain(c => c.ToString() == "AZN/RUB");
        pairs.ShouldContain(c => c.ToString() == "AMD/RUB");
        pairs.ShouldContain(c => c.ToString() == "BYN/RUB");
        pairs.ShouldContain(c => c.ToString() == "BGN/RUB");
        pairs.ShouldContain(c => c.ToString() == "BRL/RUB");
        pairs.ShouldContain(c => c.ToString() == "HUF/RUB");
        pairs.ShouldContain(c => c.ToString() == "KRW/RUB");
        pairs.ShouldContain(c => c.ToString() == "DKK/RUB");
        pairs.ShouldContain(c => c.ToString() == "USD/RUB");
        pairs.ShouldContain(c => c.ToString() == "EUR/RUB");
        pairs.ShouldContain(c => c.ToString() == "INR/RUB");
        pairs.ShouldContain(c => c.ToString() == "KZT/RUB");
        pairs.ShouldContain(c => c.ToString() == "CAD/RUB");
        pairs.ShouldContain(c => c.ToString() == "KGS/RUB");
        pairs.ShouldContain(c => c.ToString() == "CNY/RUB");
        pairs.ShouldContain(c => c.ToString() == "MDL/RUB");
        pairs.ShouldContain(c => c.ToString() == "RON/RUB");
        pairs.ShouldContain(c => c.ToString() == "TMT/RUB");
        pairs.ShouldContain(c => c.ToString() == "NOK/RUB");
        pairs.ShouldContain(c => c.ToString() == "PLN/RUB");
        pairs.ShouldContain(c => c.ToString() == "SGD/RUB");
        pairs.ShouldContain(c => c.ToString() == "TJS/RUB");
        pairs.ShouldContain(c => c.ToString() == "TRY/RUB");
        pairs.ShouldContain(c => c.ToString() == "UZS/RUB");
        pairs.ShouldContain(c => c.ToString() == "UAH/RUB");
        pairs.ShouldContain(c => c.ToString() == "GBP/RUB");
        pairs.ShouldContain(c => c.ToString() == "CZK/RUB");
        pairs.ShouldContain(c => c.ToString() == "SEK/RUB");
        pairs.ShouldContain(c => c.ToString() == "CHF/RUB");
        pairs.ShouldContain(c => c.ToString() == "ZAR/RUB");
        pairs.ShouldContain(c => c.ToString() == "JPY/RUB");

        pairs.ShouldContain(c => c.ToString() == "RUB/AUD");
        pairs.ShouldContain(c => c.ToString() == "RUB/AZN");
        pairs.ShouldContain(c => c.ToString() == "RUB/AMD");
        pairs.ShouldContain(c => c.ToString() == "RUB/BYN");
        pairs.ShouldContain(c => c.ToString() == "RUB/BGN");
        pairs.ShouldContain(c => c.ToString() == "RUB/BRL");
        pairs.ShouldContain(c => c.ToString() == "RUB/HUF");
        pairs.ShouldContain(c => c.ToString() == "RUB/KRW");
        pairs.ShouldContain(c => c.ToString() == "RUB/DKK");
        pairs.ShouldContain(c => c.ToString() == "RUB/USD");
        pairs.ShouldContain(c => c.ToString() == "RUB/EUR");
        pairs.ShouldContain(c => c.ToString() == "RUB/INR");
        pairs.ShouldContain(c => c.ToString() == "RUB/KZT");
        pairs.ShouldContain(c => c.ToString() == "RUB/CAD");
        pairs.ShouldContain(c => c.ToString() == "RUB/KGS");
        pairs.ShouldContain(c => c.ToString() == "RUB/CNY");
        pairs.ShouldContain(c => c.ToString() == "RUB/MDL");
        pairs.ShouldContain(c => c.ToString() == "RUB/RON");
        pairs.ShouldContain(c => c.ToString() == "RUB/TMT");
        pairs.ShouldContain(c => c.ToString() == "RUB/NOK");
        pairs.ShouldContain(c => c.ToString() == "RUB/PLN");
        pairs.ShouldContain(c => c.ToString() == "RUB/SGD");
        pairs.ShouldContain(c => c.ToString() == "RUB/TJS");
        pairs.ShouldContain(c => c.ToString() == "RUB/TRY");
        pairs.ShouldContain(c => c.ToString() == "RUB/UZS");
        pairs.ShouldContain(c => c.ToString() == "RUB/UAH");
        pairs.ShouldContain(c => c.ToString() == "RUB/GBP");
        pairs.ShouldContain(c => c.ToString() == "RUB/CZK");
        pairs.ShouldContain(c => c.ToString() == "RUB/SEK");
        pairs.ShouldContain(c => c.ToString() == "RUB/CHF");
        pairs.ShouldContain(c => c.ToString() == "RUB/ZAR");
        pairs.ShouldContain(c => c.ToString() == "RUB/JPY");
    }

    [Fact]
    public async Task GetCurrencyPairs005()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), default);

        pairs.ShouldContain(c => c.ToString() == "AUD/RUB");
        pairs.ShouldContain(c => c.ToString() == "BYR/RUB");
        pairs.ShouldContain(c => c.ToString() == "DKK/RUB");
        pairs.ShouldContain(c => c.ToString() == "USD/RUB");
        pairs.ShouldContain(c => c.ToString() == "EUR/RUB");
        pairs.ShouldContain(c => c.ToString() == "ISK/RUB");
        pairs.ShouldContain(c => c.ToString() == "KZT/RUB");
        pairs.ShouldContain(c => c.ToString() == "CAD/RUB");
        pairs.ShouldContain(c => c.ToString() == "CNY/RUB");
        pairs.ShouldContain(c => c.ToString() == "NOK/RUB");
        pairs.ShouldContain(c => c.ToString() == "SGD/RUB");
        pairs.ShouldContain(c => c.ToString() == "TRY/RUB");
        pairs.ShouldContain(c => c.ToString() == "UAH/RUB");
        pairs.ShouldContain(c => c.ToString() == "GBP/RUB");
        pairs.ShouldContain(c => c.ToString() == "SEK/RUB");
        pairs.ShouldContain(c => c.ToString() == "CHF/RUB");
        pairs.ShouldContain(c => c.ToString() == "JPY/RUB");

        pairs.ShouldContain(c => c.ToString() == "RUB/AUD");
        pairs.ShouldContain(c => c.ToString() == "RUB/BYR");
        pairs.ShouldContain(c => c.ToString() == "RUB/DKK");
        pairs.ShouldContain(c => c.ToString() == "RUB/USD");
        pairs.ShouldContain(c => c.ToString() == "RUB/EUR");
        pairs.ShouldContain(c => c.ToString() == "RUB/ISK");
        pairs.ShouldContain(c => c.ToString() == "RUB/KZT");
        pairs.ShouldContain(c => c.ToString() == "RUB/CAD");
        pairs.ShouldContain(c => c.ToString() == "RUB/CNY");
        pairs.ShouldContain(c => c.ToString() == "RUB/NOK");
        pairs.ShouldContain(c => c.ToString() == "RUB/SGD");
        pairs.ShouldContain(c => c.ToString() == "RUB/TRY");
        pairs.ShouldContain(c => c.ToString() == "RUB/UAH");
        pairs.ShouldContain(c => c.ToString() == "RUB/GBP");
        pairs.ShouldContain(c => c.ToString() == "RUB/SEK");
        pairs.ShouldContain(c => c.ToString() == "RUB/CHF");
        pairs.ShouldContain(c => c.ToString() == "RUB/JPY");
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
            webPairs.ShouldContain(wp => wp == pair.ToString());
        }

        foreach (var webPair in webPairs)
        {
            pairs.ShouldContain(p => p.ToString() == webPair);
        }

        (pairs.Count == webPairs.Count).ShouldBeTrue();
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

            (rate > decimal.Zero).ShouldBeTrue();
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
                        await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow().AddMinutes(1d), default)).ShouldThrowAsync<ArgumentException>();
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
                        await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default)).ShouldThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task GetExchangeRate004()
    {
        var moment = this.timeProvider.GetUtcNow().AddYears(-1);

        foreach (var pair in await this.bank.GetCurrencyPairsAsync(moment, default))
        {
            var rate = await this.bank.GetExchangeRateAsync(pair, moment, default);

            (rate > decimal.Zero).ShouldBeTrue();
        }
    }
}
