using System.Diagnostics;
using System.Globalization;
using NuGet.Versioning;

namespace TIKSN.Versioning;

public sealed class Version : IComparable<Version>, IEquatable<Version>, IComparable
{
    private const Milestone DefaultMilestone = Milestone.Release;
    private const int DefaultPreReleaseNumber = -1;

    private int preReleaseNumber;

    public Version(
        int releaseMajor,
        int releaseMinor)
    {
        this.Release = new System.Version(releaseMajor, releaseMinor);
        this.Milestone = DefaultMilestone;
        this.PreReleaseNumber = DefaultPreReleaseNumber;
        this.ReleaseDate = null;
    }

    public Version(
        int releaseMajor,
        int releaseMinor,
        DateTimeOffset releaseDate)
    {
        this.Release = new System.Version(releaseMajor, releaseMinor);
        this.Milestone = DefaultMilestone;
        this.PreReleaseNumber = DefaultPreReleaseNumber;
        this.ReleaseDate = releaseDate;
    }

    public Version(
        int releaseMajor,
        int releaseMinor,
        int releaseBuild)
    {
        this.Release = new System.Version(releaseMajor, releaseMinor, releaseBuild);
        this.Milestone = DefaultMilestone;
        this.PreReleaseNumber = DefaultPreReleaseNumber;
        this.ReleaseDate = null;
    }

    public Version(
        int releaseMajor,
        int releaseMinor,
        int releaseBuild,
        DateTimeOffset releaseDate)
    {
        this.Release = new System.Version(releaseMajor, releaseMinor, releaseBuild);
        this.Milestone = DefaultMilestone;
        this.PreReleaseNumber = DefaultPreReleaseNumber;
        this.ReleaseDate = releaseDate;
    }

    public Version(
        int releaseMajor,
        int releaseMinor,
        int releaseBuild,
        int releaseRevision)
    {
        this.Release = new System.Version(releaseMajor, releaseMinor, releaseBuild, releaseRevision);
        this.Milestone = DefaultMilestone;
        this.PreReleaseNumber = DefaultPreReleaseNumber;
        this.ReleaseDate = null;
    }

    public Version(
        int releaseMajor,
        int releaseMinor,
        int releaseBuild,
        int releaseRevision,
        DateTimeOffset releaseDate)
    {
        this.Release = new System.Version(releaseMajor, releaseMinor, releaseBuild, releaseRevision);
        this.Milestone = DefaultMilestone;
        this.PreReleaseNumber = DefaultPreReleaseNumber;
        this.ReleaseDate = releaseDate;
    }

    public Version(System.Version release)
    {
        this.Release = release;
        this.Milestone = DefaultMilestone;
        this.PreReleaseNumber = DefaultPreReleaseNumber;
        this.ReleaseDate = null;
    }

    public Version(
        System.Version release,
        DateTimeOffset releaseDate)
    {
        this.Release = release;
        this.Milestone = DefaultMilestone;
        this.PreReleaseNumber = DefaultPreReleaseNumber;
        this.ReleaseDate = releaseDate;
    }

    public Version(
        int releaseMajor,
        int releaseMinor,
        Milestone milestone)
    {
        this.Release = new System.Version(releaseMajor, releaseMinor);
        this.Milestone = milestone;
        this.PreReleaseNumber = DefaultPreReleaseNumber;
        this.ReleaseDate = null;

        this.ValidateMilestoneAndPrerelease();
    }

    public Version(
        int releaseMajor,
        int releaseMinor,
        Milestone milestone,
        DateTimeOffset releaseDate)
    {
        this.Release = new System.Version(releaseMajor, releaseMinor);
        this.Milestone = milestone;
        this.PreReleaseNumber = DefaultPreReleaseNumber;
        this.ReleaseDate = releaseDate;

        this.ValidateMilestoneAndPrerelease();
    }

    public Version(
        int releaseMajor,
        int releaseMinor,
        int releaseBuild,
        Milestone milestone)
    {
        this.Release = new System.Version(releaseMajor, releaseMinor, releaseBuild);
        this.Milestone = milestone;
        this.PreReleaseNumber = DefaultPreReleaseNumber;
        this.ReleaseDate = null;

        this.ValidateMilestoneAndPrerelease();
    }

