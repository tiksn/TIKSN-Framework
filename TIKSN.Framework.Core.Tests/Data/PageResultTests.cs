using Shouldly;
using TIKSN.Data;
using Xunit;
using static LanguageExt.Prelude;

namespace TIKSN.Tests.Data;

public class PageResultTests
{
    private static readonly char[] items = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J'];

    [Theory]
    [InlineData(null, null)]
    [InlineData(0L, 0L)]
    [InlineData(20L, 2L)]
    [InlineData(32L, 4L)]
    public void TotalPagesTest(long? totalItems, long? totalPages)
    {
        PageResult<char> pageResult = new(
            new Page(1, 10),
            items,
            Optional(totalItems));

        pageResult.TotalPages.ShouldBe(Optional(totalPages));
        pageResult.TotalItems.ShouldBe(Optional(totalItems));
    }
}
