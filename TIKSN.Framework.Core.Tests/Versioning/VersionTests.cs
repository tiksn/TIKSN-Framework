using NuGet.Versioning;
using Shouldly;
using TIKSN.Versioning;
using Xunit;

namespace TIKSN.Tests.Versioning;

public class VersionTests
{
    [Fact]
    public void Equals001()
    {
        var v = new Version(2, 4);

        v.Equals(v).ShouldBeTrue();
    }

    [Fact]
    public void Equals002()
    {
        var v1 = new Version(5, 6);
        var v2 = new Version(5, 6);

        v1.Equals(v2).ShouldBeTrue();
    }

    [Fact]
    public void Equals003()
    {
        var v1 = new Version(5, 6);
        var v2 = new Version(5, 6, 1);

        v1.Equals(v2).ShouldBeFalse();
    }

    [Fact]
    public void Equals004()
    {
        var v1 = new Version(5, 6, Milestone.Beta);
        var v2 = new Version(5, 6, Milestone.ReleaseCandidate);

        v1.Equals(v2).ShouldBeFalse();
    }

    [Fact]
    public void Equals005()
    {
        var v1 = new Version(5, 6, Milestone.ReleaseCandidate, 2);
        var v2 = new Version(5, 6, Milestone.ReleaseCandidate, 1);

        v1.Equals(v2).ShouldBeFalse();
    }

    [Fact]
    public void Equals006()
    {
        var v = new Version(2, 4);

        v.Equals(null).ShouldBeFalse();
    }

    [Fact]
    public void EqualsOperator001()
    {
        var v1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
        var v2 = new Version(10, 20, Milestone.Alpha, 2);

        (!(v1 == v2)).ShouldBeTrue();
    }

    [Fact]
    public void EqualsOperator002()
    {
        var v1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
        var v2 = new Version(10, 20, Milestone.ReleaseCandidate, 2);

        (v1 == v2).ShouldBeTrue();
    }

    [Fact]
    public void GreaterThanOperator001()
    {
        var v1 = new Version(10, 30, Milestone.ReleaseCandidate, 2);
        var v2 = new Version(10, 20, Milestone.ReleaseCandidate, 2);

        (v1 > v2).ShouldBeTrue();
    }

    [Fact]
    public void GreaterThanOperator002()
    {
        var v1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
        var v2 = new Version(10, 30, Milestone.ReleaseCandidate, 2);

        (!(v1 > v2)).ShouldBeTrue();
    }

    [Fact]
    public void GreaterThanOrEqualsOperator001()
    {
        var v1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
        var v2 = new Version(10, 20, Milestone.ReleaseCandidate, 2);

        (v1 >= v2).ShouldBeTrue();
    }

    [Fact]
    public void GreaterThanOrEqualsOperator002()
    {
        var v1 = new Version(10, 40, Milestone.ReleaseCandidate, 2);
        var v2 = new Version(10, 30, Milestone.ReleaseCandidate, 2);

        (v1 >= v2).ShouldBeTrue();
    }

    [Fact]
    public void LessThanOperator001()
    {
        var v1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
        var v2 = new Version(10, 30, Milestone.ReleaseCandidate, 2);

        (v1 < v2).ShouldBeTrue();
    }

    [Fact]
    public void LessThanOperator002()
    {
        var v1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
        var v2 = new Version(10, 20, Milestone.ReleaseCandidate, 3);

        (v1 < v2).ShouldBeTrue();
    }

    [Fact]
    public void LessThanOrEqualsOperator001()
    {
        var v1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
        var v2 = new Version(10, 20, Milestone.ReleaseCandidate, 2);

        (v1 <= v2).ShouldBeTrue();
    }

    [Fact]
    public void LessThanOrEqualsOperator002()
    {
        var v1 = new Version(10, 10, Milestone.ReleaseCandidate, 4);
        var v2 = new Version(10, 20, Milestone.ReleaseCandidate, 3);

        (v1 <= v2).ShouldBeTrue();
    }

