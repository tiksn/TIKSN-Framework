namespace TIKSN.Finance.ForeignExchange
{
	public class BankOfEngland : ICurrencyConverter
	{
		private const string UrlFormat = "http://www.bankofengland.co.uk/boeapps/iadb/fromshowcolumns.asp?CodeVer=new&xml.x=yes&Datefrom={0}&Dateto={1}&SeriesCodes={2}";

		private System.Collections.Generic.Dictionary<CurrencyPair, string> SeriesCodes;

		public BankOfEngland()
		{
			this.SeriesCodes = new System.Collections.Generic.Dictionary<CurrencyPair, string>();

			this.AddSeriesCode("en-AU", "en-US", "XUDLADD");
			this.AddSeriesCode("en-AU", "en-GB", "XUDLADS");

			this.AddSeriesCode("en-CA", "en-US", "XUDLCDD");
			this.AddSeriesCode("en-CA", "en-GB", "XUDLCDS");

			this.AddSeriesCode("zh-CN", "en-US", "XUDLBK73");

			this.AddSeriesCode("cs-CZ", "en-US", "XUDLBK27");
			this.AddSeriesCode("cs-CZ", "de-DE", "XUDLBK26");
			this.AddSeriesCode("cs-CZ", "en-GB", "XUDLBK25");

			this.AddSeriesCode("da-DK", "en-US", "XUDLDKD");
			this.AddSeriesCode("da-DK", "en-GB", "XUDLDKS");
			this.AddSeriesCode("da-DK", "de-DE", "XUDLBK76");

			this.AddSeriesCode("de-DE", "en-US", "XUDLERD");
			this.AddSeriesCode("de-DE", "en-GB", "XUDLERS");

			this.AddSeriesCode("zh-HK", "en-US", "XUDLHDD");
			this.AddSeriesCode("zh-HK", "en-GB", "XUDLHDS");

			this.AddSeriesCode("hu-HU", "en-US", "XUDLBK35");
			this.AddSeriesCode("hu-HU", "de-DE", "XUDLBK34");
			this.AddSeriesCode("hu-HU", "en-GB", "XUDLBK33");

			this.AddSeriesCode("hi-IN", "en-GB", "XUDLBK97");
			this.AddSeriesCode("hi-IN", "en-US", "XUDLBK64");

			this.AddSeriesCode("he-IL", "en-GB", "XUDLBK78");
			this.AddSeriesCode("he-IL", "en-US", "XUDLBK65");

			this.AddSeriesCode("ja-JP", "en-US", "XUDLJYD");
			this.AddSeriesCode("ja-JP", "en-GB", "XUDLJYS");
			this.AddSeriesCode("ja-JP", "de-DE", "XUDLBK63");
			this.AddSeriesCode("de-CH", "de-DE", "XUDLBK68");

			//this.AddSeriesCode("LV", "en-US", "XUDLBK43");
			//this.AddSeriesCode("LV", "de-DE", "XUDLBK42");
			//this.AddSeriesCode("LV", "en-GB", "XUDLBK39");

			this.AddSeriesCode("lt-LT", "en-US", "XUDLBK38");
			this.AddSeriesCode("lt-LT", "de-DE", "XUDLBK37");
			this.AddSeriesCode("lt-LT", "en-GB", "XUDLBK36");

			this.AddSeriesCode("ms-MY", "en-GB", "XUDLBK83");
			this.AddSeriesCode("ms-MY", "en-US", "XUDLBK66");

			this.AddSeriesCode("en-NZ", "en-US", "XUDLNDD");
			this.AddSeriesCode("en-NZ", "en-GB", "XUDLNDS");

			this.AddSeriesCode("nn-NO", "en-US", "XUDLNKD");
			this.AddSeriesCode("nn-NO", "en-GB", "XUDLNKS");

			this.AddSeriesCode("pl-PL", "en-US", "XUDLBK49");
			this.AddSeriesCode("pl-PL", "de-DE", "XUDLBK48");
			this.AddSeriesCode("pl-PL", "en-GB", "XUDLBK47");

			this.AddSeriesCode("ru-RU", "en-GB", "XUDLBK85");
			this.AddSeriesCode("ru-RU", "en-US", "XUDLBK69");

			this.AddSeriesCode("ar-SA", "en-US", "XUDLSRD");
			this.AddSeriesCode("ar-SA", "en-GB", "XUDLSRS");

			this.AddSeriesCode("zh-SG", "en-US", "XUDLSGD");
			this.AddSeriesCode("zh-SG", "en-GB", "XUDLSGS");

			this.AddSeriesCode("af-ZA", "en-US", "XUDLZRD");
			this.AddSeriesCode("af-ZA", "en-GB", "XUDLZRS");

			this.AddSeriesCode("ko-KR", "en-GB", "XUDLBK93");
			this.AddSeriesCode("ko-KR", "en-US", "XUDLBK74");

			this.AddSeriesCode("en-GB", "en-US", "XUDLGBD");

			this.AddSeriesCode("se-SE", "en-US", "XUDLSKD");
			this.AddSeriesCode("se-SE", "en-GB", "XUDLSKS");

			this.AddSeriesCode("de-CH", "en-US", "XUDLSFD");
			this.AddSeriesCode("de-CH", "en-GB", "XUDLSFS");

			this.AddSeriesCode("zh-TW", "en-US", "XUDLTWD");
			this.AddSeriesCode("zh-TW", "en-GB", "XUDLTWS");

			this.AddSeriesCode("th-TH", "en-GB", "XUDLBK87");
			this.AddSeriesCode("th-TH", "en-US", "XUDLBK72");

			this.AddSeriesCode("tr-TR", "en-GB", "XUDLBK95");
			this.AddSeriesCode("tr-TR", "en-US", "XUDLBK75");

			this.AddSeriesCode("en-US", "en-GB", "XUDLUSS");

			//this.AddSeriesCode("pt-BR", "en-US", "XUDLB8KL");

			this.AddSeriesCode("zh-CN", "en-GB", "XUDLBK89");
		}

		public Money ConvertCurrency(Money BaseMoney, CurrencyInfo CounterCurrency, System.DateTime asOn)
		{
			CurrencyPair pair = new CurrencyPair(BaseMoney.Currency, CounterCurrency);

			decimal rate = this.GetExchangeRate(pair, asOn);

			return new Money(CounterCurrency, BaseMoney.Amount * rate);
		}

		public System.Collections.Generic.IEnumerable<CurrencyPair> GetCurrencyPairs(System.DateTime asOn)
		{
			var Pairs = new System.Collections.Generic.List<CurrencyPair>();

			foreach (var pair in this.SeriesCodes.Keys)
			{
				decimal rate = GetExchangeRate(pair, asOn);

				if (rate != decimal.Zero)
				{
					Pairs.Add(pair);
				}
			}

			return Pairs;
		}

		public decimal GetExchangeRate(CurrencyPair Pair, System.DateTime asOn)
		{
			if (asOn > System.DateTime.Now)
				throw new System.ArgumentException("Exchange rate forecasting are not supported.");

			string SerieCode;

			try
			{
				SerieCode = this.SeriesCodes[Pair];
			}
			catch (System.Collections.Generic.KeyNotFoundException)
			{
				throw new System.ArgumentException("Currency pair not supported.");
			}

			string RequestUrl = string.Format(UrlFormat, ToInternalDataFormat(asOn.AddMonths(-1)), ToInternalDataFormat(asOn), SerieCode);

			System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(RequestUrl);

			System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)System.Threading.Tasks.Task.Factory.FromAsync<System.Net.WebResponse>(request.BeginGetResponse, request.EndGetResponse, null).Result;

			System.Xml.Linq.XDocument xdoc = System.Xml.Linq.XDocument.Load(response.GetResponseStream());

			System.DateTime Date = System.DateTime.MinValue;
			decimal ReverseRate = 0m;

			foreach (var item in xdoc.Element("{http://www.gesmes.org/xml/2002-08-01}Envelope").Element("{http://www.bankofengland.co.uk/boeapps/iadb/agg_series}Cube").Elements("{http://www.bankofengland.co.uk/boeapps/iadb/agg_series}Cube"))
			{
				var time = item.Attribute("TIME");
				var EstimatedRate = item.Attribute("OBS_VALUE");

				if (time != null && EstimatedRate != null)
				{
					int Year = int.Parse(time.Value.Substring(0, 4));
					int Month = int.Parse(time.Value.Substring(5, 2));
					int Day = int.Parse(time.Value.Substring(8));

					System.DateTime ItemTime = new System.DateTime(Year, Month, Day);

					if (ItemTime > Date)
					{
						ReverseRate = decimal.Parse(EstimatedRate.Value, System.Globalization.CultureInfo.InvariantCulture);
						Date = ItemTime;
					}
				}
			}

			if (ReverseRate == decimal.Zero)
				return decimal.Zero;
			else
				return decimal.One / ReverseRate;
		}

		private static string ToInternalDataFormat(System.DateTime DT)
		{
			return DT.ToString("dd/MMM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
		}

		private void AddSeriesCode(string BaseCountryCode, string CounterCountryCode, string SerieCode)
		{
			System.Globalization.RegionInfo BaseCountry = new System.Globalization.RegionInfo(BaseCountryCode);
			System.Globalization.RegionInfo CounterCountry = new System.Globalization.RegionInfo(CounterCountryCode);

			CurrencyInfo BaseCurrency = new CurrencyInfo(BaseCountry);
			CurrencyInfo CounterCurrency = new CurrencyInfo(CounterCountry);

			CurrencyPair pair = new CurrencyPair(BaseCurrency, CounterCurrency);

			this.SeriesCodes.Add(pair, SerieCode);
		}
	}
}