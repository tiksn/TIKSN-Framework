using System.Globalization;
using LanguageExt;

namespace TIKSN.Identity;

public sealed class ServiceInstanceId : IEquatable<ServiceInstanceId>
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

    public static bool operator !=(ServiceInstanceId? left, ServiceInstanceId? right) => !(left == right);

    public static bool operator ==(ServiceInstanceId? left, ServiceInstanceId? right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static ServiceInstanceId Parse(string input) => TryParse(input).IfNone(() => throw new FormatException("Invalid Service Instance ID format."));

    public static Option<ServiceInstanceId> TryParse(string input)
    {
        if (Guid.TryParse(input, out var guidValue))
        {
            return new ServiceInstanceId(guidValue);
        }

        if (Ulid.TryParse(input, out var ulidValue))
        {
            return new ServiceInstanceId(ulidValue);
        }

        if (long.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out var longValue) && longValue >= 0L)
        {
            return new ServiceInstanceId(longValue);
        }

        return Option<ServiceInstanceId>.None;
    }

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

    public override string ToString() => this.value.ToString() ?? string.Empty;
}
