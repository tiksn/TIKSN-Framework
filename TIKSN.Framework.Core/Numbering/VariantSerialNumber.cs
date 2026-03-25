using System.Globalization;
using System.Numerics;
using LanguageExt;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Prelude;

namespace TIKSN.Numbering;

#pragma warning disable CA1000 // Do not declare static members on generic types
public sealed class VariantSerialNumber<TSerial, TNumber, TVariant> : ISerialNumber<VariantSerialNumber<TSerial, TNumber, TVariant>>
    where TSerial : ISerial<TSerial>
    where TNumber : IUnsignedNumber<TNumber>
    where TVariant : ISerial<TVariant>
{
    public VariantSerialNumber(TSerial serial, TNumber number, TVariant variant)
    {
        this.Serial = serial ?? throw new ArgumentNullException(nameof(serial));
        this.Number = number ?? throw new ArgumentNullException(nameof(number));
        this.Variant = variant ?? throw new ArgumentNullException(nameof(variant));
    }

    public TNumber Number { get; }

    public TSerial Serial { get; }

    public TVariant Variant { get; }

    public static bool operator !=(VariantSerialNumber<TSerial, TNumber, TVariant> left, VariantSerialNumber<TSerial, TNumber, TVariant> right) => !Equals(left, right);

    public static bool operator ==(VariantSerialNumber<TSerial, TNumber, TVariant> left, VariantSerialNumber<TSerial, TNumber, TVariant> right) => Equals(left, right);

    public static VariantSerialNumber<TSerial, TNumber, TVariant> Parse(string s, IFormatProvider? provider)
    {
        if (TryParse(s, provider, out var result))
        {
            return result;
        }

        throw new FormatException("Input string was not in a correct format.");
    }

    public static VariantSerialNumber<TSerial, TNumber, TVariant> Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        => Parse(s.ToString(), provider);

    public static Option<VariantSerialNumber<TSerial, TNumber, TVariant>> Parse(string s, bool asciiOnly, IFormatProvider? provider)
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
            let r = new string([.. xs])
            let val = TSerial.Parse(r, asciiOnly, provider)
            from res in val.Match(
                Some: result<TSerial>,
                None: failure<TSerial>("Failed to parse a serial'"))
            select res;

        var numberParser =
            from xs in many(digit)
            let r = new string([.. xs])
            let val = parseNumber(r)
            from res in val.Match(
                Some: result<TNumber>,
                None: failure<TNumber>("Failed to parse a number'"))
            select res;

        var variantParser =
            from xs in many(letter)
            let r = new string([.. xs])
            let val = TVariant.Parse(r, asciiOnly, provider)
            from res in val.Match(
                Some: result<TVariant>,
                None: failure<TVariant>("Failed to parse a variant'"))
            select res;

        var parser =
            from serial in serialParser
            from _1 in optional(ch('-'))
            from number in numberParser
            from _2 in optional(ch('-'))
            from variant in variantParser
            from _3 in eof
            select new VariantSerialNumber<TSerial, TNumber, TVariant>(serial, number, variant);

        var result = parse(parser, s);

        return result.ToOption();
    }

    public static Option<VariantSerialNumber<TSerial, TNumber, TVariant>> Parse(ReadOnlySpan<char> s, bool asciiOnly, IFormatProvider? provider)
        => Parse(s.ToString(), asciiOnly, provider);

    public static bool TryParse(string? s, IFormatProvider? provider, out VariantSerialNumber<TSerial, TNumber, TVariant> result)
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

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out VariantSerialNumber<TSerial, TNumber, TVariant> result)
        => TryParse(s.ToString(), provider, out result);

    public bool Equals(VariantSerialNumber<TSerial, TNumber, TVariant>? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return EqualityComparer<TSerial>.Default.Equals(this.Serial, other.Serial) &&
               EqualityComparer<TNumber>.Default.Equals(this.Number, other.Number) &&
               EqualityComparer<TVariant>.Default.Equals(this.Variant, other.Variant);
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || (obj is VariantSerialNumber<TSerial, TNumber, TVariant> other && this.Equals(other));

    public override int GetHashCode() => HashCode.Combine(this.Serial, this.Number, this.Variant);

    public Option<VariantSerialNumber<TSerial, TNumber, TVariant>> GetNext()
    {
        var nextNumber = this.Number + TNumber.One;
        if (TNumber.IsZero(nextNumber))
        {
            return None;
        }

        return new VariantSerialNumber<TSerial, TNumber, TVariant>(this.Serial, nextNumber, this.Variant);
    }

    public Option<VariantSerialNumber<TSerial, TNumber, TVariant>> GetPrevious()
    {
        if (TNumber.IsZero(this.Number))
        {
            return None;
        }

        return new VariantSerialNumber<TSerial, TNumber, TVariant>(this.Serial, this.Number - TNumber.One, this.Variant);
    }

    public override string ToString()
        => this.ToString(format: null, CultureInfo.InvariantCulture);

    /// <summary>
    /// Formats the serial number using the specified format.
    /// </summary>
    /// <param name="format">Format specifier: null, empty, or "G" for "Serial-NumberVariant" (with hyphen); "N" for "SerialNumberVariant" (no hyphen).</param>
    /// <param name="formatProvider">Format provider.</param>
    /// <returns>Formatted serial number string.</returns>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var serialString = this.Serial.ToString(format: null, formatProvider);
        var numberString = this.Number.ToString(format: null, formatProvider);
        var variantString = this.Variant.ToString(format: null, formatProvider);

        return format switch
        {
            "N" => serialString + numberString + variantString,
            _ => $"{serialString}-{numberString}{variantString}",
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