    [Fact]
    public void NotEqualsOperator001()
    {
        var v1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
        var v2 = new Version(10, 20, Milestone.Alpha, 2);

        (v1 != v2).ShouldBeTrue();
    }

    [Fact]
    public void NotEqualsOperator002()
    {
        var v1 = new Version(10, 20, Milestone.ReleaseCandidate, 2);
        var v2 = new Version(10, 20, Milestone.ReleaseCandidate, 2);

        (!(v1 != v2)).ShouldBeTrue();
    }

    [Fact]
    public void PrereleaseNumber001()
        => new System.Func<object>(() => new Version(1, 2, 3, 4, Milestone.ReleaseCandidate, -5)).ShouldThrow<System.ArgumentOutOfRangeException>();

    [Fact]
    public void ToLongReleaseString001()
    {
        var v = new Version(1, 2, 3, 4, Milestone.ReleaseCandidate, 5);

        v.ToLongReleaseString().ShouldBe("1.2.3.4");
    }

    [Fact]
    public void ToPrereleaseString001()
    {
        var v = new Version(1, 2, 3, 4, Milestone.Alpha, 5);

        v.ToString().ShouldBe("1.2.3.4-alpha.5");
    }

    [Fact]
    public void ToPrereleaseString002()
    {
        var v = new Version(1, 2, 3, 4, Milestone.Alpha, 5);

        v.ToString().ShouldBe("1.2.3.4-alpha.5");
    }

    [Fact]
    public void ToPrereleaseString003()
    {
        var v = new Version(1, 2, 3, 4, Milestone.Beta, 5);

        v.ToString().ShouldBe("1.2.3.4-beta.5");
    }

    [Fact]
    public void ToPrereleaseString004()
    {
        var v = new Version(1, 2, 3, 4, Milestone.ReleaseCandidate, 5);

        v.ToString().ShouldBe("1.2.3.4-rc.5");
    }

    [Fact]
    public void ToPrereleaseString006()
    {
        var v = new Version(1, 2, 3, 4, Milestone.Release);

        v.ToString().ShouldBe("1.2.3.4");
    }

    [Fact]
    public void ToPrereleaseString007()
    {
        var v = new Version(1, 2, 3, 4, Milestone.Release);

        v.ToString().ShouldBe("1.2.3.4");
    }

    [Fact]
    public void ToPrereleaseString008() => new System.Func<object>(() => new Version(1, 2, 3, 4, (Milestone)125689, 5)).ShouldThrow<System.NotSupportedException>();

    [Fact]
    public void ToShortReleaseString001() => new Version(0, 0, 0, 0).ToShortReleaseString().ShouldBe("0");

    [Fact]
    public void ToShortReleaseString002() => new Version(0, 0, 0, 4).ToShortReleaseString().ShouldBe("0.0.0.4");

    [Fact]
    public void ToShortReleaseString003() => new Version(0, 0, 3, 0).ToShortReleaseString().ShouldBe("0.0.3");

    [Fact]
    public void ToShortReleaseString004() => new Version(0, 0, 3, 4).ToShortReleaseString().ShouldBe("0.0.3.4");

    [Fact]
    public void ToShortReleaseString005() => new Version(0, 2, 0, 0).ToShortReleaseString().ShouldBe("0.2");

    [Fact]
    public void ToShortReleaseString006() => new Version(0, 2, 0, 4).ToShortReleaseString().ShouldBe("0.2.0.4");

    [Fact]
    public void ToShortReleaseString007() => new Version(0, 2, 3, 0).ToShortReleaseString().ShouldBe("0.2.3");

    [Fact]
    public void ToShortReleaseString008() => new Version(0, 2, 3, 4).ToShortReleaseString().ShouldBe("0.2.3.4");

