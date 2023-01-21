using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TIKSN.Globalization;
using TIKSN.Time;

namespace TIKSN.Finance.ForeignExchange.Bank
{
    public class BankOfCanada : ICurrencyConverter, IExchangeRatesProvider
    {
        private const string RestURL = "https://www.bankofcanada.ca/valet/observations/group/FX_RATES_DAILY/json";
        private static readonly TimeZoneInfo bankTimeZone;
        private static readonly CurrencyInfo CanadianDollar;
        private readonly ICurrencyFactory _currencyFactory;
        private readonly ITimeProvider _timeProvider;
        private readonly Dictionary<DateTime, Dictionary<CurrencyInfo, decimal>> rates;
        private DateTimeOffset lastFetchDate;

        static BankOfCanada()
        {
            var Canada = new RegionInfo("en-CA");
            CanadianDollar = new CurrencyInfo(Canada);
            bankTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        }

        public BankOfCanada(ICurrencyFactory currencyFactory, ITimeProvider timeProvider)
        {
            this.rates = new Dictionary<DateTime, Dictionary<CurrencyInfo, decimal>>();

            this.lastFetchDate = DateTimeOffset.MinValue;
            this._currencyFactory = currencyFactory;
            this._timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        }

        public async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency,
            DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            var pair = new CurrencyPair(baseMoney.Currency, counterCurrency);

            var rate = await this.GetExchangeRateAsync(pair, asOn, cancellationToken).ConfigureAwait(false);

            return new Money(counterCurrency, baseMoney.Amount * rate);
        }

        public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            await this.FetchOnDemandAsync(cancellationToken).ConfigureAwait(false);

            if (asOn > this._timeProvider.GetCurrentTime())
            {
                throw new ArgumentException("Exchange rate forecasting are not supported.");
            }

            var result = new List<CurrencyPair>();

            foreach (var against in this.GetRatesByDate(asOn).Keys)
            {
                result.Add(new CurrencyPair(CanadianDollar, against));
                result.Add(new CurrencyPair(against, CanadianDollar));
            }

