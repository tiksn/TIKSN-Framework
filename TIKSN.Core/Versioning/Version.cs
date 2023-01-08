using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using NuGet.Versioning;

namespace TIKSN.Versioning
{
    public sealed class Version : IComparable<Version>, IEquatable<Version>
    {
        private const Milestone DefaultMilestone = Milestone.Release;
        private const int DefaultPrereleaseNumber = -1;

        private int prereleaseNumber;

        public Version(
            int releaseMajor,
            int releaseMinor)
        {
            this.Release = new System.Version(releaseMajor, releaseMinor);
            this.Milestone = DefaultMilestone;
            this.PrereleaseNumber = DefaultPrereleaseNumber;
            this.ReleaseDate = null;
        }

        public Version(
            int releaseMajor,
            int releaseMinor,
            DateTimeOffset releaseDate)
        {
            this.Release = new System.Version(releaseMajor, releaseMinor);
            this.Milestone = DefaultMilestone;
            this.PrereleaseNumber = DefaultPrereleaseNumber;
            this.ReleaseDate = releaseDate;
        }

        public Version(
            int releaseMajor,
            int releaseMinor,
            int releaseBuild)
        {
            this.Release = new System.Version(releaseMajor, releaseMinor, releaseBuild);
            this.Milestone = DefaultMilestone;
            this.PrereleaseNumber = DefaultPrereleaseNumber;
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
            this.PrereleaseNumber = DefaultPrereleaseNumber;
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
            this.PrereleaseNumber = DefaultPrereleaseNumber;
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
            this.PrereleaseNumber = DefaultPrereleaseNumber;
            this.ReleaseDate = releaseDate;
        }

        public Version(System.Version release)
        {
            this.Release = release;
            this.Milestone = DefaultMilestone;
            this.PrereleaseNumber = DefaultPrereleaseNumber;
            this.ReleaseDate = null;
        }

        public Version(
            System.Version release,
            DateTimeOffset releaseDate)
        {
            this.Release = release;
            this.Milestone = DefaultMilestone;
            this.PrereleaseNumber = DefaultPrereleaseNumber;
            this.ReleaseDate = releaseDate;
        }

        public Version(
            int releaseMajor,
            int releaseMinor,
            Milestone milestone)
        {
            this.Release = new System.Version(releaseMajor, releaseMinor);
            this.Milestone = milestone;
            this.PrereleaseNumber = DefaultPrereleaseNumber;
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
            this.PrereleaseNumber = DefaultPrereleaseNumber;
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
            this.PrereleaseNumber = DefaultPrereleaseNumber;
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
            this.PrereleaseNumber = DefaultPrereleaseNumber;
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
            this.PrereleaseNumber = DefaultPrereleaseNumber;
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
            this.PrereleaseNumber = DefaultPrereleaseNumber;
            this.ReleaseDate = releaseDate;

            this.ValidateMilestoneAndPrerelease();
        }

        public Version(
            System.Version release,
            Milestone milestone)
        {
            this.Release = release;
            this.Milestone = milestone;
            this.PrereleaseNumber = DefaultPrereleaseNumber;
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
            this.PrereleaseNumber = DefaultPrereleaseNumber;
            this.ReleaseDate = releaseDate;

            this.ValidateMilestoneAndPrerelease();
        }

        public Version(
            int releaseMajor,
            int releaseMinor,
            Milestone milestone,
            int prereleaseNumber)
        {
            this.Release = new System.Version(releaseMajor, releaseMinor);
            this.Milestone = milestone;
            this.PrereleaseNumber = prereleaseNumber;
            this.ReleaseDate = null;

            this.ValidateMilestoneAndPrerelease();
        }

        public Version(
            int releaseMajor,
            int releaseMinor,
            Milestone milestone,
            int prereleaseNumber,
            DateTimeOffset releaseDate)
        {
            this.Release = new System.Version(releaseMajor, releaseMinor);
            this.Milestone = milestone;
            this.PrereleaseNumber = prereleaseNumber;
            this.ReleaseDate = releaseDate;

            this.ValidateMilestoneAndPrerelease();
        }

        public Version(
            int releaseMajor,
            int releaseMinor,
            int releaseBuild,
            Milestone milestone,
            int prereleaseNumber)
        {
            this.Release = new System.Version(releaseMajor, releaseMinor, releaseBuild);
            this.Milestone = milestone;
            this.PrereleaseNumber = prereleaseNumber;
            this.ReleaseDate = null;

            this.ValidateMilestoneAndPrerelease();
        }

