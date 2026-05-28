using System.Globalization;

namespace TIKSN.Globalization;

public sealed class CountryInfo : IEquatable<CountryInfo>
{
    private readonly HashSet<string> regionNames;

    internal CountryInfo(string name, RegionInfo principalRegion, IReadOnlyCollection<RegionInfo> regions)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(principalRegion);
        ArgumentNullException.ThrowIfNull(regions);

        this.Name = name;
        this.TwoLetterISORegionName = name;
        this.PrincipalRegion = principalRegion;
        this.Regions = Array.AsReadOnly(regions.ToArray());
        this.regionNames = [.. this.Regions.Select(x => x.TwoLetterISORegionName)];

        if (!this.regionNames.Contains(this.PrincipalRegion.TwoLetterISORegionName))
        {
            throw new ArgumentException(
                $"Country '{this.Name}' regions must include its principal region.",
                nameof(regions));
        }
    }

    public string Name { get; }

    public RegionInfo PrincipalRegion { get; }

    public IReadOnlyCollection<RegionInfo> Regions { get; }

    public string TwoLetterISORegionName { get; }

    public static bool operator ==(CountryInfo? first, CountryInfo? second) => Equals(first, second);

    public static bool operator !=(CountryInfo? first, CountryInfo? second) => !Equals(first, second);

    public bool ContainsRegion(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return this.ContainsRegion(new RegionInfo(name));
    }

    public bool ContainsRegion(RegionInfo region)
    {
        ArgumentNullException.ThrowIfNull(region);

        return this.regionNames.Contains(region.TwoLetterISORegionName);
    }

    public bool Equals(CountryInfo? other)
    {
        if (other is null)
        {
            return false;
        }

        return string.Equals(this.Name, other.Name, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is null)
        {
            return false;
        }

        if (obj is not CountryInfo that)
        {
            return false;
        }

        return this.Equals(that);
    }

    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(this.Name);

    public override string ToString() => this.Name;
}
