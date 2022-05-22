namespace TIKSN.Finance.Tests
{
    public static class Helper
    {
        public static CurrencyInfo SampleCurrency1
        {
            get
            {
                return new CurrencyInfo(new System.Globalization.RegionInfo("US"));
            }
        }

        public static CurrencyInfo SampleCurrency2
        {
            get
            {
                return new CurrencyInfo(new System.Globalization.RegionInfo("DE"));
            }
        }

        public static CurrencyInfo SampleCurrency3
        {
            get
            {
                return new CurrencyInfo(new System.Globalization.RegionInfo("GB"));
            }
        }

        public static CurrencyPair SampleCurrencyPair1
        {
            get
            {
                return new CurrencyPair(SampleCurrency1, SampleCurrency2);
            }
        }

        public static CurrencyPair SampleCurrencyPair2
        {
            get
            {
                return new CurrencyPair(SampleCurrency1, SampleCurrency3);
            }
        }

        public static CurrencyPair SampleCurrencyPair3
        {
            get
            {
                return new CurrencyPair(SampleCurrency2, SampleCurrency3);
            }
        }

        public static System.Collections.Generic.IEnumerable<CurrencyPair> SampleCurrencyPairs1
        {
            get
            {
                var result = new System.Collections.Generic.List<CurrencyPair>();

                result.Add(SampleCurrencyPair1);
                result.Add(SampleCurrencyPair2);
                result.Add(SampleCurrencyPair3);

                return result;
            }
        }

        public static System.Collections.Generic.IEnumerable<CurrencyPair> SampleCurrencyPairs2
        {
            get
            {
                var result = new System.Collections.Generic.List<CurrencyPair>();

                result.Add(SampleCurrencyPair1);
                result.Add(SampleCurrencyPair2);

                return result;
            }
        }

        public static System.Collections.Generic.IEnumerable<CurrencyPair> SampleCurrencyPairs3
        {
            get
            {
                var result = new System.Collections.Generic.List<CurrencyPair>();

                result.Add(SampleCurrencyPair1);

                return result;
            }
        }

        public static Money SampleMoney1
        {
            get
            {
                return new Money(SampleCurrency1, 100m);
            }
        }

        public static decimal GetRandomForeignExchangeRate()
        {
            var RNG = new System.Random();

            return (decimal)RNG.NextDouble();
        }
    }
}
