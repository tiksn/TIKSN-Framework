using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TIKSN.Finance.ForeignExchange
{
	public class CentralBankOfArmenia : ICurrencyConverter
	{
		private const string RSS = "https://www.cba.am/_layouts/rssreader.aspx?rss=280F57B8-763C-4EE4-90E0-8136C13E47DA";
		private static readonly CurrencyInfo AMD = new CurrencyInfo(new RegionInfo("hy-AM"));
		private DateTimeOffset lastFetchDate;
		private Dictionary<CurrencyInfo, decimal> oneWayRates;
		private DateTimeOffset? publicationDate;

		public CentralBankOfArmenia()
		{
			this.oneWayRates = new Dictionary<CurrencyInfo, decimal>();

			this.lastFetchDate = DateTimeOffset.MinValue;
		}

		public async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency, DateTimeOffset asOn)
		{
			CurrencyPair pair = new CurrencyPair(baseMoney.Currency, counterCurrency);

			decimal rate = await GetExchangeRateAsync(pair, asOn);

			return new Money(counterCurrency, baseMoney.Amount * rate);
		}

		public async Task FetchAsync()
		{
			HttpWebRequest request = WebRequest.CreateHttp(RSS);

			HttpWebResponse response = (HttpWebResponse)await Task.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null);

			var xdoc = XDocument.Load(response.GetResponseStream());

			lock (this.oneWayRates)
			{
				foreach (var item in xdoc.Element("rss").Element("channel").Elements("item"))
				{
					var title = item.Element("title");
					var pubDate = item.Element("pubDate");

					string[] titleParts = title.Value.Split('-');

					string currencyCode = titleParts[0].Trim().ToUpper();
					decimal baseUnit = decimal.Parse(titleParts[1]);
					decimal counterUnit = decimal.Parse(titleParts[2]);

					if (currencyCode == "BRC")
						currencyCode = "BRL";

					if (currencyCode == "LVL")
						continue;

					if (string.Equals(currencyCode, "SDR", StringComparison.OrdinalIgnoreCase))
						currencyCode = "XDR";

					if (baseUnit != decimal.Zero && counterUnit != decimal.Zero)
					{
						decimal rate = counterUnit / baseUnit;

						var currency = new CurrencyInfo(currencyCode);
						oneWayRates[currency] = rate;
					}

					var publicationDate = DateTimeOffset.Parse(pubDate.Value, new CultureInfo("en-US"));

					if (!this.publicationDate.HasValue)
					{
						this.publicationDate = publicationDate;
					}
				}

				this.lastFetchDate = DateTimeOffset.Now; // this should stay at the end
			}
		}
		public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn)
		{
			await this.FetchOnDemandAsync();

			var rates = new List<CurrencyPair>();

			foreach (var rate in this.oneWayRates)
			{
				rates.Add(new CurrencyPair(rate.Key, AMD));
				rates.Add(new CurrencyPair(AMD, rate.Key));
			}

			return rates;
		}

		public async Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn)
		{
			await this.FetchOnDemandAsync();

			if (asOn > DateTimeOffset.Now)
				throw new ArgumentException("Exchange rate forecasting are not supported.");

			if (asOn < this.publicationDate.Value)
				throw new ArgumentException("Exchange rate history are not supported.");

			if (pair.CounterCurrency == AMD)
			{
				if (this.oneWayRates.ContainsKey(pair.BaseCurrency))
					return this.oneWayRates[pair.BaseCurrency];
			}
			else
			{
				if (this.oneWayRates.ContainsKey(pair.CounterCurrency))
					return decimal.One / this.oneWayRates[pair.CounterCurrency];
			}

			throw new ArgumentException("Currency pair was not found.");
		}

		private async Task FetchOnDemandAsync()
		{
			if (!this.publicationDate.HasValue)
			{
				await this.FetchAsync();
			}
			else if (DateTimeOffset.Now - lastFetchDate > TimeSpan.FromDays(1d))
			{
				await this.FetchAsync();
			}
		}
	}
}