using System.Linq;

namespace TIKSN.Finance.ForeignExchange
{
	public class BankOfCanada : ICurrencyConverter
	{
		private const string ClosingURL = "http://www.bankofcanada.ca/stats/assets/rates_rss/closing/en_all.xml";
		private const string NoonURL = "http://www.bankofcanada.ca/stats/assets/rates_rss/noon/en_all.xml";
		private static CurrencyInfo CanadianDollar;

		private System.DateTime LastFetchDate;
		private System.Collections.Generic.Dictionary<CurrencyInfo, System.Tuple<System.DateTime, decimal>> rates;

		static BankOfCanada()
		{
			System.Globalization.RegionInfo Canada = new System.Globalization.RegionInfo("en-CA");
			CanadianDollar = new CurrencyInfo(Canada);
		}

		public BankOfCanada()
		{
			this.Initialize();
		}

		public Money ConvertCurrency(Money BaseMoney, CurrencyInfo CounterCurrency, System.DateTime asOn)
		{
			CurrencyPair pair = new CurrencyPair(BaseMoney.Currency, CounterCurrency);
			decimal rate = this.GetExchangeRate(pair, asOn);

			return new Money(CounterCurrency, BaseMoney.Amount * rate);
		}

		public void Fetch()
		{
			System.Collections.Generic.Dictionary<CurrencyInfo, System.Tuple<System.DateTime, decimal>> NoonRates = this.FetchRates(NoonURL);
			System.Collections.Generic.Dictionary<CurrencyInfo, System.Tuple<System.DateTime, decimal>> ClosingRates = this.FetchRates(ClosingURL);

			this.UpdateRates(NoonRates);
			this.UpdateRates(ClosingRates);

			this.LastFetchDate = System.DateTime.Now; // must saty at the end
		}

		public System.Collections.Generic.IEnumerable<CurrencyPair> GetCurrencyPairs(System.DateTime asOn)
		{
			this.FetchOnDemand();

			var MinDate = this.rates.Min(R => R.Value.Item1);

			if (asOn > System.DateTime.Now)
				throw new System.ArgumentException("Exchange rate forecasting are not supported.");

			if (asOn < MinDate)
				throw new System.ArgumentException("Exchange rate history not supported.");

			System.Collections.Generic.List<CurrencyPair> result = new System.Collections.Generic.List<CurrencyPair>();

			foreach (CurrencyInfo against in this.rates.Keys)
			{
				result.Add(new CurrencyPair(CanadianDollar, against));
				result.Add(new CurrencyPair(against, CanadianDollar));
			}

			return result;
		}

		public decimal GetExchangeRate(CurrencyPair Pair, System.DateTime asOn)
		{
			this.FetchOnDemand();

			if (asOn > System.DateTime.Now)
				throw new System.ArgumentException("Exchange rate forecasting not supported.");

			//if (asOn.Date < System.DateTime.Now.Date)
			//    throw new System.ArgumentException("Exchange rate history not supported.");

			System.Tuple<System.DateTime, decimal> rateInfo;

			if (this.IsHomeCurrencyPair(Pair))
			{
				rateInfo = this.rates[Pair.CounterCurrency];
			}
			else
			{
				rateInfo = this.rates[Pair.BaseCurrency];
				rateInfo = new System.Tuple<System.DateTime, decimal>(rateInfo.Item1, decimal.One / rateInfo.Item2);
			}

			if (asOn < rateInfo.Item1)
				throw new System.ArgumentException("Exchange rate history not supported.");

			return rateInfo.Item2;
		}

		private void FetchOnDemand()
		{
			foreach (var rate in rates)
			{
				if (rate.Value.Item2 == decimal.Zero)
				{
					this.Fetch();

					return;
				}
			}

			if (System.DateTime.Now - this.LastFetchDate > System.TimeSpan.FromDays(1d))
			{
				this.Fetch();
			}
		}

		private System.Collections.Generic.Dictionary<CurrencyInfo, System.Tuple<System.DateTime, decimal>> FetchRates(string RssUrl)
		{
			System.Collections.Generic.Dictionary<CurrencyInfo, System.Tuple<System.DateTime, decimal>> result = new System.Collections.Generic.Dictionary<CurrencyInfo, System.Tuple<System.DateTime, decimal>>();

			System.Collections.Generic.Dictionary<string, System.Tuple<System.DateTime, decimal>> RawData = this.FetchRawData(RssUrl);

			foreach (var RawItem in RawData)
			{
				if (RawItem.Key == "BSD" || RawItem.Key == "XPF" || RawItem.Key == "FJD" || RawItem.Key == "GHS" || RawItem.Key == "ANG")
					continue;

				CurrencyInfo currency = this.rates.Keys.Single(C => C.ISOCurrencySymbol == RawItem.Key);

				result.Add(currency, RawItem.Value);
			}

			return result;
		}

		private System.Collections.Generic.Dictionary<string, System.Tuple<System.DateTime, decimal>> FetchRawData(string RssUrl)
		{
			System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(RssUrl);

			System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)System.Threading.Tasks.Task.Factory.FromAsync<System.Net.WebResponse>(request.BeginGetResponse, request.EndGetResponse, null).Result;

			System.Xml.Linq.XDocument xdoc = System.Xml.Linq.XDocument.Load(response.GetResponseStream());

			System.Collections.Generic.Dictionary<string, System.Tuple<System.DateTime, decimal>> result = new System.Collections.Generic.Dictionary<string, System.Tuple<System.DateTime, decimal>>();

