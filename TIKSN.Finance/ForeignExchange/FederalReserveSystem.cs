using System.Linq;

namespace TIKSN.Finance.ForeignExchange
{
    public class FederalReserveSystem : ICurrencyConverter
    {
        private const string DataUrlFormat = "http://www.federalreserve.gov/datadownload/Output.aspx?rel=H10&series=f72f395f2a6b3a4bbc83b2983ad62737&from={0}&to={1}&filetype=sdmx";

        private static readonly System.Collections.Generic.IEnumerable<CurrencyInfo> currencies;
        private static readonly CurrencyInfo UnitedStatesDollar;

        static FederalReserveSystem()
        {
            var codes = new System.Collections.Generic.List<string>();

            codes.Add("en-AU");
            codes.Add("pt-BR");
            codes.Add("en-CA");
            codes.Add("zh-CN");
            codes.Add("da-DK");
            codes.Add("de-DE");
            codes.Add("zh-HK");
            codes.Add("hi-IN");
            codes.Add("ja-JP");
            codes.Add("ms-MY");
            codes.Add("es-MX");
            codes.Add("en-NZ");
            codes.Add("nn-NO");
            codes.Add("zh-SG");
            codes.Add("af-ZA");
            codes.Add("ko-KR");
            codes.Add("si-LK");
            codes.Add("se-SE");
            codes.Add("de-CH");
            codes.Add("zh-TW");
            codes.Add("th-TH");
            codes.Add("en-GB");
            codes.Add("es-VE");

            var countries = codes.Select(C => new System.Globalization.RegionInfo(C));

            currencies = countries.Select(C => new CurrencyInfo(C));

            UnitedStatesDollar = new CurrencyInfo(new System.Globalization.RegionInfo("en-US"));
        }

        public FederalReserveSystem()
        {
        }

        public Money ConvertCurrency(Money BaseMoney, CurrencyInfo CounterCurrency, System.DateTime asOn)
        {
            var pair = new CurrencyPair(BaseMoney.Currency, CounterCurrency);
            var rate = this.GetExchangeRate(pair, asOn);

            return new Money(CounterCurrency, rate * BaseMoney.Amount);
        }

        public System.Collections.Generic.IEnumerable<CurrencyPair> GetCurrencyPairs(System.DateTime asOn)
        {
            ValidateDate(asOn);

            var result = new System.Collections.Generic.List<CurrencyPair>();

            var rates = GetRates(asOn);

            foreach (var SomeCurrency in rates.Keys)
            {
                result.Add(new CurrencyPair(UnitedStatesDollar, SomeCurrency));
                result.Add(new CurrencyPair(SomeCurrency, UnitedStatesDollar));
            }

            return result;
        }

        public decimal GetExchangeRate(CurrencyPair Pair, System.DateTime asOn)
        {
            ValidateDate(asOn);

            var rates = GetRates(asOn);

            if (Pair.BaseCurrency == UnitedStatesDollar)
            {
                if (rates.ContainsKey(Pair.CounterCurrency))
                    return rates[Pair.CounterCurrency];
            }
            else if (Pair.CounterCurrency == UnitedStatesDollar)
            {
                if (rates.ContainsKey(Pair.BaseCurrency))
                    return decimal.One / rates[Pair.BaseCurrency];
            }

            throw new System.ArgumentException("Currency pair not supported.");
        }

        private static System.Collections.Generic.Dictionary<CurrencyInfo, decimal> GetRates(System.DateTime asOn)
        {
            var rates = GetRawRates(asOn);
            var result = new System.Collections.Generic.Dictionary<CurrencyInfo, decimal>();

            foreach (var RawRate in rates)
            {
                var SomeCurrency = currencies.Single(C => C.ISOCurrencySymbol == RawRate.Key);

                result.Add(SomeCurrency, RawRate.Value);
            }

            return result;
        }

        private static System.Collections.Generic.Dictionary<string, decimal> GetRawRates(System.DateTime asOn)
        {
            string DataUrl = string.Format(DataUrlFormat, System.DateTime.Now.AddDays(-10d).ToString("MM/dd/yyyy"), System.DateTime.Now.ToString("MM/dd/yyyy"));

            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(DataUrl);

            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)System.Threading.Tasks.Task.Factory.FromAsync<System.Net.WebResponse>(request.BeginGetResponse, request.EndGetResponse, null).Result;

            System.Xml.Linq.XDocument xdoc = System.Xml.Linq.XDocument.Load(response.GetResponseStream());

            var result = new System.Collections.Generic.Dictionary<string, decimal>();

            foreach (var SeriesElement in xdoc.Element("{http://www.SDMX.org/resources/SDMXML/schemas/v1_0/message}MessageGroup").Element("{http://www.federalreserve.gov/structure/compact/common}DataSet").Elements("{http://www.federalreserve.gov/structure/compact/H10_H10}Series"))
            {
                var CurrencyCode = SeriesElement.Attribute("CURRENCY").Value;
                var FX = SeriesElement.Attribute("FX").Value;

                if (CurrencyCode != "NA")
                {
                    System.Collections.Generic.Dictionary<System.DateTime, decimal> rates = new System.Collections.Generic.Dictionary<System.DateTime, decimal>();

                    foreach (var ObsElement in SeriesElement.Elements("{http://www.federalreserve.gov/structure/compact/common}Obs"))
                    {
                        var ObsValue = decimal.Parse(ObsElement.Attribute("OBS_VALUE").Value);
                        var Period = System.DateTime.Parse(ObsElement.Attribute("TIME_PERIOD").Value);

                        // UNIT="Currency:_Per_USD"
                        decimal obsValueRate;

                        if (string.Equals(SeriesElement.Attribute("UNIT").Value, "Currency:_Per_USD", System.StringComparison.OrdinalIgnoreCase))
                        {
                            obsValueRate = ObsValue;
                        }
                        else
                        {
                            obsValueRate = decimal.One / ObsValue;
                        }

                        rates.Add(Period, obsValueRate);
                    }

                    var rate = rates[rates.Keys.Max()];

                    if (FX == "ZAL")
                    {
                        result.Add(CurrencyCode, rate);
                    }
                    else if (FX == "VEB")
                    {
                        result.Add("VEF", rate);
                    }
                    else
                    {
                        result.Add(FX, rate);
                    }
                }
            }

            return result;
        }

        private static void ValidateDate(System.DateTime asOn)
        {
            if (asOn > System.DateTime.Now)
                throw new System.ArgumentException("Exchange rate forecasting are not supported.");
        }
    }
}