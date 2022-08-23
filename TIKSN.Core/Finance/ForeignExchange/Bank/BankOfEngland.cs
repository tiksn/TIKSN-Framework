using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using TIKSN.Globalization;
using TIKSN.Time;

namespace TIKSN.Finance.ForeignExchange.Bank
{
    public class BankOfEngland : ICurrencyConverter, IExchangeRateProvider, IExchangeRatesProvider
    {
        private const string UrlFormat =
            "https://www.bankofengland.co.uk/boeapps/iadb/fromshowcolumns.asp?CodeVer=new&xml.x=yes&Datefrom={0}&Dateto={1}&SeriesCodes={2}";

        private static readonly Dictionary<string, CurrencyPair> Pairs;
        private static readonly Dictionary<CurrencyPair, string> SeriesCodes;
        private readonly ICurrencyFactory currencyFactory;
        private readonly IRegionFactory regionFactory;
        private readonly ITimeProvider timeProvider;

        static BankOfEngland()
        {
            SeriesCodes = new Dictionary<CurrencyPair, string>();
            Pairs = new Dictionary<string, CurrencyPair>();

            AddSeriesCode("en-AU", "en-US", "XUDLADD");
            AddSeriesCode("en-AU", "en-GB", "XUDLADS");

            AddSeriesCode("en-CA", "en-GB", "XUDLCDS");

            AddSeriesCode("zh-CN", "en-US", "XUDLBK73");

            AddSeriesCode("cs-CZ", "en-US", "XUDLBK27");
            AddSeriesCode("cs-CZ", "en-GB", "XUDLBK25");

            AddSeriesCode("da-DK", "en-US", "XUDLDKD");
            AddSeriesCode("da-DK", "en-GB", "XUDLDKS");

            AddSeriesCode("de-DE", "en-US", "XUDLERD");
            AddSeriesCode("de-DE", "en-GB", "XUDLERS");

            AddSeriesCode("zh-HK", "en-US", "XUDLHDD");
            AddSeriesCode("zh-HK", "en-GB", "XUDLHDS");

            AddSeriesCode("hu-HU", "en-US", "XUDLBK35");
            AddSeriesCode("hu-HU", "en-GB", "XUDLBK33");

            AddSeriesCode("hi-IN", "en-GB", "XUDLBK97");
            AddSeriesCode("hi-IN", "en-US", "XUDLBK64");

            AddSeriesCode("he-IL", "en-GB", "XUDLBK78");
            AddSeriesCode("he-IL", "en-US", "XUDLBK65");

            AddSeriesCode("ja-JP", "en-US", "XUDLJYD");
            AddSeriesCode("ja-JP", "en-GB", "XUDLJYS");

            //AddSeriesCode("LV", "en-US", "XUDLBK43");
            //AddSeriesCode("LV", "de-DE", "XUDLBK42");
            //AddSeriesCode("LV", "en-GB", "XUDLBK39");

            //AddSeriesCode("lt-LT", "en-US", "XUDLBK38");
            //AddSeriesCode("lt-LT", "de-DE", "XUDLBK37");
            //AddSeriesCode("lt-LT", "en-GB", "XUDLBK36");

            AddSeriesCode("ms-MY", "en-GB", "XUDLBK83");
            AddSeriesCode("ms-MY", "en-US", "XUDLBK66");

            AddSeriesCode("en-NZ", "en-US", "XUDLNDD");
            AddSeriesCode("en-NZ", "en-GB", "XUDLNDS");

            AddSeriesCode("nn-NO", "en-US", "XUDLNKD");
            AddSeriesCode("nn-NO", "en-GB", "XUDLNKS");

            AddSeriesCode("pl-PL", "en-US", "XUDLBK49");
            AddSeriesCode("pl-PL", "en-GB", "XUDLBK47");

            // AddSeriesCode("ru-RU", "en-GB", "XUDLBK85");
            // AddSeriesCode("ru-RU", "en-US", "XUDLBK69");

            AddSeriesCode("ar-SA", "en-US", "XUDLSRD");
            AddSeriesCode("ar-SA", "en-GB", "XUDLSRS");

            AddSeriesCode("zh-SG", "en-US", "XUDLSGD");
            AddSeriesCode("zh-SG", "en-GB", "XUDLSGS");

            AddSeriesCode("af-ZA", "en-US", "XUDLZRD");
            AddSeriesCode("af-ZA", "en-GB", "XUDLZRS");

            AddSeriesCode("ko-KR", "en-GB", "XUDLBK93");
            AddSeriesCode("ko-KR", "en-US", "XUDLBK74");

            AddSeriesCode("en-GB", "en-US", "XUDLGBD");

            AddSeriesCode("se-SE", "en-US", "XUDLSKD");
            AddSeriesCode("se-SE", "en-GB", "XUDLSKS");

            AddSeriesCode("de-CH", "en-US", "XUDLSFD");
            AddSeriesCode("de-CH", "en-GB", "XUDLSFS");

            AddSeriesCode("zh-TW", "en-US", "XUDLTWD");
            AddSeriesCode("zh-TW", "en-GB", "XUDLTWS");

            AddSeriesCode("th-TH", "en-GB", "XUDLBK87");
            AddSeriesCode("th-TH", "en-US", "XUDLBK72");

            AddSeriesCode("tr-TR", "en-GB", "XUDLBK95");
            AddSeriesCode("tr-TR", "en-US", "XUDLBK75");

            AddSeriesCode("en-US", "en-GB", "XUDLUSS");

            //AddSeriesCode("pt-BR", "en-US", "XUDLB8KL");

            AddSeriesCode("zh-CN", "en-GB", "XUDLBK89");
        }

