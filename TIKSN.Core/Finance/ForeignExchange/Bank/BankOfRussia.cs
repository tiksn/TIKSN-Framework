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
        private const string AddressFormat =
            "https://www.cbr.ru/scripts/XML_daily.asp?date_req={0:00}.{1:00}.{2}";

        private static readonly CurrencyInfo RussianRuble;
        private static readonly CultureInfo RussianRussia;
        private readonly ICurrencyFactory _currencyFactory;
        private readonly ITimeProvider _timeProvider;
        private readonly Dictionary<CurrencyInfo, decimal> rates;

        private DateTimeOffset? published;

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
            this._currencyFactory = currencyFactory;
            this._timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        }

        public async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency,
            DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            await this.FetchOnDemandAsync(asOn, cancellationToken).ConfigureAwait(false);

            var rate = this.GetRate(baseMoney.Currency, counterCurrency);

            return new Money(counterCurrency, baseMoney.Amount * rate);
        }

        public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            await this.FetchOnDemandAsync(asOn, cancellationToken).ConfigureAwait(false);

            var pairs = new List<CurrencyPair>();

            foreach (var foreignCurrency in this.rates.Keys)
            {
                pairs.Add(new CurrencyPair(foreignCurrency, RussianRuble));
                pairs.Add(new CurrencyPair(RussianRuble, foreignCurrency));
            }

            return pairs;
        }

        public async Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            await this.FetchOnDemandAsync(asOn, cancellationToken).ConfigureAwait(false);

            var rate = this.GetRate(pair.BaseCurrency, pair.CounterCurrency);

            return rate;
        }

        public async Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            ValidateDate(asOn, this._timeProvider);

            var thatDay = asOn.Date;

            var address = string.Format(AddressFormat, thatDay.Day, thatDay.Month, thatDay.Year);

            var result = new List<ExchangeRate>();

            using (var httpClient = new HttpClient())
            {
                var responseStream = await httpClient.GetStreamAsync(address).ConfigureAwait(false);

                var stream​Reader = new Stream​Reader(responseStream, Encoding.UTF7);

                var xdoc = XDocument.Load(stream​Reader);

                lock (this.rates)
                {
                    this.rates.Clear();

                    foreach (var valuteElement in xdoc.Element("ValCurs").Elements("Valute"))
                    {
                        var charCodeElement = valuteElement.Element("CharCode");

                        if (charCodeElement == null)
                        {
                            continue;
                        }

                        var nominalElement = valuteElement.Element("Nominal");
                        var valueElement = valuteElement.Element("Value");

                        var code = charCodeElement.Value;

                        if (code == "NULL")
                        {
                            continue;
                        }

                        var currency = this._currencyFactory.Create(charCodeElement.Value);

                        var value = decimal.Parse(valueElement.Value, RussianRussia);
                        var nominal = decimal.Parse(nominalElement.Value, RussianRussia);

                        result.Add(new ExchangeRate(new CurrencyPair(currency, RussianRuble), thatDay,
                            value / nominal));
                        result.Add(new ExchangeRate(new CurrencyPair(RussianRuble, currency), thatDay,
                            nominal / value));

                        var rate = value / nominal;

                        this.rates.Add(currency, rate);
                    }

                    this.published = asOn.Date;
                }
            }

            return result;
        }

        private async Task FetchOnDemandAsync(DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            ValidateDate(asOn, this._timeProvider);

            if (!this.published.HasValue)
            {
                _ = await this.GetExchangeRatesAsync(asOn, cancellationToken).ConfigureAwait(false);
            }
            else if (this.published.Value != asOn.Date)
            {
                _ = await this.GetExchangeRatesAsync(asOn, cancellationToken).ConfigureAwait(false);
            }
        }

        private static void ValidateDate(DateTimeOffset asOn, ITimeProvider timeProvider)
        {
            if (asOn > timeProvider.GetCurrentTime())
            {
                throw new ArgumentException("Exchange rate forecasting not supported.");
            }
        }

        private decimal GetRate(CurrencyInfo baseCurrency, CurrencyInfo counterCurrency)
        {
            if (baseCurrency == RussianRuble)
            {
                if (this.rates.TryGetValue(counterCurrency, out var counterRate))
                {
                    return decimal.One / counterRate;
                }
            }
            else if (counterCurrency == RussianRuble)
            {
                if (this.rates.TryGetValue(baseCurrency, out var rate))
                {
                    return rate;
                }
            }

            throw new ArgumentException("Currency pair not supported.");
        }
    }
}
