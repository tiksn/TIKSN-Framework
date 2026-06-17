using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spectre.Console;
using TIKSN.Data.Cache.Memory;
using TIKSN.DependencyInjection;
using TIKSN.Finance;
using TIKSN.Finance.Cache;

var services = new ServiceCollection();
services.AddFrameworkCore();

services.AddLogging(builder =>
{
    builder.SetMinimumLevel(LogLevel.Warning);
    builder.AddConsole();
});

services.AddMemoryCache();

// Configure the CacheDecorator Options
services.Configure<MemoryCacheDecoratorOptions>(_ => { });
services.Configure<MemoryCacheDecoratorOptions<MemoryCachedCurrencyConverterEntry>>(_ => { });

// Configure MemoryCachedCurrencyConverterOptions
services.Configure<MemoryCachedCurrencyConverterOptions>(options => options.CacheInterval = TimeSpan.FromHours(1));

// Register our Slow Converter
services.AddSingleton<SlowCurrencyConverter>();

// Register the Cached Converter that wraps the slow one
services.AddSingleton<ICurrencyConverter>(sp => new MemoryCachedCurrencyConverter(
    sp.GetRequiredService<SlowCurrencyConverter>(),
    sp.GetRequiredService<ILogger<MemoryCachedCurrencyConverter>>(),
    sp.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>(),
    sp.GetRequiredService<IOptions<MemoryCachedCurrencyConverterOptions>>(),
    sp.GetRequiredService<IOptions<MemoryCacheDecoratorOptions>>(),
    sp.GetRequiredService<IOptions<MemoryCacheDecoratorOptions<MemoryCachedCurrencyConverterEntry>>>()
));

await using var serviceProvider = services.BuildServiceProvider();

var converter = serviceProvider.GetRequiredService<ICurrencyConverter>();

AnsiConsole.MarkupLine("[bold cyan]TIKSN Caching Example[/]");
AnsiConsole.MarkupLine("[grey]--------------------------------------[/]");

var usd = new CurrencyInfo(new System.Globalization.RegionInfo("US"));
var eur = new CurrencyInfo(new System.Globalization.RegionInfo("FR"));
var pair = new CurrencyPair(usd, eur);

AnsiConsole.MarkupLine("[yellow]First call (should be slow)...[/]");
var sw1 = Stopwatch.StartNew();
var rate1 = await converter.GetExchangeRateAsync(pair, DateTimeOffset.UtcNow, CancellationToken.None).ConfigureAwait(false);
sw1.Stop();
AnsiConsole.MarkupLine($"[fuchsia]Rate:[/] {rate1} | [green]Time taken:[/] {sw1.ElapsedMilliseconds} ms");

AnsiConsole.MarkupLine("[yellow]Second call (should be instant because of caching)...[/]");
var sw2 = Stopwatch.StartNew();
var rate2 = await converter.GetExchangeRateAsync(pair, DateTimeOffset.UtcNow, CancellationToken.None).ConfigureAwait(false);
sw2.Stop();
AnsiConsole.MarkupLine($"[fuchsia]Rate:[/] {rate2} | [green]Time taken:[/] {sw2.ElapsedMilliseconds} ms");

#pragma warning disable CA1050 // Declare types in namespaces
#pragma warning disable S3903 // Types should be defined in named namespaces
#pragma warning disable RCS1110 // Declare type inside namespace
public class SlowCurrencyConverter : ICurrencyConverter
{
    public async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency, DateTimeOffset asOn, CancellationToken cancellationToken)
    {
        var rate = await GetExchangeRateAsync(new CurrencyPair(baseMoney.Currency, counterCurrency), asOn, cancellationToken).ConfigureAwait(false);
        return new Money(counterCurrency, baseMoney.Amount * rate);
    }

    public Task<IReadOnlyCollection<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn, CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyCollection<CurrencyPair>>(Array.Empty<CurrencyPair>());
    }

    public async Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn, CancellationToken cancellationToken)
    {
        // Simulate an expensive network call
        await Task.Delay(2000, cancellationToken).ConfigureAwait(false);
        return 0.85m;
    }
}
#pragma warning restore RCS1110 // Declare type inside namespace
#pragma warning restore S3903 // Types should be defined in named namespaces
#pragma warning restore CA1050 // Declare types in namespaces
