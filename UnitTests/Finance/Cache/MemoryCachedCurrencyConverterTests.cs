using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TIKSN.Data.Cache;
using TIKSN.DependencyInjection.Tests;
using Xunit;
using Xunit.Abstractions;

namespace TIKSN.Finance.Cache.Tests
{
	public class MemoryCachedCurrencyConverterTests
	{
		private readonly ILogger<MemoryCachedCurrencyConverter> _logger;
		private readonly IMemoryCache _memoryCache;
		private readonly IOptions<MemoryCachedCurrencyConverterOptions> _options;
		private readonly IOptions<MemoryCacheDecoratorOptions> _genericOptions;
		private readonly IOptions<MemoryCacheDecoratorOptions<MemoryCachedCurrencyConverterEntry>> _specificOptions;

		public MemoryCachedCurrencyConverterTests(ITestOutputHelper testOutputHelper)
		{
			var compositionRoot = new TestCompositionRootSetup(testOutputHelper, configureOptions: (s, c) =>
			{
				s.Configure<MemoryCachedCurrencyConverterOptions>(o =>
				{
					o.CacheInterval = TimeSpan.FromMinutes(5);
				});
			});
			var serviceProvider = compositionRoot.CreateServiceProvider();

			_logger = serviceProvider.GetRequiredService<ILogger<MemoryCachedCurrencyConverter>>();
			_memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
			_options = serviceProvider.GetRequiredService<IOptions<MemoryCachedCurrencyConverterOptions>>();
			_genericOptions = serviceProvider.GetRequiredService<IOptions<MemoryCacheDecoratorOptions>>();
			_specificOptions = serviceProvider.GetRequiredService<IOptions<MemoryCacheDecoratorOptions<MemoryCachedCurrencyConverterEntry>>>();
		}

		[Fact]
		public async Task GetCurrencyPairs001()
		{
			var moment1 = new DateTimeOffset(2015, 12, 1, 0, 0, 0, TimeSpan.FromHours(2));

			var originalConverter = Substitute.For<ICurrencyConverter>();
			var expectedPairs = new List<CurrencyPair> { new CurrencyPair(new CurrencyInfo("USD"), new CurrencyInfo("EUR")) };

			originalConverter.GetCurrencyPairsAsync(moment1).Returns(expectedPairs);

			var memoryCachedCurrencyConverter = new MemoryCachedCurrencyConverter(originalConverter, _logger, _memoryCache, _options, _genericOptions, _specificOptions);

			var actualPairs = await memoryCachedCurrencyConverter.GetCurrencyPairsAsync(moment1);

			actualPairs.Should().BeEquivalentTo(expectedPairs);
		}

		[Fact]
		public async Task GetCurrencyPairs002()
		{
			var moment1 = new DateTimeOffset(2015, 12, 1, 0, 0, 0, TimeSpan.FromHours(2));

			var originalConverter = Substitute.For<ICurrencyConverter>();
			var expectedPairs = new List<CurrencyPair> { new CurrencyPair(new CurrencyInfo("USD"), new CurrencyInfo("EUR")) };

			originalConverter.GetCurrencyPairsAsync(moment1).Returns(expectedPairs);

			var memoryCachedCurrencyConverter = new MemoryCachedCurrencyConverter(originalConverter, _logger, _memoryCache, _options, _genericOptions, _specificOptions);

			await memoryCachedCurrencyConverter.GetCurrencyPairsAsync(moment1);

			await originalConverter.Received(1).GetCurrencyPairsAsync(moment1);
		}

		[Fact]
		public async Task GetCurrencyPairs003()
		{
			var moment1 = new DateTimeOffset(2015, 12, 1, 0, 0, 0, TimeSpan.FromHours(2));

			var originalConverter = Substitute.For<ICurrencyConverter>();
			var expectedPairs = new List<CurrencyPair> { new CurrencyPair(new CurrencyInfo("USD"), new CurrencyInfo("EUR")) };

			originalConverter.GetCurrencyPairsAsync(moment1).Returns(expectedPairs);

			var memoryCachedCurrencyConverter = new MemoryCachedCurrencyConverter(originalConverter, _logger, _memoryCache, _options, _genericOptions, _specificOptions);

			await memoryCachedCurrencyConverter.GetCurrencyPairsAsync(moment1);
			await memoryCachedCurrencyConverter.GetCurrencyPairsAsync(moment1);

			await originalConverter.Received(1).GetCurrencyPairsAsync(moment1);
		}

		[Fact]
		public async Task GetCurrencyPairs004()
		{
			var moment1 = new DateTimeOffset(2015, 12, 1, 0, 0, 0, TimeSpan.FromHours(2));
			var moment2 = new DateTimeOffset(2015, 12, 2, 0, 0, 0, TimeSpan.FromHours(2));

			var originalConverter = Substitute.For<ICurrencyConverter>();
			var expectedPairs = new List<CurrencyPair> { new CurrencyPair(new CurrencyInfo("USD"), new CurrencyInfo("EUR")) };

			originalConverter.GetCurrencyPairsAsync(moment1).Returns(expectedPairs);
			originalConverter.GetCurrencyPairsAsync(moment2).Returns(expectedPairs);

			var memoryCachedCurrencyConverter = new MemoryCachedCurrencyConverter(originalConverter, _logger, _memoryCache, _options, _genericOptions, _specificOptions);

			await memoryCachedCurrencyConverter.GetCurrencyPairsAsync(moment1);
			await memoryCachedCurrencyConverter.GetCurrencyPairsAsync(moment2);

			await originalConverter.Received(1).GetCurrencyPairsAsync(moment1);
			await originalConverter.Received(1).GetCurrencyPairsAsync(moment2);
		}