        public BankOfEngland(ICurrencyFactory currencyFactory, IRegionFactory regionFactory, ITimeProvider timeProvider)
        {
            this.currencyFactory = currencyFactory;
            this.regionFactory = regionFactory;
            this.timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        }

        public async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency,
            DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            var rate = (await this.GetExchangeRateAsync(baseMoney.Currency, counterCurrency, asOn, cancellationToken).ConfigureAwait(false))
                .Rate;

            return new Money(counterCurrency, baseMoney.Amount * rate);
        }

        public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            var pairs = new List<CurrencyPair>();

            foreach (var pair in SeriesCodes.Keys)
            {
                var rate = await this.GetExchangeRateAsync(pair, asOn, cancellationToken).ConfigureAwait(false);

                if (rate != decimal.Zero)
                {
                    pairs.Add(pair);
                }
            }

            return pairs;
        }

        public async Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn,
            CancellationToken cancellationToken) =>
            (await this.GetExchangeRateAsync(pair.BaseCurrency, pair.CounterCurrency, asOn, cancellationToken).ConfigureAwait(false)).Rate;

        public async Task<ExchangeRate> GetExchangeRateAsync(
            CurrencyInfo baseCurrency,
            CurrencyInfo counterCurrency,
            DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            if (asOn > this.timeProvider.GetCurrentTime())
            {
                throw new ArgumentException("Exchange rate forecasting are not supported.");
            }

            string seriesCode;
            var pair = new CurrencyPair(baseCurrency, counterCurrency);

            try
            {
                seriesCode = SeriesCodes[pair];
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException("Currency pair not supported.");
            }

            var exchangeRates = await this.GetSeriesCodeExchangeRateAsync(seriesCode, asOn, cancellationToken).ConfigureAwait(false);

            return exchangeRates
                .Where(x => x.Pair == pair)
                .MinByWithTies(x => x.AsOn - asOn)
                .First();
        }

        public async Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            List<ExchangeRate> rates = new();
            foreach (var seriesCode in SeriesCodes)
            {
                var exchangeRates = await this.GetSeriesCodeExchangeRateAsync(seriesCode.Value, asOn, cancellationToken).ConfigureAwait(false);
                rates.AddRange(exchangeRates);
            }

            return rates;
        }

        private static void AddSeriesCode(string baseCountryCode, string counterCountryCode, string serieCode)
        {
            var baseCountry = new RegionInfo(baseCountryCode);
            var counterCountry = new RegionInfo(counterCountryCode);

            var baseCurrency = new CurrencyInfo(baseCountry);
            var counterCurrency = new CurrencyInfo(counterCountry);

            var pair = new CurrencyPair(baseCurrency, counterCurrency);

            SeriesCodes.Add(pair, serieCode);
            Pairs.Add(serieCode, pair);
        }

        private static string ToInternalDataFormat(DateTimeOffset dt) =>
            dt.ToString("dd/MMM/yyyy", CultureInfo.InvariantCulture);

        private async Task<IReadOnlyList<ExchangeRate>> GetSeriesCodeExchangeRateAsync(
            string seriesCode,
            DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            var pair = Pairs[seriesCode];

            if (asOn > this.timeProvider.GetCurrentTime())
            {
                throw new ArgumentException("Exchange rate forecasting are not supported.");
            }

            var requestUrl = string.Format(UrlFormat,
                ToInternalDataFormat(asOn.AddMonths(-1)),
                ToInternalDataFormat(asOn), seriesCode);

            using var httpClient = new HttpClient();
            var responseStream = await httpClient.GetStreamAsync(requestUrl).ConfigureAwait(false);

            var xdoc = XDocument.Load(responseStream);

            List<ExchangeRate> rates = new();

            foreach (var item in xdoc.Element("{http://www.gesmes.org/xml/2002-08-01}Envelope")
                .Element("{https://www.bankofengland.co.uk/website/agg_series}Cube")
                .Elements("{https://www.bankofengland.co.uk/website/agg_series}Cube"))
            {
                var time = item.Attribute("TIME");
                if (time is not null)
                {
                    var estimatedRate = item.Attribute("OBS_VALUE");

                    if (time != null && estimatedRate != null)
                    {
                        var year = int.Parse(time.Value.Substring(0, 4), CultureInfo.InvariantCulture);
                        var month = int.Parse(time.Value.Substring(5, 2), CultureInfo.InvariantCulture);
                        var day = int.Parse(time.Value.Substring(8), CultureInfo.InvariantCulture);

                        var itemTime = new DateTimeOffset(year, month, day, 0, 0, 0, TimeSpan.Zero);

                        var reverseRate = decimal.Parse(estimatedRate.Value, CultureInfo.InvariantCulture);
                        var rate = decimal.One / reverseRate;

                        rates.Add(new ExchangeRate(pair, itemTime, rate));
                        rates.Add(new ExchangeRate(pair.Reverse(), itemTime, reverseRate));
                    }
                }
            }

            return rates;
        }
    }
}