        public Version(
            int releaseMajor,
            int releaseMinor,
            int releaseBuild,
            Milestone milestone,
            int prereleaseNumber,
            DateTimeOffset releaseDate)
        {
            this.Release = new System.Version(releaseMajor, releaseMinor, releaseBuild);
            this.Milestone = milestone;
            this.PrereleaseNumber = prereleaseNumber;
            this.ReleaseDate = releaseDate;

            this.ValidateMilestoneAndPrerelease();
        }

        public Version(
            int releaseMajor,
            int releaseMinor,
            int releaseBuild,
            int releaseRevision,
            Milestone milestone,
            int prereleaseNumber)
        {
            this.Release = new System.Version(releaseMajor, releaseMinor, releaseBuild, releaseRevision);
            this.Milestone = milestone;
            this.PrereleaseNumber = prereleaseNumber;
            this.ReleaseDate = null;

            this.ValidateMilestoneAndPrerelease();
        }

        public Version(
            int releaseMajor,
            int releaseMinor,
            int releaseBuild,
            int releaseRevision,
            Milestone milestone,
            int prereleaseNumber,
            DateTimeOffset releaseDate)
        {
            this.Release = new System.Version(releaseMajor, releaseMinor, releaseBuild, releaseRevision);
            this.Milestone = milestone;
            this.PrereleaseNumber = prereleaseNumber;
            this.ReleaseDate = releaseDate;

            this.ValidateMilestoneAndPrerelease();
        }

        public Version(
            System.Version release,
            Milestone milestone,
            int prereleaseNumber)
        {
            this.Release = release;
            this.Milestone = milestone;
            this.PrereleaseNumber = prereleaseNumber;
            this.ReleaseDate = null;

            this.ValidateMilestoneAndPrerelease();
        }

        public Version(
            System.Version release,
            Milestone milestone,
            int prereleaseNumber,
            DateTimeOffset releaseDate)
        {
            this.Release = release;
            this.Milestone = milestone;
            this.PrereleaseNumber = prereleaseNumber;
            this.ReleaseDate = releaseDate;

            this.ValidateMilestoneAndPrerelease();
        }

        public Milestone Milestone { get; }

