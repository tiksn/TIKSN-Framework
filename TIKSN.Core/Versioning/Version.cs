using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TIKSN.Versioning
{
    public sealed class Version : IComparable<Version>, IEquatable<Version>
    {
        private const Milestone DefaultMilestone = Milestone.Release;
        private const int DefaultPrereleaseNumber = -1;

        private Milestone milestone;
        private int prereleaseNumber;
        private System.Version release;
        private DateTimeOffset? releaseDate;

        public Version(int ReleaseMajor, int ReleaseMinor)
        {
            this.release = new System.Version(ReleaseMajor, ReleaseMinor);
            this.milestone = DefaultMilestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.releaseDate = null;
        }

        public Version(int ReleaseMajor, int ReleaseMinor, DateTimeOffset ReleaseDate)
        {
            this.release = new System.Version(ReleaseMajor, ReleaseMinor);
            this.milestone = DefaultMilestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.releaseDate = ReleaseDate;
        }

        public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild)
        {
            this.release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild);
            this.milestone = DefaultMilestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.releaseDate = null;
        }

        public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, DateTimeOffset ReleaseDate)
        {
            this.release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild);
            this.milestone = DefaultMilestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.releaseDate = ReleaseDate;
        }

        public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, int ReleaseRevision)
        {
            this.release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild, ReleaseRevision);
            this.milestone = DefaultMilestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.releaseDate = null;
        }

        public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, int ReleaseRevision, DateTimeOffset ReleaseDate)
        {
            this.release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild, ReleaseRevision);
            this.milestone = DefaultMilestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.releaseDate = ReleaseDate;
        }

        public Version(System.Version Release)
        {
            this.release = Release;
            this.milestone = DefaultMilestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.releaseDate = null;
        }

        public Version(System.Version Release, DateTimeOffset ReleaseDate)
        {
            this.release = Release;
            this.milestone = DefaultMilestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.releaseDate = ReleaseDate;
        }

        public Version(int ReleaseMajor, int ReleaseMinor, Milestone Milestone)
        {
            this.release = new System.Version(ReleaseMajor, ReleaseMinor);
            this.milestone = Milestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.releaseDate = null;

            ValidateMilestoneAndPrerelease();
        }

        public Version(int ReleaseMajor, int ReleaseMinor, Milestone Milestone, DateTimeOffset ReleaseDate)
        {
            this.release = new System.Version(ReleaseMajor, ReleaseMinor);
            this.milestone = Milestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.releaseDate = ReleaseDate;

            ValidateMilestoneAndPrerelease();
        }

        public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, Milestone Milestone)
        {
            this.release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild);
            this.milestone = Milestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.releaseDate = null;

            ValidateMilestoneAndPrerelease();
        }

        public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, Milestone Milestone, DateTimeOffset ReleaseDate)
        {
            this.release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild);
            this.milestone = Milestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.releaseDate = ReleaseDate;

            ValidateMilestoneAndPrerelease();
        }

        public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, int ReleaseRevision, Milestone Milestone)
        {
            this.release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild, ReleaseRevision);
            this.milestone = Milestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.releaseDate = null;

            ValidateMilestoneAndPrerelease();
        }

        public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, int ReleaseRevision, Milestone Milestone, DateTimeOffset ReleaseDate)
        {
            this.release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild, ReleaseRevision);
            this.milestone = Milestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.releaseDate = ReleaseDate;

            ValidateMilestoneAndPrerelease();
        }

        public Version(System.Version Release, Milestone Milestone)
        {
            this.release = Release;
            this.milestone = Milestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.releaseDate = null;

            ValidateMilestoneAndPrerelease();
        }

        public Version(System.Version Release, Milestone Milestone, DateTimeOffset ReleaseDate)
        {
            this.release = Release;
            this.milestone = Milestone;
            this.prereleaseNumber = DefaultPrereleaseNumber;
            this.releaseDate = ReleaseDate;

            ValidateMilestoneAndPrerelease();
        }

        public Version(int ReleaseMajor, int ReleaseMinor, Milestone Milestone, int PrereleaseNumber)
        {
            this.release = new System.Version(ReleaseMajor, ReleaseMinor);
            this.milestone = Milestone;
            this.PrereleaseNumber = PrereleaseNumber;
            this.releaseDate = null;

            ValidateMilestoneAndPrerelease();
        }

        public Version(int ReleaseMajor, int ReleaseMinor, Milestone Milestone, int PrereleaseNumber, DateTimeOffset ReleaseDate)
        {
            this.release = new System.Version(ReleaseMajor, ReleaseMinor);
            this.milestone = Milestone;
            this.PrereleaseNumber = PrereleaseNumber;
            this.releaseDate = ReleaseDate;

            ValidateMilestoneAndPrerelease();
        }

        public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, Milestone Milestone, int PrereleaseNumber)
        {
            this.release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild);
            this.milestone = Milestone;
            this.PrereleaseNumber = PrereleaseNumber;
            this.releaseDate = null;

            ValidateMilestoneAndPrerelease();
        }

        public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, Milestone Milestone, int PrereleaseNumber, DateTimeOffset ReleaseDate)
        {
            this.release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild);
            this.milestone = Milestone;
            this.PrereleaseNumber = PrereleaseNumber;
            this.releaseDate = ReleaseDate;

            ValidateMilestoneAndPrerelease();
        }

        public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, int ReleaseRevision, Milestone Milestone, int PrereleaseNumber)
        {
            this.release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild, ReleaseRevision);
            this.milestone = Milestone;
            this.PrereleaseNumber = PrereleaseNumber;
            this.releaseDate = null;

            ValidateMilestoneAndPrerelease();
        }

        public Version(int ReleaseMajor, int ReleaseMinor, int ReleaseBuild, int ReleaseRevision, Milestone Milestone, int PrereleaseNumber, DateTimeOffset ReleaseDate)
        {
            this.release = new System.Version(ReleaseMajor, ReleaseMinor, ReleaseBuild, ReleaseRevision);
            this.milestone = Milestone;
            this.PrereleaseNumber = PrereleaseNumber;
            this.releaseDate = ReleaseDate;

            ValidateMilestoneAndPrerelease();
        }

        public Version(System.Version Release, Milestone Milestone, int PrereleaseNumber)
        {
            this.release = Release;
            this.milestone = Milestone;
            this.PrereleaseNumber = PrereleaseNumber;
            this.releaseDate = null;

            ValidateMilestoneAndPrerelease();
        }

        public Version(System.Version Release, Milestone Milestone, int PrereleaseNumber, DateTimeOffset ReleaseDate)
        {
            this.release = Release;
            this.milestone = Milestone;
            this.PrereleaseNumber = PrereleaseNumber;
            this.releaseDate = ReleaseDate;

            ValidateMilestoneAndPrerelease();
        }

        public Milestone Milestone
        {
            get
            {
                return this.milestone;
            }
        }

        public int PrereleaseNumber
        {
            get
            {
                return this.prereleaseNumber;
            }
            private set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(PrereleaseNumber));
                }
                else
                {
                    this.prereleaseNumber = value;
                }
            }
        }

        public System.Version Release
        {
            get
            {
                return this.release;
            }
        }

        public DateTimeOffset? ReleaseDate
        {
            get
            {
                return this.releaseDate;
            }
        }

        public Stability Stability
        {
            get
            {
                if (this.milestone == Milestone.Release)
                {
                    return Stability.Stable;
                }
                else
                {
                    return Stability.Unstable;
                }
            }
        }

        public static explicit operator NuGetVersion(Version version)
        {
            var releaseLabels = GetReleaseLabels(version);
            var metadata = GetMetadata(version);

            if (version.release.Revision >= 0)
                return new NuGetVersion(version.release.Major, version.release.Minor, version.release.Build, version.release.Revision, releaseLabels, metadata);

            if (version.release.Build >= 0)
                return new NuGetVersion(version.release.Major, version.release.Minor, version.release.Build, releaseLabels, metadata);

            return new NuGetVersion(version.release.Major, version.release.Minor, 0, releaseLabels, metadata);
        }

        public static explicit operator SemanticVersion(Version version)
        {
            var releaseLabels = GetReleaseLabels(version);
            var metadata = GetMetadata(version);

            if (version.release.Revision != -1)
                throw new FormatException("There is no revision in semantic version.");

            if (version.release.Build >= 0)
                return new SemanticVersion(version.release.Major, version.release.Minor, version.release.Build, releaseLabels, metadata);

            return new SemanticVersion(version.release.Major, version.release.Minor, 0, releaseLabels, metadata);
        }

        public static explicit operator Version(NuGetVersion nuGetVersion)
        {
            var (milestone, prereleaseNumber) = GetMilestoneAndPrereleaseNumber(nuGetVersion.IsPrerelease, nuGetVersion.ReleaseLabels.ToArray());

            if (nuGetVersion.HasMetadata)
                return new Version(nuGetVersion.Version, milestone, prereleaseNumber, GetReleaseDate(nuGetVersion.Metadata));

            return new Version(nuGetVersion.Version, milestone, prereleaseNumber);
        }

        public static explicit operator Version(SemanticVersion semanticVersion)
        {
            var (milestone, prereleaseNumber) = GetMilestoneAndPrereleaseNumber(semanticVersion.IsPrerelease, semanticVersion.ReleaseLabels.ToArray());

            if (semanticVersion.HasMetadata)
                return new Version(semanticVersion.Major, semanticVersion.Minor, semanticVersion.Patch, milestone, prereleaseNumber, GetReleaseDate(semanticVersion.Metadata));

            return new Version(semanticVersion.Major, semanticVersion.Minor, semanticVersion.Patch, milestone, prereleaseNumber);
        }

        public static bool operator !=(Version v1, Version v2)
        {
            return v1.CompareTo(v2) != 0;
        }

        public static bool operator <(Version v1, Version v2)
        {
            return v1.CompareTo(v2) < 0;
        }

        public static bool operator <=(Version v1, Version v2)
        {
            return v1.CompareTo(v2) <= 0;
        }

        public static bool operator ==(Version v1, Version v2)
        {
            return v1.CompareTo(v2) == 0;
        }

        public static bool operator >(Version v1, Version v2)
        {
            return v1.CompareTo(v2) > 0;
        }

        public static bool operator >=(Version v1, Version v2)
        {
            return v1.CompareTo(v2) >= 0;
        }

        public int CompareTo(Version that)
        {
            if (ReferenceEquals(this, that))
            {
                return 0;
            }
            else
            {
                int ReleaseComparison = this.release.CompareTo(that.release);

                if (ReleaseComparison == 0)
                {
                    int MilestoneComparison = this.milestone - that.milestone;

                    if (this.milestone > that.milestone)
                    {
                        return 1;
                    }
                    else if (this.milestone < that.milestone)
                    {
                        return -1;
                    }
                    else
                    {
                        System.Diagnostics.Debug.Assert(this.milestone == that.milestone);

                        if (this.prereleaseNumber > that.prereleaseNumber)
                        {
                            return 1;
                        }
                        else if (this.prereleaseNumber < that.prereleaseNumber)
                        {
                            return -1;
                        }
                        else
                        {
                            System.Diagnostics.Debug.Assert(this.prereleaseNumber == that.prereleaseNumber);

                            return 0;
                        }
                    }
                }
                else
                {
                    return ReleaseComparison;
                }
            }
        }

        public bool Equals(Version that)
        {
            if (ReferenceEquals(that, null))
                return false;
            return this.CompareTo(that) == 0;
        }

        public string ToLongReleaseString()
        {
            return this.release.ToString();
        }

        public string ToShortReleaseString()
        {
            if (this.release.Revision > 0)
            {
                return this.release.ToString(4);
            }
            else if (this.release.Build > 0)
            {
                return this.release.ToString(3);
            }
            else if (this.release.Minor > 0)
            {
                return this.release.ToString(2);
            }
            else
            {
                return this.release.ToString(1);
            }
        }

        public override string ToString()
        {
            if (this.prereleaseNumber == DefaultPrereleaseNumber && this.milestone == DefaultMilestone)
            {
                return this.ToShortReleaseString();
            }
            else
            {
                return $"{ToShortReleaseString()}-{string.Join(".", GetReleaseLabels(this))}";
            }
        }

        private static string GetMetadata(Version version)
        {
            return version.ReleaseDate?.ToString("s");
        }

        private static (Milestone milestone, int prereleaseNumber) GetMilestoneAndPrereleaseNumber(bool isPrerelease, string[] releaseLabels)
        {
            if (!isPrerelease || releaseLabels.Length == 0)
                return (Milestone.Release, DefaultPrereleaseNumber);

            if (releaseLabels.Length > 2)
                throw new FormatException("Release labels passed are more than 2.");

            var prereleaseNumber = releaseLabels.Length == 2 ? int.Parse(releaseLabels[1], CultureInfo.InvariantCulture) : DefaultPrereleaseNumber;

            var milestoneTag = releaseLabels.ElementAt(0);
            var milestone = Milestone.Release;

            switch (milestoneTag.ToLowerInvariant())
            {
                case "alpha":
                    return (Milestone.Alpha, prereleaseNumber);

                case "beta":
                    return (Milestone.Beta, prereleaseNumber);

                case "rc":
                    return (Milestone.ReleaseCandidate, prereleaseNumber);

                default:
                    throw new FormatException($"Unknown milestone tag '{milestoneTag}'.");
            }
        }

        private static DateTimeOffset GetReleaseDate(string metadata)
        {
            return DateTimeOffset.Parse(metadata);
        }

        private static IEnumerable<string> GetReleaseLabels(Version version)
        {
            if (version.Stability == Stability.Stable)
                return Array.Empty<string>();

            string milestoneTag;

            switch (version.milestone)
            {
                case Milestone.Alpha:
                    milestoneTag = "alpha";
                    break;

                case Milestone.Beta:
                    milestoneTag = "beta";
                    break;

                case Milestone.ReleaseCandidate:
                    milestoneTag = "rc";
                    break;

                case Milestone.Release:
                    milestoneTag = null;
                    break;

                default:
                    throw new NotSupportedException("Unsupported milestone name.");
            }

            if (version.prereleaseNumber == DefaultPrereleaseNumber)
            {
                return new[] { milestoneTag };
            }

            return new[]
            {
                milestoneTag,
                version.prereleaseNumber.ToString()
            };
        }

        private void ValidateMilestoneAndPrerelease()
        {
            if (Stability == Stability.Stable && prereleaseNumber != DefaultPrereleaseNumber)
                throw new FormatException("Stable version cannot have pre-release number.");

            var values = Enum.GetValues(typeof(Milestone));
            for (int i = 0; i < values.Length; i++)
            {
                var value = (Milestone)values.GetValue(i);
                if (value == milestone) return;
            }

            throw new NotSupportedException($"Milestone '{milestone}' value is not supported.");
        }
    }
}