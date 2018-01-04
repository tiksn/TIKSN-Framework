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

namespace TIKSN.Finance.ForeignExchange.Bank
{
    public class BankOfCanada : ICurrencyConverter, IExchangeRatesProvider
    {
        private const string RestURL = "https://www.bankofcanada.ca/valet/observations/group/FX_RATES_DAILY/json";
        private static CurrencyInfo CanadianDollar;

        private DateTimeOffset lastFetchDate;
        private Dictionary<DateTimeOffset, Dictionary<CurrencyInfo, decimal>> rates;
        private readonly ICurrencyFactory _currencyFactory;

        static BankOfCanada()
        {
            var Canada = new RegionInfo("en-CA");
            CanadianDollar = new CurrencyInfo(Canada);
        }

        public BankOfCanada(ICurrencyFactory currencyFactory)
        {
            rates = new Dictionary<DateTimeOffset, Dictionary<CurrencyInfo, decimal>>();

            lastFetchDate = DateTimeOffset.MinValue;
            _currencyFactory = currencyFactory;
        }

        public async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency, DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            var pair = new CurrencyPair(baseMoney.Currency, counterCurrency);

            var rate = await this.GetExchangeRateAsync(pair, asOn, cancellationToken);

            return new Money(counterCurrency, baseMoney.Amount * rate);
        }

        public async Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            var result = new List<ExchangeRate>();

            var ratesList = new List<Tuple<CurrencyInfo, DateTimeOffset, decimal>>();

            var rawData = await FetchRawDataAsync(RestURL, cancellationToken);

            var asOnDate = GetRatesDate(asOn);
            foreach (var rawItem in rawData)
            {
                var currency = _currencyFactory.Create(rawItem.Item1);

                if (asOnDate == rawItem.Item2.Date)
                    result.Add(new ExchangeRate(new CurrencyPair(currency, CanadianDollar), rawItem.Item2, rawItem.Item3));
                ratesList.Add(new Tuple<CurrencyInfo, DateTimeOffset, decimal>(currency, rawItem.Item2, rawItem.Item3));
            }

            lock (rates)
            {
                rates.Clear();

                foreach (var perDate in ratesList.GroupBy(item => item.Item2))
                {
                    rates.Add(perDate.Key, perDate.ToDictionary(k => k.Item1, v => v.Item3));
                }
            }

            lastFetchDate = DateTime.Now; // must stay at the end

            return result;
        }

        public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            await FetchOnDemandAsync(cancellationToken);

            if (asOn > DateTime.Now)
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

            if (asOn > DateTimeOffset.Now)
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

        private async Task FetchOnDemandAsync(CancellationToken cancellationToken)
        {
            if (DateTimeOffset.Now - lastFetchDate > TimeSpan.FromDays(1d))
            {
                await GetExchangeRatesAsync(DateTimeOffset.Now, cancellationToken);
            }
        }

        private async Task<List<Tuple<string, DateTimeOffset, decimal>>> FetchRawDataAsync(string restUrl, CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            {
                var responseStream = await httpClient.GetStreamAsync(restUrl);

                using (var streamReader = new StreamReader(responseStream))
                {
                    var jsonDoc = (JObject)JsonConvert.DeserializeObject(await streamReader.ReadToEndAsync());

                    var result = new List<Tuple<string, DateTimeOffset, decimal>>();

                    foreach (var observation in jsonDoc.Children().Single(item => string.Equals(item.Path, "observations", StringComparison.OrdinalIgnoreCase)).Children().Single().Children())
                    {
                        var asOn = new DateTimeOffset(observation.Value<DateTime>("d"));

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

                                    result.Add(new Tuple<string, DateTimeOffset, decimal>(targetCurrencyCode, asOn, rate));
                                }
                            }
                        }
                    }

                    return result;
                }
            }
        }

        private DateTimeOffset GetRatesDate(DateTimeOffset asOn)
        {
            var date = asOn.Date;

            if (date.DayOfWeek == DayOfWeek.Saturday)
                date = date.AddDays(-1);
            else if (date.DayOfWeek == DayOfWeek.Sunday)
                date = date.AddDays(-2);

            return date;
        }

        private Dictionary<CurrencyInfo, decimal> GetRatesByDate(DateTimeOffset asOn)
        {
            var date = GetRatesDate(asOn);

            return rates[date];
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