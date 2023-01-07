using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LanguageExt;
using NuGet.Versioning;
using SharpCompress.Common;
using static LanguageExt.Prelude;

namespace TIKSN.Versioning;

public sealed class VersionSeries : IEquatable<VersionSeries>
{
    private readonly Either<int, Version> series;

    public VersionSeries(
        int releaseMajor) =>
        this.series = Left(releaseMajor);

    public VersionSeries(
        Version version) =>
        this.series = Right(version);

    public VersionSeries(
        int releaseMajor,
        int releaseMinor) =>
        this.series = Right(new Version(releaseMajor, releaseMinor));

    public VersionSeries(
        int releaseMajor,
        int releaseMinor,
        int releaseBuild) =>
        this.series = Right(new Version(releaseMajor, releaseMinor, releaseBuild));

    public VersionSeries(
        int releaseMajor,
        int releaseMinor,
        int releaseBuild,
        int releaseRevision) =>
        this.series = Right(new Version(releaseMajor, releaseMinor, releaseBuild, releaseRevision));

    public VersionSeries(System.Version release) => this.series = Right(new Version(release));

    public VersionSeries(
        int releaseMajor,
        int releaseMinor,
        Milestone milestone) =>
        this.series = Right(new Version(releaseMajor, releaseMinor, milestone));

    public VersionSeries(
        int releaseMajor,
        int releaseMinor,
        int releaseBuild,
        Milestone milestone) =>
        this.series = Right(new Version(releaseMajor, releaseMinor, releaseBuild, milestone));

    public VersionSeries(
        int releaseMajor,
        int releaseMinor,
        int releaseBuild,
        int releaseRevision,
        Milestone milestone) =>
        this.series = Right(new Version(releaseMajor, releaseMinor, releaseBuild, releaseRevision, milestone));

    public VersionSeries(
        System.Version release,
        Milestone milestone) =>
        this.series = Right(new Version(release, milestone));

    public VersionSeries(
        int releaseMajor,
        int releaseMinor,
        Milestone milestone,
        int prereleaseNumber) =>
        this.series = Right(new Version(releaseMajor, releaseMinor, milestone, prereleaseNumber));

    public VersionSeries(
        int releaseMajor,
        int releaseMinor,
        int releaseBuild,
        Milestone milestone,
        int prereleaseNumber) =>
        this.series = Right(new Version(releaseMajor, releaseMinor, releaseBuild, milestone, prereleaseNumber));

    public VersionSeries(
        int releaseMajor,
        int releaseMinor,
        int releaseBuild,
        int releaseRevision,
        Milestone milestone,
        int prereleaseNumber) =>
        this.series = Right(new Version(releaseMajor, releaseMinor, releaseBuild, releaseRevision, milestone,
            prereleaseNumber));

    public VersionSeries(
        System.Version release,
        Milestone milestone,
        int prereleaseNumber) =>
        this.series = Right(new Version(release, milestone, prereleaseNumber));

    public bool Equals(VersionSeries other)
    {
        if (other is null)
        {
            return false;
        }

        return this.series.Match(
            thisVersion => other.series.Match(
                thisVersion.Equals,
                otherReleaseMajor => false),
            thisReleaseVersion => other.series.Match(
                otherVersion => false,
                thisReleaseVersion.Equals));
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

        if (obj is VersionSeries versionSeries)
        {
            return versionSeries.Equals(this);
        }

        return false;
    }

    public static bool operator !=(VersionSeries versionSeries1, VersionSeries versionSeries2)
        => versionSeries1?.Equals(versionSeries2) != true;

    public static bool operator ==(VersionSeries versionSeries1, VersionSeries versionSeries2)
        => versionSeries1?.Equals(versionSeries2) == true;

    public override int GetHashCode() => this.series.GetHashCode();

    public override string ToString()
        => this.series.Match(s => s.ToString(), m => m.ToString(CultureInfo.InvariantCulture));

    public static VersionSeries Parse(string input)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        if (TryParse(input, out var result))
        {
            return result;
        }

        throw new InvalidFormatException("Unable to parse string to VersionSeries");
    }

    public static bool TryParse(string input, out VersionSeries result)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        if (int.TryParse(input, out var releaseMajor))
        {
            result = new VersionSeries(releaseMajor);
            return true;
        }

        if (NuGetVersion.TryParse(input, out var nugetVersion))
        {
            result = new VersionSeries((Version)nugetVersion);
            return true;
        }

        result = null;
        return false;
    }

    public Option<Version> Matches(Version version) =>
        this.series.Match(
            seriesVersion => MatchesVersion(seriesVersion, version),
            releaseMajor => MatchesReleaseMajor(releaseMajor, version));

    private static Option<Version> MatchesReleaseMajor(int releaseMajor, Version version)
        => releaseMajor.Equals(version.Release.Major) ? version : Option<Version>.None;

    private static Option<Version> MatchesVersion(Version seriesVersion, Version version)
    {
        var seriesVersionNumbers = new[]
        {
            seriesVersion.Release.Major, seriesVersion.Release.Minor, seriesVersion.Release.Build,
            seriesVersion.Release.Revision
        }.Where(x => x != -1).ToArray();

        var versionNumbers = new[]
        {
            version.Release.Major, version.Release.Minor, version.Release.Build, version.Release.Revision
        }.Where(x => x != -1).ToArray();

        if (seriesVersionNumbers.Length > versionNumbers.Length)
        {
            return None;
        }

        if (seriesVersionNumbers.Where((seriesVersionNumber, i) => seriesVersionNumber != versionNumbers[i]).Any())
        {
            return None;
        }

        if (seriesVersion.Stability != Stability.Stable)
        {
            if (seriesVersionNumbers.Length != versionNumbers.Length)
            {
                return None;
            }

            if (seriesVersion.Milestone != version.Milestone)
            {
                return None;
            }

            if (seriesVersion.PrereleaseNumber != -1 && seriesVersion.PrereleaseNumber != version.PrereleaseNumber)
            {
                return None;
            }
        }

        return version;
    }

    public Option<IReadOnlyList<Version>> Matches(IReadOnlyCollection<Version> versions)
    {
        var result = versions
            .Select(this.Matches)
            .SelectMany(x => x)
            .ToArray();

        if (result.Length == 0)
        {
            return None;
        }

        return result;
    }
}
