using System;
using FluentAssertions;
using NuGet.Versioning;
using Xunit;

namespace TIKSN.Versioning.Tests
{
    public class VersionTests
    {
        [Fact]
        public void Equals001()
        {
            var V = new Version(2, 4);

            Assert.True(V.Equals(V));
        }

        [Fact]
        public void Equals002()
        {
            var V1 = new Version(5, 6);
            var V2 = new Version(5, 6);

            Assert.True(V1.Equals(V2));
        }

        [Fact]
        public void Equals003()
        {
            var V1 = new Version(5, 6);
            var V2 = new Version(5, 6, 1);

            Assert.False(V1.Equals(V2));
        }

        [Fact]
        public void Equals004()
        {
            var V1 = new Version(5, 6, Milestone.Beta);
            var V2 = new Version(5, 6, Milestone.ReleaseCandidate);

            Assert.False(V1.Equals(V2));
        }

        [Fact]
        public void Equals005()
        {
            var V1 = new Version(5, 6, Milestone.ReleaseCandidate, 2);
            var V2 = new Version(5, 6, Milestone.ReleaseCandidate, 1);

            Assert.False(V1.Equals(V2));
        }

        [Fact]
        public void Equals006()
        {
            var v = new Version(2, 4);

            Assert.False(v.Equals(null));
        }

        [Fact]
        public void EqualsOperator001()
        {
            var V1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
            var V2 = new Version(10, 20, Milestone.Alpha, 2);

            Assert.True(!(V1 == V2));
        }

        [Fact]
        public void EqualsOperator002()
        {
            var V1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
            var V2 = new Version(10, 20, Milestone.ReleaseCandidate, 2);

            Assert.True(V1 == V2);
        }

        [Fact]
        public void GreaterThanOperator001()
        {
            var V1 = new Version(10, 30, Milestone.ReleaseCandidate, 2);
            var V2 = new Version(10, 20, Milestone.ReleaseCandidate, 2);

            Assert.True(V1 > V2);
        }

        [Fact]
        public void GreaterThanOperator002()
        {
            var V1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
            var V2 = new Version(10, 30, Milestone.ReleaseCandidate, 2);

            Assert.True(!(V1 > V2));
        }

        [Fact]
        public void GreaterThanOrEqualsOperator001()
        {
            var V1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
            var V2 = new Version(10, 20, Milestone.ReleaseCandidate, 2);

            Assert.True(V1 >= V2);
        }

        [Fact]
        public void GreaterThanOrEqualsOperator002()
        {
            var V1 = new Version(10, 40, Milestone.ReleaseCandidate, 2);
            var V2 = new Version(10, 30, Milestone.ReleaseCandidate, 2);

            Assert.True(V1 >= V2);
        }

        [Fact]
        public void LessThanOperator001()
        {
            var V1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
            var V2 = new Version(10, 30, Milestone.ReleaseCandidate, 2);

            Assert.True(V1 < V2);
        }

        [Fact]
        public void LessThanOperator002()
        {
            var V1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
            var V2 = new Version(10, 20, Milestone.ReleaseCandidate, 3);

            Assert.True(V1 < V2);
        }

        [Fact]
        public void LessThanOrEqualsOperator001()
        {
            var V1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
            var V2 = new Version(10, 20, Milestone.ReleaseCandidate, 2);

            Assert.True(V1 <= V2);
        }

        [Fact]
        public void LessThanOrEqualsOperator002()
        {
            var V1 = new Version(10, 10, Milestone.ReleaseCandidate, 4);
            var V2 = new Version(10, 20, Milestone.ReleaseCandidate, 3);

            Assert.True(V1 <= V2);
        }

        [Fact]
        public void NotEqualsOperator001()
        {
            var V1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
            var V2 = new Version(10, 20, Milestone.Alpha, 2);

            Assert.True(V1 != V2);
        }

        [Fact]
        public void NotEqualsOperator002()
        {
            var V1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
            var V2 = new Version(10, 20, Milestone.ReleaseCandidate, 2);

            Assert.True(!(V1 != V2));
        }

        [Fact]
        public void PrereleaseNumber001() => Assert.Throws<ArgumentOutOfRangeException>(() => new Version(1, 2, 3, 4, Milestone.ReleaseCandidate, -5));

        [Fact]
        public void ToLongReleaseString001()
        {
            var V = new Version(1, 2, 3, 4, Milestone.ReleaseCandidate, 5);

            Assert.Equal("1.2.3.4", V.ToLongReleaseString());
        }

