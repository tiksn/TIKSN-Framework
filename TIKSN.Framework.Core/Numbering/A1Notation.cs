using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using LanguageExt;
using static LanguageExt.Prelude;

namespace TIKSN.Numbering;

public sealed class A1Notation<TNumber> : ISerialNumber<A1Notation<TNumber>>
    where TNumber : IUnsignedNumber<TNumber>
{
    private readonly SimpleSerialNumber<BB26, TNumber> serialNumber;

    public A1Notation(SimpleSerialNumber<BB26, TNumber> serialNumber)
    {
        this.serialNumber = serialNumber ?? throw new ArgumentNullException(nameof(serialNumber));
        if (Validate(this.serialNumber).IsNone)
        {
            throw new ArgumentException("Number cannot be zero.", nameof(serialNumber));
        }
    }

    public A1Notation(BB26 serial, TNumber number) : this(new SimpleSerialNumber<BB26, TNumber>(serial, number))
    {
    }

    public TNumber Number => this.serialNumber.Number;

    public BB26 Serial => this.serialNumber.Serial;

    public static bool operator !=(A1Notation<TNumber> left, A1Notation<TNumber> right) => !Equals(left, right);

    public static bool operator ==(A1Notation<TNumber> left, A1Notation<TNumber> right) => Equals(left, right);

    public static Option<A1Notation<TNumber>> Parse(string s, bool asciiOnly, IFormatProvider? provider) =>
        Validate(SimpleSerialNumber<BB26, TNumber>.Parse(s, asciiOnly, provider)).Map(x => new A1Notation<TNumber>(x));

    public static Option<A1Notation<TNumber>> Parse(ReadOnlySpan<char> s, bool asciiOnly, IFormatProvider? provider) =>
        Validate(SimpleSerialNumber<BB26, TNumber>.Parse(s, asciiOnly, provider)).Map(x => new A1Notation<TNumber>(x));

    public static A1Notation<TNumber> Parse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider) =>
        new(SimpleSerialNumber<BB26, TNumber>.Parse(s, provider));

    public static A1Notation<TNumber> Parse(string s, IFormatProvider? provider) =>
        new(SimpleSerialNumber<BB26, TNumber>.Parse(s, provider));

    public static bool TryParse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out A1Notation<TNumber> result)
    {
        var parsed = SimpleSerialNumber<BB26, TNumber>.TryParse(s, provider, out var serialNumber);

        result = parsed ? new A1Notation<TNumber>(serialNumber) : default;

        return parsed;
    }

    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out A1Notation<TNumber> result)
    {
        var parsed = SimpleSerialNumber<BB26, TNumber>.TryParse(s, provider, out var serialNumber);

        result = parsed ? new A1Notation<TNumber>(serialNumber) : default;

        return parsed;
    }

    public bool Equals(A1Notation<TNumber>? other) => this.serialNumber.Equals(other?.serialNumber);

    public override bool Equals(object? obj) => this.Equals(obj as A1Notation<TNumber>);

    public override int GetHashCode() => this.serialNumber.GetHashCode();

    public Option<A1Notation<TNumber>> GetNext() => this.serialNumber.GetNext().Map(x => new A1Notation<TNumber>(x));

    public Option<A1Notation<TNumber>> GetPrevious() =>
        Validate(this.serialNumber.GetPrevious()).Map(x => new A1Notation<TNumber>(x));

    public string ToString(string? format, IFormatProvider? formatProvider) =>
        this.serialNumber.ToString(format ?? "N", formatProvider);

    public override string ToString() => this.ToString(format: null, CultureInfo.InvariantCulture);

    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format,
        IFormatProvider? provider) =>
        this.serialNumber.TryFormat(destination, out charsWritten, format.Length == 0 ? "N" : format, provider);

    private static Option<SimpleSerialNumber<BB26, TNumber>>
        Validate(Option<SimpleSerialNumber<BB26, TNumber>> serialNumber) =>
        serialNumber.Bind(x => TNumber.IsZero(x.Number) ? None : Some(x));
}