        public int PrereleaseNumber
        {
            get => this.prereleaseNumber;
            private set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.PrereleaseNumber));
                }

                this.prereleaseNumber = value;
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

        public int CompareTo(Version other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
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

                if (this.PrereleaseNumber > other.PrereleaseNumber)
                {
                    return 1;
                }

                if (this.PrereleaseNumber < other.PrereleaseNumber)
                {
                    return -1;
                }

                Debug.Assert(this.PrereleaseNumber == other.PrereleaseNumber);

                return 0;
            }

            return releaseComparison;
        }

        public bool Equals(Version other)
        {
            if (other is null)
            {
                return false;
            }

            return this.CompareTo(other) == 0;
        }

        public static explicit operator NuGetVersion(Version version)
        {
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

        public static explicit operator SemanticVersion(Version version)
        {
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

        public static explicit operator Version(NuGetVersion nuGetVersion)
        {
            var (milestone, prereleaseNumber) =
                GetMilestoneAndPrereleaseNumber(nuGetVersion.IsPrerelease, nuGetVersion.ReleaseLabels.ToArray());

            var releaseNumbersCount =
                nuGetVersion.OriginalVersion
                    .Remove(nuGetVersion.OriginalVersion.Length - nuGetVersion.Release.Length)
                    .Split('.')
                    .Length;

            var release = releaseNumbersCount == 2 ? new System.Version(
                    nuGetVersion.Major,
                    nuGetVersion.Minor) :
                releaseNumbersCount == 3 ? new System.Version(
                    nuGetVersion.Major,
                    nuGetVersion.Minor,
                    nuGetVersion.Patch) :
                new System.Version(
                    nuGetVersion.Major,
                    nuGetVersion.Minor,
                    nuGetVersion.Patch,
                    nuGetVersion.Revision);

            if (nuGetVersion.HasMetadata)
            {
                return new Version(release, milestone, prereleaseNumber,
                    GetreleaseDate(nuGetVersion.Metadata));
            }

            return new Version(release, milestone, prereleaseNumber);
        }

        public static explicit operator Version(SemanticVersion semanticVersion)
        {
            var (milestone, prereleaseNumber) = GetMilestoneAndPrereleaseNumber(semanticVersion.IsPrerelease,
                semanticVersion.ReleaseLabels.ToArray());

            if (semanticVersion.HasMetadata)
            {
                return new Version(semanticVersion.Major, semanticVersion.Minor, semanticVersion.Patch, milestone,
                    prereleaseNumber, GetreleaseDate(semanticVersion.Metadata));
            }

            return new Version(semanticVersion.Major, semanticVersion.Minor, semanticVersion.Patch, milestone,
                prereleaseNumber);
        }

        public static bool operator !=(Version v1, Version v2) => v1.CompareTo(v2) != 0;

        public static bool operator <(Version v1, Version v2) => v1.CompareTo(v2) < 0;

        public static bool operator <=(Version v1, Version v2) => v1.CompareTo(v2) <= 0;

        public static bool operator ==(Version v1, Version v2) => v1.CompareTo(v2) == 0;

        public static bool operator >(Version v1, Version v2) => v1.CompareTo(v2) > 0;

        public static bool operator >=(Version v1, Version v2) => v1.CompareTo(v2) >= 0;

        public override bool Equals(object obj)
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
                       this.prereleaseNumber == DefaultPrereleaseNumber;
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = (hash * 23) + this.Release.GetHashCode();
                hash = (hash * 23) + this.Milestone.GetHashCode();
                hash = (hash * 23) + this.prereleaseNumber.GetHashCode();
                hash = (hash * 23) + (this.ReleaseDate?.GetHashCode() ?? 0);
                return hash;
            }
        }

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
            if (this.PrereleaseNumber == DefaultPrereleaseNumber && this.Milestone == DefaultMilestone)
            {
                return this.ToShortReleaseString();
            }

            return $"{this.ToShortReleaseString()}-{string.Join(".", GetReleaseLabels(this))}";
        }

        private static string GetMetadata(Version version) => version.ReleaseDate?.ToString("s");

        private static (Milestone milestone, int prereleaseNumber) GetMilestoneAndPrereleaseNumber(bool isPrerelease,
            string[] releaseLabels)
        {
            if (!isPrerelease || releaseLabels.Length == 0)
            {
                return (Milestone.Release, DefaultPrereleaseNumber);
            }

            if (releaseLabels.Length > 2)
            {
                throw new FormatException("Release labels passed are more than 2.");
            }

            var prereleaseNumber = releaseLabels.Length == 2
                ? int.Parse(releaseLabels[1], CultureInfo.InvariantCulture)
                : DefaultPrereleaseNumber;

            var milestoneTag = releaseLabels.ElementAt(0);

            return milestoneTag.ToLowerInvariant() switch
            {
                "alpha" => (Milestone.Alpha, prereleaseNumber),
                "beta" => (Milestone.Beta, prereleaseNumber),
                "rc" => (Milestone.ReleaseCandidate, prereleaseNumber),
                _ => throw new FormatException($"Unknown milestone tag '{milestoneTag}'.")
            };
        }

        private static DateTimeOffset GetreleaseDate(string metadata) => DateTimeOffset.Parse(metadata);

        private static IEnumerable<string> GetReleaseLabels(Version version)
        {
            if (version.Stability == Stability.Stable)
            {
                return Array.Empty<string>();
            }

            var milestoneTag = version.Milestone switch
            {
                Milestone.Alpha => "alpha",
                Milestone.Beta => "beta",
                Milestone.ReleaseCandidate => "rc",
                Milestone.Release => null,
                _ => throw new NotSupportedException("Unsupported milestone name.")
            };
            if (version.prereleaseNumber == DefaultPrereleaseNumber)
            {
                return new[] { milestoneTag };
            }

            return new[] { milestoneTag, version.prereleaseNumber.ToString() };
        }

        private void ValidateMilestoneAndPrerelease()
        {
            if (this.Stability == Stability.Stable && this.PrereleaseNumber != DefaultPrereleaseNumber)
            {
                throw new FormatException("Stable version cannot have pre-release number.");
            }

            var values = Enum.GetValues(typeof(Milestone));
            for (var i = 0; i < values.Length; i++)
            {
                var value = (Milestone)values.GetValue(i);
                if (value == this.Milestone)
                {
                    return;
                }
            }

            throw new NotSupportedException($"Milestone '{this.Milestone}' value is not supported.");
        }
    }
}