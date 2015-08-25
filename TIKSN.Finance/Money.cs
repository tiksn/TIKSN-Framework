namespace TIKSN.Finance
{
	public class Money : System.IEquatable<Money>, System.IComparable<Money>, System.IFormattable
	{
		private decimal amount;
		private CurrencyInfo currency;

		public Money(CurrencyInfo Currency, decimal Amount = decimal.Zero)
		{
			this.currency = Currency;
			this.amount = Amount;
		}

		public decimal Amount
		{
			get
			{
				return this.amount;
			}
		}

		public CurrencyInfo Currency
		{
			get
			{
				return this.currency;
			}
		}

		public static Money operator -(Money money)
		{
			return new Money(money.currency, -money.amount);
		}

		public static Money operator -(Money first, Money second)
		{
			AssertCurrencyIdentity(first, second);

			return new Money(first.currency, first.amount - second.amount);
		}

		public static bool operator !=(Money first, Money second)
		{
			return !first.Equals(second);
		}

		public static Money operator %(Money dividend, sbyte divisor)
		{
			return new Money(dividend.currency, dividend.amount % divisor);
		}

		public static Money operator %(Money dividend, byte divisor)
		{
			return new Money(dividend.currency, dividend.amount % divisor);
		}

		public static Money operator %(Money dividend, short divisor)
		{
			return new Money(dividend.currency, dividend.amount % divisor);
		}

		public static Money operator %(Money dividend, ushort divisor)
		{
			return new Money(dividend.currency, dividend.amount % divisor);
		}

		public static Money operator %(Money dividend, int divisor)
		{
			return new Money(dividend.currency, dividend.amount % divisor);
		}

		public static Money operator %(Money dividend, uint divisor)
		{
			return new Money(dividend.currency, dividend.amount % divisor);
		}

		public static Money operator %(Money dividend, long divisor)
		{
			return new Money(dividend.currency, dividend.amount % divisor);
		}

		public static Money operator %(Money dividend, ulong divisor)
		{
			return new Money(dividend.currency, dividend.amount % divisor);
		}

		public static Money operator %(Money dividend, decimal divisor)
		{
			return new Money(dividend.currency, dividend.amount % divisor);
		}

		public static Money operator *(Money first, sbyte second)
		{
			return new Money(first.currency, first.amount * second);
		}

		public static Money operator *(Money first, byte second)
		{
			return new Money(first.currency, first.amount * second);
		}

		public static Money operator *(Money first, short second)
		{
			return new Money(first.currency, first.amount * second);
		}

		public static Money operator *(Money first, ushort second)
		{
			return new Money(first.currency, first.amount * second);
		}

		public static Money operator *(Money first, int second)
		{
			return new Money(first.currency, first.amount * second);
		}

		public static Money operator *(Money first, uint second)
		{
			return new Money(first.currency, first.amount * second);
		}

		public static Money operator *(Money first, long second)
		{
			return new Money(first.currency, first.amount * second);
		}

		public static Money operator *(Money first, ulong second)
		{
			return new Money(first.currency, first.amount * second);
		}

		public static Money operator *(Money first, decimal second)
		{
			return new Money(first.currency, first.amount * second);
		}

		public static Money operator /(Money dividend, sbyte divisor)
		{
			return new Money(dividend.currency, dividend.amount / divisor);
		}

		public static Money operator /(Money dividend, byte divisor)
		{
			return new Money(dividend.currency, dividend.amount / divisor);
		}

		public static Money operator /(Money dividend, short divisor)
		{
			return new Money(dividend.currency, dividend.amount / divisor);
		}

		public static Money operator /(Money dividend, ushort divisor)
		{
			return new Money(dividend.currency, dividend.amount / divisor);
		}

		public static Money operator /(Money dividend, int divisor)
		{
			return new Money(dividend.currency, dividend.amount / divisor);
		}

		public static Money operator /(Money dividend, uint divisor)
		{
			return new Money(dividend.currency, dividend.amount / divisor);
		}

		public static Money operator /(Money dividend, long divisor)
		{
			return new Money(dividend.currency, dividend.amount / divisor);
		}

		public static Money operator /(Money dividend, ulong divisor)
		{
			return new Money(dividend.currency, dividend.amount / divisor);
		}

		public static Money operator /(Money dividend, decimal divisor)
		{
			return new Money(dividend.currency, dividend.amount / divisor);
		}

		public static Money operator +(Money money)
		{
			return new Money(money.currency, money.amount);
		}

		public static Money operator +(Money first, Money second)
		{
			AssertCurrencyIdentity(first, second);

			return new Money(first.currency, first.amount + second.amount);
		}

		public static bool operator <(Money first, Money second)
		{
			return first.CompareTo(second) < 0;
		}

		public static bool operator <=(Money first, Money second)
		{
			return first.CompareTo(second) <= 0;
		}

		public static bool operator ==(Money first, Money second)
		{
			return first.Equals(second);
		}

		public static bool operator >(Money first, Money second)
		{
			return first.CompareTo(second) > 0;
		}

		public static bool operator >=(Money first, Money second)
		{
			return first.CompareTo(second) >= 0;
		}

		public int CompareTo(Money that)
		{
			if (object.ReferenceEquals(this, that))
			{
				return 0;
			}
			else
			{
				AssertCurrencyIdentity(this, that);

				return this.amount.CompareTo(that.amount);
			}
		}

		public bool Equals(Money that)
		{
			if (object.ReferenceEquals(that, null))
			{
				return false;
			}
			else if (object.ReferenceEquals(this, that))
			{
				return true;
			}
			else
			{
				AssertCurrencyIdentity(this, that);

				return this.amount.Equals(that.amount);
			}
		}

		public override bool Equals(object that)
		{
			if (object.ReferenceEquals(this, that))
			{
				return true;
			}

			var thatMoney = that as Money;

			if (!object.ReferenceEquals(thatMoney, null))
			{
				return this.Equals(thatMoney);
			}

			return false;
		}

		public string ToString(string format, System.IFormatProvider formatProvider)
		{
			System.Globalization.NumberFormatInfo NFI = (System.Globalization.NumberFormatInfo)System.Globalization.NumberFormatInfo.GetInstance(formatProvider).Clone();

			System.Diagnostics.Debug.Assert(!NFI.IsReadOnly);
			string currencyFormat = "S";
			string amountFormat = "C";

			if (!string.IsNullOrEmpty(format))
			{
				currencyFormat = format.Substring(0, 1);

				amountFormat += format.Substring(1);
			}

			switch (currencyFormat)
			{
				case "S":
					NFI.CurrencySymbol = this.currency.CurrencySymbol;
					break;

				case "I":
					NFI.CurrencySymbol = this.currency.ISOCurrencySymbol;
					break;

				default:
					throw new System.FormatException();
			}

			return amount.ToString(amountFormat, NFI);
		}

		public override string ToString()
		{
			return this.ToString(string.Empty);
		}

		public string ToString(string format)
		{
			return this.ToString(format, System.Globalization.CultureInfo.CurrentCulture);
		}

		private static void AssertCurrencyIdentity(Money first, Money second)
		{
			if (!first.currency.Equals(second.currency))
			{
				throw new System.InvalidOperationException("Money values are in different currencies.");
			}
		}
	}
}