    public Version(
        int releaseMajor,
        int releaseMinor,
        int releaseBuild,
        Milestone milestone,
        DateTimeOffset releaseDate)
    {
        this.Release = new System.Version(releaseMajor, releaseMinor, releaseBuild);
        this.Milestone = milestone;
        this.PreReleaseNumber = DefaultPreReleaseNumber;
        this.ReleaseDate = releaseDate;

        this.ValidateMilestoneAndPrerelease();
    }

    public Version(
        int releaseMajor,
        int releaseMinor,
        int releaseBuild,
        int releaseRevision,
        Milestone milestone)
    {
        this.Release = new System.Version(releaseMajor, releaseMinor, releaseBuild, releaseRevision);
        this.Milestone = milestone;
        this.PreReleaseNumber = DefaultPreReleaseNumber;
        this.ReleaseDate = null;

        this.ValidateMilestoneAndPrerelease();
    }

    public Version(
        int releaseMajor,
        int releaseMinor,
        int releaseBuild,
        int releaseRevision,
        Milestone milestone,
        DateTimeOffset releaseDate)
    {
        this.Release = new System.Version(releaseMajor, releaseMinor, releaseBuild, releaseRevision);
        this.Milestone = milestone;
        this.PreReleaseNumber = DefaultPreReleaseNumber;
        this.ReleaseDate = releaseDate;

        this.ValidateMilestoneAndPrerelease();
    }

    public Version(
        System.Version release,
        Milestone milestone)
    {
        this.Release = release;
        this.Milestone = milestone;
        this.PreReleaseNumber = DefaultPreReleaseNumber;
        this.ReleaseDate = null;

        this.ValidateMilestoneAndPrerelease();
    }

    public Version(
        System.Version release,
        Milestone milestone,
        DateTimeOffset releaseDate)
    {
        this.Release = release;
        this.Milestone = milestone;
        this.PreReleaseNumber = DefaultPreReleaseNumber;
        this.ReleaseDate = releaseDate;

        this.ValidateMilestoneAndPrerelease();
    }

    public Version(
        int releaseMajor,
        int releaseMinor,
        Milestone milestone,
        int preReleaseNumber)
    {
        this.Release = new System.Version(releaseMajor, releaseMinor);
        this.Milestone = milestone;
        this.PreReleaseNumber = preReleaseNumber;
        this.ReleaseDate = null;

        this.ValidateMilestoneAndPrerelease();
    }

    public Version(
        int releaseMajor,
        int releaseMinor,
        Milestone milestone,
        int preReleaseNumber,
        DateTimeOffset releaseDate)
    {
        this.Release = new System.Version(releaseMajor, releaseMinor);
        this.Milestone = milestone;
        this.PreReleaseNumber = preReleaseNumber;
        this.ReleaseDate = releaseDate;

        this.ValidateMilestoneAndPrerelease();
    }

    public Version(
        int releaseMajor,
        int releaseMinor,
        int releaseBuild,
        Milestone milestone,
        int preReleaseNumber)
    {
        this.Release = new System.Version(releaseMajor, releaseMinor, releaseBuild);
        this.Milestone = milestone;
        this.PreReleaseNumber = preReleaseNumber;
        this.ReleaseDate = null;

        this.ValidateMilestoneAndPrerelease();
    }

    public Version(
        int releaseMajor,
        int releaseMinor,
        int releaseBuild,
        Milestone milestone,
        int preReleaseNumber,
        DateTimeOffset releaseDate)
    {
        this.Release = new System.Version(releaseMajor, releaseMinor, releaseBuild);
        this.Milestone = milestone;
        this.PreReleaseNumber = preReleaseNumber;
        this.ReleaseDate = releaseDate;

        this.ValidateMilestoneAndPrerelease();
    }

