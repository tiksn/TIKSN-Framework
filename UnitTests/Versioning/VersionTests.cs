using System;
using Xunit;

namespace TIKSN.Versioning.Tests
{
	public class VersionTests
	{
		[Fact]
		public void Equals001()
		{
			Version V = new Version(2, 4);

			Assert.True(V.Equals(V));
		}

		[Fact]
		public void Equals002()
		{
			Version V1 = new Version(5, 6);
			Version V2 = new Version(5, 6);

			Assert.True(V1.Equals(V2));
		}

		[Fact]
		public void Equals003()
		{
			Version V1 = new Version(5, 6);
			Version V2 = new Version(5, 6, 1);

			Assert.False(V1.Equals(V2));
		}

		[Fact]
		public void Equals004()
		{
			Version V1 = new Version(5, 6, Milestone.Beta);
			Version V2 = new Version(5, 6, Milestone.ReleaseCandidate);

			Assert.False(V1.Equals(V2));
		}

		[Fact]
		public void Equals005()
		{
			Version V1 = new Version(5, 6, Milestone.ReleaseCandidate, 2);
			Version V2 = new Version(5, 6, Milestone.ReleaseCandidate, 1);

			Assert.False(V1.Equals(V2));
		}

		[Fact]
		public void EqualsOperator001()
		{
			Version V1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
			Version V2 = new Version(10, 20, Milestone.PreAlpha, 2);

			Assert.True(!(V1 == V2));
		}

		[Fact]
		public void EqualsOperator002()
		{
			Version V1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
			Version V2 = new Version(10, 20, Milestone.ReleaseCandidate, 2);

			Assert.True(V1 == V2);
		}

		[Fact]
		public void GreaterThanOperator001()
		{
			Version V1 = new Version(10, 30, Milestone.ReleaseCandidate, 2);
			Version V2 = new Version(10, 20, Milestone.ReleaseCandidate, 2);

			Assert.True(V1 > V2);
		}

		[Fact]
		public void GreaterThanOperator002()
		{
			Version V1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
			Version V2 = new Version(10, 30, Milestone.ReleaseCandidate, 2);

			Assert.True(!(V1 > V2));
		}

		[Fact]
		public void GreaterThanOrEqualsOperator001()
		{
			Version V1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
			Version V2 = new Version(10, 20, Milestone.ReleaseCandidate, 2);

			Assert.True(V1 >= V2);
		}

		[Fact]
		public void GreaterThanOrEqualsOperator002()
		{
			Version V1 = new Version(10, 40, Milestone.ReleaseCandidate, 2);
			Version V2 = new Version(10, 30, Milestone.ReleaseCandidate, 2);

			Assert.True(V1 >= V2);
		}

		[Fact]
		public void LessThanOperator001()
		{
			Version V1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
			Version V2 = new Version(10, 30, Milestone.ReleaseCandidate, 2);

			Assert.True(V1 < V2);
		}

		[Fact]
		public void LessThanOperator002()
		{
			Version V1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
			Version V2 = new Version(10, 20, Milestone.ReleaseCandidate, 3);

			Assert.True(V1 < V2);
		}

		[Fact]
		public void LessThanOrEqualsOperator001()
		{
			Version V1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
			Version V2 = new Version(10, 20, Milestone.ReleaseCandidate, 2);

			Assert.True(V1 <= V2);
		}

		[Fact]
		public void LessThanOrEqualsOperator002()
		{
			Version V1 = new Version(10, 10, Milestone.ReleaseCandidate, 4);
			Version V2 = new Version(10, 20, Milestone.ReleaseCandidate, 3);

			Assert.True(V1 <= V2);
		}

		[Fact]
		public void NotEqualsOperator001()
		{
			Version V1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
			Version V2 = new Version(10, 20, Milestone.PreAlpha, 2);

			Assert.True(V1 != V2);
		}

		[Fact]
		public void NotEqualsOperator002()
		{
			Version V1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
			Version V2 = new Version(10, 20, Milestone.ReleaseCandidate, 2);

			Assert.True(!(V1 != V2));
		}

		[Fact]
		public void PrereleaseNumber001()
		{
		    Assert.Throws<ArgumentOutOfRangeException>(() => new Version(1, 2, 3, 4, Milestone.ReleaseCandidate, -5));
		}

		[Fact]
		public void ToLongReleaseString001()
		{
			Version V = new Version(1, 2, 3, 4, Milestone.ReleaseCandidate, 5);

			Assert.Equal("1.2.3.4", V.ToLongReleaseString());
		}

