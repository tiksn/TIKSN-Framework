using System.Globalization;
using System.Numerics;
using LanguageExt;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Prelude;

namespace TIKSN.Numbering;

#pragma warning disable CA1000 // Do not declare static members on generic types
public sealed class NumberFirstSerialNumber<TNumber, TSerial> :
    ISerialNumber<NumberFirstSerialNumber<TNumber, TSerial>>
    where TNumber : IUnsignedNumber<TNumber>
    where TSerial : ISerial<TSerial>
{
    public NumberFirstSerialNumber(TNumber number, TSerial serial)
    {
        this.Number = number ?? throw new ArgumentNullException(nameof(number));
        this.Serial = serial ?? throw new ArgumentNullException(nameof(serial));
    }

    public TNumber Number { get; }

    public TSerial Serial { get; }

    public static NumberFirstSerialNumber<TNumber, TSerial> Parse(string s, IFormatProvider? provider)
    {
        if (TryParse(s, provider, out var result))
        {
            return result;
        }

        throw new FormatException("Input string was not in a correct format.");
    }

    public static NumberFirstSerialNumber<TNumber, TSerial> Parse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider)
        => Parse(s.ToString(), provider);

    public static Option<NumberFirstSerialNumber<TNumber, TSerial>> Parse(
        string s,
        bool asciiOnly,
        IFormatProvider? provider)
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

        var numberParser =
            from xs in many(digit)
            let r = new string([.. xs])
            let val = parseNumber(r)
            from res in val.Match(
                Some: result<TNumber>,
                None: failure<TNumber>("Failed to parse a number'"))
            select res;

        var serialParser =
            from xs in many(letter)
            let r = new string([.. xs])
            let val = TSerial.Parse(r, asciiOnly, provider)
            from res in val.Match(
                Some: result<TSerial>,
                None: failure<TSerial>("Failed to parse a serial'"))
            select res;

        var parser =
            from number in numberParser
            from _1 in optional(ch('-'))
            from serial in serialParser
            from _2 in eof
            select new NumberFirstSerialNumber<TNumber, TSerial>(number, serial);

        var result = parse(parser, s);

        return result.ToOption();
    }

    public static Option<NumberFirstSerialNumber<TNumber, TSerial>> Parse(
        ReadOnlySpan<char> s,
        bool asciiOnly,
        IFormatProvider? provider)
        => Parse(s.ToString(), asciiOnly, provider);

    public static bool TryParse(
        string? s,
        IFormatProvider? provider,
        out NumberFirstSerialNumber<TNumber, TSerial> result)
    {
        if (s is null)
        {
            result = default!;
            return false;
        }

        var serialNumber = Parse(s, asciiOnly: false, provider);
        result = serialNumber.MatchUnsafe(x => x, () => default)!;
        return serialNumber.IsSome;
    }

    public static bool TryParse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider,
        out NumberFirstSerialNumber<TNumber, TSerial> result)
        => TryParse(s.ToString(), provider, out result);

    public static bool operator ==(
        NumberFirstSerialNumber<TNumber, TSerial> left,
        NumberFirstSerialNumber<TNumber, TSerial> right) => Equals(left, right);

    public static bool operator !=(
        NumberFirstSerialNumber<TNumber, TSerial> left,
        NumberFirstSerialNumber<TNumber, TSerial> right) => !Equals(left, right);

    public bool Equals(NumberFirstSerialNumber<TNumber, TSerial>? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return EqualityComparer<TNumber>.Default.Equals(this.Number, other.Number) &&
            EqualityComparer<TSerial>.Default.Equals(this.Serial, other.Serial);
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) ||
            (obj is NumberFirstSerialNumber<TNumber, TSerial> other && this.Equals(other));

    public override int GetHashCode() => HashCode.Combine(this.Number, this.Serial);

    public Option<NumberFirstSerialNumber<TNumber, TSerial>> GetNext()
    {
        var nextNumber = this.Number + TNumber.One;
        if (TNumber.IsZero(nextNumber))
        {
            return None;
        }

        return new NumberFirstSerialNumber<TNumber, TSerial>(nextNumber, this.Serial);
    }

    public Option<NumberFirstSerialNumber<TNumber, TSerial>> GetPrevious()
    {
        if (TNumber.IsZero(this.Number))
        {
            return None;
        }

        return new NumberFirstSerialNumber<TNumber, TSerial>(this.Number - TNumber.One, this.Serial);
    }

    public override string ToString()
        => this.ToString(format: null, CultureInfo.InvariantCulture);

    /// <summary>
    /// Formats the serial number using the specified format.
    /// </summary>
    /// <param name="format">Format specifier: null, empty, "G", or "N" for "NumberSerial" (no hyphen); "H" for "Number-Serial" (with hyphen).</param>
    /// <param name="formatProvider">Format provider.</param>
    /// <returns>Formatted serial number string.</returns>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var numberString = this.Number.ToString(format: null, formatProvider);
        var serialString = this.Serial.ToString(format: null, formatProvider);

        return format switch
        {
            "H" => $"{numberString}-{serialString}",
            _ => numberString + serialString,
        };
    }

    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        var result = this.ToString(new string(format), provider);
        charsWritten = Math.Min(result.Length, destination.Length);
        result[..charsWritten].CopyTo(destination);
        return charsWritten == result.Length;
    }
}
#pragma warning restore CA1000 // Do not declare static members on generic types