    public Version(
        int releaseMajor,
        int releaseMinor,
        int releaseBuild,
        int releaseRevision,
        Milestone milestone,
        int preReleaseNumber)
    {
        this.Release = new System.Version(releaseMajor, releaseMinor, releaseBuild, releaseRevision);
        this.Milestone = milestone;
        this.PreReleaseNumber = preReleaseNumber;
        this.ReleaseDate = null;

        this.ValidateMilestoneAndPrerelease();
    }

    public Version(
        int releaseMajor,
        int releaseMinor,
        int releaseBuild,
        int releaseRevision,
        Milestone milestone,
        int preReleaseNumber,
        DateTimeOffset releaseDate)
    {
        this.Release = new System.Version(releaseMajor, releaseMinor, releaseBuild, releaseRevision);
        this.Milestone = milestone;
        this.PreReleaseNumber = preReleaseNumber;
        this.ReleaseDate = releaseDate;

        this.ValidateMilestoneAndPrerelease();
    }

    public Version(
        System.Version release,
        Milestone milestone,
        int preReleaseNumber)
    {
        this.Release = release;
        this.Milestone = milestone;
        this.PreReleaseNumber = preReleaseNumber;
        this.ReleaseDate = null;

        this.ValidateMilestoneAndPrerelease();
    }

    public Version(
        System.Version release,
        Milestone milestone,
        int preReleaseNumber,
        DateTimeOffset releaseDate)
    {
        this.Release = release;
        this.Milestone = milestone;
        this.PreReleaseNumber = preReleaseNumber;
        this.ReleaseDate = releaseDate;

        this.ValidateMilestoneAndPrerelease();
    }

    public Milestone Milestone { get; }

    public int PreReleaseNumber
    {
        get => this.preReleaseNumber;
        private set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, -1);