        [Fact]
        public void ToPrereleaseString001()
        {
            var V = new Version(1, 2, 3, 4, Milestone.Alpha, 5);

            Assert.Equal("1.2.3.4-alpha.5", V.ToString());
        }

        [Fact]
        public void ToPrereleaseString002()
        {
            var V = new Version(1, 2, 3, 4, Milestone.Alpha, 5);

            Assert.Equal("1.2.3.4-alpha.5", V.ToString());
        }

        [Fact]
        public void ToPrereleaseString003()
        {
            var V = new Version(1, 2, 3, 4, Milestone.Beta, 5);

            Assert.Equal("1.2.3.4-beta.5", V.ToString());
        }

        [Fact]
        public void ToPrereleaseString004()
        {
            var V = new Version(1, 2, 3, 4, Milestone.ReleaseCandidate, 5);

            Assert.Equal("1.2.3.4-rc.5", V.ToString());
        }

        [Fact]
        public void ToPrereleaseString006()
        {
            var V = new Version(1, 2, 3, 4, Milestone.Release);

            Assert.Equal("1.2.3.4", V.ToString());
        }

        [Fact]
        public void ToPrereleaseString007()
        {
            var V = new Version(1, 2, 3, 4, Milestone.Release);

            Assert.Equal("1.2.3.4", V.ToString());
        }

        [Fact]
        public void ToPrereleaseString008() => Assert.Throws<NotSupportedException>(() => new Version(1, 2, 3, 4, (Milestone)125689, 5));

        [Fact]
        public void ToShortReleaseString001() => Assert.Equal(
                "0",
                new Version(0, 0, 0, 0).ToShortReleaseString());

        [Fact]
        public void ToShortReleaseString002() => Assert.Equal(
                   "0.0.0.4",
                   new Version(0, 0, 0, 4).ToShortReleaseString());

        [Fact]
        public void ToShortReleaseString003() => Assert.Equal(
                   "0.0.3",
                   new Version(0, 0, 3, 0).ToShortReleaseString());

        [Fact]
        public void ToShortReleaseString004() => Assert.Equal(
                   "0.0.3.4",
                   new Version(0, 0, 3, 4).ToShortReleaseString());

        [Fact]
        public void ToShortReleaseString005() => Assert.Equal(
                   "0.2",
                   new Version(0, 2, 0, 0).ToShortReleaseString());

        [Fact]
        public void ToShortReleaseString006() => Assert.Equal(
                   "0.2.0.4",
                   new Version(0, 2, 0, 4).ToShortReleaseString());

        [Fact]
        public void ToShortReleaseString007() => Assert.Equal(
                   "0.2.3",
                   new Version(0, 2, 3, 0).ToShortReleaseString());

        [Fact]
        public void ToShortReleaseString008() => Assert.Equal(
                   "0.2.3.4",
                   new Version(0, 2, 3, 4).ToShortReleaseString());

        [Fact]
        public void ToShortReleaseString009() => Assert.Equal(
                   "1",
                   new Version(1, 0, 0, 0).ToShortReleaseString());

        [Fact]
        public void ToShortReleaseString010() => Assert.Equal(
                   "1.0.0.4",
                   new Version(1, 0, 0, 4).ToShortReleaseString());

        [Fact]
        public void ToShortReleaseString011() => Assert.Equal(
                   "1.0.3",
                   new Version(1, 0, 3, 0).ToShortReleaseString());

        [Fact]
        public void ToShortReleaseString012() => Assert.Equal(
                   "1.0.3.4",
                   new Version(1, 0, 3, 4).ToShortReleaseString());

        [Fact]
        public void ToShortReleaseString013() => Assert.Equal(
                   "1.2",
                   new Version(1, 2, 0, 0).ToShortReleaseString());

        [Fact]
        public void ToShortReleaseString014() => Assert.Equal(
                   "1.2.0.4",
                   new Version(1, 2, 0, 4).ToShortReleaseString());

        [Fact]
        public void ToShortReleaseString015() => Assert.Equal(
                   "1.2.3",
                   new Version(1, 2, 3, 0).ToShortReleaseString());

        [Fact]
        public void ToShortReleaseString016() => Assert.Equal(
                "1.2.3.4",
                new Version(1, 2, 3, 4).ToShortReleaseString());

