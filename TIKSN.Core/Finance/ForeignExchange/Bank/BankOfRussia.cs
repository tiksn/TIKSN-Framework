using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using TIKSN.Globalization;
using TIKSN.Time;

namespace TIKSN.Finance.ForeignExchange.Bank
{
    public class BankOfRussia : ICurrencyConverter, IExchangeRatesProvider
    {
        private static readonly string AddressFormat = "http://www.cbr.ru/scripts/XML_daily.asp?date_req={0:00}.{1:00}.{2}";
        private static readonly CurrencyInfo RussianRuble;
        private static readonly CultureInfo RussianRussia;

        private DateTimeOffset? published;
        private Dictionary<CurrencyInfo, decimal> rates;
        private readonly ICurrencyFactory _currencyFactory;
        private readonly ITimeProvider _timeProvider;

        static BankOfRussia()
        {
            var russia = new RegionInfo("ru-RU");

            RussianRuble = new CurrencyInfo(russia);
            RussianRussia = new CultureInfo("ru-RU");
        }

        public BankOfRussia(ICurrencyFactory currencyFactory, ITimeProvider timeProvider)
        {
            this.rates = new Dictionary<CurrencyInfo, decimal>();
            this.published = null;
            _currencyFactory = currencyFactory;
            _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        }

        public async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency, DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            await this.FetchOnDemandAsync(asOn, cancellationToken);

            decimal rate = this.GetRate(baseMoney.Currency, counterCurrency);

            return new Money(counterCurrency, baseMoney.Amount * rate);
        }

        public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            await this.FetchOnDemandAsync(asOn, cancellationToken);

            var pairs = new List<CurrencyPair>();

            foreach (var foreignCurrency in this.rates.Keys)
            {
                pairs.Add(new CurrencyPair(foreignCurrency, RussianRuble));
                pairs.Add(new CurrencyPair(RussianRuble, foreignCurrency));
            }

            return pairs;
        }

        public async Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            await this.FetchOnDemandAsync(asOn, cancellationToken);

            decimal rate = this.GetRate(pair.BaseCurrency, pair.CounterCurrency);

            return rate;
        }

        public async Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            ValidateDate(asOn, _timeProvider);

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

        private async Task FetchOnDemandAsync(DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            ValidateDate(asOn, _timeProvider);

            if (!this.published.HasValue)
                await this.GetExchangeRatesAsync(asOn, cancellationToken);
            else if (this.published.Value != asOn.Date)
                await this.GetExchangeRatesAsync(asOn, cancellationToken);
        }

        private static void ValidateDate(DateTimeOffset asOn, ITimeProvider timeProvider)
        {
            if (asOn > timeProvider.GetCurrentTime())
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