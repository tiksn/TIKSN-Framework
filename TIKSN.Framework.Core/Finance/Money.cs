using System.Diagnostics;
using System.Globalization;

namespace TIKSN.Finance;

public sealed class Money : IEquatable<Money>, IComparable<Money>, IFormattable, IComparable
{
    public Money(CurrencyInfo currency, decimal amount = decimal.Zero)
    {
        this.Currency = currency;
        this.Amount = amount;
    }

    public decimal Amount { get; }

    public CurrencyInfo Currency { get; }

    public static Money Add(Money left, Money right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        AssertCurrenciesIdentical(left, right);

        return new(left.Currency, left.Amount + right.Amount);
    }

    public static int CompareTo(Money first, Money second)
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);

        return first.CompareTo(second);
    }

    public static decimal Divide(Money left, Money right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        AssertCurrenciesIdentical(left, right);

        return left.Amount / right.Amount;
    }

    public static Money Divide(Money left, byte right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return new(left.Currency, left.Amount / right);
    }

    public static Money Divide(Money left, sbyte right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return new(left.Currency, left.Amount / right);
    }

    public static Money Divide(Money left, short right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return new(left.Currency, left.Amount / right);
    }

    public static Money Divide(Money left, ushort right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return new(left.Currency, left.Amount / right);
    }

    public static Money Divide(Money left, int right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return new(left.Currency, left.Amount / right);
    }

    public static Money Divide(Money left, uint right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return new(left.Currency, left.Amount / right);
    }

    public static Money Divide(Money left, long right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return new(left.Currency, left.Amount / right);
    }

    public static Money Divide(Money left, ulong right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return new(left.Currency, left.Amount / right);
    }

    public static Money Divide(Money left, decimal right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return new(left.Currency, left.Amount / right);
    }

    public static bool Equals(Money? first, Money? second)
    {
        if ((first is null) && (second is null))
        {
            return true;
        }

        if (first is null)
        {
            return false;
        }

        if (second is null)
        {
            return false;
        }

        if (ReferenceEquals(first, second))
        {
            return true;
        }

        AssertCurrenciesIdentical(first, second);

        return first.Amount.Equals(second.Amount);
    }

    public static Money Mod(Money left, byte right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return new(left.Currency, left.Amount % right);
    }

    public static decimal Mod(Money left, Money right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        AssertCurrenciesIdentical(left, right);

        return left.Amount % right.Amount;
    }

    public static Money Mod(Money left, short right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return new(left.Currency, left.Amount % right);
    }

    public static Money Mod(Money left, ushort right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return new(left.Currency, left.Amount % right);
    }

    public static Money Mod(Money left, int right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return new(left.Currency, left.Amount % right);
    }

    public static Money Mod(Money left, uint right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return new(left.Currency, left.Amount % right);
    }

    public static Money Mod(Money left, long right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return new(left.Currency, left.Amount % right);
    }

    public static Money Mod(Money left, ulong right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return new(left.Currency, left.Amount % right);
    }

    public static Money Mod(Money left, decimal right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return new(left.Currency, left.Amount % right);
    }

    public static Money Multiply(Money left, byte right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return new(left.Currency, left.Amount * right);
    }

    public static Money Multiply(Money left, sbyte right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return new(left.Currency, left.Amount * right);
    }

    public static Money Multiply(Money left, short right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return new(left.Currency, left.Amount * right);
    }

    public static Money Multiply(Money left, ushort right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return new(left.Currency, left.Amount * right);
    }

    public static Money Multiply(Money left, int right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return new(left.Currency, left.Amount * right);
    }

    public static Money Multiply(Money left, uint right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return new(left.Currency, left.Amount * right);
    }

    public static Money Multiply(Money left, long right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return new(left.Currency, left.Amount * right);
    }

    public static Money Multiply(Money left, ulong right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return new(left.Currency, left.Amount * right);
    }

    public static Money Multiply(Money left, decimal right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return new(left.Currency, left.Amount * right);
    }

    public static Money Negate(Money money)
    {
        ArgumentNullException.ThrowIfNull(money);

        return new(money.Currency, -money.Amount);
    }

    public static Money operator -(Money money) => Negate(money);

    public static Money operator -(Money first, Money second) => Subtract(first, second);

    public static bool operator !=(Money first, Money second) => !Equals(first, second);

    public static Money operator %(Money dividend, sbyte divisor) => Mod(dividend, divisor);

    public static Money operator %(Money dividend, byte divisor) => Mod(dividend, divisor);

    public static Money operator %(Money dividend, short divisor) => Mod(dividend, divisor);

    public static Money operator %(Money dividend, ushort divisor) => Mod(dividend, divisor);

    public static Money operator %(Money dividend, int divisor) => Mod(dividend, divisor);

    public static Money operator %(Money dividend, uint divisor) => Mod(dividend, divisor);

    public static Money operator %(Money dividend, long divisor) => Mod(dividend, divisor);

    public static Money operator %(Money dividend, ulong divisor) => Mod(dividend, divisor);

    public static Money operator %(Money dividend, decimal divisor) => Mod(dividend, divisor);

    public static decimal operator %(Money dividend, Money divisor) => Mod(dividend, divisor);

    public static Money operator *(Money first, sbyte second) => Multiply(first, second);

    public static Money operator *(Money first, byte second) => Multiply(first, second);

    public static Money operator *(Money first, short second) => Multiply(first, second);

    public static Money operator *(Money first, ushort second) => Multiply(first, second);

    public static Money operator *(Money first, int second) => Multiply(first, second);

    public static Money operator *(Money first, uint second) => Multiply(first, second);

    public static Money operator *(Money first, long second) => Multiply(first, second);

    public static Money operator *(Money first, ulong second) => Multiply(first, second);

    public static Money operator *(Money first, decimal second) => Multiply(first, second);

    public static Money operator /(Money dividend, sbyte divisor) => Divide(dividend, divisor);

    public static decimal operator /(Money dividend, Money divisor) => Divide(dividend, divisor);

    public static Money operator /(Money dividend, byte divisor) => Divide(dividend, divisor);

    public static Money operator /(Money dividend, short divisor) => Divide(dividend, divisor);

    public static Money operator /(Money dividend, ushort divisor) => Divide(dividend, divisor);

    public static Money operator /(Money dividend, int divisor) => Divide(dividend, divisor);

    public static Money operator /(Money dividend, uint divisor) => Divide(dividend, divisor);

    public static Money operator /(Money dividend, long divisor) => Divide(dividend, divisor);

    public static Money operator /(Money dividend, ulong divisor) => Divide(dividend, divisor);

    public static Money operator /(Money dividend, decimal divisor) => Divide(dividend, divisor);

    public static Money operator +(Money money) => Plus(money);

    public static Money operator +(Money first, Money second) => Add(first, second);

    public static bool operator <(Money first, Money second) => CompareTo(first, second) < 0;

    public static bool operator <=(Money first, Money second) => CompareTo(first, second) <= 0;

    public static bool operator ==(Money first, Money second) => Equals(first, second);

    public static bool operator >(Money first, Money second) => CompareTo(first, second) > 0;

    public static bool operator >=(Money first, Money second) => CompareTo(first, second) >= 0;

    public static Money Plus(Money money)
    {
        ArgumentNullException.ThrowIfNull(money);

        return money;
    }

    public static Money Subtract(Money left, Money right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        AssertCurrenciesIdentical(left, right);

        return new(left.Currency, left.Amount - right.Amount);
    }

    public int CompareTo(Money? other)
    {
        if (other is null)
        {
            return 1;
        }

        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        AssertCurrenciesIdentical(this, other);

        return this.Amount.CompareTo(other.Amount);
    }

    public int CompareTo(object? obj)
    {
        if (obj == null)
        {
            return 1;
        }

        if (obj is Money x)
        {
            return this.CompareTo(x);
        }

        throw new ArgumentException("", nameof(obj));
    }

    public bool Equals(Money? other) => Equals(this, other);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        var thatMoney = obj as Money;

        if (thatMoney is not null)
        {
            return this.Equals(thatMoney);
        }

        return false;
    }

    public override int GetHashCode() => HashCode.Combine(this.Currency, this.Amount);

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var nfi = (NumberFormatInfo)NumberFormatInfo.GetInstance(formatProvider).Clone();

        Debug.Assert(!nfi.IsReadOnly);
        var currencyFormat = "S";
        var amountFormat = "C";

        if (!string.IsNullOrEmpty(format))
        {
            currencyFormat = format[..1];

            amountFormat += format[1..];
        }

        nfi.CurrencySymbol = currencyFormat switch
        {
            "S" => this.Currency.CurrencySymbol,
            "I" => this.Currency.ISOCurrencySymbol,
            _ => throw new FormatException(),
        };
        return this.Amount.ToString(amountFormat, nfi);
    }

    public override string ToString() => this.ToString(string.Empty, CultureInfo.CurrentCulture);

    public string ToString(string format) => this.ToString(format, CultureInfo.CurrentCulture);

    private static void AssertCurrenciesIdentical(Money first, Money second)
    {
        if (!first.Currency.Equals(second.Currency))
        {
            throw new InvalidOperationException("Money values are in different currencies.");
        }
    }
}