		[Fact]
		public void ToPrereleaseString001()
		{
			Version V = new Version(1, 2, 3, 4, Milestone.PreAlpha, 5);

			Assert.Equal("pre-alpha5", V.ToPrereleaseString());
		}

		[Fact]
		public void ToPrereleaseString002()
		{
			Version V = new Version(1, 2, 3, 4, Milestone.Alpha, 5);

			Assert.Equal("alpha5", V.ToPrereleaseString());
		}

		[Fact]
		public void ToPrereleaseString003()
		{
			Version V = new Version(1, 2, 3, 4, Milestone.Beta, 5);

			Assert.Equal("beta5", V.ToPrereleaseString());
		}

		[Fact]
		public void ToPrereleaseString004()
		{
			Version V = new Version(1, 2, 3, 4, Milestone.ReleaseCandidate, 5);

			Assert.Equal("rc5", V.ToPrereleaseString());
		}

		[Fact]
		public void ToPrereleaseString005()
		{
			Version V = new Version(1, 2, 3, 4, Milestone.RTM, 5);

			Assert.Equal("rtm5", V.ToPrereleaseString());
		}

		[Fact]
		public void ToPrereleaseString006()
		{
			Version V = new Version(1, 2, 3, 4, Milestone.GA, 5);

			Assert.Equal("ga5", V.ToPrereleaseString());
		}

		[Fact]
		public void ToPrereleaseString007()
		{
			Version V = new Version(1, 2, 3, 4, Milestone.GA);

			Assert.Equal("ga", V.ToPrereleaseString());
		}

		[Fact]
		public void ToPrereleaseString008()
		{
		    Assert.Throws<NotSupportedException>(() => new Version(1, 2, 3, 4, (Milestone) 125689, 5));
		}

		[Fact]
		public void ToShortReleaseString001()
		{
			Assert.Equal(
				new Version(0, 0, 0, 0).ToShortReleaseString(),
				"0");
		}

		[Fact]
		public void ToShortReleaseString002()
		{
			Assert.Equal(
				   new Version(0, 0, 0, 4).ToShortReleaseString(),
				   "0.0.0.4");
		}

		[Fact]
		public void ToShortReleaseString003()
		{
			Assert.Equal(
				   new Version(0, 0, 3, 0).ToShortReleaseString(),
				   "0.0.3");
		}

		[Fact]
		public void ToShortReleaseString004()
		{
			Assert.Equal(
				   new Version(0, 0, 3, 4).ToShortReleaseString(),
				   "0.0.3.4");
		}

		[Fact]
		public void ToShortReleaseString005()
		{
			Assert.Equal(
				   new Version(0, 2, 0, 0).ToShortReleaseString(),
				   "0.2");
		}

		[Fact]
		public void ToShortReleaseString006()
		{
			Assert.Equal(
				   new Version(0, 2, 0, 4).ToShortReleaseString(),
				   "0.2.0.4");
		}

		[Fact]
		public void ToShortReleaseString007()
		{
			Assert.Equal(
				   new Version(0, 2, 3, 0).ToShortReleaseString(),
				   "0.2.3");
		}

		[Fact]
		public void ToShortReleaseString008()
		{
			Assert.Equal(
				   new Version(0, 2, 3, 4).ToShortReleaseString(),
				   "0.2.3.4");
		}

		[Fact]
		public void ToShortReleaseString009()
		{
			Assert.Equal(
				   new Version(1, 0, 0, 0).ToShortReleaseString(),
				   "1");
		}

		[Fact]
		public void ToShortReleaseString010()
		{
			Assert.Equal(
				   new Version(1, 0, 0, 4).ToShortReleaseString(),
				   "1.0.0.4");
		}

		[Fact]
		public void ToShortReleaseString011()
		{
			Assert.Equal(
				   new Version(1, 0, 3, 0).ToShortReleaseString(),
				   "1.0.3");
		}

		[Fact]
		public void ToShortReleaseString012()
		{
			Assert.Equal(
				   new Version(1, 0, 3, 4).ToShortReleaseString(),
				   "1.0.3.4");
		}

		[Fact]
		public void ToShortReleaseString013()
		{
			Assert.Equal(
				   new Version(1, 2, 0, 0).ToShortReleaseString(),
				   "1.2");
		}

		[Fact]
		public void ToShortReleaseString014()
		{
			Assert.Equal(
				   new Version(1, 2, 0, 4).ToShortReleaseString(),
				   "1.2.0.4");
		}