    [Fact]
    public void ToShortReleaseString009() => new Version(1, 0, 0, 0).ToShortReleaseString().ShouldBe("1");

    [Fact]
    public void ToShortReleaseString010() => new Version(1, 0, 0, 4).ToShortReleaseString().ShouldBe("1.0.0.4");

    [Fact]
    public void ToShortReleaseString011() => new Version(1, 0, 3, 0).ToShortReleaseString().ShouldBe("1.0.3");

    [Fact]
    public void ToShortReleaseString012() => new Version(1, 0, 3, 4).ToShortReleaseString().ShouldBe("1.0.3.4");

    [Fact]
    public void ToShortReleaseString013() => new Version(1, 2, 0, 0).ToShortReleaseString().ShouldBe("1.2");

    [Fact]
    public void ToShortReleaseString014() => new Version(1, 2, 0, 4).ToShortReleaseString().ShouldBe("1.2.0.4");

    [Fact]
    public void ToShortReleaseString015() => new Version(1, 2, 3, 0).ToShortReleaseString().ShouldBe("1.2.3");

    [Fact]
    public void ToShortReleaseString016() => new Version(1, 2, 3, 4).ToShortReleaseString().ShouldBe("1.2.3.4");

    [Fact]
    public void ToShortReleaseString017() => new Version(1, 2, 3).ToShortReleaseString().ShouldBe("1.2.3");

    [Fact]
    public void ToShortReleaseString018() => new Version(1, 2).ToShortReleaseString().ShouldBe("1.2");

    [Fact]
    public void ToShortReleaseString019() => new Version(1, 0).ToShortReleaseString().ShouldBe("1");

    [Fact]
    public void ToString001()
    {
        var v = new Version(10, 20);

        v.ToString().ShouldBe("10.20");
    }

    [Fact]
    public void ToString002()
    {
        var v = new Version(10, 20, 30);

        v.ToString().ShouldBe("10.20.30");
    }

    [Fact]
    public void ToString003()
    {
        var v = new Version(10, 20, 30, 40);

        v.ToString().ShouldBe("10.20.30.40");
    }

    [Fact]
    public void ToString004()
    {
        var v = new Version(new System.Version(1, 2, 3, 4));

        v.ToString().ShouldBe("1.2.3.4");
    }

    [Fact]
    public void ToString005()
    {
        var v = new Version(10, 20, Milestone.ReleaseCandidate);

        v.ToString().ShouldBe("10.20-rc");
    }

    [Fact]
    public void ToString006()
    {
        var v = new Version(10, 20, 30, Milestone.ReleaseCandidate);

        v.ToString().ShouldBe("10.20.30-rc");
    }

    [Fact]
    public void ToString007()
    {
        var v = new Version(10, 20, 30, 40, Milestone.ReleaseCandidate);

        v.ToString().ShouldBe("10.20.30.40-rc");
    }

    [Fact]
    public void ToString008()
    {
        var v = new Version(new System.Version(1, 2, 3, 4), Milestone.ReleaseCandidate);

        v.ToString().ShouldBe("1.2.3.4-rc");
    }

    [Fact]
    public void ToString009()
    {
        var v = new Version(10, 20, Milestone.ReleaseCandidate, 2);

        v.ToString().ShouldBe("10.20-rc.2");
    }

    [Fact]
    public void ToString010()
    {
        var v = new Version(10, 20, 30, Milestone.ReleaseCandidate, 2);

        v.ToString().ShouldBe("10.20.30-rc.2");
    }

    [Fact]
    public void ToString011()
    {
        var v = new Version(10, 20, 30, 40, Milestone.ReleaseCandidate, 2);

        v.ToString().ShouldBe("10.20.30.40-rc.2");
    }

    [Fact]
    public void ToString012()
    {
        var v = new Version(new System.Version(1, 2, 3, 4), Milestone.ReleaseCandidate, 2);

        v.ToString().ShouldBe("1.2.3.4-rc.2");
    }

