﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using TIKSN.Globalization;

namespace TIKSN.Finance.ForeignExchange.Bank
{
    /// <summary>
    ///     Exchange rate converter of National Bank of Ukraine
    /// </summary>
    /// <seealso cref="TIKSN.Finance.ICurrencyConverter" />
    public class NationalBankOfUkraine : ICurrencyConverter, IExchangeRatesProvider
    {
        private const string WebServiceUrlFormat =
            "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?date={0:yyyyMMdd}";

        private static readonly string[] ignoreList = {"___"};
        private static readonly RegionInfo ukraine;
        private static readonly CultureInfo ukrainianCulture;
        private static readonly CurrencyInfo ukrainianHryvnia;
        private readonly ICurrencyFactory _currencyFactory;

        static NationalBankOfUkraine()
        {
            ukrainianCulture = new CultureInfo("uk-UA");
            ukraine = new RegionInfo("uk-UA");
            ukrainianHryvnia = new CurrencyInfo(ukraine);
        }

        public NationalBankOfUkraine(ICurrencyFactory currencyFactory) => this._currencyFactory = currencyFactory;

        /// <summary>
        ///     Converts the currency asynchronous.
        /// </summary>
        /// <param name="baseMoney">The base money.</param>
        /// <param name="counterCurrency">The counter currency.</param>
        /// <param name="asOn">As on.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency,
            DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            var rate = await this.GetExchangeRateAsync(baseMoney.Currency, counterCurrency, asOn, cancellationToken);

            return new Money(counterCurrency, baseMoney.Amount * rate);
        }

        /// <summary>
        ///     Gets the currency pairs asynchronous.
        /// </summary>
        /// <param name="asOn">As on.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            var records = await this.GetExchangeRatesAsync(asOn, cancellationToken);

            return records.Select(item => item.Pair).ToArray();
        }

        /// <summary>
        ///     Gets the exchange rate asynchronous.
        /// </summary>
        /// <param name="pair">The pair.</param>
        /// <param name="asOn">As on.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn,
            CancellationToken cancellationToken) =>
            this.GetExchangeRateAsync(pair.BaseCurrency, pair.CounterCurrency, asOn, cancellationToken);

        public async Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetStringAsync(string.Format(WebServiceUrlFormat, asOn));
                var xdocument = XDocument.Parse(response);
                var result = new HashSet<ExchangeRate>();

                foreach (var currencyElement in xdocument.Element("exchange").Elements("currency"))
                {
                    var currencyCode = currencyElement.Element("cc").Value;

                    if (ignoreList.Contains(currencyCode, StringComparer.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var rate = decimal.Parse(currencyElement.Element("rate").Value, CultureInfo.InvariantCulture);

                    if (!string.IsNullOrEmpty(currencyCode))
                    {
                        result.Add(new ExchangeRate(
                            new CurrencyPair(this._currencyFactory.Create(currencyCode), ukrainianHryvnia), asOn.Date,
                            rate));
                        result.Add(new ExchangeRate(
                            new CurrencyPair(ukrainianHryvnia, this._currencyFactory.Create(currencyCode)), asOn.Date,
                            decimal.One / rate));
                    }
                }

                return result;
            }
        }

        private Exception CreatePairNotSupportedException(CurrencyInfo baseCurrency, CurrencyInfo counterCurrency)
        {
            var ex = new NotSupportedException(
                "Currency pair is not supported by National Bank of Ukraine currency converter for given date.");

            ex.Data.Add("Base Currency", baseCurrency?.ISOCurrencySymbol);
            ex.Data.Add("Counter Currency", counterCurrency?.ISOCurrencySymbol);

            return ex;
        }

        private async Task<decimal> GetExchangeRateAsync(CurrencyInfo baseCurrency, CurrencyInfo counterCurrency,
            DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            if (counterCurrency == ukrainianHryvnia)
            {
                return await this.GetExchangeRateAsync(baseCurrency, asOn, cancellationToken);
            }

            if (baseCurrency == ukrainianHryvnia)
            {
                var counterRate = await this.GetExchangeRateAsync(counterCurrency, asOn, cancellationToken);

                return 1m / counterRate;
            }

            throw this.CreatePairNotSupportedException(baseCurrency, counterCurrency);
        }

        private async Task<decimal> GetExchangeRateAsync(CurrencyInfo currency, DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            var records = await this.GetExchangeRatesAsync(asOn, cancellationToken);
            var record = records.SingleOrDefault(item => item.Pair.BaseCurrency == currency);

            if (record == null)
            {
                throw this.CreatePairNotSupportedException(null, currency);
            }

            return record.Rate;
        }
    }
}
