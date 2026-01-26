using LanguageExt;
using static LanguageExt.Prelude;

namespace TIKSN.Identity;

public sealed class ServiceIdentity : IEquatable<ServiceIdentity>
{
    private const string ComponentSeparator = ".";
    private const string InstanceIdSeparator = ":";

    public ServiceIdentity(string applicationName, IEnumerable<string> componentNames, ServiceInstanceId instanceId)
        : this(applicationName, toSeq(componentNames), instanceId)
    {
    }

    public ServiceIdentity(string applicationName, Seq<string> componentNames, ServiceInstanceId instanceId)
    {
        if (string.IsNullOrWhiteSpace(applicationName))
        {
            throw new ArgumentException("Application name cannot be null or whitespace.", nameof(applicationName));
        }

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

    public static bool operator !=(ServiceIdentity? left, ServiceIdentity? right)
        => !(left == right);

    public static bool operator ==(ServiceIdentity? left, ServiceIdentity? right)
        => EqualityComparer<ServiceIdentity>.Default.Equals(left, right);

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

    public override string ToString()
        => this.ComponentPath.Match(
            some => $"{this.ApplicationName}{ComponentSeparator}{some}{InstanceIdSeparator}{this.InstanceId}",
            () => $"{this.ApplicationName}{InstanceIdSeparator}{this.InstanceId}");
}
