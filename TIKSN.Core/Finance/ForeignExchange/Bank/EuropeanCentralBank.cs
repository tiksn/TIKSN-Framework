﻿using System;
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
    public class EuropeanCentralBank : ICurrencyConverter, IExchangeRatesProvider
    {                                                                                    //TODO: switch to http://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml see http://www.ecb.europa.eu/stats/exchange/eurofxref/html/index.en.html (For Developers section)
        private const string DailyRatesUrl = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";
        private const string Last90DaysRatesUrl = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-hist-90d.xml";
        private const string Since1999RatesUrl = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-hist.xml";

        private static readonly CurrencyInfo Euro;
        private readonly ICurrencyFactory _currencyFactory;
        private readonly ITimeProvider _timeProvider;

        static EuropeanCentralBank()
        {
            Euro = new CurrencyInfo(new RegionInfo("de-DE"));
        }

        public EuropeanCentralBank(ICurrencyFactory currencyFactory, ITimeProvider timeProvider)
        {
            _currencyFactory = currencyFactory;
            _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        }

        public async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency, DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            var pair = new CurrencyPair(baseMoney.Currency, counterCurrency);
            decimal rate = await this.GetExchangeRateAsync(pair, asOn, cancellationToken);

            return new Money(counterCurrency, baseMoney.Amount * rate);
        }

        public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            this.VerifyDate(asOn);

            var rates = await GetExchangeRatesAsync(asOn, cancellationToken);

            var result = new List<CurrencyPair>();

            foreach (var rate in rates)
            {
                result.Add(rate.Pair);
                result.Add(new CurrencyPair(rate.Pair.CounterCurrency, rate.Pair.BaseCurrency));
            }

            return result;
        }

        public async Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            this.VerifyDate(asOn);

            var rates = await GetExchangeRatesAsync(asOn, cancellationToken);

            var rate = rates.SingleOrDefault(item => item.Pair == pair);
            if (rate == null)
            {
                throw new ArgumentException($"Currency pair '{pair}' is not found.");
            }

            return rate.Rate;
        }

        public async Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            string requestURL = GetRatesUrl(asOn, _timeProvider);

            using (var httpClient = new HttpClient())
            {
                var responseStream = await httpClient.GetStreamAsync(requestURL);

                var xdoc = XDocument.Load(responseStream);

                var groupsCubes = xdoc.Element("{http://www.gesmes.org/xml/2002-08-01}Envelope").Element("{http://www.ecb.int/vocabulary/2002-08-01/eurofxref}Cube").Elements("{http://www.ecb.int/vocabulary/2002-08-01/eurofxref}Cube");

                //var groupsCubesCollection = groupsCubes.Select(item => new Tuple<XElement, DateTimeOffset>(item, DateTimeOffset.Parse(item.Attribute("time").Value)));

                //var closestCubeDateDifference = groupsCubesCollection.Where(item => item.Item2 <= asOn).Min(item => asOn - item.Item2);

                //var groupCubes = groupsCubesCollection.First(item => asOn - item.Item2 == closestCubeDateDifference);

                var groupCubes = groupsCubes.Select(x => new Tuple<XElement, DateTimeOffset>(x, DateTimeOffset.Parse(x.Attribute("time").Value))).Where(z => z.Item2 <= asOn).OrderByDescending(y => y.Item2).First();

                var rates = new List<ExchangeRate>();

                foreach (var rateCube in groupCubes.Item1.Elements("{http://www.ecb.int/vocabulary/2002-08-01/eurofxref}Cube"))
                {
                    var currencyCode = rateCube.Attribute("currency").Value;
                    var rate = decimal.Parse(rateCube.Attribute("rate").Value);

                    rates.Add(new ExchangeRate(new CurrencyPair(Euro, _currencyFactory.Create(currencyCode)), groupCubes.Item2, rate));
                }

                return rates;
            }
        }

        private static string GetRatesUrl(DateTimeOffset asOn, ITimeProvider timeProvider)
        {
            if (asOn.Date == timeProvider.GetCurrentTime().Date)
                return DailyRatesUrl;
            else if (asOn.Date >= timeProvider.GetCurrentTime().AddDays(-90))
                return Last90DaysRatesUrl;
            else
                return Since1999RatesUrl;
        }

        private void VerifyDate(DateTimeOffset asOn)
        {
            if (asOn > _timeProvider.GetCurrentTime())
                throw new ArgumentException("Exchange rate forecasting are not supported.");
        }
    }
}