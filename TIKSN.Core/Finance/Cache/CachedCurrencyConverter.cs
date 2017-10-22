using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TIKSN.Finance.Cache
{
    public class CachedCurrencyConverter : ICurrencyConverter
    {
        private List<CachedCurrencyPairs> cachedCurrencyPairs;

        private List<CachedRate> cachedRates;

        private ICurrencyConverter originalConverter;

        public CachedCurrencyConverter(ICurrencyConverter originalConverter, TimeSpan ratesCacheInterval, TimeSpan currencyPairsCacheInterval, int? ratesCacheCapacity = null, int? currencyPairsCacheCapacity = null)
        {
            if (originalConverter == null)
                throw new ArgumentNullException("originalConverter");

            if (ratesCacheCapacity.HasValue)
            {
                if (ratesCacheCapacity.Value < 0)
                {
                    throw new ArgumentOutOfRangeException("ratesCacheCapacity", "Rates cache capacity can not be negative.");
                }
            }

            if (currencyPairsCacheCapacity.HasValue)
            {
                if (currencyPairsCacheCapacity.Value < 0)
                {
                    throw new ArgumentOutOfRangeException("currencyPairsCacheCapacity", "Currency pairs cache capacity can not be negative.");
                }
            }

            if (ratesCacheInterval < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("ratesCacheInterval", "Rates cache interval can not be negative.");
            }

            if (currencyPairsCacheInterval < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("currencyPairsCacheInterval", "Currency pairs cache interval can not be negative.");
            }

            this.originalConverter = originalConverter;
            this.RatesCacheInterval = ratesCacheInterval;
            this.CurrencyPairsCacheInterval = currencyPairsCacheInterval;
            this.RatesCacheCapacity = ratesCacheCapacity;
            this.CurrencyPairsCacheCapacity = currencyPairsCacheCapacity;

            this.cachedCurrencyPairs = new List<CachedCurrencyPairs>();
            this.cachedRates = new List<CachedRate>();
        }

        public int? CurrencyPairsCacheCapacity { get; private set; }

        public TimeSpan CurrencyPairsCacheInterval { get; private set; }

        public int CurrencyPairsCacheSize
        {
            get
            {
                return this.cachedCurrencyPairs.Count;
            }
        }

        public int? RatesCacheCapacity { get; private set; }

        public TimeSpan RatesCacheInterval { get; private set; }

        public int RatesCacheSize
        {
            get
            {
                return this.cachedRates.Count;
            }
        }

        public void Clear()
        {
            this.cachedCurrencyPairs.Clear();
            this.cachedRates.Clear();
        }

        public async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency, DateTimeOffset asOn)
        {
            if (baseMoney.Amount == decimal.Zero)
            {
                return new Money(counterCurrency, decimal.Zero);
            }
            else
            {
                var pair = new CurrencyPair(baseMoney.Currency, counterCurrency);

                var cachedRate = this.GetFromCache(this.cachedRates.Where(item => item.Pair == pair), this.RatesCacheInterval, asOn);

                if (cachedRate != null)
                {
                    return new Money(counterCurrency, baseMoney.Amount * cachedRate.ExchangeRate);
                }

                var actualMoney = await this.originalConverter.ConvertCurrencyAsync(baseMoney, counterCurrency, asOn);

                var actualRate = actualMoney.Amount / baseMoney.Amount;

                this.AddToCache(this.cachedRates, this.RatesCacheCapacity, new CachedRate(pair, actualRate, asOn));

                return actualMoney;
            }
        }

        public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn)
        {
            var cachedPairs = this.GetFromCache(this.cachedCurrencyPairs, this.CurrencyPairsCacheInterval, asOn);

            if (cachedPairs != null)
            {
                return cachedPairs.CurrencyPairs;
            }

            var actualPairs = await this.originalConverter.GetCurrencyPairsAsync(asOn);

            this.AddToCache(this.cachedCurrencyPairs, this.CurrencyPairsCacheCapacity, new CachedCurrencyPairs(actualPairs, asOn));

            return actualPairs;
        }

        public async Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn)
        {
            var cachedRate = this.GetFromCache(this.cachedRates.Where(item => item.Pair == pair), this.RatesCacheInterval, asOn);

            if (cachedRate != null)
            {
                return cachedRate.ExchangeRate;
            }

            var actualRate = await this.originalConverter.GetExchangeRateAsync(pair, asOn);

            this.AddToCache(this.cachedRates, this.RatesCacheCapacity, new CachedRate(pair, actualRate, asOn));

            return actualRate;
        }

        private static TimeSpan Absolute(TimeSpan value)
        {
            if (value < TimeSpan.Zero)
            {
                return -value;
            }
            else
            {
                return value;
            }
        }

        private static bool IsActual(DateTimeOffset cachedAsOn, DateTimeOffset actualAsOn, TimeSpan interval)
        {
            if (cachedAsOn < actualAsOn)
            {
                if (actualAsOn - cachedAsOn <= interval)
                {
                    return true;
                }
            }
            else
            {
                if (cachedAsOn - actualAsOn < interval)
                {
                    return true;
                }
            }

            return false;
        }

        private void AddToCache<T>(List<T> cache, int? capacity, T itemToCache) where T : CachedData
        {
            lock (cache)
            {
                if (capacity != 0)
                {
                    if (capacity.HasValue)
                    {
                        if (cache.Count + 1 > capacity.Value)
                        {
                            var itemToRemove = cache.OrderBy(item => item.LastAccess).First();

                            cache.Remove(itemToRemove);
                        }

                        cache.Add(itemToCache);
                    }
                    else
                    {
                        cache.Add(itemToCache);
                    }
                }
            }
        }

        private T GetFromCache<T>(IEnumerable<T> cache, TimeSpan interval, DateTimeOffset asOn) where T : CachedData
        {
            var cachedItem = cache.Where(item => IsActual(item.AsOn, asOn, interval)).OrderBy(item => Absolute(item.AsOn - asOn)).FirstOrDefault();

            if (cachedItem != null)
            {
                cachedItem.Update();
            }

            return cachedItem;
        }

        private class CachedCurrencyPairs : CachedData
        {
            public CachedCurrencyPairs(IEnumerable<CurrencyPair> currencyPairs, DateTimeOffset asOn)
            {
                this.CurrencyPairs = currencyPairs;
                this.AsOn = asOn;

                this.Update();
            }

            public IEnumerable<CurrencyPair> CurrencyPairs { get; set; }
        }

        private abstract class CachedData
        {
            public DateTimeOffset AsOn { get; set; }

            public DateTimeOffset LastAccess { get; private set; }

            public void Update()
            {
                this.LastAccess = DateTimeOffset.Now;
            }
        }

        private class CachedRate : CachedData
        {
            public CachedRate(CurrencyPair pair, decimal exchangeRate, DateTimeOffset asOn)
            {
                this.Pair = pair;
                this.ExchangeRate = exchangeRate;
                this.AsOn = asOn;

                this.Update();
            }

            public decimal ExchangeRate { get; private set; }

            public CurrencyPair Pair { get; private set; }
        }
    }
}