        [Fact]
        public void ToShortReleaseString017() => Assert.Equal(
                "1.2.3",
                new Version(1, 2, 3).ToShortReleaseString());

        [Fact]
        public void ToShortReleaseString018() => Assert.Equal(
                "1.2",
                new Version(1, 2).ToShortReleaseString());

        [Fact]
        public void ToShortReleaseString019() => Assert.Equal(
                "1",
                new Version(1, 0).ToShortReleaseString());

        [Fact]
        public void ToString001()
        {
            var V = new Version(10, 20);

            Assert.Equal("10.20", V.ToString());
        }

        [Fact]
        public void ToString002()
        {
            var V = new Version(10, 20, 30);

            Assert.Equal("10.20.30", V.ToString());
        }

        [Fact]
        public void ToString003()
        {
            var V = new Version(10, 20, 30, 40);

            Assert.Equal("10.20.30.40", V.ToString());
        }

        [Fact]
        public void ToString004()
        {
            var V = new Version(new System.Version(1, 2, 3, 4));

            Assert.Equal("1.2.3.4", V.ToString());
        }

        [Fact]
        public void ToString005()
        {
            var V = new Version(10, 20, Milestone.ReleaseCandidate);

            Assert.Equal("10.20-rc", V.ToString());
        }

        [Fact]
        public void ToString006()
        {
            var V = new Version(10, 20, 30, Milestone.ReleaseCandidate);

            Assert.Equal("10.20.30-rc", V.ToString());
        }

        [Fact]
        public void ToString007()
        {
            var V = new Version(10, 20, 30, 40, Milestone.ReleaseCandidate);

            Assert.Equal("10.20.30.40-rc", V.ToString());
        }

        [Fact]
        public void ToString008()
        {
            var V = new Version(new System.Version(1, 2, 3, 4), Milestone.ReleaseCandidate);

            Assert.Equal("1.2.3.4-rc", V.ToString());
        }

        [Fact]
        public void ToString009()
        {
            var V = new Version(10, 20, Milestone.ReleaseCandidate, 2);

            Assert.Equal("10.20-rc.2", V.ToString());
        }

        [Fact]
        public void ToString010()
        {
            var V = new Version(10, 20, 30, Milestone.ReleaseCandidate, 2);

            Assert.Equal("10.20.30-rc.2", V.ToString());
        }

        [Fact]
        public void ToString011()
        {
            var V = new Version(10, 20, 30, 40, Milestone.ReleaseCandidate, 2);

            Assert.Equal("10.20.30.40-rc.2", V.ToString());
        }

        [Fact]
        public void ToString012()
        {
            var V = new Version(new System.Version(1, 2, 3, 4), Milestone.ReleaseCandidate, 2);

            Assert.Equal("1.2.3.4-rc.2", V.ToString());
        }

        [Fact]
        public void Version001()
        {
            var V = new Version(2, 3);

            Assert.Equal(2, V.Release.Major);
            Assert.Equal(3, V.Release.Minor);
            Assert.Equal(-1, V.Release.Build);
            Assert.Equal(-1, V.Release.Revision);
            Assert.Equal(Milestone.Release, V.Milestone);
            Assert.Equal(-1, V.PrereleaseNumber);
            Assert.Equal(Stability.Stable, V.Stability);
            Assert.False(V.ReleaseDate.HasValue);
        }

        [Fact]
        public void Version003()
        {
            var V = new Version(2, 3, 5);

            Assert.Equal(2, V.Release.Major);
            Assert.Equal(3, V.Release.Minor);
            Assert.Equal(5, V.Release.Build);
            Assert.Equal(-1, V.Release.Revision);
            Assert.Equal(Milestone.Release, V.Milestone);
            Assert.Equal(-1, V.PrereleaseNumber);
            Assert.Equal(Stability.Stable, V.Stability);
            Assert.False(V.ReleaseDate.HasValue);
        }

        [Fact]
        public void Version005()
        {
            var V = new Version(2, 3, 5, 7);

            Assert.Equal(2, V.Release.Major);
            Assert.Equal(3, V.Release.Minor);
            Assert.Equal(5, V.Release.Build);
            Assert.Equal(7, V.Release.Revision);
            Assert.Equal(Milestone.Release, V.Milestone);
            Assert.Equal(-1, V.PrereleaseNumber);
            Assert.Equal(Stability.Stable, V.Stability);
            Assert.False(V.ReleaseDate.HasValue);
        }