			foreach (var ItemElement in xdoc.Element("{http://www.w3.org/1999/02/22-rdf-syntax-ns#}RDF").Elements("{http://purl.org/rss/1.0/}item"))
			{
				var ExchangeRateElement = ItemElement.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.1}statistics").Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.1}exchangeRate");
				var ValueElement = ExchangeRateElement.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.1}value");
				var BaseCurrencyElement = ExchangeRateElement.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.1}baseCurrency");
				var TargetCurrencyElement = ExchangeRateElement.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.1}targetCurrency");
				var ObservationPeriodElement = ExchangeRateElement.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.1}observationPeriod");

				System.Diagnostics.Debug.Assert(BaseCurrencyElement.Value == "CAD");

				decimal rate = decimal.Parse(ValueElement.Value);
				System.DateTime asOn = System.DateTime.Parse(ObservationPeriodElement.Value);

				result.Add(TargetCurrencyElement.Value, new System.Tuple<System.DateTime, decimal>(asOn, rate));
			}

			return result;
		}

		private void Initialize()
		{
			System.Collections.Generic.List<string> CountryCodes = new System.Collections.Generic.List<string>();
			System.Collections.Generic.List<System.Globalization.RegionInfo> Countries = new System.Collections.Generic.List<System.Globalization.RegionInfo>();
			System.Collections.Generic.List<CurrencyInfo> Currencies = new System.Collections.Generic.List<CurrencyInfo>();

			CountryCodes.Add("en-US");
			CountryCodes.Add("es-AR");
			CountryCodes.Add("en-AU");
			//TODO: CountryCodes.Add("BS");
			CountryCodes.Add("pt-BR");
			//CountryCodes.Add("XA");
			//CountryCodes.Add("XP");
			CountryCodes.Add("es-CL");
			CountryCodes.Add("zh-CN");
			CountryCodes.Add("es-CO");
			CountryCodes.Add("hr-HR");
			CountryCodes.Add("cs-CZ");
			CountryCodes.Add("da-DK");
			//CountryCodes.Add("XC");
			CountryCodes.Add("de-DE"); // Euro - Germany
			//TODO: CountryCodes.Add("FJ");
			//TODO: CountryCodes.Add("GH");
			CountryCodes.Add("es-GT");
			CountryCodes.Add("es-HN");
			CountryCodes.Add("zh-HK");
			CountryCodes.Add("hu-HU");
			CountryCodes.Add("is-IS");
			CountryCodes.Add("hi-IN");
			CountryCodes.Add("id-ID");
			CountryCodes.Add("he-IL");
			CountryCodes.Add("en-JM");
			CountryCodes.Add("ja-JP");
			CountryCodes.Add("ms-MY");
			CountryCodes.Add("es-MX");
			CountryCodes.Add("ar-MA");
			CountryCodes.Add("my-MM");
			//TODO: CountryCodes.Add("AN");
			CountryCodes.Add("en-NZ");
			CountryCodes.Add("nn-NO");
			CountryCodes.Add("ur-PK");
			CountryCodes.Add("es-PA");
			CountryCodes.Add("es-PE");
			CountryCodes.Add("en-PH");
			CountryCodes.Add("pl-PL");
			CountryCodes.Add("ro-RO");
			CountryCodes.Add("ru-RU");
			CountryCodes.Add("sr-Cyrl-RS");
			CountryCodes.Add("zh-SG");
			CountryCodes.Add("af-ZA");
			CountryCodes.Add("ko-KR");
			CountryCodes.Add("si-LK");
			CountryCodes.Add("se-SE");
			CountryCodes.Add("de-CH");
			CountryCodes.Add("zh-TW");
			CountryCodes.Add("th-TH");
			CountryCodes.Add("en-TT");
			CountryCodes.Add("ar-TN");
			CountryCodes.Add("tr-TR");
			CountryCodes.Add("ar-AE");
			CountryCodes.Add("en-GB");
			CountryCodes.Add("es-VE");
			CountryCodes.Add("vi-VN");
			CountryCodes.Add("fr-CM");
			CountryCodes.Add("en-029");

			Countries.AddRange(CountryCodes.Select(Code => new System.Globalization.RegionInfo(Code)));
			Currencies.AddRange(Countries.Select(Country => new CurrencyInfo(Country)));

			this.rates = new System.Collections.Generic.Dictionary<CurrencyInfo, System.Tuple<System.DateTime, decimal>>();

			foreach (var targetCurrency in Currencies)
			{
				rates.Add(targetCurrency, new System.Tuple<System.DateTime, decimal>(System.DateTime.MinValue, decimal.Zero));
			}

			this.LastFetchDate = System.DateTime.Now;
		}

		private bool IsHomeCurrencyPair(CurrencyPair Pair)
		{
			if (Pair.BaseCurrency == CanadianDollar)
			{
				if (this.rates.Any(R => R.Key == Pair.CounterCurrency))
					return true;
			}
			else if (Pair.CounterCurrency == CanadianDollar)
			{
				if (this.rates.Any(R => R.Key == Pair.BaseCurrency))
					return false;
			}

			throw new System.ArgumentException("Currency pair not supported.");
		}

		private void UpdateRates(System.Collections.Generic.Dictionary<CurrencyInfo, System.Tuple<System.DateTime, decimal>> NewRates)
		{
			foreach (var KeyValue in NewRates)
			{
				if (KeyValue.Value.Item1 > this.rates[KeyValue.Key].Item1)
				{
					this.rates[KeyValue.Key] = KeyValue.Value;
				}
			}
		}
	}
}