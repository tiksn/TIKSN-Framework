using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;
using Xunit;

namespace TIKSN.Finance.ForeignExchange.Bank.IntegrationTests;

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
    public async Task Calculate001Async()
    {
        foreach (var pair in await bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true))
        {
            var before = new Money(pair.BaseCurrency, 10m);
            var rate = await bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);
            var after = await bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

            Assert.True(after.Amount == rate * before.Amount);
            Assert.True(after.Currency == pair.CounterCurrency);
        }
    }

    [Fact]
    public async Task ConvertCurrency001Async()
    {
        var moment = this.timeProvider.GetUtcNow();

        foreach (var pair in await bank.GetCurrencyPairsAsync(moment, default).ConfigureAwait(true))
        {
            var beforeConversion = new Money(pair.BaseCurrency, 100m);

            var afterComversion = await bank.ConvertCurrencyAsync(beforeConversion, pair.CounterCurrency, moment, default).ConfigureAwait(true);

            Assert.True(afterComversion.Amount > 0m);
            Assert.Equal(pair.CounterCurrency, afterComversion.Currency);
        }
    }

    [Fact]
    public async Task ConvertCurrency002Async()
    {
        var us = new RegionInfo("US");
        var ru = new RegionInfo("RU");

        var usd = new CurrencyInfo(us);
        var rub = new CurrencyInfo(ru);

        var before = new Money(usd, 100m);

        _ = await
            Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await bank.ConvertCurrencyAsync(before, rub, DateTimeOffset.Now.AddMinutes(1d), default).ConfigureAwait(true)).ConfigureAwait(true);
    }

    [Fact]
    public async Task ConvertCurrency003Async()
    {
        var ao = new RegionInfo("AO");
        var bw = new RegionInfo("BW");

        var aoa = new CurrencyInfo(ao);
        var bwp = new CurrencyInfo(bw);

        var before = new Money(aoa, 100m);

        _ = await
            Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await bank.ConvertCurrencyAsync(before, bwp, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true)).ConfigureAwait(true);
    }

    [Fact]
    public async Task GetCurrencyPairs001Async()
    {
        var currencyPairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        foreach (var pair in currencyPairs)
        {
            var reversePair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

            Assert.Contains(currencyPairs, c => c == reversePair);
        }
    }

    [Fact]
    public async Task GetCurrencyPairs002Async()
    {
        var pairSet = new HashSet<CurrencyPair>();

        var currencyPairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        foreach (var pair in currencyPairs)
        {
            _ = pairSet.Add(pair);
        }

        Assert.True(pairSet.Count == currencyPairs.Count());
    }

    [Fact]
    public async Task GetCurrencyPairs003Async()
    {
        _ = await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow().AddDays(10), default).ConfigureAwait(true)).ConfigureAwait(true);
    }

    [Fact]
    public async Task GetCurrencyPairs004Async()
    {
        var pairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        Assert.Contains(pairs, c => c.ToString() == "AUD/RUB");
        Assert.Contains(pairs, c => c.ToString() == "AZN/RUB");
        Assert.Contains(pairs, c => c.ToString() == "AMD/RUB");
        Assert.Contains(pairs, c => c.ToString() == "BYN/RUB");
        Assert.Contains(pairs, c => c.ToString() == "BGN/RUB");
        Assert.Contains(pairs, c => c.ToString() == "BRL/RUB");
        Assert.Contains(pairs, c => c.ToString() == "HUF/RUB");
        Assert.Contains(pairs, c => c.ToString() == "KRW/RUB");
        Assert.Contains(pairs, c => c.ToString() == "DKK/RUB");
        Assert.Contains(pairs, c => c.ToString() == "USD/RUB");
        Assert.Contains(pairs, c => c.ToString() == "EUR/RUB");
        Assert.Contains(pairs, c => c.ToString() == "INR/RUB");
        Assert.Contains(pairs, c => c.ToString() == "KZT/RUB");
        Assert.Contains(pairs, c => c.ToString() == "CAD/RUB");
        Assert.Contains(pairs, c => c.ToString() == "KGS/RUB");
        Assert.Contains(pairs, c => c.ToString() == "CNY/RUB");
        Assert.Contains(pairs, c => c.ToString() == "MDL/RUB");
        Assert.Contains(pairs, c => c.ToString() == "RON/RUB");
        Assert.Contains(pairs, c => c.ToString() == "TMT/RUB");
        Assert.Contains(pairs, c => c.ToString() == "NOK/RUB");
        Assert.Contains(pairs, c => c.ToString() == "PLN/RUB");
        Assert.Contains(pairs, c => c.ToString() == "SGD/RUB");
        Assert.Contains(pairs, c => c.ToString() == "TJS/RUB");
        Assert.Contains(pairs, c => c.ToString() == "TRY/RUB");
        Assert.Contains(pairs, c => c.ToString() == "UZS/RUB");
        Assert.Contains(pairs, c => c.ToString() == "UAH/RUB");
        Assert.Contains(pairs, c => c.ToString() == "GBP/RUB");
        Assert.Contains(pairs, c => c.ToString() == "CZK/RUB");
        Assert.Contains(pairs, c => c.ToString() == "SEK/RUB");
        Assert.Contains(pairs, c => c.ToString() == "CHF/RUB");
        Assert.Contains(pairs, c => c.ToString() == "ZAR/RUB");
        Assert.Contains(pairs, c => c.ToString() == "JPY/RUB");

        Assert.Contains(pairs, c => c.ToString() == "RUB/AUD");
        Assert.Contains(pairs, c => c.ToString() == "RUB/AZN");
        Assert.Contains(pairs, c => c.ToString() == "RUB/AMD");
        Assert.Contains(pairs, c => c.ToString() == "RUB/BYN");
        Assert.Contains(pairs, c => c.ToString() == "RUB/BGN");
        Assert.Contains(pairs, c => c.ToString() == "RUB/BRL");
        Assert.Contains(pairs, c => c.ToString() == "RUB/HUF");
        Assert.Contains(pairs, c => c.ToString() == "RUB/KRW");
        Assert.Contains(pairs, c => c.ToString() == "RUB/DKK");
        Assert.Contains(pairs, c => c.ToString() == "RUB/USD");
        Assert.Contains(pairs, c => c.ToString() == "RUB/EUR");
        Assert.Contains(pairs, c => c.ToString() == "RUB/INR");
        Assert.Contains(pairs, c => c.ToString() == "RUB/KZT");
        Assert.Contains(pairs, c => c.ToString() == "RUB/CAD");
        Assert.Contains(pairs, c => c.ToString() == "RUB/KGS");
        Assert.Contains(pairs, c => c.ToString() == "RUB/CNY");
        Assert.Contains(pairs, c => c.ToString() == "RUB/MDL");
        Assert.Contains(pairs, c => c.ToString() == "RUB/RON");
        Assert.Contains(pairs, c => c.ToString() == "RUB/TMT");
        Assert.Contains(pairs, c => c.ToString() == "RUB/NOK");
        Assert.Contains(pairs, c => c.ToString() == "RUB/PLN");
        Assert.Contains(pairs, c => c.ToString() == "RUB/SGD");
        Assert.Contains(pairs, c => c.ToString() == "RUB/TJS");
        Assert.Contains(pairs, c => c.ToString() == "RUB/TRY");
        Assert.Contains(pairs, c => c.ToString() == "RUB/UZS");
        Assert.Contains(pairs, c => c.ToString() == "RUB/UAH");
        Assert.Contains(pairs, c => c.ToString() == "RUB/GBP");
        Assert.Contains(pairs, c => c.ToString() == "RUB/CZK");
        Assert.Contains(pairs, c => c.ToString() == "RUB/SEK");
        Assert.Contains(pairs, c => c.ToString() == "RUB/CHF");
        Assert.Contains(pairs, c => c.ToString() == "RUB/ZAR");
        Assert.Contains(pairs, c => c.ToString() == "RUB/JPY");
    }

    [Fact]
    public async Task GetCurrencyPairs005Async()
    {
        var pairs = await bank.GetCurrencyPairsAsync(new DateTime(2010, 01, 01), default).ConfigureAwait(true);

        Assert.Contains(pairs, c => c.ToString() == "AUD/RUB");
        Assert.Contains(pairs, c => c.ToString() == "BYR/RUB");
        Assert.Contains(pairs, c => c.ToString() == "DKK/RUB");
        Assert.Contains(pairs, c => c.ToString() == "USD/RUB");
        Assert.Contains(pairs, c => c.ToString() == "EUR/RUB");
        Assert.Contains(pairs, c => c.ToString() == "ISK/RUB");
        Assert.Contains(pairs, c => c.ToString() == "KZT/RUB");
        Assert.Contains(pairs, c => c.ToString() == "CAD/RUB");
        Assert.Contains(pairs, c => c.ToString() == "CNY/RUB");
        Assert.Contains(pairs, c => c.ToString() == "NOK/RUB");
        Assert.Contains(pairs, c => c.ToString() == "SGD/RUB");
        Assert.Contains(pairs, c => c.ToString() == "TRY/RUB");
        Assert.Contains(pairs, c => c.ToString() == "UAH/RUB");
        Assert.Contains(pairs, c => c.ToString() == "GBP/RUB");
        Assert.Contains(pairs, c => c.ToString() == "SEK/RUB");
        Assert.Contains(pairs, c => c.ToString() == "CHF/RUB");
        Assert.Contains(pairs, c => c.ToString() == "JPY/RUB");

        Assert.Contains(pairs, c => c.ToString() == "RUB/AUD");
        Assert.Contains(pairs, c => c.ToString() == "RUB/BYR");
        Assert.Contains(pairs, c => c.ToString() == "RUB/DKK");
        Assert.Contains(pairs, c => c.ToString() == "RUB/USD");
        Assert.Contains(pairs, c => c.ToString() == "RUB/EUR");
        Assert.Contains(pairs, c => c.ToString() == "RUB/ISK");
        Assert.Contains(pairs, c => c.ToString() == "RUB/KZT");
        Assert.Contains(pairs, c => c.ToString() == "RUB/CAD");
        Assert.Contains(pairs, c => c.ToString() == "RUB/CNY");
        Assert.Contains(pairs, c => c.ToString() == "RUB/NOK");
        Assert.Contains(pairs, c => c.ToString() == "RUB/SGD");
        Assert.Contains(pairs, c => c.ToString() == "RUB/TRY");
        Assert.Contains(pairs, c => c.ToString() == "RUB/UAH");
        Assert.Contains(pairs, c => c.ToString() == "RUB/GBP");
        Assert.Contains(pairs, c => c.ToString() == "RUB/SEK");
        Assert.Contains(pairs, c => c.ToString() == "RUB/CHF");
        Assert.Contains(pairs, c => c.ToString() == "RUB/JPY");
    }

    [Fact]
    public async Task GetCurrencyPairs006Async()
    {
        var atTheMoment = this.timeProvider.GetUtcNow();

        var pairs = await bank.GetCurrencyPairsAsync(atTheMoment, default).ConfigureAwait(true);

        var webUrl = string.Format("https://www.cbr.ru/scripts/XML_daily.asp?date_req={2:00}.{1:00}.{0}", atTheMoment.Year, atTheMoment.Month, atTheMoment.Day);

        var httpClient = this.httpClientFactory.CreateClient();
        var responseStream = await httpClient.GetStreamAsync(webUrl).ConfigureAwait(true);

        var stream​Reader = new Stream​Reader(responseStream, Encoding.UTF7);

        var xdoc = XDocument.Load(stream​Reader);

        var webPairs = new List<string>();

        foreach (var codeElement in xdoc.Element("ValCurs").Elements("Valute"))
        {
            var code = codeElement.Element("CharCode").Value.Trim().ToUpper();

            webPairs.Add($"{code}/RUB");
            webPairs.Add($"RUB/{code}");
        }

        foreach (var pair in pairs)
        {
            Assert.Contains(webPairs, wp => wp == pair.ToString());
        }

        foreach (var webPair in webPairs)
        {
            Assert.Contains(pairs, p => p.ToString() == webPair);
        }

        Assert.True(pairs.Count() == webPairs.Count);
    }

    [Fact]
    public async Task GetCurrencyPairs007Async()
    {
        for (var year = 1994; year <= this.timeProvider.GetUtcNow().Year; year++)
        {
            for (var month = 1; (year < this.timeProvider.GetUtcNow().Year && month <= 12) || month <= this.timeProvider.GetUtcNow().Month; month++)
            {
                var date = new DateTime(year, month, 1);

                _ = await bank.GetCurrencyPairsAsync(date, default).ConfigureAwait(true);
            }
        }
    }

    [Fact]
    public async Task GetExchangeRate001Async()
    {
        foreach (var pair in await bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true))
        {
            var rate = await bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

            Assert.True(rate > decimal.Zero);
        }
    }

    [Fact]
    public async Task GetExchangeRate002Async()
    {
        var us = new RegionInfo("US");
        var ru = new RegionInfo("RU");

        var usd = new CurrencyInfo(us);
        var rub = new CurrencyInfo(ru);

        var pair = new CurrencyPair(rub, usd);

        _ = await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow().AddMinutes(1d), default).ConfigureAwait(true)).ConfigureAwait(true);
    }

    [Fact]
    public async Task GetExchangeRate003Async()
    {
        var ao = new RegionInfo("AO");
        var bw = new RegionInfo("BW");

        var aoa = new CurrencyInfo(ao);
        var bwp = new CurrencyInfo(bw);

        var pair = new CurrencyPair(bwp, aoa);

        _ = await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true)).ConfigureAwait(true);
    }

    [Fact]
    public async Task GetExchangeRate004Async()
    {
        var moment = this.timeProvider.GetUtcNow().AddYears(-1);

        foreach (var pair in await bank.GetCurrencyPairsAsync(moment, default).ConfigureAwait(true))
        {
            var rate = await bank.GetExchangeRateAsync(pair, moment, default).ConfigureAwait(true);

            Assert.True(rate > decimal.Zero);
        }
    }
}
