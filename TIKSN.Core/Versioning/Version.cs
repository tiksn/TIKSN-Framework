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

        public Version(int ReleaseMajor, int ReleaseMinor)
        {
            this.Release = new System.Version(ReleaseMajor, ReleaseMinor);
            this.Milestone = DefaultMilestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.ReleaseDate = null;
        }

        public Version(int ReleaseMajor, int ReleaseMinor, DateTimeOffset ReleaseDate)
        {
            this.Release = new System.Version(ReleaseMajor, ReleaseMinor);
            this.Milestone = DefaultMilestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.ReleaseDate = ReleaseDate;
        }

        public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild)
        {
            this.Release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild);
            this.Milestone = DefaultMilestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.ReleaseDate = null;
        }

        public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, DateTimeOffset ReleaseDate)
        {
            this.Release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild);
            this.Milestone = DefaultMilestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.ReleaseDate = ReleaseDate;
        }

        public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, int ReleaseRevision)
        {
            this.Release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild, ReleaseRevision);
            this.Milestone = DefaultMilestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.ReleaseDate = null;
        }

        public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, int ReleaseRevision,
            DateTimeOffset ReleaseDate)
        {
            this.Release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild, ReleaseRevision);
            this.Milestone = DefaultMilestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.ReleaseDate = ReleaseDate;
        }

        public Version(System.Version Release)
        {
            this.Release = Release;
            this.Milestone = DefaultMilestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.ReleaseDate = null;
        }

        public Version(System.Version Release, DateTimeOffset ReleaseDate)
        {
            this.Release = Release;
            this.Milestone = DefaultMilestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.ReleaseDate = ReleaseDate;
        }

        public Version(int ReleaseMajor, int ReleaseMinor, Milestone Milestone)
        {
            this.Release = new System.Version(ReleaseMajor, ReleaseMinor);
            this.Milestone = Milestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.ReleaseDate = null;

            this.ValidateMilestoneAndPrerelease();
        }

        public Version(int ReleaseMajor, int ReleaseMinor, Milestone Milestone, DateTimeOffset ReleaseDate)
        {
            this.Release = new System.Version(ReleaseMajor, ReleaseMinor);
            this.Milestone = Milestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.ReleaseDate = ReleaseDate;

            this.ValidateMilestoneAndPrerelease();
        }

        public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, Milestone Milestone)
        {
            this.Release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild);
            this.Milestone = Milestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.ReleaseDate = null;

            this.ValidateMilestoneAndPrerelease();
        }

        public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, Milestone Milestone,
            DateTimeOffset ReleaseDate)
        {
            this.Release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild);
            this.Milestone = Milestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.ReleaseDate = ReleaseDate;

            this.ValidateMilestoneAndPrerelease();
        }

        public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, int ReleaseRevision, Milestone Milestone)
        {
            this.Release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild, ReleaseRevision);
            this.Milestone = Milestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.ReleaseDate = null;

            this.ValidateMilestoneAndPrerelease();
        }

        public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, int ReleaseRevision, Milestone Milestone,
            DateTimeOffset ReleaseDate)
        {
            this.Release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild, ReleaseRevision);
            this.Milestone = Milestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.ReleaseDate = ReleaseDate;

            this.ValidateMilestoneAndPrerelease();
        }

        public Version(System.Version Release, Milestone Milestone)
        {
            this.Release = Release;
            this.Milestone = Milestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.ReleaseDate = null;

            this.ValidateMilestoneAndPrerelease();
        }

        public Version(System.Version Release, Milestone Milestone, DateTimeOffset ReleaseDate)
        {
            this.Release = Release;
            this.Milestone = Milestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.ReleaseDate = ReleaseDate;

            this.ValidateMilestoneAndPrerelease();
        }

        public Version(int ReleaseMajor, int ReleaseMinor, Milestone Milestone, int PrereleaseNumber)
        {
            this.Release = new System.Version(ReleaseMajor, ReleaseMinor);
            this.Milestone = Milestone;
            this.PrereleaseNumber = PrereleaseNumber;
            this.ReleaseDate = null;

            this.ValidateMilestoneAndPrerelease();
        }

        public Version(int ReleaseMajor, int ReleaseMinor, Milestone Milestone, int PrereleaseNumber,
            DateTimeOffset ReleaseDate)
        {
            this.Release = new System.Version(ReleaseMajor, ReleaseMinor);
            this.Milestone = Milestone;
            this.PrereleaseNumber = PrereleaseNumber;
            this.ReleaseDate = ReleaseDate;

            this.ValidateMilestoneAndPrerelease();
        }

        public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, Milestone Milestone, int PrereleaseNumber)
        {
            this.Release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild);
            this.Milestone = Milestone;
            this.PrereleaseNumber = PrereleaseNumber;
            this.ReleaseDate = null;

            this.ValidateMilestoneAndPrerelease();
        }

        public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, Milestone Milestone, int PrereleaseNumber,
            DateTimeOffset ReleaseDate)
        {
            this.Release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild);
            this.Milestone = Milestone;
            this.PrereleaseNumber = PrereleaseNumber;
            this.ReleaseDate = ReleaseDate;

            this.ValidateMilestoneAndPrerelease();
        }

        public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, int ReleaseRevision, Milestone Milestone,
            int PrereleaseNumber)
        {
            this.Release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild, ReleaseRevision);
            this.Milestone = Milestone;
            this.PrereleaseNumber = PrereleaseNumber;
            this.ReleaseDate = null;

            this.ValidateMilestoneAndPrerelease();
        }

        public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, int ReleaseRevision, Milestone Milestone,
            int PrereleaseNumber, DateTimeOffset ReleaseDate)
        {
            this.Release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild, ReleaseRevision);
            this.Milestone = Milestone;
            this.PrereleaseNumber = PrereleaseNumber;
            this.ReleaseDate = ReleaseDate;

            this.ValidateMilestoneAndPrerelease();
        }

        public Version(System.Version Release, Milestone Milestone, int PrereleaseNumber)
        {
            this.Release = Release;
            this.Milestone = Milestone;
            this.PrereleaseNumber = PrereleaseNumber;
            this.ReleaseDate = null;

            this.ValidateMilestoneAndPrerelease();
        }

        public Version(System.Version Release, Milestone Milestone, int PrereleaseNumber, DateTimeOffset ReleaseDate)
        {
            this.Release = Release;
            this.Milestone = Milestone;
            this.PrereleaseNumber = PrereleaseNumber;
            this.ReleaseDate = ReleaseDate;

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

        public int CompareTo(Version that)
        {
            if (ReferenceEquals(this, that))
            {
                return 0;
            }

            var ReleaseComparison = this.Release.CompareTo(that.Release);

            if (ReleaseComparison == 0)
            {
                _ = this.Milestone - that.Milestone;

                if (this.Milestone > that.Milestone)
                {
                    return 1;
                }

                if (this.Milestone < that.Milestone)
                {
                    return -1;
                }

                Debug.Assert(this.Milestone == that.Milestone);

                if (this.prereleaseNumber > that.prereleaseNumber)
                {
                    return 1;
                }

                if (this.prereleaseNumber < that.prereleaseNumber)
                {
                    return -1;
                }

                Debug.Assert(this.prereleaseNumber == that.prereleaseNumber);

                return 0;
            }

            return ReleaseComparison;
        }

        public bool Equals(Version that)
        {
            if (that is null)
            {
                return false;
            }

            return this.CompareTo(that) == 0;
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

            if (nuGetVersion.HasMetadata)
            {
                return new Version(nuGetVersion.Version, milestone, prereleaseNumber,
                    GetReleaseDate(nuGetVersion.Metadata));
            }

            return new Version(nuGetVersion.Version, milestone, prereleaseNumber);
        }

        public static explicit operator Version(SemanticVersion semanticVersion)
        {
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

        public static bool operator !=(Version v1, Version v2) => v1.CompareTo(v2) != 0;

        public static bool operator <(Version v1, Version v2) => v1.CompareTo(v2) < 0;

        public static bool operator <=(Version v1, Version v2) => v1.CompareTo(v2) <= 0;

        public static bool operator ==(Version v1, Version v2) => v1.CompareTo(v2) == 0;

        public static bool operator >(Version v1, Version v2) => v1.CompareTo(v2) > 0;

        public static bool operator >=(Version v1, Version v2) => v1.CompareTo(v2) >= 0;

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
            if (this.prereleaseNumber == DefaultPrereleaseNumber && this.Milestone == DefaultMilestone)
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
                _ => throw new FormatException($"Unknown milestone tag '{milestoneTag}'."),
            };
        }

        private static DateTimeOffset GetReleaseDate(string metadata) => DateTimeOffset.Parse(metadata);

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
                _ => throw new NotSupportedException("Unsupported milestone name."),
            };
            if (version.prereleaseNumber == DefaultPrereleaseNumber)
            {
                return new[] { milestoneTag };
            }

            return new[] { milestoneTag, version.prereleaseNumber.ToString() };
        }

        private void ValidateMilestoneAndPrerelease()
        {
            if (this.Stability == Stability.Stable && this.prereleaseNumber != DefaultPrereleaseNumber)
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

            throw new NotImplementedException();
        }

        public override int GetHashCode() => throw new NotImplementedException();

        public static NuGetVersion ToNuGetVersion(Version left, Version right) => throw new NotImplementedException();

        public static SemanticVersion ToSemanticVersion(Version left, Version right) => throw new NotImplementedException();

        public static Version ToVersion(Version left, Version right) => throw new NotImplementedException();
    }
}
