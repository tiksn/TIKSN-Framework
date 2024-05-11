using System.Globalization;
using LanguageExt;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Prelude;

namespace TIKSN.Deployment;

public sealed class EnvironmentName : ISpanFormattable, IEquatable<EnvironmentName>
{
    private static readonly char[] SeparatorCharacters = ['-', '_', '+'];

    private readonly Seq<string> segments;

    private EnvironmentName(Seq<string> segments)
        => this.segments = segments;

    #region Parsing

    public static Option<EnvironmentName> Parse(
        string name,
        bool asciiOnly,
        IFormatProvider provider)
    {
        if (string.IsNullOrEmpty(name))
        {
            return None;
        }

        var culture = provider as CultureInfo ?? CultureInfo.InvariantCulture;

        var asciiLatterParser = satisfy(c => char.IsAscii(c) && char.IsLetter(c)).label("ASCII letter");

        var letterParser = asciiOnly ? asciiLatterParser : letter;

        var lettersParser = many1(letterParser);

        var separatorParser = oneOf(SeparatorCharacters);
        var segmentsParser = sepBy(lettersParser, separatorParser);

        var parser =
            from segments in segmentsParser
            from _ in eof
            select segments;

        return parser
            .Parse(name)
            .ToOption()
            .Map(x => x.Map(y => culture.TextInfo.ToTitleCase(new string(y.ToArray()))))
            .Map(x => new EnvironmentName(x));
    }

    public static Option<EnvironmentName> Parse(
        ReadOnlySpan<char> name,
        bool asciiOnly,
        IFormatProvider provider)
        => Parse(name.ToString(), asciiOnly, provider);

    #endregion Parsing

    #region Formatting

    public override string ToString()
        => this.ToString(string.Empty, formatProvider: null);

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var culture = formatProvider as CultureInfo ?? CultureInfo.InvariantCulture;

        var properCaseSegments = GetProperCaseSegments(this.segments, format, culture);

        return string.Join(SeparatorCharacters[0], properCaseSegments);

        static Seq<string> GetProperCaseSegments(
            Seq<string> segments, string? format, CultureInfo culture) => format switch
            {
                "U" or "u" => segments.Map(culture.TextInfo.ToUpper),
                "L" or "l" => segments.Map(culture.TextInfo.ToLower),
                "T" or "t" => segments.Map(x => culture.TextInfo.ToTitleCase(culture.TextInfo.ToLower(x))),
                _ => segments,
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
        result.CopyTo(destination[..charsWritten]);
        return charsWritten == result.Length;
    }

    #endregion Formatting

    #region Equality

    public bool Equals(EnvironmentName? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Enumerable.SequenceEqual(
            this.segments,
            other.segments,
            StringComparer.InvariantCultureIgnoreCase);
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || (obj is EnvironmentName other && this.Equals(other));

    public override int GetHashCode() => this.segments.GetHashCode();

    public static bool operator ==(EnvironmentName left, EnvironmentName right) => Equals(left, right);

    public static bool operator !=(EnvironmentName left, EnvironmentName right) => !Equals(left, right);

    #endregion Equality

    #region Matches

    public bool Matches(EnvironmentName other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        var segmentsTail =
            this.segments.Length <= other.segments.Length
                ? this.segments
                : this.segments.Skip(this.segments.Length - other.segments.Length);

        return Enumerable.SequenceEqual(
            segmentsTail,
            other.segments,
            StringComparer.InvariantCultureIgnoreCase);
    }

    #endregion Matches
}
