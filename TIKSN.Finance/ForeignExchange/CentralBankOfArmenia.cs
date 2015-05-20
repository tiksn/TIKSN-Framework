using System.Linq;

namespace TIKSN.Finance.ForeignExchange
{
	public class CentralBankOfArmenia : ICurrencyConverter
	{
		private const string RSS = "https://www.cba.am/_layouts/rssreader.aspx?rss=280F57B8-763C-4EE4-90E0-8136C13E47DA";
		private static readonly CurrencyInfo AMD = new CurrencyInfo(new System.Globalization.RegionInfo("hy-AM"));
		private System.DateTime LastFetchDate;
		private System.Collections.Generic.Dictionary<CurrencyInfo, decimal> OneWayRates;
		private System.DateTime? PublicationDate;

		public CentralBankOfArmenia()
		{
			this.Initialize();
		}

		public Money ConvertCurrency(Money BaseMoney, CurrencyInfo CounterCurrency, System.DateTime asOn)
		{
			CurrencyPair pair = new CurrencyPair(BaseMoney.Currency, CounterCurrency);
			decimal rate = GetExchangeRate(pair, asOn);

			return new Money(CounterCurrency, BaseMoney.Amount * rate);
		}

		public void Fetch()
		{
			System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(RSS);

			System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)System.Threading.Tasks.Task.Factory.FromAsync<System.Net.WebResponse>(request.BeginGetResponse, request.EndGetResponse, null).Result;

			System.Xml.Linq.XDocument xdoc = System.Xml.Linq.XDocument.Load(response.GetResponseStream());

			foreach (var item in xdoc.Element("rss").Element("channel").Elements("item"))
			{
				var title = item.Element("title");
				var pubDate = item.Element("pubDate");

				string[] TitleParts = title.Value.Split('-');

				string CurrencyCode = TitleParts[0].Trim().ToUpper();
				decimal BaseUnit = decimal.Parse(TitleParts[1]);
				decimal CounterUnit = decimal.Parse(TitleParts[2]);

				if (CurrencyCode == "BRC")
					CurrencyCode = "BRL";

				if (CurrencyCode == "LVL")
					continue;

				if (BaseUnit != decimal.Zero && CounterUnit != decimal.Zero)
				{
					decimal rate = CounterUnit / BaseUnit;

					var currency = this.OneWayRates.Keys.Single(R => R.ISOCurrencySymbol == CurrencyCode);
					OneWayRates[currency] = rate;
				}

				System.DateTime publicationDate = System.DateTime.Parse(pubDate.Value, new System.Globalization.CultureInfo("en-US"));

				if (!this.PublicationDate.HasValue)
				{
					this.PublicationDate = publicationDate;
				}
			}

			this.LastFetchDate = System.DateTime.Now; // this should stay at the end
		}

		public System.Collections.Generic.IEnumerable<CurrencyPair> GetCurrencyPairs(System.DateTime asOn)
		{
			this.FetchOnDemand();

			foreach (var Rate in this.OneWayRates)
			{
				yield return new CurrencyPair(Rate.Key, AMD);
				yield return new CurrencyPair(AMD, Rate.Key);
			}
		}

		public decimal GetExchangeRate(CurrencyPair Pair, System.DateTime asOn)
		{
			this.FetchOnDemand();

			if (asOn > System.DateTime.Now)
				throw new System.ArgumentException("Exchange rate forecasting are not supported.");

			if (asOn < this.PublicationDate.Value)
				throw new System.ArgumentException("Exchange rate history are not supported.");

			if (Pair.CounterCurrency == AMD)
			{
				if (this.OneWayRates.ContainsKey(Pair.BaseCurrency))
					return this.OneWayRates[Pair.BaseCurrency];
			}
			else
			{
				if (this.OneWayRates.ContainsKey(Pair.CounterCurrency))
					return decimal.One / this.OneWayRates[Pair.CounterCurrency];
			}

			throw new System.ArgumentException("Currency pair was not found.");
		}

		private void FetchOnDemand()
		{
			if (!this.PublicationDate.HasValue)
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
			System.Collections.Generic.List<string> CountryCodes = new System.Collections.Generic.List<string>();
			System.Collections.Generic.List<System.Globalization.RegionInfo> Countries = new System.Collections.Generic.List<System.Globalization.RegionInfo>();
			System.Collections.Generic.List<CurrencyInfo> Currencies = new System.Collections.Generic.List<CurrencyInfo>();

			CountryCodes.Add("ar-AE");
			CountryCodes.Add("es-AR");
			CountryCodes.Add("en-AU");
			CountryCodes.Add("bg-BG");
			CountryCodes.Add("pt-BR");
			CountryCodes.Add("be-BY");
			CountryCodes.Add("en-CA");
			CountryCodes.Add("de-CH");
			CountryCodes.Add("zh-CN");
			CountryCodes.Add("cs-CZ");
			CountryCodes.Add("da-DK");
			CountryCodes.Add("ar-EG");
			CountryCodes.Add("de-DE");
			CountryCodes.Add("en-GB");
			CountryCodes.Add("ka-GE");
			CountryCodes.Add("zh-HK");
			CountryCodes.Add("hu-HU");
			CountryCodes.Add("he-IL");
			CountryCodes.Add("hi-IN");
			CountryCodes.Add("fa-IR");
			CountryCodes.Add("is-IS");
			CountryCodes.Add("ja-JP");
			CountryCodes.Add("ky-KG");
			CountryCodes.Add("ko-KR");
			CountryCodes.Add("ar-KW");
			CountryCodes.Add("kk-KZ");
			CountryCodes.Add("ar-LB");
			CountryCodes.Add("lt-LT");
			CountryCodes.Add("ro-MD");
			CountryCodes.Add("es-MX");
			CountryCodes.Add("nn-NO");
			CountryCodes.Add("pl-PL");
			CountryCodes.Add("ro-RO");
			CountryCodes.Add("ru-RU");
			CountryCodes.Add("ar-SA");
			CountryCodes.Add("se-SE");
			CountryCodes.Add("zh-SG");
			CountryCodes.Add("ar-SY");
			CountryCodes.Add("tg-Cyrl-TJ");
			CountryCodes.Add("tk-TM");
			CountryCodes.Add("tr-TR");
			CountryCodes.Add("uk-UA");
			CountryCodes.Add("en-US");
			CountryCodes.Add("uz-Cyrl-UZ");

			Countries.AddRange(CountryCodes.Select(Code => new System.Globalization.RegionInfo(Code)));
			Currencies.AddRange(Countries.Select(Country => new CurrencyInfo(Country)));

			this.OneWayRates = new System.Collections.Generic.Dictionary<CurrencyInfo, decimal>();

			foreach (var targetCurrency in Currencies)
			{
				OneWayRates.Add(targetCurrency, decimal.Zero);
			}

			this.LastFetchDate = System.DateTime.MinValue;
		}
	}
}