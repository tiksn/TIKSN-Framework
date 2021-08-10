using System;
using System.Diagnostics;
using System.Globalization;

namespace TIKSN.Finance
{
    public class Money : IEquatable<Money>, IComparable<Money>, IFormattable
    {
        public Money(CurrencyInfo Currency, decimal Amount = decimal.Zero)
        {
            this.Currency = Currency;
            this.Amount = Amount;
        }

        public decimal Amount { get; }

        public CurrencyInfo Currency { get; }

        public int CompareTo(Money that)
        {
            if (ReferenceEquals(this, that))
            {
                return 0;
            }

            AssertCurrencyIdentity(this, that);

            return this.Amount.CompareTo(that.Amount);
        }

        public bool Equals(Money that)
        {
            if (ReferenceEquals(that, null))
            {
                return false;
            }

            if (ReferenceEquals(this, that))
            {
                return true;
            }

            AssertCurrencyIdentity(this, that);

            return this.Amount.Equals(that.Amount);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            var NFI = (NumberFormatInfo)NumberFormatInfo.GetInstance(formatProvider).Clone();

            Debug.Assert(!NFI.IsReadOnly);
            var currencyFormat = "S";
            var amountFormat = "C";

            if (!string.IsNullOrEmpty(format))
            {
                currencyFormat = format.Substring(0, 1);

                amountFormat += format.Substring(1);
            }

            switch (currencyFormat)
            {
                case "S":
                    NFI.CurrencySymbol = this.Currency.CurrencySymbol;
                    break;

                case "I":
                    NFI.CurrencySymbol = this.Currency.ISOCurrencySymbol;
                    break;

                default:
                    throw new FormatException();
            }

            return this.Amount.ToString(amountFormat, NFI);
        }

        public static Money operator -(Money money) => new(money.Currency, -money.Amount);

        public static Money operator -(Money first, Money second)
        {
            AssertCurrencyIdentity(first, second);

            return new Money(first.Currency, first.Amount - second.Amount);
        }

        public static bool operator !=(Money first, Money second) => !first.Equals(second);

        public static Money operator %(Money dividend, sbyte divisor) =>
            new(dividend.Currency, dividend.Amount % divisor);

        public static Money operator %(Money dividend, byte divisor) =>
            new(dividend.Currency, dividend.Amount % divisor);

        public static Money operator %(Money dividend, short divisor) =>
            new(dividend.Currency, dividend.Amount % divisor);

        public static Money operator %(Money dividend, ushort divisor) =>
            new(dividend.Currency, dividend.Amount % divisor);

        public static Money operator %(Money dividend, int divisor) =>
            new(dividend.Currency, dividend.Amount % divisor);

        public static Money operator %(Money dividend, uint divisor) =>
            new(dividend.Currency, dividend.Amount % divisor);

        public static Money operator %(Money dividend, long divisor) =>
            new(dividend.Currency, dividend.Amount % divisor);

        public static Money operator %(Money dividend, ulong divisor) =>
            new(dividend.Currency, dividend.Amount % divisor);

        public static Money operator %(Money dividend, decimal divisor) =>
            new(dividend.Currency, dividend.Amount % divisor);

        public static Money operator *(Money first, sbyte second) => new(first.Currency, first.Amount * second);

        public static Money operator *(Money first, byte second) => new(first.Currency, first.Amount * second);

        public static Money operator *(Money first, short second) => new(first.Currency, first.Amount * second);

        public static Money operator *(Money first, ushort second) => new(first.Currency, first.Amount * second);

        public static Money operator *(Money first, int second) => new(first.Currency, first.Amount * second);

        public static Money operator *(Money first, uint second) => new(first.Currency, first.Amount * second);

        public static Money operator *(Money first, long second) => new(first.Currency, first.Amount * second);

        public static Money operator *(Money first, ulong second) => new(first.Currency, first.Amount * second);

        public static Money operator *(Money first, decimal second) => new(first.Currency, first.Amount * second);

        public static Money operator /(Money dividend, sbyte divisor) =>
            new(dividend.Currency, dividend.Amount / divisor);

        public static Money operator /(Money dividend, byte divisor) =>
            new(dividend.Currency, dividend.Amount / divisor);

        public static Money operator /(Money dividend, short divisor) =>
            new(dividend.Currency, dividend.Amount / divisor);

        public static Money operator /(Money dividend, ushort divisor) =>
            new(dividend.Currency, dividend.Amount / divisor);

        public static Money operator /(Money dividend, int divisor) =>
            new(dividend.Currency, dividend.Amount / divisor);

        public static Money operator /(Money dividend, uint divisor) =>
            new(dividend.Currency, dividend.Amount / divisor);

        public static Money operator /(Money dividend, long divisor) =>
            new(dividend.Currency, dividend.Amount / divisor);

        public static Money operator /(Money dividend, ulong divisor) =>
            new(dividend.Currency, dividend.Amount / divisor);

        public static Money operator /(Money dividend, decimal divisor) =>
            new(dividend.Currency, dividend.Amount / divisor);

        public static Money operator +(Money money) => new(money.Currency, money.Amount);

        public static Money operator +(Money first, Money second)
        {
            AssertCurrencyIdentity(first, second);

            return new Money(first.Currency, first.Amount + second.Amount);
        }

        public static bool operator <(Money first, Money second) => first.CompareTo(second) < 0;

        public static bool operator <=(Money first, Money second) => first.CompareTo(second) <= 0;

        public static bool operator ==(Money first, Money second) => first.Equals(second);

        public static bool operator >(Money first, Money second) => first.CompareTo(second) > 0;

        public static bool operator >=(Money first, Money second) => first.CompareTo(second) >= 0;

        public override bool Equals(object that)
        {
            if (ReferenceEquals(this, that))
            {
                return true;
            }

            var thatMoney = that as Money;

            if (!ReferenceEquals(thatMoney, null))
            {
                return this.Equals(thatMoney);
            }

            return false;
        }

        public override string ToString() => this.ToString(string.Empty);

        public string ToString(string format) => this.ToString(format, CultureInfo.CurrentCulture);

        private static void AssertCurrencyIdentity(Money first, Money second)
        {
            if (!first.Currency.Equals(second.Currency))
            {
                throw new InvalidOperationException("Money values are in different currencies.");
            }
        }
    }
}
