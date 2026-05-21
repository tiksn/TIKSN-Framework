using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using LanguageExt;
using static LanguageExt.Prelude;

namespace TIKSN.Numbering;

public sealed class OneANotation<TNumber> : ISerialNumber<OneANotation<TNumber>>
    where TNumber : IUnsignedNumber<TNumber>
{
    private readonly NumberFirstSerialNumber<TNumber, BB26> serialNumber;

    public OneANotation(NumberFirstSerialNumber<TNumber, BB26> serialNumber)
    {
        this.serialNumber = serialNumber ?? throw new ArgumentNullException(nameof(serialNumber));
        if (Validate(this.serialNumber).IsNone)
        {
            throw new ArgumentException("Number cannot be zero.", nameof(serialNumber));
        }
    }

    public OneANotation(TNumber number, BB26 serial) : this(new NumberFirstSerialNumber<TNumber, BB26>(number, serial))
    {
    }

    public TNumber Number => this.serialNumber.Number;

    public BB26 Serial => this.serialNumber.Serial;

    public static Option<OneANotation<TNumber>> Parse(string s, bool asciiOnly, IFormatProvider? provider) =>
        Validate(NumberFirstSerialNumber<TNumber, BB26>.Parse(s, asciiOnly, provider))
            .Map(x => new OneANotation<TNumber>(x));

    public static Option<OneANotation<TNumber>>
        Parse(ReadOnlySpan<char> s, bool asciiOnly, IFormatProvider? provider) =>
        Validate(NumberFirstSerialNumber<TNumber, BB26>.Parse(s, asciiOnly, provider))
            .Map(x => new OneANotation<TNumber>(x));

    public static OneANotation<TNumber> Parse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider) =>
        new(NumberFirstSerialNumber<TNumber, BB26>.Parse(s, provider));

    public static OneANotation<TNumber> Parse(string s, IFormatProvider? provider) =>
        new(NumberFirstSerialNumber<TNumber, BB26>.Parse(s, provider));

    public static bool TryParse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out OneANotation<TNumber> result)
        => TryParse(s.ToString(), provider, out result);

    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out OneANotation<TNumber> result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        var serialNumber = Parse(s, asciiOnly: false, provider);
        result = serialNumber.MatchUnsafe(x => x, () => default)!;

        return serialNumber.IsSome;
    }

    public static bool operator ==(OneANotation<TNumber> left, OneANotation<TNumber> right) => Equals(left, right);

    public static bool operator !=(OneANotation<TNumber> left, OneANotation<TNumber> right) => !Equals(left, right);

    public bool Equals(OneANotation<TNumber>? other) => this.serialNumber.Equals(other?.serialNumber);

    public override bool Equals(object? obj) => this.Equals(obj as OneANotation<TNumber>);

    public override int GetHashCode() => this.serialNumber.GetHashCode();

    public Option<OneANotation<TNumber>> GetNext() =>
        this.serialNumber.GetNext().Map(x => new OneANotation<TNumber>(x));

    public Option<OneANotation<TNumber>> GetPrevious() =>
        Validate(this.serialNumber.GetPrevious()).Map(x => new OneANotation<TNumber>(x));

    public string ToString(string? format, IFormatProvider? formatProvider) =>
        this.serialNumber.ToString(format ?? "G", formatProvider);

    public override string ToString() => this.ToString(format: null, CultureInfo.InvariantCulture);

    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format,
        IFormatProvider? provider) =>
        this.serialNumber.TryFormat(destination, out charsWritten, format.Length == 0 ? "G" : format, provider);

    private static Option<NumberFirstSerialNumber<TNumber, BB26>>
        Validate(Option<NumberFirstSerialNumber<TNumber, BB26>> serialNumber) =>
        serialNumber.Bind(x => TNumber.IsZero(x.Number) ? None : Some(x));
}
