using Xunit;

namespace TIKSN.Versioning.Tests
{
	public class MilestoneTests
	{
		[Fact]
		public void Test1()
		{
			Assert.True(Milestone.PreAlpha < Milestone.Alpha);
		}

		[Fact]
		public void Test2()
		{
			Assert.True(Milestone.Alpha < Milestone.Beta);
		}

		[Fact]
		public void Test3()
		{
			Assert.True(Milestone.Beta < Milestone.ReleaseCandidate);
		}

		[Fact]
		public void Test4()
		{
			Assert.True(Milestone.ReleaseCandidate < Milestone.RTM);
		}

		[Fact]
		public void Test5()
		{
			Assert.True(Milestone.RTM < Milestone.GA);
		}
	}
}