using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly Dictionary<int, (IExchangeRatesProvider BatchProvider, IExchangeRateProvider IndividualProvider, int LongNameKey, int ShortNameKey, RegionInfo Country, TimeSpan InvalidationInterval)> _Providers;

        protected ExchangeRateServiceBase(ICurrencyFactory currencyFactory, IRegionFactory regionFactory, IExchangeRateRepository exchangeRateRepository, IForeignExchangeRepository foreignExchangeRepository)
        {
            _exchangeRateRepository = exchangeRateRepository;
            _foreignExchangeRepository = foreignExchangeRepository;

            _Providers = new Dictionary<int, (IExchangeRatesProvider BatchProvider, IExchangeRateProvider IndividualProvider, int LongNameKey, int ShortNameKey, RegionInfo Country, TimeSpan InvalidationInterval)>();

            _currencyFactory = currencyFactory;
            _regionFactory = regionFactory;

            SetupProviders();
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            foreach (var provider in _Providers)
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
            _Providers.Add(providerID, (provider, null, longNameKey, shortNameKey, _regionFactory.Create(country), invalidationInterval));
        }

        protected void AddIndividualProvider(int providerID, IExchangeRateProvider provider, int longNameKey, int shortNameKey, string country, TimeSpan invalidationInterval)
        {
            _Providers.Add(providerID, (null, provider, longNameKey, shortNameKey, _regionFactory.Create(country), invalidationInterval));
        }

        protected abstract void SetupProviders();
    }
}