using FluentAssertions;
using Xunit;

namespace TIKSN.Versioning.Tests
{
    public class MilestoneTests
    {
        [Fact]
        public void Test2() => (Milestone.Alpha < Milestone.Beta).Should().BeTrue();

        [Fact]
        public void Test3() => (Milestone.Beta < Milestone.ReleaseCandidate).Should().BeTrue();

        [Fact]
        public void Test4() => Assert.True(Milestone.ReleaseCandidate < Milestone.Release);
    }
}