		[Fact]
		public async Task GetCurrencyPairs005()
		{
			var moment1 = new DateTimeOffset(2015, 12, 1, 0, 0, 0, TimeSpan.FromHours(2));
			var moment2 = new DateTimeOffset(2015, 12, 2, 0, 0, 0, TimeSpan.FromHours(2));

			var originalConverter = Substitute.For<ICurrencyConverter>();
			var expectedPairs = new List<CurrencyPair> { new CurrencyPair(new CurrencyInfo("USD"), new CurrencyInfo("EUR")) };

			originalConverter.GetCurrencyPairsAsync(moment1).Returns(expectedPairs);
			originalConverter.GetCurrencyPairsAsync(moment2).Returns(expectedPairs);

			var memoryCachedCurrencyConverter = new MemoryCachedCurrencyConverter(originalConverter, _logger, _memoryCache, _options, _genericOptions, _specificOptions);

			await memoryCachedCurrencyConverter.GetCurrencyPairsAsync(moment1);
			await memoryCachedCurrencyConverter.GetCurrencyPairsAsync(moment1);
			await memoryCachedCurrencyConverter.GetCurrencyPairsAsync(moment2);

			await originalConverter.Received(1).GetCurrencyPairsAsync(moment1);
			await originalConverter.Received(1).GetCurrencyPairsAsync(moment2);
		}

		[Fact]
		public async Task GetCurrencyPairs006()
		{
			var moment1 = new DateTimeOffset(2015, 12, 1, 0, 0, 0, TimeSpan.FromHours(2));
			var moment11 = new DateTimeOffset(2015, 12, 1, 0, 1, 0, TimeSpan.FromHours(2));
			var moment2 = new DateTimeOffset(2015, 12, 2, 0, 0, 0, TimeSpan.FromHours(2));

			var originalConverter = Substitute.For<ICurrencyConverter>();
			var expectedPairs = new List<CurrencyPair> { new CurrencyPair(new CurrencyInfo("USD"), new CurrencyInfo("EUR")) };

			originalConverter.GetCurrencyPairsAsync(moment1).Returns(expectedPairs);
			originalConverter.GetCurrencyPairsAsync(moment2).Returns(expectedPairs);

			var memoryCachedCurrencyConverter = new MemoryCachedCurrencyConverter(originalConverter, _logger, _memoryCache, _options, _genericOptions, _specificOptions);

			await memoryCachedCurrencyConverter.GetCurrencyPairsAsync(moment1);
			await memoryCachedCurrencyConverter.GetCurrencyPairsAsync(moment11);
			await memoryCachedCurrencyConverter.GetCurrencyPairsAsync(moment2);

			await originalConverter.Received(1).GetCurrencyPairsAsync(moment1);
			await originalConverter.Received(0).GetCurrencyPairsAsync(moment11);
			await originalConverter.Received(1).GetCurrencyPairsAsync(moment2);
		}

		[Fact]
		public async Task GetExchangeRate001()
		{
			var exchangeRate = 10.23m;
			var pair = new CurrencyPair(new CurrencyInfo("USD"), new CurrencyInfo("EUR"));

			var moment1 = new DateTimeOffset(2015, 12, 1, 0, 0, 0, TimeSpan.FromHours(2));
			var moment11 = new DateTimeOffset(2015, 12, 1, 0, 1, 0, TimeSpan.FromHours(2));
			var moment2 = new DateTimeOffset(2015, 12, 2, 0, 0, 0, TimeSpan.FromHours(2));

			var originalConverter = Substitute.For<ICurrencyConverter>();

			originalConverter.GetExchangeRateAsync(pair, moment1).Returns(exchangeRate);
			originalConverter.GetExchangeRateAsync(pair, moment2).Returns(exchangeRate);

			var memoryCachedCurrencyConverter = new MemoryCachedCurrencyConverter(originalConverter, _logger, _memoryCache, _options, _genericOptions, _specificOptions);

			await memoryCachedCurrencyConverter.GetExchangeRateAsync(pair, moment1);
			await memoryCachedCurrencyConverter.GetExchangeRateAsync(pair, moment11);
			await memoryCachedCurrencyConverter.GetExchangeRateAsync(pair, moment2);

			await originalConverter.Received(1).GetExchangeRateAsync(pair, moment1);
			await originalConverter.Received(0).GetExchangeRateAsync(pair, moment11);
			await originalConverter.Received(1).GetExchangeRateAsync(pair, moment2);
		}

		[Fact]
		public async Task GetExchangeRate002()
		{
			var exchangeRate = 10.23m;
			var pair = new CurrencyPair(new CurrencyInfo("USD"), new CurrencyInfo("EUR"));

			var moment1 = new DateTimeOffset(2015, 12, 1, 0, 0, 0, TimeSpan.FromHours(2));

			var originalConverter = Substitute.For<ICurrencyConverter>();

			originalConverter.GetExchangeRateAsync(pair, moment1).Returns(exchangeRate);

			var memoryCachedCurrencyConverter = new MemoryCachedCurrencyConverter(originalConverter, _logger, _memoryCache, _options, _genericOptions, _specificOptions);

			var actualRate = await memoryCachedCurrencyConverter.GetExchangeRateAsync(pair, moment1);

			actualRate.Should().Be(exchangeRate);
		}
	}
}
