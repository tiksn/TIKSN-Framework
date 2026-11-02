using System.Diagnostics.CodeAnalysis;
using LanguageExt;
using static LanguageExt.Prelude;

namespace TIKSN.Identity;

public sealed class ServiceIdentity :
    ISpanParsable<ServiceIdentity>,
    ISpanFormattable,
    IEquatable<ServiceIdentity>
{
    public const string ComponentSeparator = ".";
    public const char ComponentSeparatorChar = '.';
    public const string InstanceIdSeparator = ":";
    public const char InstanceIdSeparatorChar = ':';

    public ServiceIdentity(string applicationName, IEnumerable<string> componentNames, ServiceInstanceId instanceId)
        : this(applicationName, Seq(componentNames), instanceId)
    {
    }

    public ServiceIdentity(string applicationName, Seq<string> componentNames, ServiceInstanceId instanceId)
    {
        if (string.IsNullOrWhiteSpace(applicationName))
        {
            throw new ArgumentException("Application name cannot be null or whitespace.", nameof(applicationName));
        }

        ArgumentNullException.ThrowIfNull(instanceId);

        this.ApplicationName = applicationName;
        this.ComponentNames = componentNames;
        this.InstanceId = instanceId;

        this.ComponentPath = componentNames.Any() ? Some(string.Join(ComponentSeparator, componentNames)) : None;
        this.ServicePath = string.Join(ComponentSeparator, componentNames.Prepend(applicationName));
    }

    public string ApplicationName { get; }
    public Seq<string> ComponentNames { get; }
    public Option<string> ComponentPath { get; }
    public ServiceInstanceId InstanceId { get; }
    public string ServicePath { get; }

    public static ServiceIdentity Parse(string s, IFormatProvider? provider)
    {
        ArgumentNullException.ThrowIfNull(s);

        return TryParse(s.AsSpan(), provider, out var result)
            ? result
            : throw new FormatException("Invalid Service Identity format.");
    }

    public static ServiceIdentity Parse(ReadOnlySpan<char> s, IFormatProvider? provider) =>
        TryParse(s, provider, out var result)
            ? result
            : throw new FormatException("Invalid Service Identity format.");

    public static ServiceIdentity Parse(string input) =>
        Parse(input, provider: null);

    public static ServiceIdentity Parse(ReadOnlySpan<char> input) =>
        Parse(input, provider: null);

    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out ServiceIdentity result)
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
        [MaybeNullWhen(false)] out ServiceIdentity result)
    {
        result = null;

        if (s.IsEmpty || s.Length != s.Trim().Length)
        {
            return false;
        }

        var colonIndex = s.IndexOf(InstanceIdSeparatorChar);
        if (colonIndex < 0)
        {
            return false;
        }

        var servicePathSpan = s[..colonIndex];
        var instanceIdSpan = s[(colonIndex + 1)..];

        if (instanceIdSpan.IndexOf(InstanceIdSeparatorChar) >= 0)
        {
            return false;
        }

        if (servicePathSpan.IsEmpty ||
            servicePathSpan.StartsWith(ComponentSeparatorChar) ||
            servicePathSpan.EndsWith(ComponentSeparatorChar))
        {
            return false;
        }

        if (!ServiceInstanceId.TryParse(instanceIdSpan, provider, out var instanceId))
        {
            return false;
        }

        string? applicationName = null;
        var componentNames = new List<string>();

        var remaining = servicePathSpan;
        while (!remaining.IsEmpty)
        {
            var dotIndex = remaining.IndexOf(ComponentSeparatorChar);
            ReadOnlySpan<char> segment;
            if (dotIndex < 0)
            {
                segment = remaining;
                remaining = default;
            }
            else
            {
                segment = remaining[..dotIndex];
                remaining = remaining[(dotIndex + 1)..];
            }

            if (segment.IsEmpty || segment.Length != segment.Trim().Length)
            {
                return false;
            }

            var segmentStr = segment.ToString();
            if (applicationName is null)
            {
                applicationName = segmentStr;
            }
            else
            {
                componentNames.Add(segmentStr);
            }
        }

        if (applicationName is null)
        {
            return false;
        }

        result = new ServiceIdentity(applicationName, Seq(componentNames), instanceId);
        return true;
    }

    public static Option<ServiceIdentity> TryParse(string? input) =>
        TryParse(input, provider: null, out var result) ? Some(result) : None;

    public static Option<ServiceIdentity> TryParse(ReadOnlySpan<char> input) =>
        TryParse(input, provider: null, out var result) ? Some(result) : None;

    public static bool operator ==(ServiceIdentity? left, ServiceIdentity? right)
        => EqualityComparer<ServiceIdentity>.Default.Equals(left, right);

    public static bool operator !=(ServiceIdentity? left, ServiceIdentity? right)
        => !(left == right);

    public ServiceIdentity CreateAnotherInstance(ServiceInstanceId newInstanceId)
        => new(this.ApplicationName, this.ComponentNames, newInstanceId);

    public override bool Equals(object? obj)
        => this.Equals(obj as ServiceIdentity);

    public bool Equals(ServiceIdentity? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.ApplicationName == other.ApplicationName &&
            this.ComponentNames.SequenceEqual(other.ComponentNames) &&
            this.InstanceId.Equals(other.InstanceId);
    }

    public override int GetHashCode() => HashCode.Combine(this.ApplicationName, this.ComponentPath, this.InstanceId);

    public override string ToString() => this.ToString(format: null, formatProvider: null);

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        if (string.IsNullOrEmpty(format) || string.Equals(format, "G", StringComparison.OrdinalIgnoreCase))
        {
            return string.Concat(this.ServicePath, InstanceIdSeparator, this.InstanceId.ToString(null, formatProvider));
        }

        if (string.Equals(format, "S", StringComparison.OrdinalIgnoreCase))
        {
            return this.ServicePath;
        }

        if (string.Equals(format, "A", StringComparison.OrdinalIgnoreCase))
        {
            return this.ApplicationName;
        }

        if (string.Equals(format, "C", StringComparison.OrdinalIgnoreCase))
        {
            return this.ComponentPath.IfNone(string.Empty);
        }

        if (string.Equals(format, "I", StringComparison.OrdinalIgnoreCase))
        {
            return this.InstanceId.ToString(null, formatProvider);
        }

        throw new FormatException($"Format specifier '{format}' is not supported.");
    }

    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        if (format.IsEmpty || format.Equals("G", StringComparison.OrdinalIgnoreCase))
        {
            var requiredLength = this.ServicePath.Length + 1;
            if (destination.Length < requiredLength)
            {
                charsWritten = 0;
                return false;
            }

            if (!this.ServicePath.AsSpan().TryCopyTo(destination))
            {
                charsWritten = 0;
                return false;
            }

            destination[this.ServicePath.Length] = InstanceIdSeparatorChar;

            if (!this.InstanceId.TryFormat(destination[requiredLength..], out var idCharsWritten, default, provider))
            {
                charsWritten = 0;
                return false;
            }

            charsWritten = requiredLength + idCharsWritten;
            return true;
        }

        if (format.Equals("S", StringComparison.OrdinalIgnoreCase))
        {
            return TryCopyStringToDestination(this.ServicePath, destination, out charsWritten);
        }

        if (format.Equals("A", StringComparison.OrdinalIgnoreCase))
        {
            return TryCopyStringToDestination(this.ApplicationName, destination, out charsWritten);
        }

        if (format.Equals("C", StringComparison.OrdinalIgnoreCase))
        {
            var componentPath = this.ComponentPath.IfNone(string.Empty);
            return TryCopyStringToDestination(componentPath, destination, out charsWritten);
        }

        if (format.Equals("I", StringComparison.OrdinalIgnoreCase))
        {
            return this.InstanceId.TryFormat(destination, out charsWritten, default, provider);
        }

        throw new FormatException($"Format specifier '{format}' is not supported.");

        static bool TryCopyStringToDestination(string value, Span<char> destination, out int charsWritten)
        {
            if (value.AsSpan().TryCopyTo(destination))
            {
                charsWritten = value.Length;
                return true;
            }

            charsWritten = 0;
            return false;
        }
    }
}
