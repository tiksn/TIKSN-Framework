using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using LanguageExt;
using LanguageExt.Parsec;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Expr;
using static LanguageExt.Parsec.Token;
using static LanguageExt.Parsec.Indent;

namespace TIKSN.Numbering;

public sealed class SimpleSerialNumber<TSerial, TNumber> : ISerialNumber<SimpleSerialNumber<TSerial, TNumber>>
    where TSerial : ISerial<TSerial>
    where TNumber : IUnsignedNumber<TNumber>
{
    public bool Equals(SimpleSerialNumber<TSerial, TNumber> other)
    {
        if (ReferenceEquals(null, other))
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

    public static bool operator ==(SimpleSerialNumber<TSerial, TNumber> left, SimpleSerialNumber<TSerial, TNumber> right) => Equals(left, right);

    public static bool operator !=(SimpleSerialNumber<TSerial, TNumber> left, SimpleSerialNumber<TSerial, TNumber> right) => !Equals(left, right);

    public TSerial Serial { get; }
    public TNumber Number { get; }

    public SimpleSerialNumber(TSerial serial, TNumber number)
    {
        this.Serial = serial ?? throw new ArgumentNullException(nameof(serial));
        this.Number = number ?? throw new ArgumentNullException(nameof(number));
    }

    public static SimpleSerialNumber<TSerial, TNumber> Parse(string s, IFormatProvider provider)
    {
        if (TryParse(s, provider, out var result))
        {
            return result;
        }

        throw new FormatException("Input string was not in a correct format.");
    }

    public static bool TryParse(string s, IFormatProvider provider, out SimpleSerialNumber<TSerial, TNumber> result)
    {
        var serialNumber = Parse(s, asciiOnly: false, provider);
        result = serialNumber.MatchUnsafe(x => x, () => default);
        return serialNumber.IsSome;
    }

    public static SimpleSerialNumber<TSerial, TNumber> Parse(ReadOnlySpan<char> s, IFormatProvider provider)
        => Parse(s.ToString(), provider);

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider provider, out SimpleSerialNumber<TSerial, TNumber> result)
        => TryParse(s.ToString(), provider, out result);

    public static Option<SimpleSerialNumber<TSerial, TNumber>> Parse(string s, bool asciiOnly, IFormatProvider provider)
    {
        if (string.IsNullOrEmpty(s))
        {
            return None;
        }

        var parseNumber = (string value) =>
        {
            if (TNumber.TryParse(value, provider, out var num))
            {
                return Some(num);
            }

            return None;
        };

        var serialParser =
            from xs in many(letter)
            let r = new string(xs.ToArray())
            let val = TSerial.Parse(r, asciiOnly, provider)
            from res in val.Match<Parser<TSerial>>(
                Some: x => result<TSerial>(x),
                None: failure<TSerial>("Failed to parse a serial'"))
            select res;

        var numberParser =
            from xs in many(digit)
            let r = new string(xs.ToArray())
            let val = parseNumber(r)
            from res in val.Match<Parser<TNumber>>(
                Some: x => result<TNumber>(x),
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

    public override string ToString()
        => this.ToString(null, CultureInfo.InvariantCulture);

    public string ToString(string format, IFormatProvider formatProvider)
        => $"{this.Serial.ToString(null, formatProvider)}-{this.Number.ToString(null, formatProvider)}";

    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format,
        IFormatProvider provider)
    {
        var result = this.ToString(null, provider);
        charsWritten = Math.Min(result.Length, destination.Length);
        result.CopyTo(destination[..charsWritten]);
        return charsWritten == result.Length;
    }

    public Option<SimpleSerialNumber<TSerial, TNumber>> GetPrevious()
    {
        if (TNumber.IsZero(this.Number))
        {
            return None;
        }

        return new SimpleSerialNumber<TSerial, TNumber>(this.Serial, this.Number - TNumber.One);
    }

    public Option<SimpleSerialNumber<TSerial, TNumber>> GetNext()
    {
        var nextNumber = this.Number + TNumber.One;
        if (TNumber.IsZero(nextNumber))
        {
            return None;
        }

        return new SimpleSerialNumber<TSerial, TNumber>(this.Serial, nextNumber);
    }
}
