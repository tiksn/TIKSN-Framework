﻿using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TIKSN.Data.Cache;

namespace TIKSN.Finance.Cache
{
	public class MemoryCachedCurrencyConverter : MemoryCacheDecoratorBase<MemoryCachedCurrencyConverterEntry>, ICurrencyConverter
	{
		private static readonly Type memoryCachedCurrencyConverterEntryType = typeof(MemoryCachedCurrencyConverterEntry);

		private readonly ILogger<MemoryCachedCurrencyConverter> _logger;
		private readonly IMemoryCache _memoryCache;
		private readonly IOptions<MemoryCachedCurrencyConverterOptions> _options;
		private readonly ICurrencyConverter _originalConverter;
		private readonly Guid instanceID;

		public MemoryCachedCurrencyConverter(
			ICurrencyConverter originalConverter,
			ILogger<MemoryCachedCurrencyConverter> logger,
			IMemoryCache memoryCache,
			IOptions<MemoryCachedCurrencyConverterOptions> options,
			IOptions<MemoryCacheDecoratorOptions> genericOptions,
			IOptions<MemoryCacheDecoratorOptions<MemoryCachedCurrencyConverterEntry>> specificOptions)
			: base(memoryCache, genericOptions, specificOptions)
		{
			_options = options;
			_originalConverter = originalConverter;
			_memoryCache = memoryCache;
			instanceID = Guid.NewGuid();
			_logger = logger;
		}

		public async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency, DateTimeOffset asOn)
		{
			var cacheKey = Tuple.Create(memoryCachedCurrencyConverterEntryType,
				MemoryCachedCurrencyConverterEntryKind.ExchangeRate, instanceID, GetCacheIntervalKey(asOn),
				baseMoney.Currency, counterCurrency);

			var cacheEntry = await GetFromMemoryCacheAsync(cacheKey, () => OriginalConvertCurrencyAsync(baseMoney, counterCurrency, asOn));

			return new Money(counterCurrency, baseMoney.Amount * cacheEntry.ExchangeRate);
		}

		public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn)
		{
			var cacheKey = Tuple.Create(memoryCachedCurrencyConverterEntryType, MemoryCachedCurrencyConverterEntryKind.CurrencyPairs, instanceID, GetCacheIntervalKey(asOn));

			var cacheEntry = await GetFromMemoryCacheAsync(cacheKey, () => GetOriginalCurrencyPairsAsync(asOn));

			return cacheEntry.CurrencyPairs;
		}

		public async Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn)
		{
			var cacheKey = Tuple.Create(memoryCachedCurrencyConverterEntryType,
				MemoryCachedCurrencyConverterEntryKind.ExchangeRate, instanceID, GetCacheIntervalKey(asOn),
				pair.BaseCurrency, pair.CounterCurrency);

			var cacheEntry = await GetFromMemoryCacheAsync(cacheKey, () => GetOriginalExchangeRateAsync(pair, asOn));

			return cacheEntry.ExchangeRate;
		}

		private long GetCacheIntervalKey(DateTimeOffset asOn)
		{
			if (_options.Value.CacheInterval.Ticks == 0L)
			{
				_logger.LogWarning("CacheInterval is 0, which makes caching redundant.");

				return 0L;
			}

			return asOn.Ticks / _options.Value.CacheInterval.Ticks;
		}

		private async Task<MemoryCachedCurrencyConverterEntry> GetOriginalCurrencyPairsAsync(DateTimeOffset asOn)
		{
			var currencyPairs = await _originalConverter.GetCurrencyPairsAsync(asOn);

			return MemoryCachedCurrencyConverterEntry.CreateForCurrencyPairs(currencyPairs);
		}

		private async Task<MemoryCachedCurrencyConverterEntry> GetOriginalExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn)
		{
			var exchangeRate = await _originalConverter.GetExchangeRateAsync(pair, asOn);

			return MemoryCachedCurrencyConverterEntry.CreateForExchangeRate(exchangeRate);
		}

		private async Task<MemoryCachedCurrencyConverterEntry> OriginalConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency, DateTimeOffset asOn)
		{
			var convertedMoney = await _originalConverter.ConvertCurrencyAsync(baseMoney, counterCurrency, asOn);

			var exchangeRate = convertedMoney.Amount / baseMoney.Amount;

			return MemoryCachedCurrencyConverterEntry.CreateForExchangeRate(exchangeRate);
		}
	}
}
