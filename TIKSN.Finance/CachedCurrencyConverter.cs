using System.Linq;

namespace TIKSN.Finance
{
	public class CachedCurrencyConverter : ICurrencyConverter
	{
		private System.Collections.Generic.List<CachedCurrencyPairs> cachedCurrencyPairs;

		private System.Collections.Generic.List<CachedRate> cachedRates;

		private ICurrencyConverter originalConverter;

		public CachedCurrencyConverter(ICurrencyConverter originalConverter, System.TimeSpan ratesCacheInterval, System.TimeSpan currencyPairsCacheInterval, int? ratesCacheCapacity = null, int? currencyPairsCacheCapacity = null)
		{
			if (originalConverter == null)
				throw new System.ArgumentNullException("originalConverter");

			if (ratesCacheCapacity.HasValue)
			{
				if (ratesCacheCapacity.Value < 0)
				{
					throw new System.ArgumentOutOfRangeException("ratesCacheCapacity", "Rates cache capacity can not be negative.");
				}
			}

			if (currencyPairsCacheCapacity.HasValue)
			{
				if (currencyPairsCacheCapacity.Value < 0)
				{
					throw new System.ArgumentOutOfRangeException("currencyPairsCacheCapacity", "Currency pairs cache capacity can not be negative.");
				}
			}

			if (ratesCacheInterval < System.TimeSpan.Zero)
			{
				throw new System.ArgumentOutOfRangeException("ratesCacheInterval", "Rates cache interval can not be negative.");
			}

			if (currencyPairsCacheInterval < System.TimeSpan.Zero)
			{
				throw new System.ArgumentOutOfRangeException("currencyPairsCacheInterval", "Currency pairs cache interval can not be negative.");
			}

			this.originalConverter = originalConverter;
			this.RatesCacheInterval = ratesCacheInterval;
			this.CurrencyPairsCacheInterval = currencyPairsCacheInterval;
			this.RatesCacheCapacity = ratesCacheCapacity;
			this.CurrencyPairsCacheCapacity = currencyPairsCacheCapacity;

			this.cachedCurrencyPairs = new System.Collections.Generic.List<CachedCurrencyPairs>();
			this.cachedRates = new System.Collections.Generic.List<CachedRate>();
		}

		public int? CurrencyPairsCacheCapacity { get; private set; }

		public System.TimeSpan CurrencyPairsCacheInterval { get; private set; }

		public int CurrencyPairsCacheSize
		{
			get
			{
				return this.cachedCurrencyPairs.Count;
			}
		}

		public int? RatesCacheCapacity { get; private set; }

		public System.TimeSpan RatesCacheInterval { get; private set; }

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

		public Money ConvertCurrency(Money BaseMoney, CurrencyInfo CounterCurrency, System.DateTime asOn)
		{
			if (BaseMoney.Amount == decimal.Zero)
			{
				return new Money(CounterCurrency, decimal.Zero);
			}
			else
			{
				var pair = new CurrencyPair(BaseMoney.Currency, CounterCurrency);

				var cachedRate = this.GetFromCache(this.cachedRates.Where(item => item.Pair == pair), this.RatesCacheInterval, asOn);

				if (cachedRate != null)
				{
					return new Money(CounterCurrency, BaseMoney.Amount * cachedRate.ExchangeRate);
				}

				var actualMoney = this.originalConverter.ConvertCurrency(BaseMoney, CounterCurrency, asOn);

				var actualRate = actualMoney.Amount / BaseMoney.Amount;

				this.AddToCache(this.cachedRates, this.RatesCacheCapacity, new CachedRate(pair, actualRate, asOn));

				return actualMoney;
			}
		}

		public System.Collections.Generic.IEnumerable<CurrencyPair> GetCurrencyPairs(System.DateTime asOn)
		{
			var cachedPairs = this.GetFromCache(this.cachedCurrencyPairs, this.CurrencyPairsCacheInterval, asOn);

			if (cachedPairs != null)
			{
				return cachedPairs.CurrencyPairs;
			}

			var actualPairs = this.originalConverter.GetCurrencyPairs(asOn);

			this.AddToCache(this.cachedCurrencyPairs, this.CurrencyPairsCacheCapacity, new CachedCurrencyPairs(actualPairs, asOn));

			return actualPairs;
		}

		public decimal GetExchangeRate(CurrencyPair Pair, System.DateTime asOn)
		{
			var cachedRate = this.GetFromCache(this.cachedRates.Where(item => item.Pair == Pair), this.RatesCacheInterval, asOn);

			if (cachedRate != null)
			{
				return cachedRate.ExchangeRate;
			}

			var actualRate = this.originalConverter.GetExchangeRate(Pair, asOn);

			this.AddToCache(this.cachedRates, this.RatesCacheCapacity, new CachedRate(Pair, actualRate, asOn));

			return actualRate;
		}

		private static System.TimeSpan Absolute(System.TimeSpan value)
		{
			if (value < System.TimeSpan.Zero)
			{
				return -value;
			}
			else
			{
				return value;
			}
		}

		private static bool IsActual(System.DateTime cachedAsOn, System.DateTime actualAsOn, System.TimeSpan interval)
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

		private void AddToCache<T>(System.Collections.Generic.List<T> cache, int? capacity, T itemToCache) where T : CachedData
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

		private T GetFromCache<T>(System.Collections.Generic.IEnumerable<T> cache, System.TimeSpan interval, System.DateTime asOn) where T : CachedData
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
			public CachedCurrencyPairs(System.Collections.Generic.IEnumerable<CurrencyPair> currencyPairs, System.DateTime asOn)
			{
				this.CurrencyPairs = currencyPairs;
				this.AsOn = asOn;

				this.Update();
			}

			public System.Collections.Generic.IEnumerable<CurrencyPair> CurrencyPairs { get; set; }
		}

		private abstract class CachedData
		{
			public System.DateTime AsOn { get; set; }

			public System.DateTime LastAccess { get; private set; }

			public void Update()
			{
				this.LastAccess = System.DateTime.Now;
			}
		}

		private class CachedRate : CachedData
		{
			public CachedRate(CurrencyPair pair, decimal exchangeRate, System.DateTime asOn)
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