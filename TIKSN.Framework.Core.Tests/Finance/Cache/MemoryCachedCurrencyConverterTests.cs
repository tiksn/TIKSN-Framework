using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Serilog;
using TIKSN.Data.Cache.Memory;
using TIKSN.DependencyInjection;
using TIKSN.Finance;
using TIKSN.Finance.Cache;
using Xunit;
using Xunit.Abstractions;

namespace TIKSN.Tests.Finance.Cache;

public class MemoryCachedCurrencyConverterTests
{
    private readonly ILogger<MemoryCachedCurrencyConverter> logger;
    private readonly IMemoryCache memoryCache;
    private readonly IOptions<MemoryCachedCurrencyConverterOptions> options;
    private readonly IOptions<MemoryCacheDecoratorOptions> genericOptions;
    private readonly IOptions<MemoryCacheDecoratorOptions<MemoryCachedCurrencyConverterEntry>> specificOptions;

    public MemoryCachedCurrencyConverterTests(ITestOutputHelper testOutputHelper)
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        _ = services.AddLogging(builder =>
        {
            _ = builder.AddDebug();
            var loggger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.TestOutput(testOutputHelper)
                .CreateLogger();
            _ = builder.AddSerilog(loggger);
        });
        _ = services.Configure<MemoryCachedCurrencyConverterOptions>(o => o.CacheInterval = TimeSpan.FromMinutes(5));
        var serviceProvider = services.BuildServiceProvider();