		[Fact]
		public void ToShortReleaseString015()
		{
			Assert.Equal(
				   new Version(1, 2, 3, 0).ToShortReleaseString(),
				   "1.2.3");
		}

		[Fact]
		public void ToShortReleaseString016()
		{
			Assert.Equal(
				new Version(1, 2, 3, 4).ToShortReleaseString(),
				"1.2.3.4");
		}

		[Fact]
		public void ToShortReleaseString017()
		{
			Assert.Equal(
				new Version(1, 2, 3).ToShortReleaseString(),
				"1.2.3");
		}

		[Fact]
		public void ToShortReleaseString018()
		{
			Assert.Equal(
				new Version(1, 2).ToShortReleaseString(),
				"1.2");
		}

		[Fact]
		public void ToShortReleaseString019()
		{
			Assert.Equal(
				new Version(1, 0).ToShortReleaseString(),
				"1");
		}

		[Fact]
		public void ToString001()
		{
			Version V = new Version(10, 20);

			Assert.Equal("10.20", V.ToString());
		}

		[Fact]
		public void ToString002()
		{
			Version V = new Version(10, 20, 30);

			Assert.Equal("10.20.30", V.ToString());
		}

		[Fact]
		public void ToString003()
		{
			Version V = new Version(10, 20, 30, 40);

			Assert.Equal("10.20.30.40", V.ToString());
		}

		[Fact]
		public void ToString004()
		{
			Version V = new Version(new System.Version(1, 2, 3, 4));

			Assert.Equal("1.2.3.4", V.ToString());
		}

		[Fact]
		public void ToString005()
		{
			Version V = new Version(10, 20, Milestone.ReleaseCandidate);

			Assert.Equal("10.20-rc", V.ToString());
		}

		[Fact]
		public void ToString006()
		{
			Version V = new Version(10, 20, 30, Milestone.ReleaseCandidate);

			Assert.Equal("10.20.30-rc", V.ToString());
		}

		[Fact]
		public void ToString007()
		{
			Version V = new Version(10, 20, 30, 40, Milestone.ReleaseCandidate);

			Assert.Equal("10.20.30.40-rc", V.ToString());
		}

		[Fact]
		public void ToString008()
		{
			Version V = new Version(new System.Version(1, 2, 3, 4), Milestone.ReleaseCandidate);

			Assert.Equal("1.2.3.4-rc", V.ToString());
		}

		[Fact]
		public void ToString009()
		{
			Version V = new Version(10, 20, Milestone.ReleaseCandidate, 2);

			Assert.Equal("10.20-rc2", V.ToString());
		}

		[Fact]
		public void ToString010()
		{
			Version V = new Version(10, 20, 30, Milestone.ReleaseCandidate, 2);

			Assert.Equal("10.20.30-rc2", V.ToString());
		}

		[Fact]
		public void ToString011()
		{
			Version V = new Version(10, 20, 30, 40, Milestone.ReleaseCandidate, 2);

			Assert.Equal("10.20.30.40-rc2", V.ToString());
		}

		[Fact]
		public void ToString012()
		{
			Version V = new Version(new System.Version(1, 2, 3, 4), Milestone.ReleaseCandidate, 2);

			Assert.Equal("1.2.3.4-rc2", V.ToString());
		}

		[Fact]
		public void Version001()
		{
			Version V = new Version(2, 3);

			Assert.Equal(2, V.Release.Major);
			Assert.Equal(3, V.Release.Minor);
			Assert.Equal(-1, V.Release.Build);
			Assert.Equal(-1, V.Release.Revision);
			Assert.Equal(Milestone.GA, V.Milestone);
			Assert.Equal(-1, V.PrereleaseNumber);
			Assert.Equal(Stability.Stable, V.Stability);
			Assert.False(V.ReleaseDate.HasValue);
		}

		[Fact]
		public void Version003()
		{
			Version V = new Version(2, 3, 5);

			Assert.Equal(2, V.Release.Major);
			Assert.Equal(3, V.Release.Minor);
			Assert.Equal(5, V.Release.Build);
			Assert.Equal(-1, V.Release.Revision);
			Assert.Equal(Milestone.GA, V.Milestone);
			Assert.Equal(-1, V.PrereleaseNumber);
			Assert.Equal(Stability.Stable, V.Stability);
			Assert.False(V.ReleaseDate.HasValue);
		}

