﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TIKSN.Finance.ForeignExchange
{
    public class BankOfRussia : ICurrencyConverter
    {
        private static readonly string AddressFormat = "http://www.cbr.ru/scripts/XML_daily.asp?date_req={0:00}.{1:00}.{2}";
        private static readonly CurrencyInfo RussianRuble;
        private static readonly CultureInfo RussianRussia;

        private DateTime? published;
        private Dictionary<CurrencyInfo, decimal> rates;

        static BankOfRussia()
        {
            var russia = new RegionInfo("ru-RU");

            RussianRuble = new CurrencyInfo(russia);
            RussianRussia = new CultureInfo("ru-RU");
        }

        public BankOfRussia()
        {
            this.rates = new Dictionary<CurrencyInfo, decimal>();
            this.published = null;
        }

        public async Task<Money> ConvertCurrencyAsync(Money BaseMoney, CurrencyInfo CounterCurrency, DateTimeOffset asOn)
        {
            await this.FetchOnDemandAsync(asOn);

            decimal rate = this.GetRate(BaseMoney.Currency, CounterCurrency);

            return new Money(CounterCurrency, BaseMoney.Amount * rate);
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

        public async Task<decimal> GetExchangeRateAsync(CurrencyPair Pair, DateTimeOffset asOn)
        {
            await this.FetchOnDemandAsync(asOn);

            decimal rate = this.GetRate(Pair.BaseCurrency, Pair.CounterCurrency);

            return rate;
        }

        private async Task FetchAsync(DateTimeOffset asOn)
        {
            var thatDay = asOn.Date;

            string address = string.Format(AddressFormat, thatDay.Day, thatDay.Month, thatDay.Year);

            HttpWebRequest request = WebRequest.CreateHttp(address);

            HttpWebResponse response = (HttpWebResponse)await Task.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null);

            var xdoc = XDocument.Load(response.GetResponseStream());

            lock (this.rates)
            {
                this.rates.Clear();

                foreach (var ValuteElement in xdoc.Element("ValCurs").Elements("Valute"))
                {
                    var charCodeElement = ValuteElement.Element("CharCode");
                    var nominalElement = ValuteElement.Element("Nominal");
                    var valueElement = ValuteElement.Element("Value");

                    var code = charCodeElement.Value;

                    if (code == "XDR" || code == "ATS" || code == "BEF" || code == "GRD" || code == "IEP" || code == "ESP" || code == "ITL" || code == "DEM" || code == "NLG" || code == "PTE" || code == "TRL" || code == "FIM" || code == "FRF" || code == "XEU" || code == "NULL" || code == "EEK" || code == "LVL" || code == "BYB" || code == "AZM")
                        continue;

                    var Currency = new CurrencyInfo(charCodeElement.Value);
                    decimal rate = decimal.Parse(valueElement.Value, RussianRussia) / decimal.Parse(nominalElement.Value, RussianRussia);

                    this.rates.Add(Currency, rate);
                }

                this.published = asOn.Date;
            }
        }

        private async Task FetchOnDemandAsync(DateTimeOffset asOn)
        {
            if (asOn > DateTimeOffset.Now)
                throw new ArgumentException("Exchange rate forecasting not supported.");

            if (!this.published.HasValue)
                await this.FetchAsync(asOn);
            else if (this.published.Value != asOn.Date)
                await this.FetchAsync(asOn);
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