using System.Globalization;
using System.Reflection;
using LanguageExt;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Prelude;

namespace TIKSN.Numbering.Acronyms;

#pragma warning disable CA1000 // Do not declare static members on generic types
#pragma warning disable MA0018 // Do not declare static members on generic types (deprecated; use CA1000 instead)
#pragma warning disable S4035 // Classes implementing "IEquatable<T>" should be sealed
public abstract class Acronym<TSelf> : ISerial<TSelf>
#pragma warning restore S4035 // Classes implementing "IEquatable<T>" should be sealed
    where TSelf : Acronym<TSelf>, IAcronymLength
{
    private readonly string value;

    protected Acronym(string value)
        => this.value = value ?? throw new ArgumentNullException(nameof(value));

    public static bool operator !=(Acronym<TSelf> left, Acronym<TSelf> right) => !Equals(left, right);

    public static bool operator ==(Acronym<TSelf> left, Acronym<TSelf> right) => Equals(left, right);

    public static TSelf Parse(string s, IFormatProvider? provider)
    {
        if (TryParse(s, provider, out var result))
        {
            return result;
        }

        throw new FormatException("Input string was not in a correct format.");
    }

    public static TSelf Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        => Parse(s.ToString(), provider);

    public static Option<TSelf> Parse(string s, bool asciiOnly, IFormatProvider? provider)
    {
        if (string.IsNullOrEmpty(s))
        {
            return None;
        }

        var asciiLatterParser = satisfy(c => char.IsAscii(c) && char.IsLetter(c)).label("ASCII letter");

        var letterParser = asciiOnly ? asciiLatterParser : letter;

        var culture = provider as CultureInfo ?? CultureInfo.InvariantCulture;

        var lettersParser =
            from letters in count(TSelf.LetterCount, letterParser.Map(x => char.ToUpper(x, culture)))
            from _ in eof
            select letters;

        var parser = asString(lettersParser);

        var result = parse(parser, s);

        return result
            .ToOption()
            .Map(x => Create(x, culture))
            .Match(s => Optional(s), Option<TSelf>.None);
    }

    public static Option<TSelf> Parse(ReadOnlySpan<char> s, bool asciiOnly, IFormatProvider? provider)
        => Parse(s.ToString(), asciiOnly, provider);

    public static bool TryParse(string? s, IFormatProvider? provider, out TSelf result)
    {
        if (s is null)
        {
            result = default!;
            return false;
        }

        var acronym = Parse(s, asciiOnly: false, provider);
        result = acronym.MatchUnsafe(x => x, () => default)!;
        return acronym.IsSome;
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out TSelf result)
        => TryParse(s.ToString(), provider, out result);

    public bool Equals(TSelf? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return string.Equals(this.value, other.value, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return this.Equals(obj as TSelf);
    }

    public override int GetHashCode()
        => (this.value?.GetHashCode(StringComparison.Ordinal)) ?? 0;

    public override string ToString()
        => this.value;

    public string ToString(string? format, IFormatProvider? formatProvider)
        => this.value;

    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        var result = this.value;
        charsWritten = Math.Min(result.Length, destination.Length);
        result.CopyTo(destination[..charsWritten]);
        return charsWritten == result.Length;
    }

#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields

    private static TSelf? Create(string s, CultureInfo culture)
        => (TSelf?)Activator.CreateInstance(typeof(TSelf),
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            binder: null,
            [s],
            culture);

#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
}
#pragma warning restore MA0018 // Do not declare static members on generic types (deprecated; use CA1000 instead)
#pragma warning restore CA1000 // Do not declare static members on generic types