            this.preReleaseNumber = value;
        }
    }

    public System.Version Release { get; }

    public DateTimeOffset? ReleaseDate { get; }

    public Stability Stability
    {
        get
        {
            if (this.Milestone == Milestone.Release)
            {
                return Stability.Stable;
            }

            return Stability.Unstable;
        }
    }

    public static explicit operator NuGetVersion(Version version)
        => ToNuGetVersion(version);

    public static explicit operator SemanticVersion(Version version)
        => ToSemanticVersion(version);

    public static explicit operator Version(NuGetVersion nuGetVersion)
        => FromNuGetVersion(nuGetVersion);

    public static explicit operator Version(SemanticVersion semanticVersion)
        => FromSemanticVersion(semanticVersion);

    public static Version FromNuGetVersion(NuGetVersion nuGetVersion)
    {
        ArgumentNullException.ThrowIfNull(nuGetVersion);

        var (milestone, prereleaseNumber) =
            GetMilestoneAndPrereleaseNumber(nuGetVersion.IsPrerelease, nuGetVersion.ReleaseLabels.ToArray());

        var releaseNumbersCount =
            (nuGetVersion.OriginalVersion?
                .Remove(nuGetVersion.OriginalVersion.Length - nuGetVersion.Release.Length)
                .Split('.')
                .Length) ?? 0;

        System.Version release;
        if (releaseNumbersCount == 2)
        {
            release = new System.Version(
                        nuGetVersion.Major,
                        nuGetVersion.Minor);
        }
        else if (releaseNumbersCount == 3)
        {
            release = new System.Version(
                        nuGetVersion.Major,
                        nuGetVersion.Minor,
                        nuGetVersion.Patch);
        }
        else
        {
            release = new System.Version(
                        nuGetVersion.Major,
                        nuGetVersion.Minor,
                        nuGetVersion.Patch,
                        nuGetVersion.Revision);
        }

        if (nuGetVersion.HasMetadata)
        {
            return new Version(release, milestone, prereleaseNumber,
                GetReleaseDate(nuGetVersion.Metadata));
        }

        return new Version(release, milestone, prereleaseNumber);
    }

    public static Version FromSemanticVersion(SemanticVersion semanticVersion)
    {
        ArgumentNullException.ThrowIfNull(semanticVersion);

        var (milestone, prereleaseNumber) = GetMilestoneAndPrereleaseNumber(semanticVersion.IsPrerelease,
            semanticVersion.ReleaseLabels.ToArray());

        if (semanticVersion.HasMetadata)
        {
            return new Version(semanticVersion.Major, semanticVersion.Minor, semanticVersion.Patch, milestone,
                prereleaseNumber, GetReleaseDate(semanticVersion.Metadata));
        }

        return new Version(semanticVersion.Major, semanticVersion.Minor, semanticVersion.Patch, milestone,
            prereleaseNumber);
    }

    public static bool operator !=(Version v1, Version v2)
    {
        ArgumentNullException.ThrowIfNull(v1);
        ArgumentNullException.ThrowIfNull(v2);

        return v1.CompareTo(v2) != 0;
    }

    public static bool operator <(Version v1, Version v2)
    {
        ArgumentNullException.ThrowIfNull(v1);
        ArgumentNullException.ThrowIfNull(v2);

        return v1.CompareTo(v2) < 0;
    }

    public static bool operator <=(Version v1, Version v2)
    {
        ArgumentNullException.ThrowIfNull(v1);
        ArgumentNullException.ThrowIfNull(v2);

        return v1.CompareTo(v2) <= 0;
    }

    public static bool operator ==(Version v1, Version v2)
    {
        ArgumentNullException.ThrowIfNull(v1);
        ArgumentNullException.ThrowIfNull(v2);

        return v1.CompareTo(v2) == 0;
    }

    public static bool operator >(Version v1, Version v2)
    {
        ArgumentNullException.ThrowIfNull(v1);
        ArgumentNullException.ThrowIfNull(v2);

        return v1.CompareTo(v2) > 0;
    }

    public static bool operator >=(Version v1, Version v2)
    {
        ArgumentNullException.ThrowIfNull(v1);
        ArgumentNullException.ThrowIfNull(v2);

        return v1.CompareTo(v2) >= 0;
    }

    public static NuGetVersion ToNuGetVersion(Version version)
    {
        ArgumentNullException.ThrowIfNull(version);

        var releaseLabels = GetReleaseLabels(version);
        var metadata = GetMetadata(version);

        if (version.Release.Revision >= 0)
        {
            return new NuGetVersion(version.Release.Major, version.Release.Minor, version.Release.Build,
                version.Release.Revision, releaseLabels, metadata);
        }

        if (version.Release.Build >= 0)
        {
            return new NuGetVersion(version.Release.Major, version.Release.Minor, version.Release.Build,
                releaseLabels, metadata);
        }

        return new NuGetVersion(version.Release.Major, version.Release.Minor, 0, releaseLabels, metadata);
    }

    public static SemanticVersion ToSemanticVersion(Version version)
    {
        ArgumentNullException.ThrowIfNull(version);

        var releaseLabels = GetReleaseLabels(version);
        var metadata = GetMetadata(version);

        if (version.Release.Revision != -1)
        {
            throw new FormatException("There is no revision in semantic version.");
        }

        if (version.Release.Build >= 0)
        {
            return new SemanticVersion(version.Release.Major, version.Release.Minor, version.Release.Build,
                releaseLabels, metadata);
        }

        return new SemanticVersion(version.Release.Major, version.Release.Minor, 0, releaseLabels, metadata);
    }

    public int CompareTo(Version? other)
    {
        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        if (other is null)
        {
            return 1;
        }

        var releaseComparison = this.Release.CompareTo(other.Release);

        if (releaseComparison == 0)
        {
            _ = this.Milestone - other.Milestone;

            if (this.Milestone > other.Milestone)
            {
                return 1;
            }

            if (this.Milestone < other.Milestone)
            {
                return -1;
            }

            Debug.Assert(this.Milestone == other.Milestone);

            if (this.PreReleaseNumber > other.PreReleaseNumber)
            {
                return 1;
            }

            if (this.PreReleaseNumber < other.PreReleaseNumber)
            {
                return -1;
            }

            Debug.Assert(this.PreReleaseNumber == other.PreReleaseNumber);

            return 0;
        }

        return releaseComparison;
    }

    public int CompareTo(object? obj)
    {
        if (obj == null)
        {
            return 1;
        }

        if (obj is Version x)
        {
            return this.CompareTo(x);
        }

        throw new ArgumentException("Unable to compare to another type of object", nameof(obj));
    }

    public bool Equals(Version? other)
    {
        if (other is null)
        {
            return false;
        }

        return this.CompareTo(other) == 0;
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

        if (obj is Version version)
        {
            return version.Equals(this);
        }

        if (obj is System.Version systemVersion)
        {
            return this.Release.CompareTo(systemVersion) == 0 &&
                this.Milestone == DefaultMilestone &&
                this.preReleaseNumber == DefaultPreReleaseNumber;
        }

        return false;
    }

    public override int GetHashCode()
        => HashCode.Combine(this.Release, this.Milestone, this.PreReleaseNumber, this.ReleaseDate);

    public string ToLongReleaseString() => this.Release.ToString();

    public string ToShortReleaseString()
    {
        if (this.Release.Revision > 0)
        {
            return this.Release.ToString(4);
        }

        if (this.Release.Build > 0)
        {
            return this.Release.ToString(3);
        }

        if (this.Release.Minor > 0)
        {
            return this.Release.ToString(2);
        }

        return this.Release.ToString(1);
    }

    public override string ToString()
    {
        if (this.PreReleaseNumber == DefaultPreReleaseNumber && this.Milestone == DefaultMilestone)
        {
            return this.ToShortReleaseString();
        }

        return $"{this.ToShortReleaseString()}-{string.Join('.', GetReleaseLabels(this))}";
    }

    private static string GetMetadata(Version version) => version.ReleaseDate?.ToString("s") ?? string.Empty;

    private static (Milestone milestone, int prereleaseNumber) GetMilestoneAndPrereleaseNumber(bool isPrerelease,
        string[] releaseLabels)
    {
        if (!isPrerelease || releaseLabels.Length == 0)
        {
            return (Milestone.Release, DefaultPreReleaseNumber);
        }

        if (releaseLabels.Length > 2)
        {
            throw new FormatException("Release labels passed are more than 2.");
        }

        var prereleaseNumber = releaseLabels.Length == 2
            ? int.Parse(releaseLabels[1], CultureInfo.InvariantCulture)
            : DefaultPreReleaseNumber;

        var milestoneTag = releaseLabels[0];

        return milestoneTag.ToUpperInvariant() switch
        {
            "ALPHA" => (Milestone.Alpha, prereleaseNumber),
            "BETA" => (Milestone.Beta, prereleaseNumber),
            "RC" => (Milestone.ReleaseCandidate, prereleaseNumber),
            _ => throw new FormatException($"Unknown milestone tag '{milestoneTag}'."),
        };
    }

    private static DateTimeOffset GetReleaseDate(string metadata) => DateTimeOffset.Parse(metadata, CultureInfo.InvariantCulture);

    private static string[] GetReleaseLabels(Version version)
    {
        if (version.Stability == Stability.Stable)
        {
            return [];
        }

        var milestoneTag = version.Milestone switch
        {
            Milestone.Alpha => "alpha",
            Milestone.Beta => "beta",
            Milestone.ReleaseCandidate => "rc",
            Milestone.Release => null,
            _ => throw new NotSupportedException("Unsupported milestone name."),
        };

        if (milestoneTag is null)
        {
            return [];
        }

        if (version.preReleaseNumber == DefaultPreReleaseNumber)
        {
            return [milestoneTag];
        }

        return [milestoneTag, version.preReleaseNumber.ToString(CultureInfo.InvariantCulture)];
    }

    private void ValidateMilestoneAndPrerelease()
    {
        if (this.Stability == Stability.Stable && this.PreReleaseNumber != DefaultPreReleaseNumber)
        {
            throw new FormatException("Stable version cannot have pre-release number.");
        }

        var values = Enum.GetValues<Milestone>();
        for (var i = 0; i < values.Length; i++)
        {
            var objectValue = values.GetValue(i)
                ?? throw new InvalidOperationException("Enum value cannot be NULL.");

            var value = (Milestone)objectValue;
            if (value == this.Milestone)
            {
                return;
            }
        }

        throw new NotSupportedException($"Milestone '{this.Milestone}' value is not supported.");
    }
}