        [Fact]
        public void Version006()
        {
            var V = new Version(new System.Version(2, 3, 5, 7));

            Assert.Equal(2, V.Release.Major);
            Assert.Equal(3, V.Release.Minor);
            Assert.Equal(5, V.Release.Build);
            Assert.Equal(7, V.Release.Revision);
            Assert.Equal(Milestone.Release, V.Milestone);
            Assert.Equal(-1, V.PrereleaseNumber);
            Assert.Equal(Stability.Stable, V.Stability);
            Assert.False(V.ReleaseDate.HasValue);
        }

        [Fact]
        public void Version007()
        {
            var V = new Version(new System.Version(2, 3, 5, 7), Milestone.Beta);

            Assert.Equal(2, V.Release.Major);
            Assert.Equal(3, V.Release.Minor);
            Assert.Equal(5, V.Release.Build);
            Assert.Equal(7, V.Release.Revision);
            Assert.Equal(Milestone.Beta, V.Milestone);
            Assert.Equal(-1, V.PrereleaseNumber);
            Assert.Equal(Stability.Unstable, V.Stability);
            Assert.False(V.ReleaseDate.HasValue);
        }

        [Fact]
        public void Version008()
        {
            var V = new Version(2, 3, 5, 7, Milestone.Beta);

            Assert.Equal(2, V.Release.Major);
            Assert.Equal(3, V.Release.Minor);
            Assert.Equal(5, V.Release.Build);
            Assert.Equal(7, V.Release.Revision);
            Assert.Equal(Milestone.Beta, V.Milestone);
            Assert.Equal(-1, V.PrereleaseNumber);
            Assert.Equal(Stability.Unstable, V.Stability);
            Assert.False(V.ReleaseDate.HasValue);
        }

        [Fact]
        public static void Version010()
        {
            var V = new Version(2, 3, 5, Milestone.Beta);

            Assert.Equal(2, V.Release.Major);
            Assert.Equal(3, V.Release.Minor);
            Assert.Equal(5, V.Release.Build);
            Assert.Equal(-1, V.Release.Revision);
            Assert.Equal(Milestone.Beta, V.Milestone);
            Assert.Equal(-1, V.PrereleaseNumber);
            Assert.Equal(Stability.Unstable, V.Stability);
            Assert.False(V.ReleaseDate.HasValue);
        }

        [Fact]
        public static void Version011()
        {
            var V = new Version(2, 3, Milestone.Beta);

            Assert.Equal(2, V.Release.Major);
            Assert.Equal(3, V.Release.Minor);
            Assert.Equal(-1, V.Release.Build);
            Assert.Equal(-1, V.Release.Revision);
            Assert.Equal(Milestone.Beta, V.Milestone);
            Assert.Equal(-1, V.PrereleaseNumber);
            Assert.Equal(Stability.Unstable, V.Stability);
            Assert.False(V.ReleaseDate.HasValue);
        }

        [Fact]
        public static void Version012()
        {
            var V = new Version(2, 3, Milestone.Beta, 1);

            Assert.Equal(2, V.Release.Major);
            Assert.Equal(3, V.Release.Minor);
            Assert.Equal(-1, V.Release.Build);
            Assert.Equal(-1, V.Release.Revision);
            Assert.Equal(Milestone.Beta, V.Milestone);
            Assert.Equal(1, V.PrereleaseNumber);
            Assert.Equal(Stability.Unstable, V.Stability);
            Assert.False(V.ReleaseDate.HasValue);
        }

        [Fact]
        public static void Version013()
        {
            var V = new Version(2, 3, 5, Milestone.Beta, 1);

            Assert.Equal(2, V.Release.Major);
            Assert.Equal(3, V.Release.Minor);
            Assert.Equal(5, V.Release.Build);
            Assert.Equal(-1, V.Release.Revision);
            Assert.Equal(Milestone.Beta, V.Milestone);
            Assert.Equal(1, V.PrereleaseNumber);
            Assert.Equal(Stability.Unstable, V.Stability);
            Assert.False(V.ReleaseDate.HasValue);
        }

