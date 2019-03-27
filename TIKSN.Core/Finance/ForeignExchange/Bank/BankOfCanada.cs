using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TIKSN.Globalization;
using TIKSN.Time;

namespace TIKSN.Finance.ForeignExchange.Bank
{
    public class BankOfCanada : ICurrencyConverter, IExchangeRatesProvider
    {
        private const string RestURL = "https://www.bankofcanada.ca/valet/observations/group/FX_RATES_DAILY/json";
        private static TimeZoneInfo bankTimeZone;
        private static CurrencyInfo CanadianDollar;
        private readonly ICurrencyFactory _currencyFactory;
        private readonly ITimeProvider _timeProvider;
        private DateTimeOffset lastFetchDate;
        private Dictionary<DateTime, Dictionary<CurrencyInfo, decimal>> rates;

        static BankOfCanada()
        {
            var Canada = new RegionInfo("en-CA");
            CanadianDollar = new CurrencyInfo(Canada);
            bankTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        }

        public BankOfCanada(ICurrencyFactory currencyFactory, ITimeProvider timeProvider)
        {
            rates = new Dictionary<DateTime, Dictionary<CurrencyInfo, decimal>>();

            lastFetchDate = DateTimeOffset.MinValue;
            _currencyFactory = currencyFactory;
            _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        }

        public async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency, DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            var pair = new CurrencyPair(baseMoney.Currency, counterCurrency);

            var rate = await this.GetExchangeRateAsync(pair, asOn, cancellationToken);

            return new Money(counterCurrency, baseMoney.Amount * rate);
        }

        public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            await FetchOnDemandAsync(cancellationToken);

            if (asOn > _timeProvider.GetCurrentTime())
                throw new ArgumentException("Exchange rate forecasting are not supported.");

            var result = new List<CurrencyPair>();

            foreach (CurrencyInfo against in GetRatesByDate(asOn).Keys)
            {
                result.Add(new CurrencyPair(CanadianDollar, against));
                result.Add(new CurrencyPair(against, CanadianDollar));
            }

            return result;
        }

        public async Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            await FetchOnDemandAsync(cancellationToken);

            if (asOn > _timeProvider.GetCurrentTime())
                throw new ArgumentException("Exchange rate forecasting not supported.");

            if (IsHomeCurrencyPair(pair, asOn))
            {
                return decimal.One / GetRatesByDate(asOn)[pair.CounterCurrency];
            }
            else
            {
                return GetRatesByDate(asOn)[pair.BaseCurrency];
            }
        }

        public async Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            var result = new List<ExchangeRate>();

            var ratesList = new List<Tuple<CurrencyInfo, DateTime, decimal>>();

            var rawData = await FetchRawDataAsync(RestURL, cancellationToken);

            var asOnDate = GetRatesDate(asOn);
            foreach (var rawItem in rawData)
            {
                var currency = _currencyFactory.Create(rawItem.Item1);

                if (asOnDate == rawItem.Item2.Date)
                    result.Add(new ExchangeRate(new CurrencyPair(currency, CanadianDollar), rawItem.Item2, rawItem.Item3));
                ratesList.Add(new Tuple<CurrencyInfo, DateTime, decimal>(currency, rawItem.Item2, rawItem.Item3));
            }

            lock (rates)
            {
                rates.Clear();

                foreach (var perDate in ratesList.GroupBy(item => item.Item2))
                {
                    rates.Add(perDate.Key, perDate.ToDictionary(k => k.Item1, v => v.Item3));
                }
            }

            lastFetchDate = _timeProvider.GetCurrentTime(); // must stay at the end

            return result;
        }

        private static DateTimeOffset ConvertToBankTimeZone(DateTimeOffset date)
        {
            return TimeZoneInfo.ConvertTime(date, bankTimeZone);
        }

        private async Task FetchOnDemandAsync(CancellationToken cancellationToken)
        {
            if (_timeProvider.GetCurrentTime() - lastFetchDate > TimeSpan.FromDays(1d))
            {
                await GetExchangeRatesAsync(_timeProvider.GetCurrentTime(), cancellationToken);
            }
        }

        private async Task<List<Tuple<string, DateTime, decimal>>> FetchRawDataAsync(string restUrl, CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            {
                var responseStream = await httpClient.GetStreamAsync(restUrl);

                using (var streamReader = new StreamReader(responseStream))
                {
                    var jsonDoc = (JObject)JsonConvert.DeserializeObject(await streamReader.ReadToEndAsync());

                    var result = new List<Tuple<string, DateTime, decimal>>();

                    foreach (var observation in jsonDoc.Children().Single(item => string.Equals(item.Path, "observations", StringComparison.OrdinalIgnoreCase)).Children().Single().Children())
                    {
                        var asOn = observation.Value<DateTime>("d");

                        foreach (JProperty observationProperty in observation.Children())
                        {
                            if (observationProperty.Name.StartsWith("FX", StringComparison.OrdinalIgnoreCase))
                            {
                                Debug.Assert(observationProperty.Name.EndsWith("CAD"));

                                var targetCurrencyCode = observationProperty.Name.Substring(2, 3);

                                var valueObject = observationProperty.Value.Children().OfType<JProperty>().FirstOrDefault(item => string.Equals(item.Name, "v", StringComparison.OrdinalIgnoreCase));

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
            }
        }

        private Dictionary<CurrencyInfo, decimal> GetRatesByDate(DateTimeOffset asOn)
        {
            var nowInBuilding = ConvertToBankTimeZone(_timeProvider.GetCurrentTime());
            var date = GetRatesDate(asOn);

            if (rates.ContainsKey(date))
                return rates[date];
            else if (nowInBuilding.Date == date && nowInBuilding.TimeOfDay < TimeSpan.FromHours(12))
                return rates[nowInBuilding.AddHours(-12).Date];

            return rates[date]; // Exception will be thrown
        }

        private DateTime GetRatesDate(DateTimeOffset asOn)
        {
            var date = ConvertToBankTimeZone(asOn).Date;

            if (date.DayOfWeek == DayOfWeek.Saturday)
                date = date.AddDays(-1);
            else if (date.DayOfWeek == DayOfWeek.Sunday)
                date = date.AddDays(-2);

            return date;
        }

        private bool IsHomeCurrencyPair(CurrencyPair Pair, DateTimeOffset asOn)
        {
            if (Pair.BaseCurrency == CanadianDollar)
            {
                if (GetRatesByDate(asOn).Any(R => R.Key == Pair.CounterCurrency))
                    return true;
            }
            else if (Pair.CounterCurrency == CanadianDollar)
            {
                if (GetRatesByDate(asOn).Any(R => R.Key == Pair.BaseCurrency))
                    return false;
            }

            throw new ArgumentException("Currency pair not supported.");
        }
    }
}