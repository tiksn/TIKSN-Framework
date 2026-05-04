using System.Collections.Generic;
using System.Linq;
using Shouldly;
using TIKSN.Numbering;
using Xunit;

namespace TIKSN.Tests.Numbering;

public class BB26Tests
{
    public static IEnumerable<TheoryDataRow<int, string>> GetBB26NumberAndStringTestData()
    {
        yield return new TheoryDataRow<int, string>(p1: 1, "A");
        yield return new TheoryDataRow<int, string>(p1: 2, "B");
        yield return new TheoryDataRow<int, string>(p1: 3, "C");
        yield return new TheoryDataRow<int, string>(p1: 4, "D");
        yield return new TheoryDataRow<int, string>(p1: 10, "J");
        yield return new TheoryDataRow<int, string>(p1: 11, "K");
        yield return new TheoryDataRow<int, string>(p1: 21, "U");
        yield return new TheoryDataRow<int, string>(p1: 25, "Y");
        yield return new TheoryDataRow<int, string>(p1: 26, "Z");
        yield return new TheoryDataRow<int, string>(p1: 27, "AA");
        yield return new TheoryDataRow<int, string>(p1: 28, "AB");
        yield return new TheoryDataRow<int, string>(p1: 29, "AC");
        yield return new TheoryDataRow<int, string>(p1: 702, "ZZ");
        yield return new TheoryDataRow<int, string>(p1: 703, "AAA");
        yield return new TheoryDataRow<int, string>(p1: 731, "ABC");
        yield return new TheoryDataRow<int, string>(p1: 760, "ACF");
        yield return new TheoryDataRow<int, string>(p1: 1_457, "BDA");
        yield return new TheoryDataRow<int, string>(p1: 1_482, "BDZ");
        yield return new TheoryDataRow<int, string>(p1: 16_384, "XFD");
        yield return new TheoryDataRow<int, string>(p1: 18_278, "ZZZ");
        yield return new TheoryDataRow<int, string>(p1: 3_752_127, "HELLO");
    }

    public static IEnumerable<TheoryDataRow<string, int>> GetBB26StringAndNumberTestData() =>
        GetBB26NumberAndStringTestData().Select(x => new TheoryDataRow<string, int>(x.Data.Item2, x.Data.Item1));

    [Theory]
    [MemberData(nameof(GetBB26NumberAndStringTestData))]
    public void GivenBB26_WhenToStringIsCalled_ThenResultShouldBe(int number, string expectedString)
    {
        // Arrange
        var bb26 = new BB26(number);

        // Act
        var actualString = bb26.ToString();

        // Assert
        actualString.ShouldBe(expectedString);
    }

    [Theory]
    [MemberData(nameof(GetBB26StringAndNumberTestData))]
    public void GiveninputString_WhenToStringIsCalled_ThenResultShouldBe(string inputString, int expectedNumber)
    {
        // Arrange
        var bb26 = BB26.Parse(inputString, provider: null);

        // Act
        var actualNumber = bb26.Number;
        var actualIndex = bb26.Index;

        // Assert
        actualNumber.ShouldBe(expectedNumber);
        actualIndex.ShouldBe(expectedNumber - 1);
    }
}
