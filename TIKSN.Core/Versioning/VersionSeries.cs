using System;
using System.Globalization;
using LanguageExt;
using NuGet.Versioning;
using SharpCompress.Common;
using static LanguageExt.Prelude;

namespace TIKSN.Versioning;

public sealed class VersionSeries
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
}
