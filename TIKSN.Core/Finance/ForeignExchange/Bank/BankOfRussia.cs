using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TIKSN.Globalization;

namespace TIKSN.Finance.ForeignExchange.Bank
{
    public class BankOfRussia : ICurrencyConverter, IExchangeRateProvider
    {
        private static readonly string AddressFormat = "http://www.cbr.ru/scripts/XML_daily.asp?date_req={0:00}.{1:00}.{2}";
        private static readonly CurrencyInfo RussianRuble;
        private static readonly CultureInfo RussianRussia;

        private DateTime? published;
        private Dictionary<CurrencyInfo, decimal> rates;
        private readonly ICurrencyFactory _currencyFactory;

        static BankOfRussia()
        {
            var russia = new RegionInfo("ru-RU");

            RussianRuble = new CurrencyInfo(russia);
            RussianRussia = new CultureInfo("ru-RU");
        }

        public BankOfRussia(ICurrencyFactory currencyFactory)
        {
            this.rates = new Dictionary<CurrencyInfo, decimal>();
            this.published = null;
            _currencyFactory = currencyFactory;
        }

        public async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency, DateTimeOffset asOn)
        {
            await this.FetchOnDemandAsync(asOn);

            decimal rate = this.GetRate(baseMoney.Currency, counterCurrency);

            return new Money(counterCurrency, baseMoney.Amount * rate);
        }

        public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn)
        {
            await this.FetchOnDemandAsync(asOn);

            var pairs = new List<CurrencyPair>();

            foreach (var foreignCurrency in this.rates.Keys)
            {
                pairs.Add(new CurrencyPair(foreignCurrency, RussianRuble));
                pairs.Add(new CurrencyPair(RussianRuble, foreignCurrency));
            }

            return pairs;
        }

        public async Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn)
        {
            await this.FetchOnDemandAsync(asOn);

            decimal rate = this.GetRate(pair.BaseCurrency, pair.CounterCurrency);

            return rate;
        }

        public async Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(DateTimeOffset asOn)
        {
            ValidateDate(asOn);

            var thatDay = asOn.Date;

            string address = string.Format(AddressFormat, thatDay.Day, thatDay.Month, thatDay.Year);

            var result = new List<ExchangeRate>();

            using (var httpClient = new HttpClient())
            {
                var responseStream = await httpClient.GetStreamAsync(address);

                var stream​Reader = new Stream​Reader(responseStream, Encoding.UTF7);

                var xdoc = XDocument.Load(stream​Reader);

                lock (this.rates)
                {
                    this.rates.Clear();

                    foreach (var ValuteElement in xdoc.Element("ValCurs").Elements("Valute"))
                    {
                        var charCodeElement = ValuteElement.Element("CharCode");
                        var nominalElement = ValuteElement.Element("Nominal");
                        var valueElement = ValuteElement.Element("Value");

                        var code = charCodeElement.Value;

                        if (code == "NULL")
                            continue;

                        var currency = _currencyFactory.Create(charCodeElement.Value);

                        var value = decimal.Parse(valueElement.Value, RussianRussia);
                        var nominal = decimal.Parse(nominalElement.Value, RussianRussia);

                        result.Add(new ExchangeRate(new CurrencyPair(currency, RussianRuble), thatDay, value / nominal));
                        result.Add(new ExchangeRate(new CurrencyPair(RussianRuble, currency), thatDay, nominal / value));

                        decimal rate = value / nominal;

                        this.rates.Add(currency, rate);
                    }

                    this.published = asOn.Date;
                }
            }

            return result;
        }

        private async Task FetchOnDemandAsync(DateTimeOffset asOn)
        {
            ValidateDate(asOn);

            if (!this.published.HasValue)
                await this.GetExchangeRatesAsync(asOn);
            else if (this.published.Value != asOn.Date)
                await this.GetExchangeRatesAsync(asOn);
        }

        private static void ValidateDate(DateTimeOffset asOn)
        {
            if (asOn > DateTimeOffset.Now)
                throw new ArgumentException("Exchange rate forecasting not supported.");
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

            throw new ArgumentException("Currency pair not supported.");
        }
    }
}