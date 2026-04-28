using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using LanguageExt;
using LanguageExt.Parsec;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Prelude;

namespace TIKSN.Numbering;

public sealed class BB26 : IComparable<BB26>, IComparable, ISequentialNavigator<BB26>, ISerial<BB26>
{
    public BB26(int number)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(number);

        this.Number = number;
    }

    public int Index => this.Number - 1;

    public int Number { get; }

    private static IReadOnlyDictionary<char, int> CharToNumberMap { get; } = Enumerable.Range(0, 26).SelectMany(i =>
        new[]
        {
            Tuple((char)('A' + i), i + 1),
            Tuple((char)('a' + i), i + 1)
        }).ToFrozenDictionary(k => k.Item1, v => v.Item2);

    private static IReadOnlyDictionary<int, char> NumberToCharMap { get; } =
        Enumerable.Range(0, 26).ToFrozenDictionary(i => i + 1, i => (char)('A' + i));

    private static Parser<BB26> Parser { get; } =
        from chars in many1(satisfy(CharToNumberMap.ContainsKey))
        from _ in eof
        let number = chars.Aggregate(0, (acc, c) => (acc * 26) + CharToNumberMap[c])
        select new BB26(number);

    public static bool operator !=(BB26 left, BB26 right) => !Equals(left, right);

    public static bool operator <(BB26 left, BB26 right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(BB26 left, BB26 right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator ==(BB26 left, BB26 right) => Equals(left, right);

    public static bool operator >(BB26 left, BB26 right) => left?.CompareTo(right) > 0;

    public static bool operator >=(BB26 left, BB26 right) => left is null ? right is null : left.CompareTo(right) >= 0;

    public static Option<BB26> Parse(string s, bool asciiOnly, IFormatProvider? provider)
    {
        var result = parse(Parser, s);

        return result.ToOption();
    }

    public static Option<BB26> Parse(ReadOnlySpan<char> s, bool asciiOnly, IFormatProvider? provider)
        => Parse(s.ToString(), asciiOnly, provider);

    public static BB26 Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s.ToString(), provider);

    public static BB26 Parse(string s, IFormatProvider? provider)
    {
        if (TryParse(s, provider, out var result))
        {
            return result;
        }

        throw new FormatException("Input string was not in a correct format.");
    }

    public static bool TryParse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out BB26 result)
        => TryParse(s.ToString(), provider, out result);

    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out BB26 result)
    {
        if (s is null)
        {
            result = default!;
            return false;
        }

        var bb26Number = Parse(s, asciiOnly: false, provider);
        result = bb26Number.MatchUnsafe(x => x, () => default)!;
        return bb26Number.IsSome;
    }

    public int CompareTo(BB26? other)
    {
        if (other is null)
        {
            return 1;
        }

        return this.Number.CompareTo(other.Number);
    }

    public int CompareTo(object? obj)
    {
        if (obj is BB26 other)
        {
            return this.CompareTo(other);
        }

        throw new ArgumentException($"Object must be of type {nameof(BB26)}");
    }

    public bool Equals(BB26? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.Number == other.Number;
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || (obj is BB26 other && this.Equals(other));

    public override int GetHashCode() => this.Number;

    public Option<BB26> GetNext()
    {
        if (this.Number == int.MaxValue)
        {
            return None;
        }

        return new BB26(this.Number + 1);
    }

    public Option<BB26> GetPrevious()
    {
        if (this.Number == 1)
        {
            return None;
        }

        return new BB26(this.Number - 1);
    }

    public override string ToString() => this.ToString(format: null, CultureInfo.InvariantCulture);

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var carry = this.Number;
        var chars = new List<char>();
        do
        {
            var remainderNumber = carry % NumberToCharMap.Count;
            if (remainderNumber == 0)
            {
                remainderNumber = NumberToCharMap.Count;
            }

            chars.Add(NumberToCharMap[remainderNumber]);
            carry -= remainderNumber;
            carry /= NumberToCharMap.Count;
        } while (carry != 0);

        chars.Reverse();

        return new string([.. chars]);
    }

    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        var result = this.ToString(format: null, provider);
        charsWritten = Math.Min(result.Length, destination.Length);
        result.CopyTo(destination[..charsWritten]);
        return charsWritten == result.Length;
    }
}