            return result;
        }

        public async Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            await this.FetchOnDemandAsync(cancellationToken).ConfigureAwait(false);

            if (asOn > this._timeProvider.GetCurrentTime())
            {
                throw new ArgumentException("Exchange rate forecasting not supported.");
            }

            if (this.IsHomeCurrencyPair(pair, asOn))
            {
                return decimal.One / this.GetRatesByDate(asOn)[pair.CounterCurrency];
            }

            return this.GetRatesByDate(asOn)[pair.BaseCurrency];
        }

        public async Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            var result = new List<ExchangeRate>();

            var ratesList = new List<Tuple<CurrencyInfo, DateTime, decimal>>();

            var rawData = await FetchRawDataAsync(RestURL).ConfigureAwait(false);

            var asOnDate = GetRatesDate(asOn);
            foreach (var rawItem in rawData)
            {
                var currency = this._currencyFactory.Create(rawItem.Item1);

                if (asOnDate == rawItem.Item2.Date)
                {
                    result.Add(new ExchangeRate(new CurrencyPair(currency, CanadianDollar), rawItem.Item2,
                        rawItem.Item3));
                }

                ratesList.Add(new Tuple<CurrencyInfo, DateTime, decimal>(currency, rawItem.Item2, rawItem.Item3));
            }

            lock (this.rates)
            {
                this.rates.Clear();

                foreach (var perDate in ratesList.GroupBy(item => item.Item2))
                {
                    this.rates.Add(perDate.Key, perDate.ToDictionary(k => k.Item1, v => v.Item3));
                }
            }

            this.lastFetchDate = this._timeProvider.GetCurrentTime(); // must stay at the end

            return result;
        }

        private static DateTimeOffset ConvertToBankTimeZone(DateTimeOffset date) =>
            TimeZoneInfo.ConvertTime(date, bankTimeZone);

        private static async Task<List<Tuple<string, DateTime, decimal>>> FetchRawDataAsync(string restUrl)
        {
            using var httpClient = new HttpClient();
            var responseStream = await httpClient.GetStreamAsync(restUrl).ConfigureAwait(false);

            using var streamReader = new StreamReader(responseStream);
            var jsonDoc = (JObject)JsonConvert.DeserializeObject(await streamReader.ReadToEndAsync().ConfigureAwait(false));

            var result = new List<Tuple<string, DateTime, decimal>>();

            foreach (var observation in jsonDoc.Children()
                .Single(item => string.Equals(item.Path, "observations", StringComparison.OrdinalIgnoreCase))
                .Children().Single().Children())
            {
                var asOn = observation.Value<DateTime>("d");

                foreach (JProperty observationProperty in observation.Children())
                {
                    if (observationProperty.Name.StartsWith("FX", StringComparison.OrdinalIgnoreCase))
                    {
                        Debug.Assert(observationProperty.Name.EndsWith("CAD"));

                        var targetCurrencyCode = observationProperty.Name.Substring(2, 3);

                        var valueObject = observationProperty.Value.Children().OfType<JProperty>()
                            .FirstOrDefault(item =>
                                string.Equals(item.Name, "v", StringComparison.OrdinalIgnoreCase));

                        if (valueObject != null)
                        {
                            var rate = (decimal)valueObject.Value;

                            result.Add(new Tuple<string, DateTime, decimal>(targetCurrencyCode, asOn, rate));
                        }
                    }
                }
            }

            return result;
        }

        private static DateTime GetRatesDate(DateTimeOffset asOn)
        {
            var date = ConvertToBankTimeZone(asOn).Date;

            if (date.DayOfWeek == DayOfWeek.Saturday)
            {
                return date.AddDays(-1);
            }

            if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                return date.AddDays(-2);
            }

            return date;
        }

        private async Task FetchOnDemandAsync(CancellationToken cancellationToken)
        {
            if (this._timeProvider.GetCurrentTime() - this.lastFetchDate > TimeSpan.FromDays(1d))
            {
                _ = await this.GetExchangeRatesAsync(this._timeProvider.GetCurrentTime(), cancellationToken).ConfigureAwait(false);
            }
        }

        private Dictionary<CurrencyInfo, decimal> GetRatesByDate(DateTimeOffset asOn)
        {
            _ = ConvertToBankTimeZone(this._timeProvider.GetCurrentTime());
            var date = GetRatesDate(asOn);

            if (this.rates.TryGetValue(date, out var valueAtDate))
            {
                return valueAtDate;
            }

            if (date.DayOfWeek == DayOfWeek.Saturday && this.rates.TryGetValue(date.AddDays(-1).Date, out var valueAtYesterday))
            {
                return valueAtYesterday;
            }

            if (date.DayOfWeek == DayOfWeek.Sunday && this.rates.TryGetValue(date.AddDays(-2).Date, out var valueAtTwoDaysAgo))
            {
                return valueAtTwoDaysAgo;
            }

            if (date.DayOfWeek == DayOfWeek.Monday && this.rates.TryGetValue(date.AddDays(-3).Date, out var valueAtThreeDaysAgo))
            {
                return valueAtThreeDaysAgo;
            }

            if (this.rates.TryGetValue(date.AddDays(-1).Date, out var otherwiseValueAtYesterday))
            {
                return otherwiseValueAtYesterday;
            }

            return this.rates[date]; // Exception will be thrown
        }

        private bool IsHomeCurrencyPair(CurrencyPair pair, DateTimeOffset asOn)
        {
            if (pair.BaseCurrency == CanadianDollar)
            {
                if (this.GetRatesByDate(asOn).Any(r => r.Key == pair.CounterCurrency))
                {
                    return true;
                }
            }
            else if (pair.CounterCurrency == CanadianDollar)
            {
                if (this.GetRatesByDate(asOn).Any(r => r.Key == pair.BaseCurrency))
                {
                    return false;
                }
            }

            throw new ArgumentException("Currency pair not supported.");
        }
    }
}
