namespace TIKSN.Versioning.Tests
{
	[Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
	public class MilestoneTests
	{
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Test1()
		{
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Milestone.PreAlpha < Milestone.Alpha);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Test2()
		{
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Milestone.Alpha < Milestone.Beta);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Test3()
		{
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Milestone.Beta < Milestone.ReleaseCandidate);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Test4()
		{
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Milestone.ReleaseCandidate < Milestone.RTM);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Test5()
		{
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Milestone.RTM < Milestone.GA);
		}
	}
}