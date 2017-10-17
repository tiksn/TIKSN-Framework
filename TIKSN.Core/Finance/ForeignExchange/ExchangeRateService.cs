using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TIKSN.Finance.ForeignExchange.Bank;
using TIKSN.Globalization;

namespace TIKSN.Finance.ForeignExchange
{
	public class ExchangeRateService : IExchangeRateService
	{
		private readonly Dictionary<int, IExchangeRateProvider> _providers;

		public ExchangeRateService(ICurrencyFactory currencyFactory)
		{
			_providers = new Dictionary<int, IExchangeRateProvider>();

			_providers.Add(9596, new CentralBankOfArmenia(currencyFactory));
		}

		public Task InitializeAsync()
		{
			throw new NotImplementedException();
		}
	}
}
