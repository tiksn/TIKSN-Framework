namespace TIKSN.Versioning.Tests
{
	[Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
	public class VersionTests
	{
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Equals001()
		{
			Version V = new Version(2, 4);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(V.Equals(V));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Equals002()
		{
			Version V1 = new Version(5, 6);
			Version V2 = new Version(5, 6);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(V1.Equals(V2));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Equals003()
		{
			Version V1 = new Version(5, 6);
			Version V2 = new Version(5, 6, 1);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(V1.Equals(V2));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Equals004()
		{
			Version V1 = new Version(5, 6, Milestone.Beta);
			Version V2 = new Version(5, 6, Milestone.ReleaseCandidate);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(V1.Equals(V2));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Equals005()
		{
			Version V1 = new Version(5, 6, Milestone.ReleaseCandidate, 2);
			Version V2 = new Version(5, 6, Milestone.ReleaseCandidate, 1);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(V1.Equals(V2));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void EqualsOperator001()
		{
			Version V1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
			Version V2 = new Version(10, 20, Milestone.PreAlpha, 2);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(!(V1 == V2));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void EqualsOperator002()
		{
			Version V1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
			Version V2 = new Version(10, 20, Milestone.ReleaseCandidate, 2);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(V1 == V2);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GreaterThanOperator001()
		{
			Version V1 = new Version(10, 30, Milestone.ReleaseCandidate, 2);
			Version V2 = new Version(10, 20, Milestone.ReleaseCandidate, 2);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(V1 > V2);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GreaterThanOperator002()
		{
			Version V1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
			Version V2 = new Version(10, 30, Milestone.ReleaseCandidate, 2);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(!(V1 > V2));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GreaterThanOrEqualsOperator001()
		{
			Version V1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
			Version V2 = new Version(10, 20, Milestone.ReleaseCandidate, 2);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(V1 >= V2);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GreaterThanOrEqualsOperator002()
		{
			Version V1 = new Version(10, 40, Milestone.ReleaseCandidate, 2);
			Version V2 = new Version(10, 30, Milestone.ReleaseCandidate, 2);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(V1 >= V2);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void LessThanOperator001()
		{
			Version V1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
			Version V2 = new Version(10, 30, Milestone.ReleaseCandidate, 2);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(V1 < V2);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void LessThanOperator002()
		{
			Version V1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
			Version V2 = new Version(10, 20, Milestone.ReleaseCandidate, 3);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(V1 < V2);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void LessThanOrEqualsOperator001()
		{
			Version V1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
			Version V2 = new Version(10, 20, Milestone.ReleaseCandidate, 2);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(V1 <= V2);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void LessThanOrEqualsOperator002()
		{
			Version V1 = new Version(10, 10, Milestone.ReleaseCandidate, 4);
			Version V2 = new Version(10, 20, Milestone.ReleaseCandidate, 3);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(V1 <= V2);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void NotEqualsOperator001()
		{
			Version V1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
			Version V2 = new Version(10, 20, Milestone.PreAlpha, 2);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(V1 != V2);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void NotEqualsOperator002()
		{
			Version V1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
			Version V2 = new Version(10, 20, Milestone.ReleaseCandidate, 2);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(!(V1 != V2));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void PrereleaseNumber001()
		{
			try
			{
				Version V = new Version(1, 2, 3, 4, Milestone.ReleaseCandidate, -5);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.ArgumentOutOfRangeException)
			{
			}
			catch
			{
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToLongReleaseString001()
		{
			Version V = new Version(1, 2, 3, 4, Milestone.ReleaseCandidate, 5);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("1.2.3.4", V.ToLongReleaseString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToPrereleaseString001()
		{
			Version V = new Version(1, 2, 3, 4, Milestone.PreAlpha, 5);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("pre-alpha5", V.ToPrereleaseString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToPrereleaseString002()
		{
			Version V = new Version(1, 2, 3, 4, Milestone.Alpha, 5);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("alpha5", V.ToPrereleaseString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToPrereleaseString003()
		{
			Version V = new Version(1, 2, 3, 4, Milestone.Beta, 5);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("beta5", V.ToPrereleaseString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToPrereleaseString004()
		{
			Version V = new Version(1, 2, 3, 4, Milestone.ReleaseCandidate, 5);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("rc5", V.ToPrereleaseString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToPrereleaseString005()
		{
			Version V = new Version(1, 2, 3, 4, Milestone.RTM, 5);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("rtm5", V.ToPrereleaseString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToPrereleaseString006()
		{
			Version V = new Version(1, 2, 3, 4, Milestone.GA, 5);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("ga5", V.ToPrereleaseString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToPrereleaseString007()
		{
			Version V = new Version(1, 2, 3, 4, Milestone.GA);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("ga", V.ToPrereleaseString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToPrereleaseString008()
		{
			Version V = new Version(1, 2, 3, 4, (Milestone)125689, 5);

			try
			{
				V.ToPrereleaseString();

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.NotSupportedException)
			{
			}
			catch
			{
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToShortReleaseString001()
		{
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>(
				new Version(0, 0, 0, 0).ToShortReleaseString(),
				"0");
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToShortReleaseString002()
		{
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>(
				   new Version(0, 0, 0, 4).ToShortReleaseString(),
				   "0.0.0.4");
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToShortReleaseString003()
		{
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>(
				   new Version(0, 0, 3, 0).ToShortReleaseString(),
				   "0.0.3");
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToShortReleaseString004()
		{
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>(
				   new Version(0, 0, 3, 4).ToShortReleaseString(),
				   "0.0.3.4");
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToShortReleaseString005()
		{
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>(
				   new Version(0, 2, 0, 0).ToShortReleaseString(),
				   "0.2");
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToShortReleaseString006()
		{
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>(
				   new Version(0, 2, 0, 4).ToShortReleaseString(),
				   "0.2.0.4");
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToShortReleaseString007()
		{
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>(
				   new Version(0, 2, 3, 0).ToShortReleaseString(),
				   "0.2.3");
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToShortReleaseString008()
		{
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>(
				   new Version(0, 2, 3, 4).ToShortReleaseString(),
				   "0.2.3.4");
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToShortReleaseString009()
		{
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>(
				   new Version(1, 0, 0, 0).ToShortReleaseString(),
				   "1");
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToShortReleaseString010()
		{
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>(
				   new Version(1, 0, 0, 4).ToShortReleaseString(),
				   "1.0.0.4");
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToShortReleaseString011()
		{
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>(
				   new Version(1, 0, 3, 0).ToShortReleaseString(),
				   "1.0.3");
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToShortReleaseString012()
		{
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>(
				   new Version(1, 0, 3, 4).ToShortReleaseString(),
				   "1.0.3.4");
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToShortReleaseString013()
		{
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>(
				   new Version(1, 2, 0, 0).ToShortReleaseString(),
				   "1.2");
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToShortReleaseString014()
		{
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>(
				   new Version(1, 2, 0, 4).ToShortReleaseString(),
				   "1.2.0.4");
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToShortReleaseString015()
		{
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>(
				   new Version(1, 2, 3, 0).ToShortReleaseString(),
				   "1.2.3");
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToShortReleaseString016()
		{
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>(
				new Version(1, 2, 3, 4).ToShortReleaseString(),
				"1.2.3.4");
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToShortReleaseString017()
		{
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>(
				new Version(1, 2, 3).ToShortReleaseString(),
				"1.2.3");
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToShortReleaseString018()
		{
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>(
				new Version(1, 2).ToShortReleaseString(),
				"1.2");
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToShortReleaseString019()
		{
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>(
				new Version(1, 0).ToShortReleaseString(),
				"1");
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString001()
		{
			Version V = new Version(10, 20);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("10.20", V.ToString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString002()
		{
			Version V = new Version(10, 20, 30);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("10.20.30", V.ToString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString003()
		{
			Version V = new Version(10, 20, 30, 40);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("10.20.30.40", V.ToString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString004()
		{
			Version V = new Version(new System.Version(1, 2, 3, 4));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("1.2.3.4", V.ToString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString005()
		{
			Version V = new Version(10, 20, Milestone.ReleaseCandidate);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("10.20-rc", V.ToString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString006()
		{
			Version V = new Version(10, 20, 30, Milestone.ReleaseCandidate);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("10.20.30-rc", V.ToString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString007()
		{
			Version V = new Version(10, 20, 30, 40, Milestone.ReleaseCandidate);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("10.20.30.40-rc", V.ToString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString008()
		{
			Version V = new Version(new System.Version(1, 2, 3, 4), Milestone.ReleaseCandidate);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("1.2.3.4-rc", V.ToString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString009()
		{
			Version V = new Version(10, 20, Milestone.ReleaseCandidate, 2);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("10.20-rc2", V.ToString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString010()
		{
			Version V = new Version(10, 20, 30, Milestone.ReleaseCandidate, 2);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("10.20.30-rc2", V.ToString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString011()
		{
			Version V = new Version(10, 20, 30, 40, Milestone.ReleaseCandidate, 2);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("10.20.30.40-rc2", V.ToString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString012()
		{
			Version V = new Version(new System.Version(1, 2, 3, 4), Milestone.ReleaseCandidate, 2);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("1.2.3.4-rc2", V.ToString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Version001()
		{
			Version V = new Version(2, 3);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, V.Release.Major);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, V.Release.Minor);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.Release.Build);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.Release.Revision);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Milestone>(Milestone.GA, V.Milestone);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.PrereleaseNumber);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Stability>(Stability.Stable, V.Stability);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(V.ReleaseDate.HasValue);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Version003()
		{
			Version V = new Version(2, 3, 5);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, V.Release.Major);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, V.Release.Minor);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(5, V.Release.Build);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.Release.Revision);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Milestone>(Milestone.GA, V.Milestone);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.PrereleaseNumber);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Stability>(Stability.Stable, V.Stability);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(V.ReleaseDate.HasValue);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Version005()
		{
			Version V = new Version(2, 3, 5, 7);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, V.Release.Major);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, V.Release.Minor);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(5, V.Release.Build);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(7, V.Release.Revision);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Milestone>(Milestone.GA, V.Milestone);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.PrereleaseNumber);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Stability>(Stability.Stable, V.Stability);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(V.ReleaseDate.HasValue);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Version006()
		{
			Version V = new Version(new System.Version(2, 3, 5, 7));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, V.Release.Major);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, V.Release.Minor);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(5, V.Release.Build);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(7, V.Release.Revision);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Milestone>(Milestone.GA, V.Milestone);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.PrereleaseNumber);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Stability>(Stability.Stable, V.Stability);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(V.ReleaseDate.HasValue);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Version007()
		{
			Version V = new Version(new System.Version(2, 3, 5, 7), Milestone.Beta);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, V.Release.Major);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, V.Release.Minor);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(5, V.Release.Build);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(7, V.Release.Revision);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Milestone>(Milestone.Beta, V.Milestone);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.PrereleaseNumber);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Stability>(Stability.Unstable, V.Stability);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(V.ReleaseDate.HasValue);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Version008()
		{
			Version V = new Version(2, 3, 5, 7, Milestone.Beta);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, V.Release.Major);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, V.Release.Minor);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(5, V.Release.Build);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(7, V.Release.Revision);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Milestone>(Milestone.Beta, V.Milestone);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.PrereleaseNumber);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Stability>(Stability.Unstable, V.Stability);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(V.ReleaseDate.HasValue);
		}

		public void Version010()
		{
			Version V = new Version(2, 3, 5, Milestone.Beta);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, V.Release.Major);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, V.Release.Minor);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(5, V.Release.Build);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.Release.Revision);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Milestone>(Milestone.Beta, V.Milestone);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.PrereleaseNumber);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Stability>(Stability.Unstable, V.Stability);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(V.ReleaseDate.HasValue);
		}

		public void Version011()
		{
			Version V = new Version(2, 3, Milestone.Beta);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, V.Release.Major);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, V.Release.Minor);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.Release.Build);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.Release.Revision);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Milestone>(Milestone.Beta, V.Milestone);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.PrereleaseNumber);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Stability>(Stability.Unstable, V.Stability);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(V.ReleaseDate.HasValue);
		}

		public void Version012()
		{
			Version V = new Version(2, 3, Milestone.Beta, 1);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, V.Release.Major);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, V.Release.Minor);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.Release.Build);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.Release.Revision);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Milestone>(Milestone.Beta, V.Milestone);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1, V.PrereleaseNumber);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Stability>(Stability.Unstable, V.Stability);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(V.ReleaseDate.HasValue);
		}

		public void Version013()
		{
			Version V = new Version(2, 3, 5, Milestone.Beta, 1);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, V.Release.Major);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, V.Release.Minor);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(5, V.Release.Build);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.Release.Revision);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Milestone>(Milestone.Beta, V.Milestone);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1, V.PrereleaseNumber);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Stability>(Stability.Unstable, V.Stability);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(V.ReleaseDate.HasValue);
		}

		public void Version014()
		{
			Version V = new Version(2, 3, 5, 7, Milestone.Beta, 1);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, V.Release.Major);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, V.Release.Minor);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(5, V.Release.Build);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(7, V.Release.Revision);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Milestone>(Milestone.Beta, V.Milestone);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1, V.PrereleaseNumber);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Stability>(Stability.Unstable, V.Stability);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(V.ReleaseDate.HasValue);
		}

		public void Version015()
		{
			Version V = new Version(2, 3, 5, 7, Milestone.Beta, 1);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, V.Release.Major);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, V.Release.Minor);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(5, V.Release.Build);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(7, V.Release.Revision);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Milestone>(Milestone.Beta, V.Milestone);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1, V.PrereleaseNumber);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Stability>(Stability.Unstable, V.Stability);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(V.ReleaseDate.HasValue);
		}

		public void Version016()
		{
			Version V = new Version(new System.Version(2, 3, 5, 7), Milestone.Beta, 1);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, V.Release.Major);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, V.Release.Minor);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(5, V.Release.Build);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(7, V.Release.Revision);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Milestone>(Milestone.Beta, V.Milestone);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1, V.PrereleaseNumber);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Stability>(Stability.Unstable, V.Stability);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(V.ReleaseDate.HasValue);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Version017()
		{
			Version V = new Version(new System.Version(2, 3, 5, 7), new System.DateTime(1985, 11, 20));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, V.Release.Major);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, V.Release.Minor);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(5, V.Release.Build);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(7, V.Release.Revision);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Milestone>(Milestone.GA, V.Milestone);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.PrereleaseNumber);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Stability>(Stability.Stable, V.Stability);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(V.ReleaseDate.HasValue);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1985, V.ReleaseDate.Value.Year);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Version018()
		{
			Version V = new Version(2, 3, 5, 7, new System.DateTime(1985, 11, 20));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, V.Release.Major);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, V.Release.Minor);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(5, V.Release.Build);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(7, V.Release.Revision);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Milestone>(Milestone.GA, V.Milestone);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.PrereleaseNumber);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Stability>(Stability.Stable, V.Stability);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(V.ReleaseDate.HasValue);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1985, V.ReleaseDate.Value.Year);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Version019()
		{
			Version V = new Version(2, 3, 5, new System.DateTime(1985, 11, 20));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, V.Release.Major);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, V.Release.Minor);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(5, V.Release.Build);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.Release.Revision);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Milestone>(Milestone.GA, V.Milestone);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.PrereleaseNumber);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Stability>(Stability.Stable, V.Stability);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(V.ReleaseDate.HasValue);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1985, V.ReleaseDate.Value.Year);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Version020()
		{
			Version V = new Version(2, 3, new System.DateTime(1985, 11, 20));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, V.Release.Major);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, V.Release.Minor);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.Release.Build);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.Release.Revision);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Milestone>(Milestone.GA, V.Milestone);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.PrereleaseNumber);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Stability>(Stability.Stable, V.Stability);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(V.ReleaseDate.HasValue);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1985, V.ReleaseDate.Value.Year);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Version021()
		{
			Version V = new Version(2, 3, Milestone.RTM, new System.DateTime(1985, 11, 20));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, V.Release.Major);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, V.Release.Minor);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.Release.Build);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.Release.Revision);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Milestone>(Milestone.RTM, V.Milestone);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.PrereleaseNumber);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Stability>(Stability.Stable, V.Stability);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(V.ReleaseDate.HasValue);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1985, V.ReleaseDate.Value.Year);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Version022()
		{
			Version V = new Version(2, 3, 5, Milestone.RTM, new System.DateTime(1985, 11, 20));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, V.Release.Major);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, V.Release.Minor);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(5, V.Release.Build);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.Release.Revision);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Milestone>(Milestone.RTM, V.Milestone);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.PrereleaseNumber);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Stability>(Stability.Stable, V.Stability);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(V.ReleaseDate.HasValue);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1985, V.ReleaseDate.Value.Year);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Version023()
		{
			Version V = new Version(2, 3, 5, 7, Milestone.RTM, new System.DateTime(1985, 11, 20));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, V.Release.Major);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, V.Release.Minor);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(5, V.Release.Build);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(7, V.Release.Revision);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Milestone>(Milestone.RTM, V.Milestone);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.PrereleaseNumber);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Stability>(Stability.Stable, V.Stability);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(V.ReleaseDate.HasValue);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1985, V.ReleaseDate.Value.Year);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Version024()
		{
			Version V = new Version(new System.Version(2, 3, 5, 7), Milestone.RTM, new System.DateTime(1985, 11, 20));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, V.Release.Major);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, V.Release.Minor);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(5, V.Release.Build);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(7, V.Release.Revision);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Milestone>(Milestone.RTM, V.Milestone);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.PrereleaseNumber);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Stability>(Stability.Stable, V.Stability);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(V.ReleaseDate.HasValue);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1985, V.ReleaseDate.Value.Year);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Version025()
		{
			Version V = new Version(new System.Version(2, 3, 5, 7), Milestone.RTM, 1, new System.DateTime(1985, 11, 20));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, V.Release.Major);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, V.Release.Minor);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(5, V.Release.Build);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(7, V.Release.Revision);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Milestone>(Milestone.RTM, V.Milestone);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1, V.PrereleaseNumber);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Stability>(Stability.Stable, V.Stability);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(V.ReleaseDate.HasValue);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1985, V.ReleaseDate.Value.Year);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Version026()
		{
			Version V = new Version(2, 3, 5, 7, Milestone.RTM, 1, new System.DateTime(1985, 11, 20));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, V.Release.Major);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, V.Release.Minor);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(5, V.Release.Build);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(7, V.Release.Revision);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Milestone>(Milestone.RTM, V.Milestone);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1, V.PrereleaseNumber);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Stability>(Stability.Stable, V.Stability);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(V.ReleaseDate.HasValue);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1985, V.ReleaseDate.Value.Year);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Version027()
		{
			Version V = new Version(2, 3, 5, Milestone.RTM, 1, new System.DateTime(1985, 11, 20));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, V.Release.Major);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, V.Release.Minor);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(5, V.Release.Build);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.Release.Revision);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Milestone>(Milestone.RTM, V.Milestone);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1, V.PrereleaseNumber);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Stability>(Stability.Stable, V.Stability);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(V.ReleaseDate.HasValue);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1985, V.ReleaseDate.Value.Year);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Version028()
		{
			Version V = new Version(2, 3, Milestone.RTM, 1, new System.DateTime(1985, 11, 20));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, V.Release.Major);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, V.Release.Minor);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.Release.Build);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(-1, V.Release.Revision);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Milestone>(Milestone.RTM, V.Milestone);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1, V.PrereleaseNumber);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Stability>(Stability.Stable, V.Stability);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(V.ReleaseDate.HasValue);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1985, V.ReleaseDate.Value.Year);
		}
	}
}