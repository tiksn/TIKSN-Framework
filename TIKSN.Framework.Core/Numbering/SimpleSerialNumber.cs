using System.Globalization;
using System.Numerics;
using LanguageExt;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Prelude;

namespace TIKSN.Numbering;

#pragma warning disable CA1000 // Do not declare static members on generic types
#pragma warning disable MA0018 // Do not declare static members on generic types (deprecated; use CA1000 instead)
public sealed class SimpleSerialNumber<TSerial, TNumber> : ISerialNumber<SimpleSerialNumber<TSerial, TNumber>>
    where TSerial : ISerial<TSerial>
    where TNumber : IUnsignedNumber<TNumber>
{
    public SimpleSerialNumber(TSerial serial, TNumber number)
    {
        this.Serial = serial ?? throw new ArgumentNullException(nameof(serial));
        this.Number = number ?? throw new ArgumentNullException(nameof(number));
    }

    public TNumber Number { get; }

    public TSerial Serial { get; }

    public static bool operator !=(SimpleSerialNumber<TSerial, TNumber> left, SimpleSerialNumber<TSerial, TNumber> right) => !Equals(left, right);

    public static bool operator ==(SimpleSerialNumber<TSerial, TNumber> left, SimpleSerialNumber<TSerial, TNumber> right) => Equals(left, right);

    public static SimpleSerialNumber<TSerial, TNumber> Parse(string s, IFormatProvider provider)
    {
        if (TryParse(s, provider, out var result))
        {
            return result;
        }

        throw new FormatException("Input string was not in a correct format.");
    }

    public static SimpleSerialNumber<TSerial, TNumber> Parse(ReadOnlySpan<char> s, IFormatProvider provider)
        => Parse(s.ToString(), provider);

    public static Option<SimpleSerialNumber<TSerial, TNumber>> Parse(string s, bool asciiOnly, IFormatProvider provider)
    {
        if (string.IsNullOrEmpty(s))
        {
            return None;
        }

        Option<TNumber> parseNumber(string value)
        {
            if (TNumber.TryParse(value, provider, out var num))
            {
                return Some(num);
            }

            return None;
        }

        var serialParser =
            from xs in many(letter)
            let r = new string(xs.ToArray())
            let val = TSerial.Parse(r, asciiOnly, provider)
            from res in val.Match(
                Some: result<TSerial>,
                None: failure<TSerial>("Failed to parse a serial'"))
            select res;

        var numberParser =
            from xs in many(digit)
            let r = new string(xs.ToArray())
            let val = parseNumber(r)
            from res in val.Match(
                Some: result<TNumber>,
                None: failure<TNumber>("Failed to parse a number'"))
            select res;

        var parser =
            from serial in serialParser
            from _1 in optional(ch('-'))
            from number in numberParser
            from _2 in eof
            select new SimpleSerialNumber<TSerial, TNumber>(serial, number);

        var result = parse(parser, s);

        return result.ToOption();
    }

    public static Option<SimpleSerialNumber<TSerial, TNumber>> Parse(ReadOnlySpan<char> s, bool asciiOnly, IFormatProvider provider)
        => Parse(s.ToString(), asciiOnly, provider);

    public static bool TryParse(string s, IFormatProvider provider, out SimpleSerialNumber<TSerial, TNumber> result)
    {
        var serialNumber = Parse(s, asciiOnly: false, provider);
        result = serialNumber.MatchUnsafe(x => x, () => default);
        return serialNumber.IsSome;
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider provider, out SimpleSerialNumber<TSerial, TNumber> result)
        => TryParse(s.ToString(), provider, out result);

    public bool Equals(SimpleSerialNumber<TSerial, TNumber> other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return EqualityComparer<TSerial>.Default.Equals(this.Serial, other.Serial) && EqualityComparer<TNumber>.Default.Equals(this.Number, other.Number);
    }

    public override bool Equals(object obj)
        => ReferenceEquals(this, obj) || (obj is SimpleSerialNumber<TSerial, TNumber> other && this.Equals(other));

    public override int GetHashCode() => HashCode.Combine(this.Serial, this.Number);

    public Option<SimpleSerialNumber<TSerial, TNumber>> GetNext()
    {
        var nextNumber = this.Number + TNumber.One;
        if (TNumber.IsZero(nextNumber))
        {
            return None;
        }

        return new SimpleSerialNumber<TSerial, TNumber>(this.Serial, nextNumber);
    }

    public Option<SimpleSerialNumber<TSerial, TNumber>> GetPrevious()
    {
        if (TNumber.IsZero(this.Number))
        {
            return None;
        }

        return new SimpleSerialNumber<TSerial, TNumber>(this.Serial, this.Number - TNumber.One);
    }

    public override string ToString()
        => this.ToString(format: null, CultureInfo.InvariantCulture);

    public string ToString(string format, IFormatProvider formatProvider)
        => $"{this.Serial.ToString(format: null, formatProvider)}-{this.Number.ToString(format: null, formatProvider)}";

    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format,
        IFormatProvider provider)
    {
        var result = this.ToString(format: null, provider);
        charsWritten = Math.Min(result.Length, destination.Length);
        result.CopyTo(destination[..charsWritten]);
        return charsWritten == result.Length;
    }
}
#pragma warning restore MA0018 // Do not declare static members on generic types (deprecated; use CA1000 instead)
#pragma warning restore CA1000 // Do not declare static members on generic types