        [Fact]
        public static void Version014()
        {
            var V = new Version(2, 3, 5, 7, Milestone.Beta, 1);

            Assert.Equal(2, V.Release.Major);
            Assert.Equal(3, V.Release.Minor);
            Assert.Equal(5, V.Release.Build);
            Assert.Equal(7, V.Release.Revision);
            Assert.Equal(Milestone.Beta, V.Milestone);
            Assert.Equal(1, V.PrereleaseNumber);
            Assert.Equal(Stability.Unstable, V.Stability);
            Assert.False(V.ReleaseDate.HasValue);
        }

        [Fact]
        public static void Version015()
        {
            var V = new Version(2, 3, 5, 7, Milestone.Beta, 1);

            Assert.Equal(2, V.Release.Major);
            Assert.Equal(3, V.Release.Minor);
            Assert.Equal(5, V.Release.Build);
            Assert.Equal(7, V.Release.Revision);
            Assert.Equal(Milestone.Beta, V.Milestone);
            Assert.Equal(1, V.PrereleaseNumber);
            Assert.Equal(Stability.Unstable, V.Stability);
            Assert.False(V.ReleaseDate.HasValue);
        }

        [Fact]
        public static void Version016()
        {
            var V = new Version(new System.Version(2, 3, 5, 7), Milestone.Beta, 1);

            Assert.Equal(2, V.Release.Major);
            Assert.Equal(3, V.Release.Minor);
            Assert.Equal(5, V.Release.Build);
            Assert.Equal(7, V.Release.Revision);
            Assert.Equal(Milestone.Beta, V.Milestone);
            Assert.Equal(1, V.PrereleaseNumber);
            Assert.Equal(Stability.Unstable, V.Stability);
            Assert.False(V.ReleaseDate.HasValue);
        }

        [Fact]
        public void Version017()
        {
            var V = new Version(new System.Version(2, 3, 5, 7), new DateTime(1985, 11, 20));

            Assert.Equal(2, V.Release.Major);
            Assert.Equal(3, V.Release.Minor);
            Assert.Equal(5, V.Release.Build);
            Assert.Equal(7, V.Release.Revision);
            Assert.Equal(Milestone.Release, V.Milestone);
            Assert.Equal(-1, V.PrereleaseNumber);
            Assert.Equal(Stability.Stable, V.Stability);
            Assert.True(V.ReleaseDate.HasValue);
            Assert.Equal(1985, V.ReleaseDate.Value.Year);
        }

        [Fact]
        public void Version018()
        {
            var V = new Version(2, 3, 5, 7, new DateTime(1985, 11, 20));

            Assert.Equal(2, V.Release.Major);
            Assert.Equal(3, V.Release.Minor);
            Assert.Equal(5, V.Release.Build);
            Assert.Equal(7, V.Release.Revision);
            Assert.Equal(Milestone.Release, V.Milestone);
            Assert.Equal(-1, V.PrereleaseNumber);
            Assert.Equal(Stability.Stable, V.Stability);
            Assert.True(V.ReleaseDate.HasValue);
            Assert.Equal(1985, V.ReleaseDate.Value.Year);
        }

        [Fact]
        public void Version019()
        {
            var V = new Version(2, 3, 5, new DateTime(1985, 11, 20));

            Assert.Equal(2, V.Release.Major);
            Assert.Equal(3, V.Release.Minor);
            Assert.Equal(5, V.Release.Build);
            Assert.Equal(-1, V.Release.Revision);
            Assert.Equal(Milestone.Release, V.Milestone);
            Assert.Equal(-1, V.PrereleaseNumber);
            Assert.Equal(Stability.Stable, V.Stability);
            Assert.True(V.ReleaseDate.HasValue);
            Assert.Equal(1985, V.ReleaseDate.Value.Year);
        }

        [Fact]
        public void Version020()
        {
            var V = new Version(2, 3, new DateTime(1985, 11, 20));

            Assert.Equal(2, V.Release.Major);
            Assert.Equal(3, V.Release.Minor);
            Assert.Equal(-1, V.Release.Build);
            Assert.Equal(-1, V.Release.Revision);
            Assert.Equal(Milestone.Release, V.Milestone);
            Assert.Equal(-1, V.PrereleaseNumber);
            Assert.Equal(Stability.Stable, V.Stability);
            Assert.True(V.ReleaseDate.HasValue);
            Assert.Equal(1985, V.ReleaseDate.Value.Year);
        }