		[Fact]
		public void Version005()
		{
			Version V = new Version(2, 3, 5, 7);

			Assert.Equal(2, V.Release.Major);
			Assert.Equal(3, V.Release.Minor);
			Assert.Equal(5, V.Release.Build);
			Assert.Equal(7, V.Release.Revision);
			Assert.Equal(Milestone.GA, V.Milestone);
			Assert.Equal(-1, V.PrereleaseNumber);
			Assert.Equal(Stability.Stable, V.Stability);
			Assert.False(V.ReleaseDate.HasValue);
		}

		[Fact]
		public void Version006()
		{
			Version V = new Version(new System.Version(2, 3, 5, 7));

			Assert.Equal(2, V.Release.Major);
			Assert.Equal(3, V.Release.Minor);
			Assert.Equal(5, V.Release.Build);
			Assert.Equal(7, V.Release.Revision);
			Assert.Equal(Milestone.GA, V.Milestone);
			Assert.Equal(-1, V.PrereleaseNumber);
			Assert.Equal(Stability.Stable, V.Stability);
			Assert.False(V.ReleaseDate.HasValue);
		}

		[Fact]
		public void Version007()
		{
			Version V = new Version(new System.Version(2, 3, 5, 7), Milestone.Beta);

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
			Version V = new Version(2, 3, 5, 7, Milestone.Beta);

			Assert.Equal(2, V.Release.Major);
			Assert.Equal(3, V.Release.Minor);
			Assert.Equal(5, V.Release.Build);
			Assert.Equal(7, V.Release.Revision);
			Assert.Equal(Milestone.Beta, V.Milestone);
			Assert.Equal(-1, V.PrereleaseNumber);
			Assert.Equal(Stability.Unstable, V.Stability);
			Assert.False(V.ReleaseDate.HasValue);
		}

		public void Version010()
		{
			Version V = new Version(2, 3, 5, Milestone.Beta);

			Assert.Equal(2, V.Release.Major);
			Assert.Equal(3, V.Release.Minor);
			Assert.Equal(5, V.Release.Build);
			Assert.Equal(-1, V.Release.Revision);
			Assert.Equal(Milestone.Beta, V.Milestone);
			Assert.Equal(-1, V.PrereleaseNumber);
			Assert.Equal(Stability.Unstable, V.Stability);
			Assert.False(V.ReleaseDate.HasValue);
		}

		public void Version011()
		{
			Version V = new Version(2, 3, Milestone.Beta);

			Assert.Equal(2, V.Release.Major);
			Assert.Equal(3, V.Release.Minor);
			Assert.Equal(-1, V.Release.Build);
			Assert.Equal(-1, V.Release.Revision);
			Assert.Equal(Milestone.Beta, V.Milestone);
			Assert.Equal(-1, V.PrereleaseNumber);
			Assert.Equal(Stability.Unstable, V.Stability);
			Assert.False(V.ReleaseDate.HasValue);
		}

		public void Version012()
		{
			Version V = new Version(2, 3, Milestone.Beta, 1);

			Assert.Equal(2, V.Release.Major);
			Assert.Equal(3, V.Release.Minor);
			Assert.Equal(-1, V.Release.Build);
			Assert.Equal(-1, V.Release.Revision);
			Assert.Equal(Milestone.Beta, V.Milestone);
			Assert.Equal(1, V.PrereleaseNumber);
			Assert.Equal(Stability.Unstable, V.Stability);
			Assert.False(V.ReleaseDate.HasValue);
		}

		public void Version013()
		{
			Version V = new Version(2, 3, 5, Milestone.Beta, 1);

			Assert.Equal(2, V.Release.Major);
			Assert.Equal(3, V.Release.Minor);
			Assert.Equal(5, V.Release.Build);
			Assert.Equal(-1, V.Release.Revision);
			Assert.Equal(Milestone.Beta, V.Milestone);
			Assert.Equal(1, V.PrereleaseNumber);
			Assert.Equal(Stability.Unstable, V.Stability);
			Assert.False(V.ReleaseDate.HasValue);
		}

		public void Version014()
		{
			Version V = new Version(2, 3, 5, 7, Milestone.Beta, 1);

			Assert.Equal(2, V.Release.Major);
			Assert.Equal(3, V.Release.Minor);
			Assert.Equal(5, V.Release.Build);
			Assert.Equal(7, V.Release.Revision);
			Assert.Equal(Milestone.Beta, V.Milestone);
			Assert.Equal(1, V.PrereleaseNumber);
			Assert.Equal(Stability.Unstable, V.Stability);
			Assert.False(V.ReleaseDate.HasValue);
		}

