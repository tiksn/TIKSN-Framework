using System.Linq;

namespace TIKSN.Finance.ForeignExchange
{
	public class BankOfRussia : ICurrencyConverter
	{
		private static readonly string AddressFormat = "http://www.cbr.ru/scripts/XML_daily.asp?date_req={0:00}.{1:00}.{2}";
		private static readonly System.Collections.Generic.List<CurrencyInfo> currencies;
		private static readonly CurrencyInfo RussianRuble;

		private System.DateTime? published;
		private System.Collections.Generic.Dictionary<CurrencyInfo, decimal> rates;

		static BankOfRussia()
		{
			var Russia = new System.Globalization.RegionInfo("ru-RU");

			RussianRuble = new CurrencyInfo(Russia);

			var countries = new System.Collections.Generic.List<string>();

			countries.Add("en-AU");
			countries.Add("az-Cyrl-AZ");
			countries.Add("hy-AM");
			countries.Add("be-BY");
			countries.Add("bg-BG");
			countries.Add("pt-BR");
			countries.Add("hu-HU");
			countries.Add("ko-KR");
			countries.Add("da-DK");
			countries.Add("en-US");
			countries.Add("de-DE");
			countries.Add("hi-IN");
			countries.Add("kk-KZ");
			countries.Add("en-CA");
			countries.Add("ky-KG");
			countries.Add("zh-CN");
			countries.Add("lt-LT");
			countries.Add("ro-MD");
			countries.Add("ro-RO");
			countries.Add("tk-TM");
			countries.Add("nn-NO");
			countries.Add("pl-PL");
			countries.Add("zh-SG");
			countries.Add("tg-Cyrl-TJ");
			countries.Add("tr-TR");
			countries.Add("uz-Cyrl-UZ");
			countries.Add("uk-UA");
			countries.Add("en-GB");
			countries.Add("cs-CZ");
			countries.Add("sv-SE");
			countries.Add("de-CH");
			countries.Add("af-ZA");
			countries.Add("ja-JP");
			countries.Add("is-IS");
			countries.Add("ka-GE");

			var regions = countries.Select(C => new System.Globalization.RegionInfo(C));

			currencies = new System.Collections.Generic.List<CurrencyInfo>();
			currencies.AddRange(regions.Select(R => new CurrencyInfo(R)));
		}

		public BankOfRussia()
		{
			this.rates = new System.Collections.Generic.Dictionary<CurrencyInfo, decimal>();
			this.published = null;
		}

		public Money ConvertCurrency(Money BaseMoney, CurrencyInfo CounterCurrency, System.DateTime asOn)
		{
			this.FetchOnDemand(asOn);

			decimal rate = this.GetRate(BaseMoney.Currency, CounterCurrency);

			return new Money(CounterCurrency, BaseMoney.Amount * rate);
		}

		public System.Collections.Generic.IEnumerable<CurrencyPair> GetCurrencyPairs(System.DateTime asOn)
		{
			this.FetchOnDemand(asOn);

			var pairs = new System.Collections.Generic.List<CurrencyPair>();

			foreach (var ForeignCurrency in this.rates.Keys)
			{
				pairs.Add(new CurrencyPair(ForeignCurrency, RussianRuble));
				pairs.Add(new CurrencyPair(RussianRuble, ForeignCurrency));
			}

			return pairs;
		}

		public decimal GetExchangeRate(CurrencyPair Pair, System.DateTime asOn)
		{
			this.FetchOnDemand(asOn);

			decimal rate = this.GetRate(Pair.BaseCurrency, Pair.CounterCurrency);

			return rate;
		}

		private void Fetch(System.DateTime asOn)
		{
			var RussianRussia = new System.Globalization.CultureInfo("ru-RU");

			this.rates.Clear();

			var ThatDay = asOn.Date;

			string address = string.Format(AddressFormat, ThatDay.Day, ThatDay.Month, ThatDay.Year);

			System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(address);

			System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)System.Threading.Tasks.Task.Factory.FromAsync<System.Net.WebResponse>(request.BeginGetResponse, request.EndGetResponse, null).Result;

			System.Xml.Linq.XDocument xdoc = System.Xml.Linq.XDocument.Load(response.GetResponseStream());

			foreach (var ValuteElement in xdoc.Element("ValCurs").Elements("Valute"))
			{
				var CharCodeElement = ValuteElement.Element("CharCode");
				var NominalElement = ValuteElement.Element("Nominal");
				var ValueElement = ValuteElement.Element("Value");

				var code = CharCodeElement.Value;

				if (code == "XDR" || code == "ATS" || code == "BEF" || code == "GRD" || code == "IEP" || code == "ESP" || code == "ITL" || code == "DEM" || code == "NLG" || code == "PTE" || code == "TRL" || code == "FIM" || code == "FRF" || code == "XEU" || code == "NULL" || code == "EEK" || code == "LVL" || code == "BYB" || code == "AZM")
					continue;

				var Currency = currencies.Single(C => C.ToString() == CharCodeElement.Value);
				decimal rate = decimal.Parse(ValueElement.Value, RussianRussia) / decimal.Parse(NominalElement.Value, RussianRussia);

				this.rates.Add(Currency, rate);
			}

			this.published = asOn.Date;
		}

		private void FetchOnDemand(System.DateTime asOn)
		{
			if (asOn > System.DateTime.Now)
				throw new System.ArgumentException("Exchange rate forecasting not supported.");

			if (!this.published.HasValue)
				this.Fetch(asOn);
			else if (this.published.Value != System.DateTime.Today)
				this.Fetch(asOn);
		}

		private decimal GetRate(CurrencyInfo BaseCurrency, CurrencyInfo CounterCurrency)
		{
			if (BaseCurrency == RussianRuble)
			{
				if (rates.ContainsKey(CounterCurrency))
					return decimal.One / rates[CounterCurrency];
			}
			else if (CounterCurrency == RussianRuble)
			{
				if (this.rates.ContainsKey(BaseCurrency))
					return this.rates[BaseCurrency];
			}

			throw new System.ArgumentException("Currency pair not supported.");
		}
	}
}