        [Fact]
        public void Version021()
        {
            var V = new Version(2, 3, Milestone.Release, new DateTime(1985, 11, 20));

            Assert.Equal(2, V.Release.Major);
            Assert.Equal(3, V.Release.Minor);
            Assert.Equal(-1, V.Release.Build);
            Assert.Equal(-1, V.Release.Revision);
            Assert.Equal(Milestone.Release, V.Milestone);
            Assert.Equal(-1, V.PrereleaseNumber);
            Assert.Equal(Stability.Stable, V.Stability);
            Assert.True(V.ReleaseDate.HasValue);
            Assert.Equal(1985, V.ReleaseDate.Value.Year);
        }

        [Fact]
        public void Version022()
        {
            var V = new Version(2, 3, 5, Milestone.Release, new DateTime(1985, 11, 20));

            Assert.Equal(2, V.Release.Major);
            Assert.Equal(3, V.Release.Minor);
            Assert.Equal(5, V.Release.Build);
            Assert.Equal(-1, V.Release.Revision);
            Assert.Equal(Milestone.Release, V.Milestone);
            Assert.Equal(-1, V.PrereleaseNumber);
            Assert.Equal(Stability.Stable, V.Stability);
            Assert.True(V.ReleaseDate.HasValue);
            Assert.Equal(1985, V.ReleaseDate.Value.Year);
        }

        [Fact]
        public void Version023()
        {
            var V = new Version(2, 3, 5, 7, Milestone.Release, new DateTime(1985, 11, 20));

            Assert.Equal(2, V.Release.Major);
            Assert.Equal(3, V.Release.Minor);
            Assert.Equal(5, V.Release.Build);
            Assert.Equal(7, V.Release.Revision);
            Assert.Equal(Milestone.Release, V.Milestone);
            Assert.Equal(-1, V.PrereleaseNumber);
            Assert.Equal(Stability.Stable, V.Stability);
            Assert.True(V.ReleaseDate.HasValue);
            Assert.Equal(1985, V.ReleaseDate.Value.Year);
        }

        [Fact]
        public void Version024()
        {
            var V = new Version(new System.Version(2, 3, 5, 7), Milestone.Release, new DateTime(1985, 11, 20));

            Assert.Equal(2, V.Release.Major);
            Assert.Equal(3, V.Release.Minor);
            Assert.Equal(5, V.Release.Build);
            Assert.Equal(7, V.Release.Revision);
            Assert.Equal(Milestone.Release, V.Milestone);
            Assert.Equal(-1, V.PrereleaseNumber);
            Assert.Equal(Stability.Stable, V.Stability);
            Assert.True(V.ReleaseDate.HasValue);
            Assert.Equal(1985, V.ReleaseDate.Value.Year);
        }

        [Fact]
        public void Version025()
        {
            var V = new Version(new System.Version(2, 3, 5, 7), Milestone.Release, new DateTime(1985, 11, 20));

            Assert.Equal(2, V.Release.Major);
            Assert.Equal(3, V.Release.Minor);
            Assert.Equal(5, V.Release.Build);
            Assert.Equal(7, V.Release.Revision);
            Assert.Equal(Milestone.Release, V.Milestone);
            Assert.Equal(Stability.Stable, V.Stability);
            Assert.True(V.ReleaseDate.HasValue);
            Assert.Equal(1985, V.ReleaseDate.Value.Year);
        }

        [Fact]
        public void Version026()
        {
            var V = new Version(2, 3, 5, 7, Milestone.Release, new DateTime(1985, 11, 20));

            Assert.Equal(2, V.Release.Major);
            Assert.Equal(3, V.Release.Minor);
            Assert.Equal(5, V.Release.Build);
            Assert.Equal(7, V.Release.Revision);
            Assert.Equal(Milestone.Release, V.Milestone);
            Assert.Equal(Stability.Stable, V.Stability);
            Assert.True(V.ReleaseDate.HasValue);
            Assert.Equal(1985, V.ReleaseDate.Value.Year);
        }

        [Fact]
        public void Version027()
        {
            var V = new Version(2, 3, 5, Milestone.ReleaseCandidate, 1, new DateTime(1985, 11, 20));

            Assert.Equal(2, V.Release.Major);
            Assert.Equal(3, V.Release.Minor);
            Assert.Equal(5, V.Release.Build);
            Assert.Equal(-1, V.Release.Revision);
            Assert.Equal(Milestone.ReleaseCandidate, V.Milestone);
            Assert.Equal(1, V.PrereleaseNumber);
            Assert.Equal(Stability.Unstable, V.Stability);
            Assert.True(V.ReleaseDate.HasValue);
            Assert.Equal(1985, V.ReleaseDate.Value.Year);
        }

