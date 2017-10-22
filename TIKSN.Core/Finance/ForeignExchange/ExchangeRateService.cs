using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TIKSN.Finance.ForeignExchange.Bank;
using TIKSN.Finance.ForeignExchange.Cumulative;
using TIKSN.Globalization;

namespace TIKSN.Finance.ForeignExchange
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly Dictionary<int, IExchangeRatesProvider> _providers;

        public ExchangeRateService(ICurrencyFactory currencyFactory, IRegionFactory regionFactory)
        {
            _providers = new Dictionary<int, IExchangeRatesProvider>();

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

        public Task InitializeAsync()
        {
            throw new NotImplementedException();
        }
    }
}