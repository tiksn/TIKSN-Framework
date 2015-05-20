using System.Linq;

namespace TIKSN.Finance.ForeignExchange
{
	public class SwissNationalBank : ICurrencyConverter
	{
		private static readonly System.Collections.Generic.List<CurrencyInfo> ForeignCurrencies;
		private static readonly CurrencyInfo SwissFranc;
		private static string RSSURL = "http://www.snb.ch/selector/en/mmr/exfeed/rss";
		private System.Collections.Generic.Dictionary<System.Tuple<CurrencyInfo, System.DateTime>, decimal> ForeignRates;

		static SwissNationalBank()
		{
			var Switzerland = new System.Globalization.RegionInfo("de-CH");

			SwissFranc = new CurrencyInfo(Switzerland);

			var CountryCodes = new System.Collections.Generic.List<string>();

			CountryCodes.Add("en-US");
			CountryCodes.Add("en-GB");
			CountryCodes.Add("de-DE");
			CountryCodes.Add("ja-JP");

			ForeignCurrencies = new System.Collections.Generic.List<CurrencyInfo>();

			ForeignCurrencies.AddRange(CountryCodes.Select(CC => new CurrencyInfo(new System.Globalization.RegionInfo(CC))));
		}

		public SwissNationalBank()
		{
			this.ForeignRates = new System.Collections.Generic.Dictionary<System.Tuple<CurrencyInfo, System.DateTime>, decimal>();
		}

		public Money ConvertCurrency(Money BaseMoney, CurrencyInfo CounterCurrency, System.DateTime asOn)
		{
			this.FetchOnDemand();

			decimal rate = this.GetRate(BaseMoney.Currency, CounterCurrency, asOn);

			return new Money(CounterCurrency, BaseMoney.Amount * rate);
		}

		public System.Collections.Generic.IEnumerable<CurrencyPair> GetCurrencyPairs(System.DateTime asOn)
		{
			this.FetchOnDemand();

			var Pairs = new System.Collections.Generic.List<CurrencyPair>();

			var TodayCurrencies = this.FilterByDate(asOn);

			foreach (var currency in TodayCurrencies)
			{
				Pairs.Add(new CurrencyPair(SwissFranc, currency.Key));
				Pairs.Add(new CurrencyPair(currency.Key, SwissFranc));
			}

			return Pairs;
		}

		public decimal GetExchangeRate(CurrencyPair Pair, System.DateTime asOn)
		{
			this.FetchOnDemand();

			decimal rate = this.GetRate(Pair.BaseCurrency, Pair.CounterCurrency, asOn);

			return rate;
		}

		private void Fetch()
		{
			this.ForeignRates.Clear();

			System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(RSSURL);

			System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)System.Threading.Tasks.Task.Factory.FromAsync<System.Net.WebResponse>(request.BeginGetResponse, request.EndGetResponse, null).Result;

			System.Xml.Linq.XDocument xdoc = System.Xml.Linq.XDocument.Load(response.GetResponseStream());

			foreach (var ItemElement in xdoc.Element("{http://www.w3.org/1999/02/22-rdf-syntax-ns#}RDF").Elements("{http://purl.org/rss/1.0/}item"))
			{
				var DateElement = ItemElement.Element("{http://purl.org/dc/elements/1.1/}date");
				var ExchangeRateElement = ItemElement.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}statistics").Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}exchangeRate");
				var ValueElement = ExchangeRateElement.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}observation").Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}value");
				var TargetCurrencyElement = ExchangeRateElement.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}targetCurrency");

				var date = System.Convert.ToDateTime(DateElement.Value);
				var currencyCode = TargetCurrencyElement.Value;
				decimal rate = decimal.Parse(ValueElement.Value);

				var currency = ForeignCurrencies.Single(C => C.ISOCurrencySymbol == currencyCode);

				System.Diagnostics.Debug.Assert(ExchangeRateElement.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}observation").Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}unit").Value == "CHF");

				this.ForeignRates.Add(new System.Tuple<CurrencyInfo, System.DateTime>(currency, date), rate);
			}
		}

		private void FetchOnDemand()
		{
			if (this.ForeignRates.Count == 0)
				this.Fetch();
			else if (this.ForeignRates.Any(R => R.Key.Item2.Date == System.DateTime.Today))
				this.Fetch();
		}

		private System.Collections.Generic.Dictionary<CurrencyInfo, decimal> FilterByDate(System.DateTime asOn)
		{
			if (asOn > System.DateTime.Now)
				throw new System.ArgumentException("Exchange rate forecasting not supported.");

			var MaxDate = this.ForeignRates.Max(R => R.Key.Item2);
			var MinDate = this.ForeignRates.Min(R => R.Key.Item2);

			if (asOn < MinDate)
				throw new System.ArgumentException("Exchange rate history not supported.");

			System.DateTime FilterDate;

			if (asOn > MaxDate)
			{
				FilterDate = MaxDate;
			}
			else
			{
				FilterDate = asOn;
			}

			var FilteredResults = this.ForeignRates.Where(R => R.Key.Item2.Date == FilterDate.Date).Select(R => new System.Tuple<CurrencyInfo, decimal>(R.Key.Item1, R.Value));

			var Results = new System.Collections.Generic.Dictionary<CurrencyInfo, decimal>();

			foreach (var FilteredResult in FilteredResults)
			{
				Results.Add(FilteredResult.Item1, FilteredResult.Item2);
			}

			return Results;
		}

		private decimal GetRate(CurrencyInfo BaseCurrency, CurrencyInfo CounterCurrency, System.DateTime asOn)
		{
			var Filtered = this.FilterByDate(asOn);

			if (BaseCurrency == SwissFranc)
			{
				if (Filtered.ContainsKey(CounterCurrency))
					return 1 / Filtered[CounterCurrency];
			}
			else if (CounterCurrency == SwissFranc)
			{
				if (Filtered.ContainsKey(BaseCurrency))
					return Filtered[BaseCurrency];
			}

			throw new System.ArgumentException("Currency pair not supported.");
		}
	}
}