        [Theory]
        [InlineData("1.2.3")]
        [InlineData("1.2.3.4")]
        [InlineData("1.2.3.4-alpha.1")]
        [InlineData("1.2.3.4-beta.2")]
        [InlineData("1.2.3.4-rc.3")]
        [InlineData("1.2.3.4-alpha.3")]
        public void NuGetVersionConvertToVersionString(string nugetVersion)
        {
            var nVersion = new NuGetVersion(nugetVersion);
            var version = (Version)nVersion;

            _ = version.ToString().Should().Be(nVersion.ToString());
        }

        [Theory]
        [InlineData("1.2.3")]
        [InlineData("1.2.3-alpha.1")]
        [InlineData("1.2.3-beta.2")]
        [InlineData("1.2.3-rc.3")]
        [InlineData("1.2.3-alpha.3")]
        public void SemanticVersionConvertToVersionString(string semVersion)
        {
            var nVersion = SemanticVersion.Parse(semVersion);
            var version = (Version)nVersion;

            _ = version.ToString().Should().Be(nVersion.ToString());
        }

        [Theory]
        [InlineData("1.2.3", "1.2.3")]
        [InlineData("1.2.3", "1.2.3-alpha.1")]
        [InlineData("1.2.3-beta.4", "1.2.3-alpha.1")]
        [InlineData("1.2.3-beta.4", "1.2.3-beta.1")]
        [InlineData("1.2.3-beta.4", "1.2.3-rc.1")]
        [InlineData("1.2.3-alpha.4", "1.2.3-rc.1")]
        public void EqualityAndComparisonCheckWithNuGet(string v1, string v2)
        {
            var nv1 = new NuGetVersion(v1);
            var nv2 = new NuGetVersion(v2);
            var version1 = (Version)nv1;
            var version2 = (Version)nv2;

            var nequals = nv1 == nv2;
            var versionEquals = version1 == version2;

            _ = versionEquals.Should().Be(nequals);
            _ = (version1 > version2).Should().Be(nv1 > nv2);
            _ = (version1 >= version2).Should().Be(nv1 >= nv2);
            _ = (version1 < version2).Should().Be(nv1 < nv2);
            _ = (version1 <= version2).Should().Be(nv1 <= nv2);
        }

        [Fact]
        public void GivenVersion_WhenHashCodeRequested_ThenItShouldBeValid()
        {
            // Arrange
            var nv1 = new NuGetVersion("1.2.3");
            var nv2 = new NuGetVersion("4.5.6-beta.12");
            var nv3 = new NuGetVersion("7.8-rc");
            var nv4 = new NuGetVersion("9.10-alpha.2");
            var version1 = (Version)nv1;
            var version2 = (Version)nv2;
            var version3 = (Version)nv3;
            var version4 = (Version)nv4;

            // Act
            var hash1 = version1.GetHashCode();
            var hash2 = version2.GetHashCode();
            var hash3 = version3.GetHashCode();
            var hash4 = version4.GetHashCode();

            // Assert
            hash1.Should().NotBe(hash2);
            hash2.Should().NotBe(hash3);
            hash3.Should().NotBe(hash4);
            hash4.Should().NotBe(hash1);
        }

        [Fact]
        public void GivenVersions_WhenEqualityChecked_ThenItShouldBeCorrect()
        {
            // Arrange
            var nv1 = new NuGetVersion("1.2.3");
            var nv2 = new NuGetVersion("4.5.6-beta.12");
            var nv3 = new NuGetVersion("7.8-rc");
            var nv4 = new NuGetVersion("7.8-rc");
            var version1 = (Version)nv1;
            var version2 = (Version)nv2;
            var version3 = (Version)nv3;
            var version4 = (Version)nv4;
            var version5 = new System.Version(1, 2, 3);
            object version6 = new Version(1, 2, 3);

            // Assert
            version1.Equals(version1).Should().BeTrue();
            version1.Equals(version2).Should().BeFalse();
            version2.Equals(version3).Should().BeFalse();
            version3.Equals(version4).Should().BeTrue();
            version1.Equals(version5).Should().BeTrue();
            version1.Equals(version6).Should().BeTrue();
            version1.Equals("Welcome").Should().BeFalse();
            version1.Equals(null).Should().BeFalse();
        }
    }
}