    [Fact]
    public void Version001()
    {
        var v = new Version(2, 3);

        v.Release.Major.ShouldBe(2);
        v.Release.Minor.ShouldBe(3);
        v.Release.Build.ShouldBe(-1);
        v.Release.Revision.ShouldBe(-1);
        v.Milestone.ShouldBe(Milestone.Release);
        v.PreReleaseNumber.ShouldBe(-1);
        v.Stability.ShouldBe(Stability.Stable);
        v.ReleaseDate.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void Version003()
    {
        var v = new Version(2, 3, 5);

        v.Release.Major.ShouldBe(2);
        v.Release.Minor.ShouldBe(3);
        v.Release.Build.ShouldBe(5);
        v.Release.Revision.ShouldBe(-1);
        v.Milestone.ShouldBe(Milestone.Release);
        v.PreReleaseNumber.ShouldBe(-1);
        v.Stability.ShouldBe(Stability.Stable);
        v.ReleaseDate.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void Version005()
    {
        var v = new Version(2, 3, 5, 7);

        v.Release.Major.ShouldBe(2);
        v.Release.Minor.ShouldBe(3);
        v.Release.Build.ShouldBe(5);
        v.Release.Revision.ShouldBe(7);
        v.Milestone.ShouldBe(Milestone.Release);
        v.PreReleaseNumber.ShouldBe(-1);
        v.Stability.ShouldBe(Stability.Stable);
        v.ReleaseDate.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void Version006()
    {
        var v = new Version(new System.Version(2, 3, 5, 7));

        v.Release.Major.ShouldBe(2);
        v.Release.Minor.ShouldBe(3);
        v.Release.Build.ShouldBe(5);
        v.Release.Revision.ShouldBe(7);
        v.Milestone.ShouldBe(Milestone.Release);
        v.PreReleaseNumber.ShouldBe(-1);
        v.Stability.ShouldBe(Stability.Stable);
        v.ReleaseDate.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void Version007()
    {
        var v = new Version(new System.Version(2, 3, 5, 7), Milestone.Beta);

        v.Release.Major.ShouldBe(2);
        v.Release.Minor.ShouldBe(3);
        v.Release.Build.ShouldBe(5);
        v.Release.Revision.ShouldBe(7);
        v.Milestone.ShouldBe(Milestone.Beta);
        v.PreReleaseNumber.ShouldBe(-1);
        v.Stability.ShouldBe(Stability.Unstable);
        v.ReleaseDate.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void Version008()
    {
        var v = new Version(2, 3, 5, 7, Milestone.Beta);

        v.Release.Major.ShouldBe(2);
        v.Release.Minor.ShouldBe(3);
        v.Release.Build.ShouldBe(5);
        v.Release.Revision.ShouldBe(7);
        v.Milestone.ShouldBe(Milestone.Beta);
        v.PreReleaseNumber.ShouldBe(-1);
        v.Stability.ShouldBe(Stability.Unstable);
        v.ReleaseDate.HasValue.ShouldBeFalse();
    }

    [Fact]
    public static void Version010()
    {
        var v = new Version(2, 3, 5, Milestone.Beta);

        v.Release.Major.ShouldBe(2);
        v.Release.Minor.ShouldBe(3);
        v.Release.Build.ShouldBe(5);
        v.Release.Revision.ShouldBe(-1);
        v.Milestone.ShouldBe(Milestone.Beta);
        v.PreReleaseNumber.ShouldBe(-1);
        v.Stability.ShouldBe(Stability.Unstable);
        v.ReleaseDate.HasValue.ShouldBeFalse();
    }

    [Fact]
    public static void Version011()
    {
        var v = new Version(2, 3, Milestone.Beta);

        v.Release.Major.ShouldBe(2);
        v.Release.Minor.ShouldBe(3);
        v.Release.Build.ShouldBe(-1);
        v.Release.Revision.ShouldBe(-1);
        v.Milestone.ShouldBe(Milestone.Beta);
        v.PreReleaseNumber.ShouldBe(-1);
        v.Stability.ShouldBe(Stability.Unstable);
        v.ReleaseDate.HasValue.ShouldBeFalse();
    }

    [Fact]
    public static void Version012()
    {
        var v = new Version(2, 3, Milestone.Beta, 1);

        v.Release.Major.ShouldBe(2);
        v.Release.Minor.ShouldBe(3);
        v.Release.Build.ShouldBe(-1);
        v.Release.Revision.ShouldBe(-1);
        v.Milestone.ShouldBe(Milestone.Beta);
        v.PreReleaseNumber.ShouldBe(1);
        v.Stability.ShouldBe(Stability.Unstable);
        v.ReleaseDate.HasValue.ShouldBeFalse();
    }

    [Fact]
    public static void Version013()
    {
        var v = new Version(2, 3, 5, Milestone.Beta, 1);

        v.Release.Major.ShouldBe(2);
        v.Release.Minor.ShouldBe(3);
        v.Release.Build.ShouldBe(5);
        v.Release.Revision.ShouldBe(-1);
        v.Milestone.ShouldBe(Milestone.Beta);
        v.PreReleaseNumber.ShouldBe(1);
        v.Stability.ShouldBe(Stability.Unstable);
        v.ReleaseDate.HasValue.ShouldBeFalse();
    }

    [Fact]
    public static void Version014()
    {
        var v = new Version(2, 3, 5, 7, Milestone.Beta, 1);

        v.Release.Major.ShouldBe(2);
        v.Release.Minor.ShouldBe(3);
        v.Release.Build.ShouldBe(5);
        v.Release.Revision.ShouldBe(7);
        v.Milestone.ShouldBe(Milestone.Beta);
        v.PreReleaseNumber.ShouldBe(1);
        v.Stability.ShouldBe(Stability.Unstable);
        v.ReleaseDate.HasValue.ShouldBeFalse();
    }

    [Fact]
    public static void Version015()
    {
        var v = new Version(2, 3, 5, 7, Milestone.Beta, 1);

        v.Release.Major.ShouldBe(2);
        v.Release.Minor.ShouldBe(3);
        v.Release.Build.ShouldBe(5);
        v.Release.Revision.ShouldBe(7);
        v.Milestone.ShouldBe(Milestone.Beta);
        v.PreReleaseNumber.ShouldBe(1);
        v.Stability.ShouldBe(Stability.Unstable);
        v.ReleaseDate.HasValue.ShouldBeFalse();
    }

    [Fact]
    public static void Version016()
    {
        var v = new Version(new System.Version(2, 3, 5, 7), Milestone.Beta, 1);

        v.Release.Major.ShouldBe(2);
        v.Release.Minor.ShouldBe(3);
        v.Release.Build.ShouldBe(5);
        v.Release.Revision.ShouldBe(7);
        v.Milestone.ShouldBe(Milestone.Beta);
        v.PreReleaseNumber.ShouldBe(1);
        v.Stability.ShouldBe(Stability.Unstable);
        v.ReleaseDate.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void Version017()
    {
        var v = new Version(new System.Version(2, 3, 5, 7), new System.DateTime(1985, 11, 20));

        v.Release.Major.ShouldBe(2);
        v.Release.Minor.ShouldBe(3);
        v.Release.Build.ShouldBe(5);
        v.Release.Revision.ShouldBe(7);
        v.Milestone.ShouldBe(Milestone.Release);
        v.PreReleaseNumber.ShouldBe(-1);
        v.Stability.ShouldBe(Stability.Stable);
        v.ReleaseDate.HasValue.ShouldBeTrue();
        v.ReleaseDate.Value.Year.ShouldBe(1985);
    }

    [Fact]
    public void Version018()
    {
        var v = new Version(2, 3, 5, 7, new System.DateTime(1985, 11, 20));

        v.Release.Major.ShouldBe(2);
        v.Release.Minor.ShouldBe(3);
        v.Release.Build.ShouldBe(5);
        v.Release.Revision.ShouldBe(7);
        v.Milestone.ShouldBe(Milestone.Release);
        v.PreReleaseNumber.ShouldBe(-1);
        v.Stability.ShouldBe(Stability.Stable);
        v.ReleaseDate.HasValue.ShouldBeTrue();
        v.ReleaseDate.Value.Year.ShouldBe(1985);
    }

    [Fact]
    public void Version019()
    {
        var v = new Version(2, 3, 5, new System.DateTime(1985, 11, 20));

        v.Release.Major.ShouldBe(2);
        v.Release.Minor.ShouldBe(3);
        v.Release.Build.ShouldBe(5);
        v.Release.Revision.ShouldBe(-1);
        v.Milestone.ShouldBe(Milestone.Release);
        v.PreReleaseNumber.ShouldBe(-1);
        v.Stability.ShouldBe(Stability.Stable);
        v.ReleaseDate.HasValue.ShouldBeTrue();
        v.ReleaseDate.Value.Year.ShouldBe(1985);
    }

    [Fact]
    public void Version020()
    {
        var v = new Version(2, 3, new System.DateTime(1985, 11, 20));

        v.Release.Major.ShouldBe(2);
        v.Release.Minor.ShouldBe(3);
        v.Release.Build.ShouldBe(-1);
        v.Release.Revision.ShouldBe(-1);
        v.Milestone.ShouldBe(Milestone.Release);
        v.PreReleaseNumber.ShouldBe(-1);
        v.Stability.ShouldBe(Stability.Stable);
        v.ReleaseDate.HasValue.ShouldBeTrue();
        v.ReleaseDate.Value.Year.ShouldBe(1985);
    }

    [Fact]
    public void Version021()
    {
        var v = new Version(2, 3, Milestone.Release, new System.DateTime(1985, 11, 20));

        v.Release.Major.ShouldBe(2);
        v.Release.Minor.ShouldBe(3);
        v.Release.Build.ShouldBe(-1);
        v.Release.Revision.ShouldBe(-1);
        v.Milestone.ShouldBe(Milestone.Release);
        v.PreReleaseNumber.ShouldBe(-1);
        v.Stability.ShouldBe(Stability.Stable);
        v.ReleaseDate.HasValue.ShouldBeTrue();
        v.ReleaseDate.Value.Year.ShouldBe(1985);
    }

    [Fact]
    public void Version022()
    {
        var v = new Version(2, 3, 5, Milestone.Release, new System.DateTime(1985, 11, 20));

        v.Release.Major.ShouldBe(2);
        v.Release.Minor.ShouldBe(3);
        v.Release.Build.ShouldBe(5);
        v.Release.Revision.ShouldBe(-1);
        v.Milestone.ShouldBe(Milestone.Release);
        v.PreReleaseNumber.ShouldBe(-1);
        v.Stability.ShouldBe(Stability.Stable);
        v.ReleaseDate.HasValue.ShouldBeTrue();
        v.ReleaseDate.Value.Year.ShouldBe(1985);
    }

    [Fact]
    public void Version023()
    {
        var v = new Version(2, 3, 5, 7, Milestone.Release, new System.DateTime(1985, 11, 20));

        v.Release.Major.ShouldBe(2);
        v.Release.Minor.ShouldBe(3);
        v.Release.Build.ShouldBe(5);
        v.Release.Revision.ShouldBe(7);
        v.Milestone.ShouldBe(Milestone.Release);
        v.PreReleaseNumber.ShouldBe(-1);
        v.Stability.ShouldBe(Stability.Stable);
        v.ReleaseDate.HasValue.ShouldBeTrue();
        v.ReleaseDate.Value.Year.ShouldBe(1985);
    }

    [Fact]
    public void Version024()
    {
        var v = new Version(new System.Version(2, 3, 5, 7), Milestone.Release, new System.DateTime(1985, 11, 20));

        v.Release.Major.ShouldBe(2);
        v.Release.Minor.ShouldBe(3);
        v.Release.Build.ShouldBe(5);
        v.Release.Revision.ShouldBe(7);
        v.Milestone.ShouldBe(Milestone.Release);
        v.PreReleaseNumber.ShouldBe(-1);
        v.Stability.ShouldBe(Stability.Stable);
        v.ReleaseDate.HasValue.ShouldBeTrue();
        v.ReleaseDate.Value.Year.ShouldBe(1985);
    }

    [Fact]
    public void Version025()
    {
        var v = new Version(new System.Version(2, 3, 5, 7), Milestone.Release, new System.DateTime(1985, 11, 20));

        v.Release.Major.ShouldBe(2);
        v.Release.Minor.ShouldBe(3);
        v.Release.Build.ShouldBe(5);
        v.Release.Revision.ShouldBe(7);
        v.Milestone.ShouldBe(Milestone.Release);
        v.Stability.ShouldBe(Stability.Stable);
        v.ReleaseDate.HasValue.ShouldBeTrue();
        v.ReleaseDate.Value.Year.ShouldBe(1985);
    }

    [Fact]
    public void Version026()
    {
        var v = new Version(2, 3, 5, 7, Milestone.Release, new System.DateTime(1985, 11, 20));

        v.Release.Major.ShouldBe(2);
        v.Release.Minor.ShouldBe(3);
        v.Release.Build.ShouldBe(5);
        v.Release.Revision.ShouldBe(7);
        v.Milestone.ShouldBe(Milestone.Release);
        v.Stability.ShouldBe(Stability.Stable);
        v.ReleaseDate.HasValue.ShouldBeTrue();
        v.ReleaseDate.Value.Year.ShouldBe(1985);
    }

    [Fact]
    public void Version027()
    {
        var v = new Version(2, 3, 5, Milestone.ReleaseCandidate, 1, new System.DateTime(1985, 11, 20));

        v.Release.Major.ShouldBe(2);
        v.Release.Minor.ShouldBe(3);
        v.Release.Build.ShouldBe(5);
        v.Release.Revision.ShouldBe(-1);
        v.Milestone.ShouldBe(Milestone.ReleaseCandidate);
        v.PreReleaseNumber.ShouldBe(1);
        v.Stability.ShouldBe(Stability.Unstable);
        v.ReleaseDate.HasValue.ShouldBeTrue();
        v.ReleaseDate.Value.Year.ShouldBe(1985);
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

        version.ToString().ShouldBe(nVersion.ToString());
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

        version.ToString().ShouldBe(nVersion.ToString());
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

        versionEquals.ShouldBe(nequals);
        (version1 > version2).ShouldBe(nv1 > nv2);
        (version1 >= version2).ShouldBe(nv1 >= nv2);
        (version1 < version2).ShouldBe(nv1 < nv2);
        (version1 <= version2).ShouldBe(nv1 <= nv2);
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
        hash1.ShouldNotBe(hash2);
        hash2.ShouldNotBe(hash3);
        hash3.ShouldNotBe(hash4);
        hash4.ShouldNotBe(hash1);
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
        version1.Equals(version1).ShouldBeTrue();
        version1.Equals(version2).ShouldBeFalse();
        version2.Equals(version3).ShouldBeFalse();
        version3.Equals(version4).ShouldBeTrue();
        version1.Equals(version5).ShouldBeTrue();
        version1.Equals(version6).ShouldBeTrue();
        version1.Equals("Welcome").ShouldBeFalse();
        version1.Equals(null).ShouldBeFalse();
    }
}
