using FluentAssertions;
using TIKSN.Versioning;
using Xunit;

namespace TIKSN.Tests.Versioning;

public class MilestoneTests
{
    [Fact]
    public void Test2() => (Milestone.Alpha < Milestone.Beta).Should().BeTrue();

    [Fact]
    public void Test3() => (Milestone.Beta < Milestone.ReleaseCandidate).Should().BeTrue();

    [Fact]
    public void Test4() => (Milestone.ReleaseCandidate < Milestone.Release).Should().BeTrue();
}
