using System;
using System.Globalization;
using System.Reflection;
using LanguageExt;
using LanguageExt.Parsec;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Expr;
using static LanguageExt.Parsec.Token;

namespace TIKSN.Numbering.Acronyms;

public abstract class Acronym<TSelf> : ISerial<TSelf>
    where TSelf : Acronym<TSelf>
{
    private readonly string value;

    protected Acronym(string value)
        => this.value = value ?? throw new ArgumentNullException(nameof(value));

    public override string ToString() => this.value;

    public static TSelf Parse(string s, IFormatProvider provider)
    {
        if (TryParse(s, provider, out var result))
        {
            return result;
        }

        throw new FormatException("Input string was not in a correct format.");
    }

    public static bool TryParse(string s, IFormatProvider provider, out TSelf result)
    {
        var acronym = Parse(s, asciiOnly: false, provider);
        result = acronym.MatchUnsafe(x => x, () => default);
        return acronym.IsSome;
    }

    public static TSelf Parse(ReadOnlySpan<char> s, IFormatProvider provider)
        => Parse(s.ToString(), provider);

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider provider, out TSelf result)
        => TryParse(s.ToString(), provider, out result);

    public static Option<TSelf> Parse(string s, bool asciiOnly, IFormatProvider provider)
    {
        if (string.IsNullOrEmpty(s))
        {
            return None;
        }

        var asciiLatterParser = satisfy(c => char.IsAscii(c) && char.IsLetter(c)).label("ASCII letter");

        var letterParser = asciiOnly ? asciiLatterParser : letter;

        var culture = provider as CultureInfo ?? CultureInfo.InvariantCulture;

        var lettersParser =
            from letters in count(GetLetterCount(), letterParser.Map(x => char.ToUpper(x, culture)))
            from _ in eof
            select letters;

        var parser = asString(lettersParser);

        var result = parse(parser, s);

        return result.ToOption().Select(x => Create(x, culture));
    }

    public static Option<TSelf> Parse(ReadOnlySpan<char> s, bool asciiOnly, IFormatProvider provider)
        => Parse(s.ToString(), asciiOnly, provider);

    private static TSelf Create(string s, CultureInfo culture)
        => (TSelf)Activator.CreateInstance(typeof(TSelf),
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new object[] {s},
            culture);

    private static int GetLetterCount()
        => (int)typeof(TSelf).GetField("LetterCount", BindingFlags.NonPublic | BindingFlags.Static)
            .GetValue(null);
}