        this.logger = serviceProvider.GetRequiredService<ILogger<MemoryCachedCurrencyConverter>>();
        this.memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
        this.options = serviceProvider.GetRequiredService<IOptions<MemoryCachedCurrencyConverterOptions>>();
        this.genericOptions = serviceProvider.GetRequiredService<IOptions<MemoryCacheDecoratorOptions>>();
        this.specificOptions = serviceProvider.GetRequiredService<IOptions<MemoryCacheDecoratorOptions<MemoryCachedCurrencyConverterEntry>>>();
    }

    [Fact]
    public async Task GetCurrencyPairs001Async()
    {
        var moment1 = new DateTimeOffset(2015, 12, 1, 0, 0, 0, TimeSpan.FromHours(2));

        var originalConverter = Substitute.For<ICurrencyConverter>();
        var expectedPairs = new List<CurrencyPair> { new CurrencyPair(new CurrencyInfo("USD"), new CurrencyInfo("EUR")) };

        _ = originalConverter.GetCurrencyPairsAsync(moment1, default).Returns(expectedPairs);

        var memoryCachedCurrencyConverter = new MemoryCachedCurrencyConverter(originalConverter, this.logger, this.memoryCache, this.options, this.genericOptions, this.specificOptions);

        var actualPairs = await memoryCachedCurrencyConverter.GetCurrencyPairsAsync(moment1, default).ConfigureAwait(true);

        _ = actualPairs.Should().BeEquivalentTo(expectedPairs);
    }

    [Fact]
    public async Task GetCurrencyPairs002Async()
    {
        var moment1 = new DateTimeOffset(2015, 12, 1, 0, 0, 0, TimeSpan.FromHours(2));

        var originalConverter = Substitute.For<ICurrencyConverter>();
        var expectedPairs = new List<CurrencyPair> { new(new CurrencyInfo("USD"), new CurrencyInfo("EUR")) };

        _ = originalConverter.GetCurrencyPairsAsync(moment1, default).Returns(expectedPairs);

        var memoryCachedCurrencyConverter = new MemoryCachedCurrencyConverter(originalConverter, this.logger, this.memoryCache, this.options, this.genericOptions, this.specificOptions);

        _ = await memoryCachedCurrencyConverter.GetCurrencyPairsAsync(moment1, default).ConfigureAwait(true);

        _ = await originalConverter.Received(1).GetCurrencyPairsAsync(moment1, default).ConfigureAwait(true);
    }

    [Fact]
    public async Task GetCurrencyPairs003Async()
    {
        var moment1 = new DateTimeOffset(2015, 12, 1, 0, 0, 0, TimeSpan.FromHours(2));

        var originalConverter = Substitute.For<ICurrencyConverter>();
        var expectedPairs = new List<CurrencyPair> { new(new CurrencyInfo("USD"), new CurrencyInfo("EUR")) };

        _ = originalConverter.GetCurrencyPairsAsync(moment1, default).Returns(expectedPairs);

        var memoryCachedCurrencyConverter = new MemoryCachedCurrencyConverter(originalConverter, this.logger, this.memoryCache, this.options, this.genericOptions, this.specificOptions);

        _ = await memoryCachedCurrencyConverter.GetCurrencyPairsAsync(moment1, default).ConfigureAwait(true);
        _ = await memoryCachedCurrencyConverter.GetCurrencyPairsAsync(moment1, default).ConfigureAwait(true);

        _ = await originalConverter.Received(1).GetCurrencyPairsAsync(moment1, default).ConfigureAwait(true);
    }

    [Fact]
    public async Task GetCurrencyPairs004Async()
    {
        var moment1 = new DateTimeOffset(2015, 12, 1, 0, 0, 0, TimeSpan.FromHours(2));
        var moment2 = new DateTimeOffset(2015, 12, 2, 0, 0, 0, TimeSpan.FromHours(2));

        var originalConverter = Substitute.For<ICurrencyConverter>();
        var expectedPairs = new List<CurrencyPair> { new(new CurrencyInfo("USD"), new CurrencyInfo("EUR")) };

        _ = originalConverter.GetCurrencyPairsAsync(moment1, default).Returns(expectedPairs);
        _ = originalConverter.GetCurrencyPairsAsync(moment2, default).Returns(expectedPairs);

        var memoryCachedCurrencyConverter = new MemoryCachedCurrencyConverter(originalConverter, this.logger, this.memoryCache, this.options, this.genericOptions, this.specificOptions);

        _ = await memoryCachedCurrencyConverter.GetCurrencyPairsAsync(moment1, default).ConfigureAwait(true);
        _ = await memoryCachedCurrencyConverter.GetCurrencyPairsAsync(moment2, default).ConfigureAwait(true);

        _ = await originalConverter.Received(1).GetCurrencyPairsAsync(moment1, default).ConfigureAwait(true);
        _ = await originalConverter.Received(1).GetCurrencyPairsAsync(moment2, default).ConfigureAwait(true);
    }

    [Fact]
    public async Task GetCurrencyPairs005Async()
    {
        var moment1 = new DateTimeOffset(2015, 12, 1, 0, 0, 0, TimeSpan.FromHours(2));
        var moment2 = new DateTimeOffset(2015, 12, 2, 0, 0, 0, TimeSpan.FromHours(2));

        var originalConverter = Substitute.For<ICurrencyConverter>();
        var expectedPairs = new List<CurrencyPair> { new(new CurrencyInfo("USD"), new CurrencyInfo("EUR")) };

        _ = originalConverter.GetCurrencyPairsAsync(moment1, default).Returns(expectedPairs);
        _ = originalConverter.GetCurrencyPairsAsync(moment2, default).Returns(expectedPairs);

        var memoryCachedCurrencyConverter = new MemoryCachedCurrencyConverter(originalConverter, this.logger, this.memoryCache, this.options, this.genericOptions, this.specificOptions);

        _ = await memoryCachedCurrencyConverter.GetCurrencyPairsAsync(moment1, default).ConfigureAwait(true);
        _ = await memoryCachedCurrencyConverter.GetCurrencyPairsAsync(moment1, default).ConfigureAwait(true);
        _ = await memoryCachedCurrencyConverter.GetCurrencyPairsAsync(moment2, default).ConfigureAwait(true);

        _ = await originalConverter.Received(1).GetCurrencyPairsAsync(moment1, default).ConfigureAwait(true);
        _ = await originalConverter.Received(1).GetCurrencyPairsAsync(moment2, default).ConfigureAwait(true);
    }

    [Fact]
    public async Task GetCurrencyPairs006Async()
    {
        var moment1 = new DateTimeOffset(2015, 12, 1, 0, 0, 0, TimeSpan.FromHours(2));
        var moment11 = new DateTimeOffset(2015, 12, 1, 0, 1, 0, TimeSpan.FromHours(2));
        var moment2 = new DateTimeOffset(2015, 12, 2, 0, 0, 0, TimeSpan.FromHours(2));

        var originalConverter = Substitute.For<ICurrencyConverter>();
        var expectedPairs = new List<CurrencyPair> { new(new CurrencyInfo("USD"), new CurrencyInfo("EUR")) };

        _ = originalConverter.GetCurrencyPairsAsync(moment1, default).Returns(expectedPairs);
        _ = originalConverter.GetCurrencyPairsAsync(moment2, default).Returns(expectedPairs);

        var memoryCachedCurrencyConverter = new MemoryCachedCurrencyConverter(originalConverter, this.logger, this.memoryCache, this.options, this.genericOptions, this.specificOptions);

        _ = await memoryCachedCurrencyConverter.GetCurrencyPairsAsync(moment1, default).ConfigureAwait(true);
        _ = await memoryCachedCurrencyConverter.GetCurrencyPairsAsync(moment11, default).ConfigureAwait(true);
        _ = await memoryCachedCurrencyConverter.GetCurrencyPairsAsync(moment2, default).ConfigureAwait(true);

        _ = await originalConverter.Received(1).GetCurrencyPairsAsync(moment1, default).ConfigureAwait(true);
        _ = await originalConverter.Received(0).GetCurrencyPairsAsync(moment11, default).ConfigureAwait(true);
        _ = await originalConverter.Received(1).GetCurrencyPairsAsync(moment2, default).ConfigureAwait(true);
    }

    [Fact]
    public async Task GetExchangeRate001Async()
    {
        var exchangeRate = 10.23m;
        var pair = new CurrencyPair(new CurrencyInfo("USD"), new CurrencyInfo("EUR"));

        var moment1 = new DateTimeOffset(2015, 12, 1, 0, 0, 0, TimeSpan.FromHours(2));
        var moment11 = new DateTimeOffset(2015, 12, 1, 0, 1, 0, TimeSpan.FromHours(2));
        var moment2 = new DateTimeOffset(2015, 12, 2, 0, 0, 0, TimeSpan.FromHours(2));

        var originalConverter = Substitute.For<ICurrencyConverter>();

        _ = originalConverter.GetExchangeRateAsync(pair, moment1, default).Returns(exchangeRate);
        _ = originalConverter.GetExchangeRateAsync(pair, moment2, default).Returns(exchangeRate);

        var memoryCachedCurrencyConverter = new MemoryCachedCurrencyConverter(originalConverter, this.logger, this.memoryCache, this.options, this.genericOptions, this.specificOptions);

        _ = await memoryCachedCurrencyConverter.GetExchangeRateAsync(pair, moment1, default).ConfigureAwait(true);
        _ = await memoryCachedCurrencyConverter.GetExchangeRateAsync(pair, moment11, default).ConfigureAwait(true);
        _ = await memoryCachedCurrencyConverter.GetExchangeRateAsync(pair, moment2, default).ConfigureAwait(true);

        _ = await originalConverter.Received(1).GetExchangeRateAsync(pair, moment1, default).ConfigureAwait(true);
        _ = await originalConverter.Received(0).GetExchangeRateAsync(pair, moment11, default).ConfigureAwait(true);
        _ = await originalConverter.Received(1).GetExchangeRateAsync(pair, moment2, default).ConfigureAwait(true);
    }

    [Fact]
    public async Task GetExchangeRate002Async()
    {
        var exchangeRate = 10.23m;
        var pair = new CurrencyPair(new CurrencyInfo("USD"), new CurrencyInfo("EUR"));

        var moment1 = new DateTimeOffset(2015, 12, 1, 0, 0, 0, TimeSpan.FromHours(2));

        var originalConverter = Substitute.For<ICurrencyConverter>();

        _ = originalConverter.GetExchangeRateAsync(pair, moment1, default).Returns(exchangeRate);

        var memoryCachedCurrencyConverter = new MemoryCachedCurrencyConverter(originalConverter, this.logger, this.memoryCache, this.options, this.genericOptions, this.specificOptions);

        var actualRate = await memoryCachedCurrencyConverter.GetExchangeRateAsync(pair, moment1, default).ConfigureAwait(true);

        _ = actualRate.Should().Be(exchangeRate);
    }
}