		public void Version015()
		{
			Version V = new Version(2, 3, 5, 7, Milestone.Beta, 1);

			Assert.Equal(2, V.Release.Major);
			Assert.Equal(3, V.Release.Minor);
			Assert.Equal(5, V.Release.Build);
			Assert.Equal(7, V.Release.Revision);
			Assert.Equal(Milestone.Beta, V.Milestone);
			Assert.Equal(1, V.PrereleaseNumber);
			Assert.Equal(Stability.Unstable, V.Stability);
			Assert.False(V.ReleaseDate.HasValue);
		}

		public void Version016()
		{
			Version V = new Version(new System.Version(2, 3, 5, 7), Milestone.Beta, 1);

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
			Version V = new Version(new System.Version(2, 3, 5, 7), new System.DateTime(1985, 11, 20));

			Assert.Equal(2, V.Release.Major);
			Assert.Equal(3, V.Release.Minor);
			Assert.Equal(5, V.Release.Build);
			Assert.Equal(7, V.Release.Revision);
			Assert.Equal(Milestone.GA, V.Milestone);
			Assert.Equal(-1, V.PrereleaseNumber);
			Assert.Equal(Stability.Stable, V.Stability);
			Assert.True(V.ReleaseDate.HasValue);
			Assert.Equal(1985, V.ReleaseDate.Value.Year);
		}

		[Fact]
		public void Version018()
		{
			Version V = new Version(2, 3, 5, 7, new System.DateTime(1985, 11, 20));

			Assert.Equal(2, V.Release.Major);
			Assert.Equal(3, V.Release.Minor);
			Assert.Equal(5, V.Release.Build);
			Assert.Equal(7, V.Release.Revision);
			Assert.Equal(Milestone.GA, V.Milestone);
			Assert.Equal(-1, V.PrereleaseNumber);
			Assert.Equal(Stability.Stable, V.Stability);
			Assert.True(V.ReleaseDate.HasValue);
			Assert.Equal(1985, V.ReleaseDate.Value.Year);
		}

		[Fact]
		public void Version019()
		{
			Version V = new Version(2, 3, 5, new System.DateTime(1985, 11, 20));

			Assert.Equal(2, V.Release.Major);
			Assert.Equal(3, V.Release.Minor);
			Assert.Equal(5, V.Release.Build);
			Assert.Equal(-1, V.Release.Revision);
			Assert.Equal(Milestone.GA, V.Milestone);
			Assert.Equal(-1, V.PrereleaseNumber);
			Assert.Equal(Stability.Stable, V.Stability);
			Assert.True(V.ReleaseDate.HasValue);
			Assert.Equal(1985, V.ReleaseDate.Value.Year);
		}

		[Fact]
		public void Version020()
		{
			Version V = new Version(2, 3, new System.DateTime(1985, 11, 20));

			Assert.Equal(2, V.Release.Major);
			Assert.Equal(3, V.Release.Minor);
			Assert.Equal(-1, V.Release.Build);
			Assert.Equal(-1, V.Release.Revision);
			Assert.Equal(Milestone.GA, V.Milestone);
			Assert.Equal(-1, V.PrereleaseNumber);
			Assert.Equal(Stability.Stable, V.Stability);
			Assert.True(V.ReleaseDate.HasValue);
			Assert.Equal(1985, V.ReleaseDate.Value.Year);
		}

		[Fact]
		public void Version021()
		{
			Version V = new Version(2, 3, Milestone.RTM, new System.DateTime(1985, 11, 20));

			Assert.Equal(2, V.Release.Major);
			Assert.Equal(3, V.Release.Minor);
			Assert.Equal(-1, V.Release.Build);
			Assert.Equal(-1, V.Release.Revision);
			Assert.Equal(Milestone.RTM, V.Milestone);
			Assert.Equal(-1, V.PrereleaseNumber);
			Assert.Equal(Stability.Stable, V.Stability);
			Assert.True(V.ReleaseDate.HasValue);
			Assert.Equal(1985, V.ReleaseDate.Value.Year);
		}

		[Fact]
		public void Version022()
		{
			Version V = new Version(2, 3, 5, Milestone.RTM, new System.DateTime(1985, 11, 20));

			Assert.Equal(2, V.Release.Major);
			Assert.Equal(3, V.Release.Minor);
			Assert.Equal(5, V.Release.Build);
			Assert.Equal(-1, V.Release.Revision);
			Assert.Equal(Milestone.RTM, V.Milestone);
			Assert.Equal(-1, V.PrereleaseNumber);
			Assert.Equal(Stability.Stable, V.Stability);
			Assert.True(V.ReleaseDate.HasValue);
			Assert.Equal(1985, V.ReleaseDate.Value.Year);
		}

