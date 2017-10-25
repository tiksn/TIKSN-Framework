using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using TIKSN.Finance.ForeignExchange.Bank;
using TIKSN.Finance.ForeignExchange.Cumulative;
using TIKSN.Finance.ForeignExchange.Data;
using TIKSN.Globalization;

namespace TIKSN.Finance.ForeignExchange
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly Dictionary<int, (IExchangeRatesProvider Provider, int LongNameKey, int ShortNameKey, RegionInfo Country)> _providers;
        private readonly IExchangeRateRepository _exchangeRateRepository;
        private readonly IForeignExchangeRepository _foreignExchangeRepository;

        public ExchangeRateService(ICurrencyFactory currencyFactory, IRegionFactory regionFactory, IExchangeRateRepository exchangeRateRepository, IForeignExchangeRepository foreignExchangeRepository)
        {
            _exchangeRateRepository = exchangeRateRepository;
            _foreignExchangeRepository = foreignExchangeRepository;

            _providers = new Dictionary<int, (IExchangeRatesProvider Provider, int LongNameKey, int ShortNameKey, RegionInfo Country)>();

            _providers.Add(9596, new CentralBankOfArmenia(currencyFactory));
            _providers.Add(2893, new MyCurrencyDotNet(currencyFactory, regionFactory));
            _providers.Add(2328, new BankOfRussia(currencyFactory));
            _providers.Add(1234, new NationalBankOfUkraine(currencyFactory));
            _providers.Add(9488, new ReserveBankOfAustralia(currencyFactory));
            _providers.Add(5691, new SwissNationalBank(currencyFactory));
            _providers.Add(1171, new BankOfCanada(currencyFactory));
            _providers.Add(4010, new EuropeanCentralBank(currencyFactory));
            _providers.Add(7761, new FederalReserveSystem(currencyFactory));
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            foreach (var provider in _providers)
            {
                var forex = await _foreignExchangeRepository.GetAsync(provider.Key, cancellationToken);

                if(forex == null)
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
    }
}