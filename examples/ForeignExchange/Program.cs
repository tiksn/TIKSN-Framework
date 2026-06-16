using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using TIKSN.DependencyInjection;
using TIKSN.Finance;
using TIKSN.Finance.ForeignExchange.Bank;
using TIKSN.Globalization;

var services = new ServiceCollection();
services.AddFrameworkCore();
services.AddLogging();

await using var serviceProvider = services.BuildServiceProvider();

var currencyFactory = serviceProvider.GetRequiredService<ICurrencyFactory>();
var currencyPairFactory = serviceProvider.GetRequiredService<ICurrencyPairFactory>();
var timeProvider = serviceProvider.GetRequiredService<TimeProvider>();

// Create Currencies
var usd = currencyFactory.Create("USD");
var eur = currencyFactory.Create("EUR");
var gbp = currencyFactory.Create("GBP");
var cad = currencyFactory.Create("CAD");
var jpy = currencyFactory.Create("JPY");
var amd = currencyFactory.Create("AMD");
var sgd = currencyFactory.Create("SGD");
var pln = currencyFactory.Create("PLN");
var uah = currencyFactory.Create("UAH");
var aud = currencyFactory.Create("AUD");
var chf = currencyFactory.Create("CHF");

// Create Currency Pairs
var gbpUsd = currencyPairFactory.Create(gbp, usd);
var eurUsd = currencyPairFactory.Create(eur, usd);
var cadUsd = currencyPairFactory.Create(cad, usd);
var usdJpy = currencyPairFactory.Create(usd, jpy);
var amdUsd = currencyPairFactory.Create(amd, usd);
var sgdUsd = currencyPairFactory.Create(sgd, usd);
var plnUsd = currencyPairFactory.Create(pln, usd);
var uahUsd = currencyPairFactory.Create(uah, usd);
var audUsd = currencyPairFactory.Create(aud, usd);
var chfUsd = currencyPairFactory.Create(chf, usd);

AnsiConsole.WriteLine("Fetching exchange rates from various Central Banks...");
AnsiConsole.WriteLine();

var date = timeProvider.GetUtcNow();

async Task FetchAndPrintExchangeRateAsync(string bankName, ICurrencyConverter bank, CurrencyPair pair)
{
    try
    {
        var rate = await bank.GetExchangeRateAsync(pair, date, CancellationToken.None).ConfigureAwait(false);
        AnsiConsole.MarkupLine($"[bold cyan]{bankName}[/]: [yellow]{pair}[/] Rate: [green]{rate}[/] (As of [fuchsia]{date:d}[/])");
    }
    catch (Exception ex)
    {
        AnsiConsole.MarkupLine($"[bold cyan]{bankName}[/]: [red]Failed to retrieve exchange rate for {pair}. Error: {ex.Message}[/]");
    }

    var reversePair = currencyPairFactory.Reverse(pair);
    try
    {
        var reverseRate = await bank.GetExchangeRateAsync(reversePair, date, CancellationToken.None).ConfigureAwait(false);
        AnsiConsole.MarkupLine($"[bold cyan]{bankName}[/]: [yellow]{reversePair}[/] Rate: [green]{reverseRate}[/] (As of [fuchsia]{date:d}[/])");
    }
    catch (Exception ex)
    {
        AnsiConsole.MarkupLine($"[bold cyan]{bankName}[/]: [red]Failed to retrieve exchange rate for {reversePair}. Error: {ex.Message}[/]");
    }
}

await FetchAndPrintExchangeRateAsync("Bank of Canada", serviceProvider.GetRequiredService<IBankOfCanada>(), cadUsd).ConfigureAwait(false);
await FetchAndPrintExchangeRateAsync("Bank of England", serviceProvider.GetRequiredService<IBankOfEngland>(), gbpUsd).ConfigureAwait(false);
// For Bank of Japan, it usually provides rates of foreign currencies against JPY
await FetchAndPrintExchangeRateAsync("Bank of Japan", serviceProvider.GetRequiredService<IBankOfJapan>(), usdJpy).ConfigureAwait(false);
await FetchAndPrintExchangeRateAsync("Central Bank of Armenia", serviceProvider.GetRequiredService<ICentralBankOfArmenia>(), amdUsd).ConfigureAwait(false);
await FetchAndPrintExchangeRateAsync("European Central Bank", serviceProvider.GetRequiredService<IEuropeanCentralBank>(), eurUsd).ConfigureAwait(false);
await FetchAndPrintExchangeRateAsync("Federal Reserve System", serviceProvider.GetRequiredService<IFederalReserveSystem>(), eurUsd).ConfigureAwait(false);
await FetchAndPrintExchangeRateAsync("Monetary Authority of Singapore", serviceProvider.GetRequiredService<IMonetaryAuthorityOfSingapore>(), sgdUsd).ConfigureAwait(false);
await FetchAndPrintExchangeRateAsync("National Bank of Poland", serviceProvider.GetRequiredService<INationalBankOfPoland>(), plnUsd).ConfigureAwait(false);
await FetchAndPrintExchangeRateAsync("National Bank of Ukraine", serviceProvider.GetRequiredService<INationalBankOfUkraine>(), uahUsd).ConfigureAwait(false);
await FetchAndPrintExchangeRateAsync("Reserve Bank of Australia", serviceProvider.GetRequiredService<IReserveBankOfAustralia>(), audUsd).ConfigureAwait(false);
await FetchAndPrintExchangeRateAsync("Swiss National Bank", serviceProvider.GetRequiredService<ISwissNationalBank>(), chfUsd).ConfigureAwait(false);
