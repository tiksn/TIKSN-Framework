using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using LanguageExt;
using static LanguageExt.Prelude;

namespace TIKSN.Identity;

public sealed class ServiceInstanceId :
    ISpanParsable<ServiceInstanceId>,
    ISpanFormattable,
    IEquatable<ServiceInstanceId>
{
    private readonly object value;

    private ServiceInstanceId(object value) => this.value = value;

    public static ServiceInstanceId Create(long value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        return new ServiceInstanceId(value);
    }

    public static ServiceInstanceId Create(int value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        return new ServiceInstanceId(value);
    }

    public static ServiceInstanceId Create(Guid value) => new(value);

    public static ServiceInstanceId Create(Ulid value) => new(value);

    public static ServiceInstanceId Parse(string s, IFormatProvider? provider)
    {
        ArgumentNullException.ThrowIfNull(s);
        return TryParse(s.AsSpan(), provider, out var result)
            ? result
            : throw new FormatException("Invalid Service Instance ID format.");
    }

    public static ServiceInstanceId Parse(ReadOnlySpan<char> s, IFormatProvider? provider) =>
        TryParse(s, provider, out var result)
            ? result
            : throw new FormatException("Invalid Service Instance ID format.");

    public static ServiceInstanceId Parse(string input) =>
        Parse(input, provider: null);

    public static ServiceInstanceId Parse(ReadOnlySpan<char> input) =>
        Parse(input, provider: null);

    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out ServiceInstanceId result)
    {
        if (s is null)
        {
            result = null;
            return false;
        }

        return TryParse(s.AsSpan(), provider, out result);
    }

    public static bool TryParse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out ServiceInstanceId result)
    {
        if (s.IsEmpty || s.Length != s.Trim().Length)
        {
            result = null;
            return false;
        }

        if (Guid.TryParse(s, provider, out var guidValue))
        {
            result = new ServiceInstanceId(guidValue);
            return true;
        }

        if (Ulid.TryParse(s, provider, out var ulidValue))
        {
            result = new ServiceInstanceId(ulidValue);
            return true;
        }

        if (long.TryParse(s, NumberStyles.Integer, provider ?? CultureInfo.InvariantCulture, out var longValue) &&
            longValue >= 0L)
        {
            result = new ServiceInstanceId(longValue);
            return true;
        }

        result = null;
        return false;
    }

    public static Option<ServiceInstanceId> TryParse(string input) =>
        TryParse(input, provider: null, out var result) ? Some(result) : None;

    public static Option<ServiceInstanceId> TryParse(ReadOnlySpan<char> input) =>
        TryParse(input, provider: null, out var result) ? Some(result) : None;

    public static bool operator ==(ServiceInstanceId? left, ServiceInstanceId? right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(ServiceInstanceId? left, ServiceInstanceId? right) => !(left == right);

    public override bool Equals(object? obj) => this.Equals(obj as ServiceInstanceId);

    public bool Equals(ServiceInstanceId? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.value.GetType() == other.value.GetType() && this.value.Equals(other.value);
    }

    public override int GetHashCode() => this.value.GetHashCode();

    public override string ToString() => this.ToString(format: null, formatProvider: null);

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        if (this.value is IFormattable formattable)
        {
            return formattable.ToString(format, formatProvider);
        }

        return this.value.ToString() ?? string.Empty;
    }

    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        if (this.value is ISpanFormattable spanFormattable)
        {
            return spanFormattable.TryFormat(destination, out charsWritten, format, provider);
        }

        var formatted = this.ToString(format.IsEmpty ? null : format.ToString(), provider);
        if (formatted.AsSpan().TryCopyTo(destination))
        {
            charsWritten = formatted.Length;
            return true;
        }

        charsWritten = 0;
        return false;
    }
}
