using System.Linq;

namespace TIKSN.Finance.ForeignExchange
{
	public class ReserveBankOfAustralia : ICurrencyConverter
	{
		private const string RSS = "http://www.rba.gov.au/rss/rss-cb-exchange-rates.xml";

		private static CurrencyInfo AustralianDollar;

		private System.DateTime LastFetchDate;
		private System.DateTime PublishedDate;
		private System.Collections.Generic.Dictionary<CurrencyInfo, decimal> rates;

		static ReserveBankOfAustralia()
		{
			var Australia = new System.Globalization.RegionInfo("en-AU");
			AustralianDollar = new CurrencyInfo(Australia);
		}

		public ReserveBankOfAustralia()
		{
			this.Initialize();
		}

		public Money ConvertCurrency(Money BaseMoney, CurrencyInfo CounterCurrency, System.DateTime asOn)
		{
			var pair = new CurrencyPair(BaseMoney.Currency, CounterCurrency);
			decimal rate = this.GetExchangeRate(pair, asOn);

			return new Money(CounterCurrency, rate * BaseMoney.Amount);
		}

		public void Fetch()
		{
			System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(RSS);

			System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)System.Threading.Tasks.Task.Factory.FromAsync<System.Net.WebResponse>(request.BeginGetResponse, request.EndGetResponse, null).Result;

			System.Xml.Linq.XDocument xdoc = System.Xml.Linq.XDocument.Load(response.GetResponseStream());

			foreach (var item in xdoc.Element("{http://www.w3.org/1999/02/22-rdf-syntax-ns#}RDF").Elements("{http://purl.org/rss/1.0/}item"))
			{
				var ExchangeRateElement = item.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}statistics").Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}exchangeRate");
				var BaseCurrencyElement = ExchangeRateElement.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}baseCurrency");
				var TargetCurrencyElement = ExchangeRateElement.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}targetCurrency");
				var ObservationValueElement = ExchangeRateElement.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}observation").Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}value");
				var PeriodElement = ExchangeRateElement.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}observationPeriod").Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}period");

				System.Diagnostics.Debug.Assert(BaseCurrencyElement.Value == "AUD");

				string CounterCurrencyCode = TargetCurrencyElement.Value;

				if (CounterCurrencyCode == "PGK" || CounterCurrencyCode == "XXX" || CounterCurrencyCode == "XDR")
					continue;

				decimal ExchangeRate = decimal.Parse(ObservationValueElement.Value);
				System.DateTime Period = System.DateTime.Parse(PeriodElement.Value);

				CurrencyInfo ForeignCurrency = this.rates.Keys.Single(R => R.ISOCurrencySymbol == CounterCurrencyCode);

				this.rates[ForeignCurrency] = ExchangeRate;

				this.PublishedDate = Period;
			}

			this.LastFetchDate = System.DateTime.Now;
		}

		public System.Collections.Generic.IEnumerable<CurrencyPair> GetCurrencyPairs(System.DateTime asOn)
		{
			this.FetchOnDemand();

			this.VerifyDate(asOn);

			var pairs = new System.Collections.Generic.List<CurrencyPair>();

			pairs.AddRange(this.rates.Keys.Select(R => new CurrencyPair(AustralianDollar, R)));
			pairs.AddRange(this.rates.Keys.Select(R => new CurrencyPair(R, AustralianDollar)));

			return pairs;
		}

		public decimal GetExchangeRate(CurrencyPair Pair, System.DateTime asOn)
		{
			this.FetchOnDemand();

			this.VerifyDate(asOn);

			if (Pair.BaseCurrency == AustralianDollar)
			{
				if (this.rates.ContainsKey(Pair.CounterCurrency))
					return this.rates[Pair.CounterCurrency];
			}
			else if (Pair.CounterCurrency == AustralianDollar)
			{
				if (this.rates.ContainsKey(Pair.BaseCurrency))
					return decimal.One / this.rates[Pair.BaseCurrency];
			}

			throw new System.ArgumentException("Currency pair not supported.");
		}

		private void FetchOnDemand()
		{
			if (this.PublishedDate == System.DateTime.MinValue)
			{
				this.Fetch();
			}
			else if (System.DateTime.Now - this.LastFetchDate > System.TimeSpan.FromDays(1d))
			{
				this.Fetch();
			}
		}

		private void Initialize()
		{
			this.PublishedDate = System.DateTime.MinValue;

			this.rates = new System.Collections.Generic.Dictionary<CurrencyInfo, decimal>();

			var Countries = new System.Collections.Generic.List<string>();

			Countries.Add("en-US");
			Countries.Add("zh-CN");
			Countries.Add("ja-JP");
			Countries.Add("de-DE");
			Countries.Add("ko-KR");
			Countries.Add("en-GB");
			Countries.Add("zh-SG");
			Countries.Add("hi-IN");
			Countries.Add("th-TH");
			Countries.Add("en-NZ");
			Countries.Add("zh-TW");
			Countries.Add("ms-MY");
			Countries.Add("id-ID");
			Countries.Add("vi-VN");
			Countries.Add("ar-AE");
			// Countries.Add("PG");
			Countries.Add("zh-HK");
			Countries.Add("en-CA");
			//Countries.Add("af-ZA");
			Countries.Add("de-CH");
			// Countries.Add("SE");
			Countries.Add("fil-PH");

			var currencies = Countries.Select(C => new CurrencyInfo(new System.Globalization.RegionInfo(C)));

			foreach (var currency in currencies)
			{
				this.rates.Add(currency, decimal.Zero);
			}

			this.LastFetchDate = System.DateTime.MinValue;
		}

		private void VerifyDate(System.DateTime asOn)
		{
			if (asOn > System.DateTime.Now)
				throw new System.ArgumentException("Exchange rate forecasting are not supported.");

			if (asOn < this.PublishedDate)
				throw new System.ArgumentException("Exchange rate history not supported.");
		}
	}
}