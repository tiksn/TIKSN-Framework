using System.Linq;

namespace TIKSN.Finance.Tests.ForeignExchange
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class FederalReserveSystemTests
    {
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void Calculation001()
        {
            var Bank = new Finance.ForeignExchange.FederalReserveSystem();

            var pairs = Bank.GetCurrencyPairs(System.DateTime.Now);

            foreach (var pair in pairs)
            {
                var Before = new Money(pair.BaseCurrency);
                var rate = Bank.GetExchangeRate(pair, System.DateTime.Now);

                var After = Bank.ConvertCurrency(Before, pair.CounterCurrency, System.DateTime.Now);

                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(After.Amount == rate * Before.Amount);
            }
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void ConversionDirection001()
        {
            var Bank = new Finance.ForeignExchange.FederalReserveSystem();

            var USDollar = new CurrencyInfo(new System.Globalization.RegionInfo("US"));
            var PoundSterling = new CurrencyInfo(new System.Globalization.RegionInfo("GB"));

            var BeforeInPound = new Money(PoundSterling, 100m);

            var AfterInDollar = Bank.ConvertCurrency(BeforeInPound, USDollar, System.DateTime.Now);

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(BeforeInPound.Amount < AfterInDollar.Amount);
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void ConvertCurrency001()
        {
            var Bank = new Finance.ForeignExchange.FederalReserveSystem();

            foreach (var pair in Bank.GetCurrencyPairs(System.DateTime.Now))
            {
                var Before = new Money(pair.BaseCurrency, 10m);
                var After = Bank.ConvertCurrency(Before, pair.CounterCurrency, System.DateTime.Now);

                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(After.Amount > decimal.Zero);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(After.Currency == pair.CounterCurrency);
            }
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void ConvertCurrency002()
        {
            var Bank = new Finance.ForeignExchange.FederalReserveSystem();

            var LastYear = System.DateTime.Now.AddYears(-1);

            foreach (var pair in Bank.GetCurrencyPairs(LastYear))
            {
                var Before = new Money(pair.BaseCurrency, 10m);
                var After = Bank.ConvertCurrency(Before, pair.CounterCurrency, LastYear);

                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(After.Amount > decimal.Zero);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(After.Currency == pair.CounterCurrency);
            }
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void ConvertCurrency003()
        {
            var Bank = new Finance.ForeignExchange.FederalReserveSystem();

            foreach (var pair in Bank.GetCurrencyPairs(System.DateTime.Now))
            {
                var Before = new Money(pair.BaseCurrency, 10m);

                try
                {
                    var After = Bank.ConvertCurrency(Before, pair.CounterCurrency, System.DateTime.Now.AddMinutes(1d));

                    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
                }
                catch (System.ArgumentException)
                {
                }
                catch (System.Exception ex)
                {
                    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail(ex.Message);
                }
            }
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void ConvertCurrency004()
        {
            var Bank = new Finance.ForeignExchange.FederalReserveSystem();

            var Before = new Money(new CurrencyInfo(new System.Globalization.RegionInfo("AL")), 10m);

            try
            {
                var After = Bank.ConvertCurrency(Before, new CurrencyInfo(new System.Globalization.RegionInfo("AM")), System.DateTime.Now.AddMinutes(1d));

                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
            }
            catch (System.ArgumentException)
            {
            }
            catch (System.Exception ex)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail(ex.Message);
            }
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void GetCurrencyPairs001()
        {
            var Bank = new Finance.ForeignExchange.FederalReserveSystem();

            var pairs = Bank.GetCurrencyPairs(System.DateTime.Now);

            foreach (var pair in pairs)
            {
                var reversed = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C == reversed));
            }
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void GetCurrencyPairs002()
        {
            var Bank = new Finance.ForeignExchange.FederalReserveSystem();

            var pairs = Bank.GetCurrencyPairs(System.DateTime.Now);

            var uniquePairs = new System.Collections.Generic.HashSet<CurrencyPair>();

            foreach (var pair in pairs)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(uniquePairs.Add(pair));
            }

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(uniquePairs.Count == pairs.Count());
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void GetCurrencyPairs003()
        {
            var Bank = new Finance.ForeignExchange.FederalReserveSystem();

            try
            {
                var pairs = Bank.GetCurrencyPairs(System.DateTime.Now.AddMinutes(1d));

                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
            }
            catch (System.ArgumentException)
            {
            }
            catch (System.Exception ex)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail(ex.Message);
            }
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void GetCurrencyPairs004()
        {
            var Bank = new Finance.ForeignExchange.FederalReserveSystem();

            var pairs = Bank.GetCurrencyPairs(System.DateTime.Now);

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "AUD/USD"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "BRL/USD"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "CAD/USD"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "CNY/USD"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "DKK/USD"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "EUR/USD"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "HKD/USD"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "INR/USD"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "JPY/USD"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "MYR/USD"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "MXN/USD"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "NZD/USD"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "NOK/USD"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "SGD/USD"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "ZAR/USD"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "KRW/USD"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "LKR/USD"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "SEK/USD"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "CHF/USD"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "TWD/USD"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "THB/USD"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "GBP/USD"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "VEF/USD"));

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/AUD"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/BRL"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/CAD"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/CNY"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/DKK"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/EUR"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/HKD"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/INR"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/JPY"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/MYR"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/MXN"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/NZD"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/NOK"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/SGD"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/ZAR"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/KRW"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/LKR"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/SEK"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/CHF"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/TWD"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/THB"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/GBP"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/VEF"));

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(23 * 2, pairs.Count());
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void GetExchangeRate001()
        {
            var Bank = new Finance.ForeignExchange.FederalReserveSystem();

            foreach (var pair in Bank.GetCurrencyPairs(System.DateTime.Now))
            {
                var rate = Bank.GetExchangeRate(pair, System.DateTime.Now);

                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(rate > decimal.Zero);
            }
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void GetExchangeRate002()
        {
            var Bank = new Finance.ForeignExchange.FederalReserveSystem();

            var LastYear = System.DateTime.Now.AddYears(-1);

            foreach (var pair in Bank.GetCurrencyPairs(LastYear))
            {
                var rate = Bank.GetExchangeRate(pair, LastYear);

                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(rate > decimal.Zero);
            }
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void GetExchangeRate003()
        {
            var Bank = new Finance.ForeignExchange.FederalReserveSystem();

            foreach (var pair in Bank.GetCurrencyPairs(System.DateTime.Now))
            {
                try
                {
                    var rate = Bank.GetExchangeRate(pair, System.DateTime.Now.AddMinutes(1d));

                    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
                }
                catch (System.ArgumentException)
                {
                }
                catch (System.Exception ex)
                {
                    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail(ex.Message);
                }
            }
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void GetExchangeRate004()
        {
            var Bank = new Finance.ForeignExchange.FederalReserveSystem();

            var pair = new CurrencyPair(new CurrencyInfo(new System.Globalization.RegionInfo("AL")), new CurrencyInfo(new System.Globalization.RegionInfo("AM")));

            try
            {
                var rate = Bank.GetExchangeRate(pair, System.DateTime.Now.AddMinutes(1d));

                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
            }
            catch (System.ArgumentException)
            {
            }
            catch (System.Exception ex)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail(ex.Message);
            }
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void GetExchangeRate005()
        {
            var Bank = new Finance.ForeignExchange.FederalReserveSystem();

            var pair = new CurrencyPair(new CurrencyInfo(new System.Globalization.RegionInfo("US")), new CurrencyInfo(new System.Globalization.RegionInfo("CN")));

            var rate = Bank.GetExchangeRate(pair, System.DateTime.Now);

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(rate > decimal.One);
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void GetExchangeRate006()
        {
            var Bank = new Finance.ForeignExchange.FederalReserveSystem();

            var pair = new CurrencyPair(new CurrencyInfo(new System.Globalization.RegionInfo("US")), new CurrencyInfo(new System.Globalization.RegionInfo("SG")));

            var rate = Bank.GetExchangeRate(pair, System.DateTime.Now);

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(rate > decimal.One);
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void KeepCurrenciesPairsUpdated()
        {
            // In case or failure, check currency pair information from FRS website and set deadline up to 3 month.

            System.DateTime Deadline = new System.DateTime(2015, 3, 1);

            if (System.DateTime.Now > Deadline)
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
        }
    }
}