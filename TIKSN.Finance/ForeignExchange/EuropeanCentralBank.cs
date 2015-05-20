using System.Linq;

namespace TIKSN.Finance.ForeignExchange
{
	public class EuropeanCentralBank : ICurrencyConverter
	{
		private const string UrlFormat = "http://sdw.ecb.int/quickviewexport.do?SERIES_KEY=EXR.D.{0}.EUR.SP00.A&type=sdmx&start={1}&end={2}"; // Sample: "http://sdw.ecb.int/quickviewexport.do?SERIES_KEY=EXR.D.USD.EUR.SP00.A&type=sdmx&start=01-01-2013&end=01-03-2013"
		//TODO: switch to http://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml see http://www.ecb.europa.eu/stats/exchange/eurofxref/html/index.en.html (For Developers section)

		private readonly static System.Collections.Generic.List<CurrencyInfo> currencies;
		private readonly static CurrencyInfo Euro;

		static EuropeanCentralBank()
		{
			var countries = new System.Collections.Generic.List<string>();

			countries.Add("en-US");
			countries.Add("ja-JP");
			countries.Add("bg-BG");
			countries.Add("cs-CZ");
			countries.Add("da-DK");
			countries.Add("en-GB");
			countries.Add("hu-HU");
			countries.Add("lt-LT");
			countries.Add("pl-PL");
			countries.Add("ro-RO");
			countries.Add("se-SE");
			countries.Add("de-CH");
			countries.Add("nn-NO");
			countries.Add("hr-HR");
			countries.Add("ru-RU");
			countries.Add("tr-TR");
			countries.Add("en-AU");
			countries.Add("pt-BR");
			countries.Add("en-CA");
			countries.Add("zh-CN");
			countries.Add("zh-HK");
			countries.Add("id-ID");
			countries.Add("he-IL");
			countries.Add("hi-IN");
			countries.Add("ko-KR");
			countries.Add("es-MX");
			countries.Add("ms-MY");
			countries.Add("en-NZ");
			countries.Add("fil-PH");
			countries.Add("zh-SG");
			countries.Add("th-TH");
			countries.Add("af-ZA");
			countries.Add("is-IS");
			countries.Add("es-AR");
			countries.Add("ar-DZ");
			countries.Add("ar-MA");
			countries.Add("zh-TW");
			//countries.Add("EG");
			//countries.Add("es-VE");
			//countries.Add("es-CL");

			currencies = new System.Collections.Generic.List<CurrencyInfo>();

			currencies.AddRange(countries.Select(C => new CurrencyInfo(new System.Globalization.RegionInfo(C))));

			Euro = new CurrencyInfo(new System.Globalization.RegionInfo("de-DE"));
		}

		public EuropeanCentralBank()
		{
		}

		public Money ConvertCurrency(Money BaseMoney, CurrencyInfo CounterCurrency, System.DateTime asOn)
		{
			var pair = new CurrencyPair(BaseMoney.Currency, CounterCurrency);
			decimal rate = this.GetExchangeRate(pair, asOn);

			return new Money(CounterCurrency, BaseMoney.Amount * rate);
		}

		public System.Collections.Generic.IEnumerable<CurrencyPair> GetCurrencyPairs(System.DateTime asOn)
		{
			this.VerifyDate(asOn);

			var result = new System.Collections.Generic.List<CurrencyPair>();

			foreach (var foreign in currencies)
			{
				result.Add(new CurrencyPair(Euro, foreign));
				result.Add(new CurrencyPair(foreign, Euro));
			}

			return result;
		}

		public decimal GetExchangeRate(CurrencyPair Pair, System.DateTime asOn)
		{
			this.VerifyDate(asOn);

			bool Reverse;
			string ForeignCurrencyCode;

			if (Pair.BaseCurrency == Euro)
			{
				Reverse = false;
				ForeignCurrencyCode = Pair.CounterCurrency.ISOCurrencySymbol;
			}
			else if (Pair.CounterCurrency == Euro)
			{
				Reverse = true;
				ForeignCurrencyCode = Pair.BaseCurrency.ISOCurrencySymbol;
			}
			else
			{
				throw new System.ArgumentException("One of currency pair should be Euro.");
			}

			System.DateTime StartDay = System.DateTime.Now.AddDays(-3d);

			if (ForeignCurrencyCode == "ISK")
			{
				StartDay = new System.DateTime(2008, 12, 3);
			}

			string RequestURL = string.Format(UrlFormat, ForeignCurrencyCode, StartDay.ToString("dd-MM-yyyy"), System.DateTime.Now.ToString("dd-MM-yyyy"));

			System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(RequestURL);

			System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)System.Threading.Tasks.Task.Factory.FromAsync<System.Net.WebResponse>(request.BeginGetResponse, request.EndGetResponse, null).Result;

			var xdoc = System.Xml.Linq.XDocument.Load(response.GetResponseStream());

			var Obs = xdoc.Element("{http://www.SDMX.org/resources/SDMXML/schemas/v2_0/message}MessageGroup").Element("{http://www.ecb.int/vocabulary/stats/exr}DataSet").Element("{http://www.ecb.int/vocabulary/stats/exr}Series").Element("{http://www.ecb.int/vocabulary/stats/exr}Obs");

			string ObsValue = Obs.Attribute("OBS_VALUE").Value;

			decimal rate = decimal.Parse(ObsValue);

			if (Reverse)
			{
				rate = decimal.One / rate;
			}

			return rate;
		}

		private void VerifyDate(System.DateTime asOn)
		{
			if (asOn > System.DateTime.Now)
				throw new System.ArgumentException("Exchange rate forecasting are not supported.");
		}
	}
}