using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using TIKSN.Finance.ForeignExchange.Data;
using TIKSN.Globalization;

namespace TIKSN.Finance.ForeignExchange
{
    public abstract class ExchangeRateServiceBase : IExchangeRateService
    {
        protected readonly ICurrencyFactory _currencyFactory;
        protected readonly IRegionFactory _regionFactory;
        private readonly IExchangeRateRepository _exchangeRateRepository;
        private readonly IForeignExchangeRepository _foreignExchangeRepository;
        private readonly Dictionary<int, (IExchangeRatesProvider BatchProvider, IExchangeRateProvider IndividualProvider, int LongNameKey, int ShortNameKey, RegionInfo Country, TimeSpan InvalidationInterval)> _providers;

        protected ExchangeRateServiceBase(ICurrencyFactory currencyFactory, IRegionFactory regionFactory, IExchangeRateRepository exchangeRateRepository, IForeignExchangeRepository foreignExchangeRepository)
        {
            _exchangeRateRepository = exchangeRateRepository;
            _foreignExchangeRepository = foreignExchangeRepository;

            _providers = new Dictionary<int, (IExchangeRatesProvider BatchProvider, IExchangeRateProvider IndividualProvider, int LongNameKey, int ShortNameKey, RegionInfo Country, TimeSpan InvalidationInterval)>();

            _currencyFactory = currencyFactory;
            _regionFactory = regionFactory;

            SetupProviders();
        }

        public async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency, DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            var pair = new CurrencyPair(baseMoney.Currency, counterCurrency);

            var rate = await GetExchangeRateAsync(pair, asOn, cancellationToken);

            return new Money(counterCurrency, baseMoney.Amount * rate);
        }

        public Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            foreach (var provider in _providers)
            {
                var ticksToIntervalRatio = ason.Ticks / interval.Ticks * interval.Ticks;
                var dateFrom = new DateTimeOffset(ticksToIntervalRatio, ason.Offset);
                var dateTo = new DateTimeOffset((ticksToIntervalRatio + 1) * interval.Ticks, ason.Offset);

                var rate = await _exchangeRateRepository.GetAsync(pair.BaseCurrency.ISOCurrencySymbol, pair.CounterCurrency.ISOCurrencySymbol, dateFrom, dateTo, cancellationToken);

                if (rate == null)
                {
                    await FetchExchangeRatesAsync(pair, asOn, cancellationToken);
                }
            }
        }

        private async Task FetchExchangeRatesAsync(IExchangeRatesProvider batchProvider, DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            var exchangeRates = await batchProvider.GetExchangeRatesAsync(asOn);

            await SaveExchangeRatesAsync(exchangeRates, cancellationToken);
        }

        private async Task FetchExchangeRatesAsync(IExchangeRateProvider individualProvider, CurrencyPair pair, DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            var exchangeRate = await individualProvider.GetExchangeRateAsync(pair.BaseCurrency, pair.CounterCurrency, asOn);

            await SaveExchangeRatesAsync(new[] { exchangeRate }, cancellationToken);
        }

        private async Task SaveExchangeRatesAsync(int foreignExchangeID, IEnumerable<ExchangeRate> exchangeRates, CancellationToken cancellationToken)
        {
            var entities = exchangeRates.Select(item => new ExchangeRateEntity
            {
                ID = 0, //TODO generate ID
                AsOn = item.AsOn,
                BaseCurrencyCode = item.Pair.BaseCurrency.ISOCurrencySymbol,
                CounterCurrencyCode = item.Pair.CounterCurrency.ISOCurrencySymbol,
                ForeignExchangeID = foreignExchangeID,
                Rate = item.Rate
            }).ToArray();

            await _exchangeRateRepository.AddRangeAsync(entities, cancellationToken);
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            foreach (var provider in _providers)
            {
                var forex = await _foreignExchangeRepository.GetAsync(provider.Key, cancellationToken);

                if (forex == null)
                {
                    forex = new ForeignExchangeEntity
                    {
                        ID = provider.Key,
                        LongNameKey = provider.Value.LongNameKey,
                        ShortNameKey = provider.Value.ShortNameKey,
                        CountryCode = provider.Value.Country.Name
                    };

                    await _foreignExchangeRepository.AddAsync(forex, cancellationToken);
                }
            }
        }

        protected void AddBatchProvider(int providerID, IExchangeRatesProvider provider, int longNameKey, int shortNameKey, string country, TimeSpan invalidationInterval)
        {
            _providers.Add(providerID, (provider, null, longNameKey, shortNameKey, _regionFactory.Create(country), invalidationInterval));
        }

        protected void AddIndividualProvider(int providerID, IExchangeRateProvider provider, int longNameKey, int shortNameKey, string country, TimeSpan invalidationInterval)
        {
            _providers.Add(providerID, (null, provider, longNameKey, shortNameKey, _regionFactory.Create(country), invalidationInterval));
        }

        protected abstract void SetupProviders();
    }
}