		[Fact]
		public void Version023()
		{
			Version V = new Version(2, 3, 5, 7, Milestone.RTM, new System.DateTime(1985, 11, 20));

			Assert.Equal(2, V.Release.Major);
			Assert.Equal(3, V.Release.Minor);
			Assert.Equal(5, V.Release.Build);
			Assert.Equal(7, V.Release.Revision);
			Assert.Equal(Milestone.RTM, V.Milestone);
			Assert.Equal(-1, V.PrereleaseNumber);
			Assert.Equal(Stability.Stable, V.Stability);
			Assert.True(V.ReleaseDate.HasValue);
			Assert.Equal(1985, V.ReleaseDate.Value.Year);
		}

		[Fact]
		public void Version024()
		{
			Version V = new Version(new System.Version(2, 3, 5, 7), Milestone.RTM, new System.DateTime(1985, 11, 20));

			Assert.Equal(2, V.Release.Major);
			Assert.Equal(3, V.Release.Minor);
			Assert.Equal(5, V.Release.Build);
			Assert.Equal(7, V.Release.Revision);
			Assert.Equal(Milestone.RTM, V.Milestone);
			Assert.Equal(-1, V.PrereleaseNumber);
			Assert.Equal(Stability.Stable, V.Stability);
			Assert.True(V.ReleaseDate.HasValue);
			Assert.Equal(1985, V.ReleaseDate.Value.Year);
		}

		[Fact]
		public void Version025()
		{
			Version V = new Version(new System.Version(2, 3, 5, 7), Milestone.RTM, 1, new System.DateTime(1985, 11, 20));

			Assert.Equal(2, V.Release.Major);
			Assert.Equal(3, V.Release.Minor);
			Assert.Equal(5, V.Release.Build);
			Assert.Equal(7, V.Release.Revision);
			Assert.Equal(Milestone.RTM, V.Milestone);
			Assert.Equal(1, V.PrereleaseNumber);
			Assert.Equal(Stability.Stable, V.Stability);
			Assert.True(V.ReleaseDate.HasValue);
			Assert.Equal(1985, V.ReleaseDate.Value.Year);
		}

		[Fact]
		public void Version026()
		{
			Version V = new Version(2, 3, 5, 7, Milestone.RTM, 1, new System.DateTime(1985, 11, 20));

			Assert.Equal(2, V.Release.Major);
			Assert.Equal(3, V.Release.Minor);
			Assert.Equal(5, V.Release.Build);
			Assert.Equal(7, V.Release.Revision);
			Assert.Equal(Milestone.RTM, V.Milestone);
			Assert.Equal(1, V.PrereleaseNumber);
			Assert.Equal(Stability.Stable, V.Stability);
			Assert.True(V.ReleaseDate.HasValue);
			Assert.Equal(1985, V.ReleaseDate.Value.Year);
		}

		[Fact]
		public void Version027()
		{
			Version V = new Version(2, 3, 5, Milestone.RTM, 1, new System.DateTime(1985, 11, 20));

			Assert.Equal(2, V.Release.Major);
			Assert.Equal(3, V.Release.Minor);
			Assert.Equal(5, V.Release.Build);
			Assert.Equal(-1, V.Release.Revision);
			Assert.Equal(Milestone.RTM, V.Milestone);
			Assert.Equal(1, V.PrereleaseNumber);
			Assert.Equal(Stability.Stable, V.Stability);
			Assert.True(V.ReleaseDate.HasValue);
			Assert.Equal(1985, V.ReleaseDate.Value.Year);
		}

		[Fact]
		public void Version028()
		{
			Version V = new Version(2, 3, Milestone.RTM, 1, new System.DateTime(1985, 11, 20));

			Assert.Equal(2, V.Release.Major);
			Assert.Equal(3, V.Release.Minor);
			Assert.Equal(-1, V.Release.Build);
			Assert.Equal(-1, V.Release.Revision);
			Assert.Equal(Milestone.RTM, V.Milestone);
			Assert.Equal(1, V.PrereleaseNumber);
			Assert.Equal(Stability.Stable, V.Stability);
			Assert.True(V.ReleaseDate.HasValue);
			Assert.Equal(1985, V.ReleaseDate.Value.Year);
		}
	}
}